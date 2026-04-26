import { yupResolver } from '@hookform/resolvers/yup';
import { Box, Grid, IconButton, Link, Stack, Table, TableBody, TableCell, TableHead, TableRow, TextField, Typography } from '@mui/material';
import { Add, Delete } from '@mui/icons-material';
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
  createGoodsReceipt,
  createGoodsReceiptItem,
  createMedicineBatch,
  deleteGoodsReceipt,
  deleteGoodsReceiptItem,
  getGoodsReceiptById,
  getGoodsReceiptItemsPaged,
  getGoodsReceiptPaged,
  getMedicineBatchPaged,
  getMedicinePaged,
  getPurchaseOrderItemsPaged,
  getPurchaseOrdersPaged,
  updateGoodsReceipt,
} from '@/services/pharmacyService';
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
  goodsReceiptNo: string;
  purchaseOrderId: string;
  receivedOn: string;
  statusReferenceValueId: string;
  notes: string;
};

const hSchema = Yup.object({
  goodsReceiptNo: Yup.string().trim().required().max(80),
  purchaseOrderId: Yup.string().trim().required().matches(/^\d+$/),
  receivedOn: Yup.string().required(),
  statusReferenceValueId: Yup.string().trim().required().matches(/^\d+$/),
  notes: Yup.string().trim().max(2000).default(''),
});

