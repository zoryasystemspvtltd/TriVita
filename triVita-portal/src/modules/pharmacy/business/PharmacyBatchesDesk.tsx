import { yupResolver } from '@hookform/resolvers/yup';
import { Box, Grid, Link, Stack, TextField, Typography } from '@mui/material';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useEffect, useMemo, useState } from 'react';
import { Controller, useForm, type Resolver } from 'react-hook-form';
import * as Yup from 'yup';
import { DataTable } from '@/components/common/DataTable';
import { LookupSelect } from '@/components/common/LookupSelect';
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
  createMedicineBatch,
  deleteMedicineBatch,
  getMedicineBatchById,
  getMedicineBatchPaged,
  getMedicinePaged,
  updateMedicineBatch,
} from '@/services/pharmacyService';
import { getApiErrorMessage } from '@/utils/errorMap';

type Row = Record<string, unknown> & { id?: number };

function pickStr(r: Record<string, unknown>, ...keys: string[]) {
  for (const k of keys) {
    const v = r[k];
    if (v != null && String(v).trim() !== '') return String(v);
  }
  return '';
}

type BatchForm = {
  medicineId: string;
  batchNo: string;
  expiryDate: string;
  mrp: string;
  purchaseRate: string;
  manufacturingDate: string;
};

const batchSchema = Yup.object({
  medicineId: Yup.string().required().matches(/^\d+$/, 'Select a medicine'),
  batchNo: Yup.string().trim().required().max(80),
  expiryDate: Yup.string().default(''),
  mrp: Yup.string().trim().default(''),
  purchaseRate: Yup.string().trim().default(''),
  manufacturingDate: Yup.string().default(''),
});

