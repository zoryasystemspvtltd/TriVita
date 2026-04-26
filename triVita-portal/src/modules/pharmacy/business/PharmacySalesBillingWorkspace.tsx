import { yupResolver } from '@hookform/resolvers/yup';
import { Box, Grid, Link, Stack, Table, TableBody, TableCell, TableHead, TableRow, TextField, Typography } from '@mui/material';
import { Add, Delete } from '@mui/icons-material';
import IconButton from '@mui/material/IconButton';
import { useMutation, useQueries, useQuery, useQueryClient } from '@tanstack/react-query';
import { useEffect, useMemo, useState } from 'react';
import { Controller, useForm, type Resolver } from 'react-hook-form';
import * as Yup from 'yup';
import { DataTable } from '@/components/common/DataTable';
import { LookupSelect } from '@/components/common/LookupSelect';
import { PatientSearchField } from '@/components/pharmacy/PatientSearchField';
import { DetailKv } from '@/components/masters/DetailKv';
import { DetailDrawer } from '@/components/layout/DetailDrawer';
import { PageHeader } from '@/components/layout/PageHeader';
import { FormSection } from '@/components/forms/FormSection';
import { FormGroup } from '@/components/ds/FormGroup';
import { TriVitaButton } from '@/components/ds/TriVitaButton';
import { TriVitaModal } from '@/components/ds/TriVitaModal';
import { TriVitaTextField } from '@/components/ds/TriVitaTextField';
import { useToast } from '@/components/toast/ToastProvider';
import {
  createPharmacySale,
  createPharmacySalesItem,
  deletePharmacySale,
  deletePharmacySalesItem,
  getMedicineBatchPaged,
  getMedicinePaged,
  getPharmacySaleById,
  getPharmacySalesItemsPaged,
  getPharmacySalesPaged,
  updatePharmacySale,
} from '@/services/pharmacyService';
import { getPatientMasterById } from '@/services/hmsService';
import { buildPharmacyReferenceStatusOptions } from '@/utils/pharmacyStatusOptions';
import { getApiErrorMessage } from '@/utils/errorMap';

type Row = Record<string, unknown> & { id?: number };

function pickStr(r: Record<string, unknown>, ...keys: string[]) {
  for (const k of keys) {
    const v = r[k];
    if (v != null && String(v).trim() !== '') return String(v);
  }
  return '';
}

type HForm = {
  salesNo: string;
  salesDate: string;
  statusReferenceValueId: string;
  currencyCode: string;
  paymentTotal: string;
  notes: string;
};

const hSchema = Yup.object({
  salesNo: Yup.string().trim().required().max(80),
  salesDate: Yup.string().required(),
  statusReferenceValueId: Yup.string().trim().required().matches(/^\d+$/),
  currencyCode: Yup.string().trim().max(8).default(''),
  paymentTotal: Yup.string().trim().default(''),
  notes: Yup.string().trim().max(2000).default(''),
});