export function PharmacyGoodsReceiptWorkspace() {
  const qc = useQueryClient();
  const { showToast } = useToast();
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const [search, setSearch] = useState('');
  const [searchApplied, setSearchApplied] = useState('');
  const [drawerRow, setDrawerRow] = useState<Row | null>(null);
  const [modal, setModal] = useState<null | { mode: 'create' } | { mode: 'edit'; id: number }>(null);
  const [deleteId, setDeleteId] = useState<number | null>(null);
  const [lineCtx, setLineCtx] = useState<null | { grId: number; poId: number }>(null);
  const [lineDraft, setLineDraft] = useState({
    purchaseOrderItemId: '',
    medicineBatchId: '',
    quantityReceived: '',
    purchaseRate: '',
    expiryDate: '',
    mrp: '',
  });
  const [batchMini, setBatchMini] = useState<null | { medicineId: number }>(null);
  const [batchForm, setBatchForm] = useState({ batchNo: '', expiryDate: '', mrp: '', purchaseRate: '' });

  useEffect(() => {
    const t = window.setTimeout(() => setSearchApplied(search), 400);
    return () => window.clearTimeout(t);
  }, [search]);

  const list = useQuery({
    queryKey: ['pharmacy', 'grn', page, pageSize, searchApplied],
    queryFn: () => getGoodsReceiptPaged({ page: page + 1, pageSize, search: searchApplied || undefined }),
  });

  const statusMap = useQuery({
    queryKey: ['pharmacy', 'status-opts'],
    queryFn: buildPharmacyReferenceStatusOptions,
    staleTime: 120_000,
  });

  const sm = useMemo(() => {
    const m = new Map<string, string>();
    for (const o of statusMap.data ?? []) m.set(o.value, o.label);
    return m;
  }, [statusMap.data]);

  const rows = useMemo(
    () => (list.data?.success && list.data.data ? [...list.data.data.items] : []) as Row[],
    [list.data]
  );
  const total = list.data?.success && list.data.data ? list.data.data.totalCount : 0;

  const poList = useQuery({
    queryKey: ['pharmacy', 'grn-po-pick'],
    queryFn: () => getPurchaseOrdersPaged({ page: 1, pageSize: 200 }),
    staleTime: 60_000,
  });

  const poOpts = useMemo(() => {
    if (!poList.data?.success || !poList.data.data) return [];
    return (poList.data.data.items as Row[]).map((r) => ({
      value: String(r.id ?? ''),
      label: `${pickStr(r, 'purchaseOrderNo', 'PurchaseOrderNo')} — ${pickStr(r, 'supplierName', 'SupplierName')}`,
    }));
  }, [poList.data]);

  const detail = useQuery({
    queryKey: ['pharmacy', 'grn-detail', drawerRow?.id],
    queryFn: () => getGoodsReceiptById(Number(drawerRow!.id)),
    enabled: Boolean(drawerRow?.id),
  });

  const editSeed = useQuery({
    queryKey: ['pharmacy', 'grn-edit', modal != null && modal.mode === 'edit' ? modal.id : null],
    queryFn: () => getGoodsReceiptById((modal as { mode: 'edit'; id: number }).id),
    enabled: modal != null && modal.mode === 'edit',
  });

  const poItems = useQuery({
    queryKey: ['pharmacy', 'grn-po-items', lineCtx?.poId],
    queryFn: () => getPurchaseOrderItemsPaged({ page: 1, pageSize: 500 }),
    enabled: lineCtx != null,
  });

  const grItems = useQuery({
    queryKey: ['pharmacy', 'grn-items', lineCtx?.grId],
    queryFn: () => getGoodsReceiptItemsPaged({ page: 1, pageSize: 500 }),
    enabled: lineCtx != null,
  });

  const medicines = useQuery({
    queryKey: ['pharmacy', 'grn-medicines'],
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
    queryKey: ['pharmacy', 'grn-batches', lineDraft.purchaseOrderItemId],
    queryFn: () => getMedicineBatchPaged({ page: 1, pageSize: 500 }),
    enabled: lineCtx != null,
  });

  const poItemRows = useMemo(() => {
    if (!poItems.data?.success || !poItems.data.data || lineCtx == null) return [];
    return (poItems.data.data.items as Row[]).filter((x) => Number(x.purchaseOrderId ?? x.PurchaseOrderId) === lineCtx.poId);
  }, [poItems.data, lineCtx]);

  const grLineRows = useMemo(() => {
    if (!grItems.data?.success || !grItems.data.data || lineCtx == null) return [];
    return (grItems.data.data.items as Row[]).filter((x) => Number(x.goodsReceiptId ?? x.GoodsReceiptId) === lineCtx.grId);
  }, [grItems.data, lineCtx]);

  const poLineLabelByItemId = useMemo(() => {
    const m = new Map<number, string>();
    for (const it of poItemRows) {
      const id = Number(it.id);
      if (!Number.isFinite(id)) continue;
      const ln = it.lineNum ?? it.LineNum;
      m.set(id, `PO line ${String(ln ?? '—')}`);
    }
    return m;
  }, [poItemRows]);

  const {
    control,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<HForm>({
    resolver: yupResolver(hSchema) as Resolver<HForm>,
    defaultValues: {
      goodsReceiptNo: '',
      purchaseOrderId: '',
      receivedOn: new Date().toISOString().slice(0, 10),
      statusReferenceValueId: '',
      notes: '',
    },
  });

  useEffect(() => {
    if (!modal) return;
    if (modal.mode === 'create') {
      reset({
        goodsReceiptNo: '',
        purchaseOrderId: '',
        receivedOn: new Date().toISOString().slice(0, 10),
        statusReferenceValueId: statusMap.data?.[0]?.value ?? '',
        notes: '',
      });
      return;
    }
    const d = editSeed.data;
    if (d?.success && d.data) {
      const r = d.data as Row;
      reset({
        goodsReceiptNo: pickStr(r, 'goodsReceiptNo', 'GoodsReceiptNo'),
        purchaseOrderId: String(r.purchaseOrderId ?? r.PurchaseOrderId ?? ''),
        receivedOn: pickStr(r, 'receivedOn', 'ReceivedOn').slice(0, 10),
        statusReferenceValueId: String(r.statusReferenceValueId ?? ''),
        notes: pickStr(r, 'notes', 'Notes'),
      });
    }
  }, [modal, editSeed.data, reset, statusMap.data]);

  const saveMut = useMutation({
    mutationFn: async (args: { v: HForm; editId?: number }) => {
      const body = {
        goodsReceiptNo: args.v.goodsReceiptNo.trim(),
        purchaseOrderId: Number(args.v.purchaseOrderId),
        receivedOn: new Date(args.v.receivedOn).toISOString(),
        statusReferenceValueId: Number(args.v.statusReferenceValueId),
        notes: args.v.notes.trim() || undefined,
      };
      if (args.editId != null) return updateGoodsReceipt(args.editId, body);
      return createGoodsReceipt(body);
    },
    onSuccess: (res, vars) => {
      if (!res.success) {
        showToast(res.message ?? 'Save failed', 'error');
        return;
      }
      showToast('Goods receipt saved', 'success');
      const created = res.data as Row | undefined;
      const newId = vars.editId ?? (created?.id != null ? Number(created.id) : NaN);
      const poId = Number(vars.v.purchaseOrderId);
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'grn'] });
      setModal(null);
      if (Number.isFinite(newId) && Number.isFinite(poId)) {
        setLineCtx({ grId: newId, poId });
      }
    },
    onError: (e) => showToast(getApiErrorMessage(e), 'error'),
  });

  const delMut = useMutation({
    mutationFn: (id: number) => deleteGoodsReceipt(id),
    onSuccess: (res) => {
      if (!res.success) {
        showToast(res.message ?? 'Delete failed', 'error');
        return;
      }
      showToast('Deleted', 'success');
      setDeleteId(null);
      setDrawerRow(null);
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'grn'] });
    },
    onError: (e) => showToast(getApiErrorMessage(e), 'error'),
  });

  const createBatchMut = useMutation({
    mutationFn: async () => {
      if (batchMini == null) throw new Error('No medicine');
      return createMedicineBatch({
        medicineId: batchMini.medicineId,
        batchNo: batchForm.batchNo.trim(),
        expiryDate: batchForm.expiryDate.trim() ? new Date(batchForm.expiryDate).toISOString() : undefined,
        mrp: batchForm.mrp.trim() ? Number(batchForm.mrp) : undefined,
        purchaseRate: batchForm.purchaseRate.trim() ? Number(batchForm.purchaseRate) : undefined,
      });
    },
    onSuccess: (res) => {
      if (!res.success) {
        showToast(res.message ?? 'Batch create failed', 'error');
        return;
      }
      const id = (res.data as Row | undefined)?.id;
      if (id != null) setLineDraft((d) => ({ ...d, medicineBatchId: String(id) }));
      showToast('Batch created', 'success');
      setBatchMini(null);
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'grn-batches'] });
    },
    onError: (e) => showToast(getApiErrorMessage(e), 'error'),
  });

  const addLineMut = useMutation({
    mutationFn: async () => {
      if (lineCtx == null) throw new Error('No ctx');
      const poItem = poItemRows.find((x) => String(x.id) === lineDraft.purchaseOrderItemId);
      if (!poItem) throw new Error('PO item');
      const nextLine = grLineRows.length === 0 ? 1 : Math.max(...grLineRows.map((r) => Number(r.lineNum ?? 0))) + 1;
      return createGoodsReceiptItem({
        goodsReceiptId: lineCtx.grId,
        purchaseOrderItemId: Number(lineDraft.purchaseOrderItemId),
        lineNum: nextLine,
        medicineId: Number(poItem.medicineId ?? poItem.MedicineId),
        medicineBatchId: Number(lineDraft.medicineBatchId),
        quantityReceived: Number(lineDraft.quantityReceived),
        purchaseRate: lineDraft.purchaseRate.trim() ? Number(lineDraft.purchaseRate) : undefined,
        expiryDate: lineDraft.expiryDate.trim() ? new Date(lineDraft.expiryDate).toISOString() : undefined,
        mrp: lineDraft.mrp.trim() ? Number(lineDraft.mrp) : undefined,
      });
    },
    onSuccess: (res) => {
      if (!res.success) {
        showToast(res.message ?? 'Line failed', 'error');
        return;
      }
      showToast('Receipt line saved', 'success');
      setLineDraft({
        purchaseOrderItemId: '',
        medicineBatchId: '',
        quantityReceived: '',
        purchaseRate: '',
        expiryDate: '',
        mrp: '',
      });
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'grn-items'] });
    },
    onError: (e) => showToast(getApiErrorMessage(e), 'error'),
  });

  const delLineMut = useMutation({
    mutationFn: (id: number) => deleteGoodsReceiptItem(id),
    onSuccess: (res) => {
      if (!res.success) {
        showToast(res.message ?? 'Delete failed', 'error');
        return;
      }
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'grn-items'] });
    },
    onError: (e) => showToast(getApiErrorMessage(e), 'error'),
  });

  const batchOptions = useMemo(() => {
    if (!batches.data?.success || !batches.data.data) return [];
    const sel = poItemRows.find((x) => String(x.id) === lineDraft.purchaseOrderItemId);
    const mid = sel ? Number(sel.medicineId ?? sel.MedicineId) : NaN;
    return (batches.data.data.items as Row[])
      .filter((b) => !Number.isFinite(mid) || Number(b.medicineId ?? b.MedicineId) === mid)
      .map((b) => ({
        value: String(b.id ?? ''),
        label: `${pickStr(b, 'batchNo', 'BatchNo')} · exp ${pickStr(b, 'expiryDate', 'ExpiryDate').slice(0, 10)}`,
      }));
  }, [batches.data, lineDraft.purchaseOrderItemId, poItemRows]);

  const detailData = (detail.data?.success ? detail.data.data : null) as Row | null;

  const poLabelMap = useMemo(() => {
    const m = new Map<number, string>();
    if (poList.data?.success && poList.data.data) {
      for (const r of poList.data.data.items as Row[]) {
        const id = Number(r.id);
        if (Number.isFinite(id)) {
          m.set(id, `${pickStr(r, 'purchaseOrderNo', 'PurchaseOrderNo')} — ${pickStr(r, 'supplierName', 'SupplierName')}`);
        }
      }
    }
    return m;
  }, [poList.data]);

  return (
    <Stack spacing={2}>
      <PageHeader title="Goods receipt (GRN)" />

      <FormSection title="Search">
        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} alignItems={{ sm: 'center' }} justifyContent="space-between">
          <TextField size="small" label="Search" value={search} onChange={(e) => setSearch(e.target.value)} sx={{ flex: 1, minWidth: 220 }} />
          <Box sx={{ display: 'flex', justifyContent: 'flex-end' }}>
            <TriVitaButton variant="contained" onClick={() => setModal({ mode: 'create' })}>
              New GRN
            </TriVitaButton>
          </Box>
        </Stack>
      </FormSection>

      <FormSection title="Goods receipts">
        <DataTable<Row>
          tableAriaLabel="GRN"
          columns={[
            { id: 'goodsReceiptNo', label: 'GRN #', format: (r) => pickStr(r, 'goodsReceiptNo', 'GoodsReceiptNo') },
            {
              id: 'purchaseOrderId',
              label: 'Purchase order',
              format: (r) => poLabelMap.get(Number(r.purchaseOrderId ?? r.PurchaseOrderId)) ?? '—',
            },
            { id: 'receivedOn', label: 'Received', format: (r) => (r.receivedOn != null ? String(r.receivedOn).slice(0, 10) : '—') },
            {
              id: 'statusReferenceValueId',
              label: 'Status',
              format: (r) => sm.get(String(r.statusReferenceValueId ?? '')) ?? '—',
            },
            {
              id: '_a',
              label: 'Actions',
              minWidth: 220,
              format: (r) => {
                const id = Number(r.id);
                const poId = Number(r.purchaseOrderId ?? r.PurchaseOrderId);
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
                        if (Number.isFinite(poId)) setLineCtx({ grId: id, poId });
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
                    <Link
                      component="button"
                      type="button"
                      variant="body2"
                      onClick={() => {
                        if (Number.isFinite(poId)) setLineCtx({ grId: id, poId });
                      }}
                    >
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
          emptyTitle="No goods receipts"
        />
      </FormSection>

      {lineCtx != null ? (
        <FormSection
          title="Receipt lines"
          action={<TriVitaButton onClick={() => setLineCtx(null)}>Close</TriVitaButton>}
        >
          <Table size="small" sx={{ mb: 2 }}>
            <TableHead>
              <TableRow>
                <TableCell>PO line</TableCell>
                <TableCell>Medicine</TableCell>
                <TableCell align="right">Qty received</TableCell>
                <TableCell align="right">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {grLineRows.map((lr) => (
                <TableRow key={String(lr.id)} hover>
                  <TableCell>
                  {poLineLabelByItemId.get(Number(lr.purchaseOrderItemId ?? lr.PurchaseOrderItemId)) ?? '—'}
                </TableCell>
                  <TableCell>{medMap.get(Number(lr.medicineId ?? lr.MedicineId)) ?? '—'}</TableCell>
                  <TableCell align="right">{String(lr.quantityReceived ?? '')}</TableCell>
                  <TableCell align="right">
                    <IconButton size="small" onClick={() => lr.id != null && delLineMut.mutate(Number(lr.id))}>
                      <Delete fontSize="small" />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
          <Typography variant="subtitle2" sx={{ mb: 1 }}>
            Add line from PO
          </Typography>
          <Stack spacing={2}>
            <TriVitaTextField
              select
              SelectProps={{ native: true }}
              label="PO line"
              size="small"
              value={lineDraft.purchaseOrderItemId}
              onChange={(e) => setLineDraft((d) => ({ ...d, purchaseOrderItemId: e.target.value, medicineBatchId: '' }))}
            >
              <option value="">Select line</option>
              {poItemRows.map((it) => (
                <option key={String(it.id)} value={String(it.id)}>
                  #{String(it.lineNum ?? '')} — medicine {medMap.get(Number(it.medicineId ?? it.MedicineId)) ?? '—'}
                </option>
              ))}
            </TriVitaTextField>
            <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} alignItems={{ sm: 'center' }}>
              <TriVitaTextField
                select
                SelectProps={{ native: true }}
                label="Batch"
                size="small"
                value={lineDraft.medicineBatchId}
                onChange={(e) => setLineDraft((d) => ({ ...d, medicineBatchId: e.target.value }))}
                sx={{ flex: 1 }}
              >
                <option value="">Select batch</option>
                {batchOptions.map((o) => (
                  <option key={o.value} value={o.value}>
                    {o.label}
                  </option>
                ))}
              </TriVitaTextField>
              <TriVitaButton
                variant="outlined"
                onClick={() => {
                  const it = poItemRows.find((x) => String(x.id) === lineDraft.purchaseOrderItemId);
                  if (it) setBatchMini({ medicineId: Number(it.medicineId ?? it.MedicineId) });
                }}
                disabled={!lineDraft.purchaseOrderItemId}
              >
                New batch
              </TriVitaButton>
            </Stack>
            <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
              <TriVitaTextField
                label="Qty received"
                size="small"
                value={lineDraft.quantityReceived}
                onChange={(e) => setLineDraft((d) => ({ ...d, quantityReceived: e.target.value }))}
              />
              <TriVitaTextField
                label="Purchase rate"
                size="small"
                value={lineDraft.purchaseRate}
                onChange={(e) => setLineDraft((d) => ({ ...d, purchaseRate: e.target.value }))}
              />
              <TriVitaTextField
                label="Expiry"
                type="date"
                size="small"
                InputLabelProps={{ shrink: true }}
                value={lineDraft.expiryDate}
                onChange={(e) => setLineDraft((d) => ({ ...d, expiryDate: e.target.value }))}
              />
              <TriVitaTextField label="MRP" size="small" value={lineDraft.mrp} onChange={(e) => setLineDraft((d) => ({ ...d, mrp: e.target.value }))} />
            </Stack>
            <Box sx={{ display: 'flex', justifyContent: 'flex-end' }}>
              <TriVitaButton
                variant="contained"
                startIcon={<Add />}
                disabled={
                  addLineMut.isPending ||
                  !lineDraft.purchaseOrderItemId ||
                  !lineDraft.medicineBatchId ||
                  !lineDraft.quantityReceived.trim()
                }
                onClick={() => addLineMut.mutate()}
              >
                Add line
              </TriVitaButton>
            </Box>
          </Stack>
        </FormSection>
      ) : null}

      <DetailDrawer
        open={drawerRow != null}
        onClose={() => setDrawerRow(null)}
        title={detailData ? pickStr(detailData, 'goodsReceiptNo', 'GoodsReceiptNo') : 'GRN'}
        subtitle={detailData ? poLabelMap.get(Number(detailData.purchaseOrderId ?? detailData.PurchaseOrderId)) : undefined}
      >
        {detailData ? (
          <Stack spacing={1}>
            <DetailKv label="GRN number" value={pickStr(detailData, 'goodsReceiptNo', 'GoodsReceiptNo')} />
            <DetailKv label="Purchase order" value={poLabelMap.get(Number(detailData.purchaseOrderId ?? detailData.PurchaseOrderId)) ?? ''} />
            <DetailKv label="Received on" value={detailData.receivedOn != null ? String(detailData.receivedOn).slice(0, 10) : ''} />
            <DetailKv label="Status" value={sm.get(String(detailData.statusReferenceValueId ?? '')) ?? ''} />
            <DetailKv label="Notes" value={pickStr(detailData, 'notes', 'Notes')} />
          </Stack>
        ) : null}
      </DetailDrawer>

      <TriVitaModal
        open={Boolean(modal)}
        onClose={() => setModal(null)}
        title={modal?.mode === 'edit' ? 'Edit GRN' : 'New GRN'}
        actions={
          <Stack direction="row" spacing={2} justifyContent="flex-end" sx={{ width: '100%' }}>
            <TriVitaButton onClick={() => setModal(null)}>Cancel</TriVitaButton>
            <TriVitaButton type="submit" form="grn-header" variant="contained" disabled={saveMut.isPending}>
              Save
            </TriVitaButton>
          </Stack>
        }
      >
        <Box
          component="form"
          id="grn-header"
          noValidate
          onSubmit={handleSubmit((v) => saveMut.mutate({ v, editId: modal != null && modal.mode === 'edit' ? modal.id : undefined }))}
        >
          <FormGroup>
            <Grid item xs={12} md={6}>
              <Controller
                name="goodsReceiptNo"
                control={control}
                render={({ field }) => (
                  <TriVitaTextField {...field} label="GRN number" required error={Boolean(errors.goodsReceiptNo)} helperText={errors.goodsReceiptNo?.message} />
                )}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <Controller
                name="purchaseOrderId"
                control={control}
                render={({ field }) => (
                  <TriVitaTextField {...field} select SelectProps={{ native: true }} label="Purchase order" required error={Boolean(errors.purchaseOrderId)} helperText={errors.purchaseOrderId?.message}>
                    <option value="">Select</option>
                    {poOpts.map((o) => (
                      <option key={o.value} value={o.value}>
                        {o.label}
                      </option>
                    ))}
                  </TriVitaTextField>
                )}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <Controller
                name="receivedOn"
                control={control}
                render={({ field }) => (
                  <TriVitaTextField {...field} type="date" InputLabelProps={{ shrink: true }} label="Received on" required error={Boolean(errors.receivedOn)} helperText={errors.receivedOn?.message} />
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
                queryKey={['pharmacy', 'grn-status']}
                loadOptions={async () => {
                  const o = await buildPharmacyReferenceStatusOptions();
                  return o.length ? o : [{ value: '1', label: 'Default' }];
                }}
              />
            </Grid>
            <Grid item xs={12}>
              <Controller
                name="notes"
                control={control}
                render={({ field }) => <TriVitaTextField {...field} label="Notes" multiline minRows={2} />}
              />
            </Grid>
          </FormGroup>
        </Box>
      </TriVitaModal>

      <TriVitaModal open={batchMini != null} onClose={() => setBatchMini(null)} title="New batch" actions={
        <Stack direction="row" spacing={2} justifyContent="flex-end" sx={{ width: '100%' }}>
          <TriVitaButton onClick={() => setBatchMini(null)}>Cancel</TriVitaButton>
          <TriVitaButton variant="contained" disabled={createBatchMut.isPending || !batchForm.batchNo.trim()} onClick={() => createBatchMut.mutate()}>
            Create
          </TriVitaButton>
        </Stack>
      }>
        <Stack spacing={2}>
          <TriVitaTextField label="Batch number" required value={batchForm.batchNo} onChange={(e) => setBatchForm((b) => ({ ...b, batchNo: e.target.value }))} />
          <TriVitaTextField label="Expiry" type="date" InputLabelProps={{ shrink: true }} value={batchForm.expiryDate} onChange={(e) => setBatchForm((b) => ({ ...b, expiryDate: e.target.value }))} />
          <TriVitaTextField label="MRP" value={batchForm.mrp} onChange={(e) => setBatchForm((b) => ({ ...b, mrp: e.target.value }))} />
          <TriVitaTextField label="Purchase rate" value={batchForm.purchaseRate} onChange={(e) => setBatchForm((b) => ({ ...b, purchaseRate: e.target.value }))} />
        </Stack>
      </TriVitaModal>

      <TriVitaModal open={deleteId != null} onClose={() => setDeleteId(null)} title="Delete GRN" actions={
        <Stack direction="row" spacing={2} justifyContent="flex-end" sx={{ width: '100%' }}>
          <TriVitaButton onClick={() => setDeleteId(null)}>Cancel</TriVitaButton>
          <TriVitaButton color="error" variant="contained" onClick={() => deleteId != null && delMut.mutate(deleteId)} disabled={delMut.isPending}>
            Delete
          </TriVitaButton>
        </Stack>
      }>
        <Typography>Delete this goods receipt?</Typography>
      </TriVitaModal>
    </Stack>
  );
}