export function PharmacyBatchesDesk() {
  const qc = useQueryClient();
  const { showToast } = useToast();
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const [search, setSearch] = useState('');
  const [searchApplied, setSearchApplied] = useState('');
  const [drawerId, setDrawerId] = useState<number | null>(null);
  const [modal, setModal] = useState<null | { mode: 'create' } | { mode: 'edit'; id: number }>(null);
  const [deleteId, setDeleteId] = useState<number | null>(null);

  useEffect(() => {
    const t = window.setTimeout(() => setSearchApplied(search), 400);
    return () => window.clearTimeout(t);
  }, [search]);

  useEffect(() => {
    setPage(0);
  }, [searchApplied]);

  const list = useQuery({
    queryKey: ['pharmacy', 'batches', page, pageSize, searchApplied],
    queryFn: () => getMedicineBatchPaged({ page: page + 1, pageSize, search: searchApplied || undefined }),
  });

  const medicines = useQuery({
    queryKey: ['pharmacy', 'batches-med-map'],
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

  const rows = useMemo(
    () => (list.data?.success && list.data.data ? [...list.data.data.items] : []) as Row[],
    [list.data]
  );
  const total = list.data?.success && list.data.data ? list.data.data.totalCount : 0;

  const detail = useQuery({
    queryKey: ['pharmacy', 'batch-detail', drawerId],
    queryFn: () => getMedicineBatchById(drawerId!),
    enabled: drawerId != null,
  });

  const editSeed = useQuery({
    queryKey: ['pharmacy', 'batch-edit', modal != null && modal.mode === 'edit' ? modal.id : null],
    queryFn: () => getMedicineBatchById((modal as { mode: 'edit'; id: number }).id),
    enabled: modal != null && modal.mode === 'edit',
  });

  const {
    control,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<BatchForm>({
    resolver: yupResolver(batchSchema) as Resolver<BatchForm>,
    defaultValues: {
      medicineId: '',
      batchNo: '',
      expiryDate: '',
      mrp: '',
      purchaseRate: '',
      manufacturingDate: '',
    },
  });

  useEffect(() => {
    if (!modal) return;
    if (modal.mode === 'create') {
      reset({
        medicineId: '',
        batchNo: '',
        expiryDate: '',
        mrp: '',
        purchaseRate: '',
        manufacturingDate: '',
      });
      return;
    }
    const d = editSeed.data;
    if (d?.success && d.data) {
      const r = d.data as Row;
      reset({
        medicineId: String(r.medicineId ?? r.MedicineId ?? ''),
        batchNo: pickStr(r, 'batchNo', 'BatchNo'),
        expiryDate: r.expiryDate != null ? String(r.expiryDate).slice(0, 10) : '',
        mrp: r.mrp != null ? String(r.mrp) : '',
        purchaseRate: r.purchaseRate != null ? String(r.purchaseRate) : '',
        manufacturingDate: r.manufacturingDate != null ? String(r.manufacturingDate).slice(0, 10) : '',
      });
    }
  }, [modal, editSeed.data, reset]);

  const saveMut = useMutation({
    mutationFn: async (args: { v: BatchForm; editId?: number }) => {
      const body = {
        medicineId: Number(args.v.medicineId),
        batchNo: args.v.batchNo.trim(),
        expiryDate: args.v.expiryDate.trim() ? new Date(args.v.expiryDate).toISOString() : undefined,
        mrp: args.v.mrp.trim() ? Number(args.v.mrp) : undefined,
        purchaseRate: args.v.purchaseRate.trim() ? Number(args.v.purchaseRate) : undefined,
        manufacturingDate: args.v.manufacturingDate.trim() ? new Date(args.v.manufacturingDate).toISOString() : undefined,
      };
      if (args.editId != null) return updateMedicineBatch(args.editId, body);
      return createMedicineBatch(body);
    },
    onSuccess: (res) => {
      if (!res.success) {
        showToast(res.message ?? 'Save failed', 'error');
        return;
      }
      showToast('Batch saved', 'success');
      setModal(null);
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'batches'] });
    },
    onError: (e) => showToast(getApiErrorMessage(e), 'error'),
  });

  const delMut = useMutation({
    mutationFn: (id: number) => deleteMedicineBatch(id),
    onSuccess: (res) => {
      if (!res.success) {
        showToast(res.message ?? 'Delete failed', 'error');
        return;
      }
      showToast('Deleted', 'success');
      setDeleteId(null);
      setDrawerId(null);
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'batches'] });
    },
    onError: (e) => showToast(getApiErrorMessage(e), 'error'),
  });

  const detailData = (detail.data?.success ? detail.data.data : null) as Row | null;
  const editIdForLookup = modal != null && modal.mode === 'edit' ? modal.id : null;

  return (
    <Stack spacing={2}>
      <PageHeader title="Medicine batches" subtitle="Lot, expiry, and pricing for formulary stock." />

      <FormSection title="Search">
        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} alignItems={{ sm: 'center' }} justifyContent="space-between">
          <TextField size="small" label="Search" value={search} onChange={(e) => setSearch(e.target.value)} sx={{ flex: 1, minWidth: 220 }} />
          <Box sx={{ display: 'flex', justifyContent: 'flex-end' }}>
            <TriVitaButton variant="contained" onClick={() => setModal({ mode: 'create' })}>
              New batch
            </TriVitaButton>
          </Box>
        </Stack>
      </FormSection>

      <FormSection title="Batch register">
        <DataTable<Row>
          tableAriaLabel="Medicine batches"
          columns={[
            {
              id: 'medicine',
              label: 'Medicine',
              format: (r) => medMap.get(Number(r.medicineId ?? r.MedicineId)) ?? '—',
            },
            { id: 'batchNo', label: 'Batch #', format: (r) => pickStr(r, 'batchNo', 'BatchNo') },
            { id: 'expiryDate', label: 'Expiry', format: (r) => (r.expiryDate != null ? String(r.expiryDate).slice(0, 10) : '—') },
            { id: 'mrp', label: 'MRP', align: 'right', format: (r) => (r.mrp != null ? String(r.mrp) : '—') },
            {
              id: '_a',
              label: 'Actions',
              minWidth: 200,
              format: (r) => {
                const id = Number(r.id);
                return (
                  <Stack direction="row" spacing={1} flexWrap="wrap" useFlexGap>
                    <Link component="button" type="button" variant="body2" onClick={() => setDrawerId(id)}>
                      View
                    </Link>
                    <Typography variant="body2" color="text.disabled">
                      |
                    </Typography>
                    <Link component="button" type="button" variant="body2" onClick={() => setModal({ mode: 'edit', id })}>
                      Edit
                    </Link>
                    <Typography variant="body2" color="text.disabled">
                      |
                    </Typography>
                    <Link component="button" type="button" variant="body2" color="error" onClick={() => setDeleteId(id)}>
                      Delete
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
          emptyTitle="No batches"
        />
      </FormSection>

      <DetailDrawer
        open={drawerId != null}
        onClose={() => setDrawerId(null)}
        title={detailData ? pickStr(detailData, 'batchNo', 'BatchNo') : 'Batch'}
        subtitle={detailData ? medMap.get(Number(detailData.medicineId ?? detailData.MedicineId)) : undefined}
      >
        {detailData ? (
          <Stack spacing={1}>
            <DetailKv label="Medicine" value={medMap.get(Number(detailData.medicineId ?? detailData.MedicineId)) ?? ''} />
            <DetailKv label="Batch number" value={pickStr(detailData, 'batchNo', 'BatchNo')} />
            <DetailKv label="Expiry" value={detailData.expiryDate != null ? String(detailData.expiryDate).slice(0, 10) : ''} />
            <DetailKv label="MRP" value={detailData.mrp != null ? String(detailData.mrp) : ''} />
            <DetailKv label="Purchase rate" value={detailData.purchaseRate != null ? String(detailData.purchaseRate) : ''} />
            <DetailKv
              label="Manufacturing date"
              value={detailData.manufacturingDate != null ? String(detailData.manufacturingDate).slice(0, 10) : ''}
            />
          </Stack>
        ) : null}
      </DetailDrawer>

      <TriVitaModal
        open={Boolean(modal)}
        onClose={() => setModal(null)}
        title={modal?.mode === 'edit' ? 'Edit batch' : 'New batch'}
        actions={
          <Stack direction="row" spacing={2} justifyContent="flex-end" sx={{ width: '100%' }}>
            <TriVitaButton onClick={() => setModal(null)}>Cancel</TriVitaButton>
            <TriVitaButton type="submit" form="batch-form" variant="contained" disabled={saveMut.isPending}>
              Save
            </TriVitaButton>
          </Stack>
        }
      >
        <Box
          component="form"
          id="batch-form"
          onSubmit={handleSubmit((v) => saveMut.mutate({ v, editId: modal != null && modal.mode === 'edit' ? modal.id : undefined }))}
          noValidate
        >
          <FormGroup>
            <Grid item xs={12} md={6}>
              <LookupSelect<Record<string, string>>
                name="medicineId"
                control={control as never}
                label="Medicine"
                required
                editId={editIdForLookup}
                queryKey={['pharmacy', 'batch-medicine-opts']}
                loadOptions={async () => {
                  const res = await getMedicinePaged({ page: 1, pageSize: 500 });
                  if (!res.success || !res.data) return [];
                  return (res.data.items as Row[]).map((m) => ({
                    value: String(m.id ?? ''),
                    label: pickStr(m, 'medicineName', 'MedicineName'),
                  }));
                }}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <Controller
                name="batchNo"
                control={control}
                render={({ field }) => (
                  <TriVitaTextField {...field} label="Batch number" required error={Boolean(errors.batchNo)} helperText={errors.batchNo?.message} />
                )}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <Controller
                name="expiryDate"
                control={control}
                render={({ field }) => (
                  <TriVitaTextField {...field} type="date" InputLabelProps={{ shrink: true }} label="Expiry date" error={Boolean(errors.expiryDate)} helperText={errors.expiryDate?.message} />
                )}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <Controller
                name="manufacturingDate"
                control={control}
                render={({ field }) => (
                  <TriVitaTextField {...field} type="date" InputLabelProps={{ shrink: true }} label="Manufacturing date" />
                )}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <Controller name="mrp" control={control} render={({ field }) => <TriVitaTextField {...field} label="MRP" type="number" />} />
            </Grid>
            <Grid item xs={12} md={6}>
              <Controller name="purchaseRate" control={control} render={({ field }) => <TriVitaTextField {...field} label="Purchase rate" type="number" />} />
            </Grid>
          </FormGroup>
        </Box>
      </TriVitaModal>

      <TriVitaModal
        open={deleteId != null}
        onClose={() => setDeleteId(null)}
        title="Delete batch"
        actions={
          <Stack direction="row" spacing={2} justifyContent="flex-end" sx={{ width: '100%' }}>
            <TriVitaButton onClick={() => setDeleteId(null)}>Cancel</TriVitaButton>
            <TriVitaButton color="error" variant="contained" onClick={() => deleteId != null && delMut.mutate(deleteId)} disabled={delMut.isPending}>
              Delete
            </TriVitaButton>
          </Stack>
        }
      >
        <Typography>Delete this batch?</Typography>
      </TriVitaModal>
    </Stack>
  );
}