export function PharmacySalesBillingWorkspace() {
  const qc = useQueryClient();
  const { showToast } = useToast();
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const [search, setSearch] = useState('');
  const [searchApplied, setSearchApplied] = useState('');
  const [drawerRow, setDrawerRow] = useState<Row | null>(null);
  const [modal, setModal] = useState<null | { mode: 'create' } | { mode: 'edit'; id: number }>(null);
  const [deleteId, setDeleteId] = useState<number | null>(null);
  const [patient, setPatient] = useState<{ id: number; label: string } | null>(null);
  const [lineSaleId, setLineSaleId] = useState<number | null>(null);
  const [lineDraft, setLineDraft] = useState({
    medicineId: '',
    medicineBatchId: '',
    quantitySold: '',
    unitPrice: '',
  });

  useEffect(() => {
    const t = window.setTimeout(() => setSearchApplied(search), 400);
    return () => window.clearTimeout(t);
  }, [search]);

  const list = useQuery({
    queryKey: ['pharmacy', 'sales', page, pageSize, searchApplied],
    queryFn: () => getPharmacySalesPaged({ page: page + 1, pageSize, search: searchApplied || undefined }),
  });

  const statusOptsQuery = useQuery({
    queryKey: ['pharmacy', 'status-opts'],
    queryFn: buildPharmacyReferenceStatusOptions,
    staleTime: 120_000,
  });

  const sm = useMemo(() => {
    const m = new Map<string, string>();
    for (const o of statusOptsQuery.data ?? []) m.set(o.value, o.label);
    return m;
  }, [statusOptsQuery.data]);

  const rows = useMemo(
    () => (list.data?.success && list.data.data ? [...list.data.data.items] : []) as Row[],
    [list.data]
  );
  const total = list.data?.success && list.data.data ? list.data.data.totalCount : 0;

  const patientIds = useMemo(() => {
    const s = new Set<number>();
    for (const r of rows) {
      const pid = Number(r.patientId ?? r.PatientId);
      if (Number.isFinite(pid)) s.add(pid);
    }
    return [...s].sort((a, b) => a - b);
  }, [rows]);

  const patientResults = useQueries({
    queries: patientIds.map((id) => ({
      queryKey: ['hms', 'patient-master', id],
      queryFn: () => getPatientMasterById(id),
      staleTime: 300_000,
    })),
  });

  const pl = useMemo(() => {
    const m = new Map<number, string>();
    patientResults.forEach((q, i) => {
      const id = patientIds[i];
      if (q.data?.success && q.data.data) m.set(id, q.data.data.fullName);
    });
    return m;
  }, [patientIds, patientResults]);

  const detail = useQuery({
    queryKey: ['pharmacy', 'sale-detail', drawerRow?.id],
    queryFn: () => getPharmacySaleById(Number(drawerRow!.id)),
    enabled: Boolean(drawerRow?.id),
  });

  const editSeed = useQuery({
    queryKey: ['pharmacy', 'sale-edit', modal != null && modal.mode === 'edit' ? modal.id : null],
    queryFn: () => getPharmacySaleById((modal as { mode: 'edit'; id: number }).id),
    enabled: modal != null && modal.mode === 'edit',
  });

  const editPatientId = useMemo(() => {
    if (modal?.mode !== 'edit' || !editSeed.data?.success || !editSeed.data.data) return null;
    const r = editSeed.data.data as Row;
    const pid = Number(r.patientId ?? r.PatientId);
    return Number.isFinite(pid) ? pid : null;
  }, [modal, editSeed.data]);

  const editPatientQ = useQuery({
    queryKey: ['hms', 'patient-master', 'sale-edit', editPatientId],
    queryFn: () => getPatientMasterById(editPatientId!),
    enabled: modal?.mode === 'edit' && editPatientId != null,
  });

  useEffect(() => {
    if (modal?.mode === 'edit' && editPatientQ.data?.success && editPatientQ.data.data) {
      const p = editPatientQ.data.data;
      setPatient({ id: p.id, label: p.fullName });
    }
  }, [modal?.mode, editPatientQ.data]);

  const saleLines = useQuery({
    queryKey: ['pharmacy', 'sale-lines', lineSaleId],
    queryFn: () => getPharmacySalesItemsPaged({ page: 1, pageSize: 500 }),
    enabled: lineSaleId != null,
  });

  const medicines = useQuery({
    queryKey: ['pharmacy', 'sale-meds'],
    queryFn: () => getMedicinePaged({ page: 1, pageSize: 500 }),
    staleTime: 120_000,
  });

  const medMap = useMemo(() => {
    const m = new Map<number, string>();
    const d = medicines.data?.success ? medicines.data.data : null;
    for (const x of (d?.items ?? []) as Row[]) {
      const id = Number(x.id);
      if (Number.isFinite(id)) m.set(id, pickStr(x, 'medicineName', 'MedicineName'));
    }
    return m;
  }, [medicines.data]);

  const batches = useQuery({
    queryKey: ['pharmacy', 'sale-batches', lineDraft.medicineId],
    queryFn: () => getMedicineBatchPaged({ page: 1, pageSize: 500 }),
    enabled: lineSaleId != null,
  });

  const batchOpts = useMemo(() => {
    if (!batches.data?.success || !batches.data.data) return [];
    const mid = Number(lineDraft.medicineId);
    return (batches.data.data.items as Row[])
      .filter((b) => !Number.isFinite(mid) || Number(b.medicineId ?? b.MedicineId) === mid)
      .map((b) => ({
        value: String(b.id ?? ''),
        label: `${pickStr(b, 'batchNo', 'BatchNo')} · exp ${String(b.expiryDate ?? '').slice(0, 10)}`,
      }));
  }, [batches.data, lineDraft.medicineId]);

  const lineRows = useMemo(() => {
    if (!saleLines.data?.success || !saleLines.data.data || lineSaleId == null) return [];
    return (saleLines.data.data.items as Row[]).filter((x) => Number(x.pharmacySalesId ?? x.PharmacySalesId) === lineSaleId);
  }, [saleLines.data, lineSaleId]);

  const {
    control,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<HForm>({
    resolver: yupResolver(hSchema) as Resolver<HForm>,
    defaultValues: {
      salesNo: '',
      salesDate: new Date().toISOString().slice(0, 10),
      statusReferenceValueId: '',
      currencyCode: '',
      paymentTotal: '',
      notes: '',
    },
  });

  useEffect(() => {
    if (!modal) return;
    if (modal.mode === 'create') {
      reset({
        salesNo: '',
        salesDate: new Date().toISOString().slice(0, 10),
        statusReferenceValueId: statusOptsQuery.data?.[0]?.value ?? '',
        currencyCode: '',
        paymentTotal: '',
        notes: '',
      });
      setPatient(null);
      return;
    }
    const d = editSeed.data;
    if (d?.success && d.data) {
      const r = d.data as Row;
      reset({
        salesNo: pickStr(r, 'salesNo', 'SalesNo'),
        salesDate: pickStr(r, 'salesDate', 'SalesDate').slice(0, 10),
        statusReferenceValueId: String(r.statusReferenceValueId ?? ''),
        currencyCode: pickStr(r, 'currencyCode', 'CurrencyCode'),
        paymentTotal: r.paymentTotal != null ? String(r.paymentTotal) : '',
        notes: pickStr(r, 'notes', 'Notes'),
      });
    }
  }, [modal, editSeed.data, reset, statusOptsQuery.data]);

  const saveMut = useMutation({
    mutationFn: async (args: { v: HForm; editId?: number; patientId: number }) => {
      const body = {
        salesNo: args.v.salesNo.trim(),
        patientId: args.patientId,
        salesDate: new Date(args.v.salesDate).toISOString(),
        statusReferenceValueId: Number(args.v.statusReferenceValueId),
        currencyCode: args.v.currencyCode.trim() || undefined,
        paymentTotal: args.v.paymentTotal.trim() ? Number(args.v.paymentTotal) : undefined,
        notes: args.v.notes.trim() || undefined,
      };
      if (args.editId != null) return updatePharmacySale(args.editId, body);
      return createPharmacySale(body);
    },
    onSuccess: (res, vars) => {
      if (!res.success) {
        showToast(res.message ?? 'Save failed', 'error');
        return;
      }
      showToast('Sale saved', 'success');
      const created = res.data as Row | undefined;
      const newId = vars.editId ?? (created?.id != null ? Number(created.id) : NaN);
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'sales'] });
      setModal(null);
      if (Number.isFinite(newId)) setLineSaleId(newId);
    },
    onError: (e) => showToast(getApiErrorMessage(e), 'error'),
  });

  const delMut = useMutation({
    mutationFn: (id: number) => deletePharmacySale(id),
    onSuccess: (res) => {
      if (!res.success) {
        showToast(res.message ?? 'Delete failed', 'error');
        return;
      }
      showToast('Deleted', 'success');
      setDeleteId(null);
      setDrawerRow(null);
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'sales'] });
    },
    onError: (e) => showToast(getApiErrorMessage(e), 'error'),
  });

  const addLineMut = useMutation({
    mutationFn: async () => {
      if (lineSaleId == null) throw new Error('No sale');
      const nextLine = lineRows.length === 0 ? 1 : Math.max(...lineRows.map((r) => Number(r.lineNum ?? 0))) + 1;
      const qty = Number(lineDraft.quantitySold);
      const price = Number(lineDraft.unitPrice);
      return createPharmacySalesItem({
        pharmacySalesId: lineSaleId,
        lineNum: nextLine,
        medicineId: Number(lineDraft.medicineId),
        medicineBatchId: Number(lineDraft.medicineBatchId),
        quantitySold: qty,
        unitPrice: Number.isFinite(price) ? price : undefined,
        lineTotal: Number.isFinite(price) && Number.isFinite(qty) ? qty * price : undefined,
      });
    },
    onSuccess: (res) => {
      if (!res.success) {
        showToast(res.message ?? 'Line failed', 'error');
        return;
      }
      setLineDraft({ medicineId: '', medicineBatchId: '', quantitySold: '', unitPrice: '' });
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'sale-lines'] });
    },
    onError: (e) => showToast(getApiErrorMessage(e), 'error'),
  });

  const delLineMut = useMutation({
    mutationFn: (id: number) => deletePharmacySalesItem(id),
    onSuccess: (res) => {
      if (!res.success) {
        showToast(res.message ?? 'Delete failed', 'error');
        return;
      }
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'sale-lines'] });
    },
    onError: (e) => showToast(getApiErrorMessage(e), 'error'),
  });

  const onSave = (v: HForm) => {
    if (!patient) {
      showToast('Select a patient', 'warning');
      return;
    }
    saveMut.mutate({ v, editId: modal != null && modal.mode === 'edit' ? modal.id : undefined, patientId: patient.id });
  };

  const lineTotalPreview = useMemo(() => {
    const q = Number(lineDraft.quantitySold);
    const p = Number(lineDraft.unitPrice);
    if (Number.isFinite(q) && Number.isFinite(p)) return (q * p).toFixed(2);
    return '—';
  }, [lineDraft.quantitySold, lineDraft.unitPrice]);

  const detailData = (detail.data?.success ? detail.data.data : null) as Row | null;

  return (
    <Stack spacing={2}>
      <PageHeader title="Sales / billing" />

      <FormSection title="Search">
        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} alignItems={{ sm: 'center' }} justifyContent="space-between">
          <TextField size="small" label="Search" value={search} onChange={(e) => setSearch(e.target.value)} sx={{ flex: 1, minWidth: 220 }} />
          <Box sx={{ display: 'flex', justifyContent: 'flex-end' }}>
            <TriVitaButton variant="contained" onClick={() => setModal({ mode: 'create' })}>
              New sale
            </TriVitaButton>
          </Box>
        </Stack>
      </FormSection>

      <FormSection title="Sales register">
        <DataTable<Row>
          tableAriaLabel="Sales"
          columns={[
            { id: 'salesNo', label: 'Sale #', format: (r) => pickStr(r, 'salesNo', 'SalesNo') },
            {
              id: 'patientId',
              label: 'Patient',
              format: (r) => {
                const pid = Number(r.patientId ?? r.PatientId);
                return pl.get(pid) ?? '—';
              },
            },
            { id: 'salesDate', label: 'Date', format: (r) => (r.salesDate != null ? String(r.salesDate).slice(0, 10) : '—') },
            {
              id: 'paymentTotal',
              label: 'Total',
              align: 'right',
              format: (r) => (r.paymentTotal != null ? String(r.paymentTotal) : '—'),
            },
            {
              id: '_a',
              label: 'Actions',
              minWidth: 220,
              format: (r) => {
                const id = Number(r.id);
                return (
                  <Stack direction="row" spacing={1} flexWrap="wrap" useFlexGap>
                    <Link component="button" type="button" variant="body2" onClick={() => setDrawerRow(r)}>
                      View
                    </Link>
                    <Typography variant="body2" color="text.disabled">
                      |
                    </Typography>
                    <Link
                      component="button"
                      type="button"
                      variant="body2"
                      onClick={() => {
                        setModal({ mode: 'edit', id });
                        setLineSaleId(id);
                      }}
                    >
                      Edit
                    </Link>
                    <Typography variant="body2" color="text.disabled">
                      |
                    </Typography>
                    <Link component="button" type="button" variant="body2" color="error" onClick={() => setDeleteId(id)}>
                      Delete
                    </Link>
                    <Typography variant="body2" color="text.disabled">
                      |
                    </Typography>
                    <Link component="button" type="button" variant="body2" onClick={() => setLineSaleId(id)}>
                      Lines
                    </Link>
                  </Stack>
                );
              },
            },
          ]}
          rows={rows}
          rowKey={(r) => String(r.id ?? '')}
          totalCount={total}
          page={page}
          pageSize={pageSize}
          onPageChange={(p, ps) => {
            setPage(p);
            setPageSize(ps);
          }}
          loading={list.isLoading}
          emptyTitle="No sales"
        />
      </FormSection>

      {lineSaleId != null ? (
        <FormSection title="Sale lines" action={<TriVitaButton onClick={() => setLineSaleId(null)}>Close</TriVitaButton>}>
          <Table size="small" sx={{ mb: 2 }}>
            <TableHead>
              <TableRow>
                <TableCell>Medicine</TableCell>
                <TableCell align="right">Qty</TableCell>
                <TableCell align="right">Unit price</TableCell>
                <TableCell align="right">Line total</TableCell>
                <TableCell align="right">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {lineRows.map((lr) => (
                <TableRow key={String(lr.id)} hover>
                  <TableCell>{medMap.get(Number(lr.medicineId ?? lr.MedicineId)) ?? '—'}</TableCell>
                  <TableCell align="right">{String(lr.quantitySold ?? '')}</TableCell>
                  <TableCell align="right">{String(lr.unitPrice ?? '')}</TableCell>
                  <TableCell align="right">{String(lr.lineTotal ?? '')}</TableCell>
                  <TableCell align="right">
                    <IconButton size="small" onClick={() => lr.id != null && delLineMut.mutate(Number(lr.id))}>
                      <Delete fontSize="small" />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
          <Typography variant="subtitle2">Add line</Typography>
          <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} sx={{ mt: 1 }} alignItems={{ sm: 'flex-end' }}>
            <TriVitaTextField
              select
              SelectProps={{ native: true }}
              label="Medicine"
              size="small"
              value={lineDraft.medicineId}
              onChange={(e) => setLineDraft((d) => ({ ...d, medicineId: e.target.value, medicineBatchId: '' }))}
              sx={{ flex: 1, minWidth: 200 }}
            >
              <option value="">Select</option>
              {[...medMap.entries()]
                .sort((a, b) => a[1].localeCompare(b[1]))
                .map(([id, name]) => (
                  <option key={id} value={String(id)}>
                    {name}
                  </option>
                ))}
            </TriVitaTextField>
            <TriVitaTextField
              select
              SelectProps={{ native: true }}
              label="Batch"
              size="small"
              value={lineDraft.medicineBatchId}
              onChange={(e) => setLineDraft((d) => ({ ...d, medicineBatchId: e.target.value }))}
              sx={{ flex: 1, minWidth: 200 }}
            >
              <option value="">Select</option>
              {batchOpts.map((o) => (
                <option key={o.value} value={o.value}>
                  {o.label}
                </option>
              ))}
            </TriVitaTextField>
            <TriVitaTextField label="Qty" size="small" value={lineDraft.quantitySold} onChange={(e) => setLineDraft((d) => ({ ...d, quantitySold: e.target.value }))} sx={{ width: 100 }} />
            <TriVitaTextField label="Unit price" size="small" value={lineDraft.unitPrice} onChange={(e) => setLineDraft((d) => ({ ...d, unitPrice: e.target.value }))} sx={{ width: 120 }} />
            <Typography variant="body2" sx={{ minWidth: 80 }}>
              Line: {lineTotalPreview}
            </Typography>
            <TriVitaButton
              variant="contained"
              startIcon={<Add />}
              disabled={addLineMut.isPending || !lineDraft.medicineId || !lineDraft.medicineBatchId || !lineDraft.quantitySold.trim()}
              onClick={() => addLineMut.mutate()}
            >
              Add
            </TriVitaButton>
          </Stack>
        </FormSection>
      ) : null}

      <DetailDrawer
        open={drawerRow != null}
        onClose={() => setDrawerRow(null)}
        title={detailData ? pickStr(detailData, 'salesNo', 'SalesNo') : 'Sale'}
        subtitle={detailData ? pl.get(Number(detailData.patientId ?? detailData.PatientId)) ?? '' : undefined}
      >
        {detailData ? (
          <Stack spacing={1}>
            <DetailKv label="Sale number" value={pickStr(detailData, 'salesNo', 'SalesNo')} />
            <DetailKv label="Patient" value={pl.get(Number(detailData.patientId ?? detailData.PatientId)) ?? ''} />
            <DetailKv label="Sale date" value={detailData.salesDate != null ? String(detailData.salesDate).slice(0, 10) : ''} />
            <DetailKv label="Status" value={sm.get(String(detailData.statusReferenceValueId ?? '')) ?? ''} />
            <DetailKv label="Currency" value={pickStr(detailData, 'currencyCode', 'CurrencyCode')} />
            <DetailKv label="Payment total" value={detailData.paymentTotal != null ? String(detailData.paymentTotal) : ''} />
            <DetailKv label="Notes" value={pickStr(detailData, 'notes', 'Notes')} />
          </Stack>
        ) : null}
      </DetailDrawer>

      <TriVitaModal
        open={Boolean(modal)}
        onClose={() => setModal(null)}
        title={modal?.mode === 'edit' ? 'Edit sale' : 'New sale'}
        actions={
          <Stack direction="row" spacing={2} justifyContent="flex-end" sx={{ width: '100%' }}>
            <TriVitaButton onClick={() => setModal(null)}>Cancel</TriVitaButton>
            <TriVitaButton type="submit" form="sale-header" variant="contained" disabled={saveMut.isPending}>
              Save
            </TriVitaButton>
          </Stack>
        }
      >
        <Box component="form" id="sale-header" onSubmit={handleSubmit(onSave)} noValidate>
          <PatientSearchField value={patient} onChange={setPatient} />
          <FormGroup>
            <Grid item xs={12} md={6}>
              <Controller
                name="salesNo"
                control={control}
                render={({ field }) => (
                  <TriVitaTextField {...field} label="Sale number" required error={Boolean(errors.salesNo)} helperText={errors.salesNo?.message} />
                )}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <Controller
                name="salesDate"
                control={control}
                render={({ field }) => (
                  <TriVitaTextField {...field} type="date" InputLabelProps={{ shrink: true }} label="Sale date" required error={Boolean(errors.salesDate)} helperText={errors.salesDate?.message} />
                )}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <LookupSelect<Record<string, string>>
                name="statusReferenceValueId"
                control={control as never}
                label="Status"
                required
                editId={null}
                queryKey={['pharmacy', 'sale-status']}
                loadOptions={async () => {
                  const o = await buildPharmacyReferenceStatusOptions();
                  return o.length ? o : [{ value: '1', label: 'Default' }];
                }}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <Controller
                name="currencyCode"
                control={control}
                render={({ field }) => <TriVitaTextField {...field} label="Currency code" />}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <Controller
                name="paymentTotal"
                control={control}
                render={({ field }) => <TriVitaTextField {...field} label="Payment total" type="number" />}
              />
            </Grid>
            <Grid item xs={12}>
              <Controller name="notes" control={control} render={({ field }) => <TriVitaTextField {...field} label="Notes" multiline minRows={2} />} />
            </Grid>
          </FormGroup>
        </Box>
      </TriVitaModal>

      <TriVitaModal open={deleteId != null} onClose={() => setDeleteId(null)} title="Delete sale" actions={
        <Stack direction="row" spacing={2} justifyContent="flex-end" sx={{ width: '100%' }}>
          <TriVitaButton onClick={() => setDeleteId(null)}>Cancel</TriVitaButton>
          <TriVitaButton color="error" variant="contained" onClick={() => deleteId != null && delMut.mutate(deleteId)} disabled={delMut.isPending}>
            Delete
          </TriVitaButton>
        </Stack>
      }>
        <Typography>Delete this sale?</Typography>
      </TriVitaModal>
    </Stack>
  );
}
