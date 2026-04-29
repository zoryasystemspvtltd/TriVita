import { yupResolver } from '@hookform/resolvers/yup';
import { Box, Grid, IconButton, Link, Stack, Table, TableBody, TableCell, TableHead, TableRow, TextField, Typography } from '@mui/material';
import { Add, Delete } from '@mui/icons-material';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useEffect, useMemo, useState } from 'react';
import { Controller, useFieldArray, useForm, useWatch, type Resolver } from 'react-hook-form';
import * as Yup from 'yup';
import { DataTable } from '@/components/common/DataTable';
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
  deleteGoodsReceipt,
  getGoodsReceiptById,
  getGoodsReceiptPaged,
  getMedicinePaged,
  getSupplierPaged,
  getPurchaseOrderById,
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
  mode: 'withPO' | 'withoutPO';
  purchaseOrderId: string;
  supplierId: string;
  receivedOn: string;
  statusReferenceValueId: string;
  discountAmount: string;
  gstPercent: string;
  otherTaxAmount: string;
  notes: string;
  items: {
    id?: number;
    purchaseOrderItemId?: string;
    medicineId: string;
    orderedQuantity?: string;
    quantityReceived: string;
    unitPrice: string;
    batchNo: string;
    expiryDate: string;
    mrp: string;
    notes?: string;
  }[];
};

const hSchema = Yup.object({
  goodsReceiptNo: Yup.string().trim().required().max(80),
  mode: Yup.mixed<'withPO' | 'withoutPO'>().oneOf(['withPO', 'withoutPO']).required(),
  purchaseOrderId: Yup.string()
    .trim()
    .when('mode', {
      is: 'withPO',
      then: (s) => s.required().matches(/^\d+$/),
      otherwise: (s) => s.notRequired(),
    }),
  supplierId: Yup.string()
    .trim()
    .when('mode', {
      is: 'withoutPO',
      then: (s) => s.required().matches(/^\d+$/),
      otherwise: (s) => s.notRequired(),
    }),
  receivedOn: Yup.string().required(),
  statusReferenceValueId: Yup.string().trim().required().matches(/^\d+$/),
  discountAmount: Yup.string().trim().default(''),
  gstPercent: Yup.string().trim().default(''),
  otherTaxAmount: Yup.string().trim().default(''),
  notes: Yup.string().trim().max(2000).default(''),
  items: Yup.array()
    .of(
      Yup.object({
        id: Yup.number().notRequired(),
        purchaseOrderItemId: Yup.string().trim().default(''),
        medicineId: Yup.string().trim().required().matches(/^\d+$/, 'Select medicine'),
        orderedQuantity: Yup.string().trim().default(''),
        quantityReceived: Yup.string().trim().required(),
        unitPrice: Yup.string().trim().required(),
        batchNo: Yup.string().trim().required(),
        expiryDate: Yup.string().trim().required(),
        mrp: Yup.string().trim().default(''),
        notes: Yup.string().trim().default(''),
      })
    )
    .min(1, 'Add at least one item')
    .required(),
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
  const [lineDraft, setLineDraft] = useState({
    purchaseOrderItemId: '',
    medicineId: '',
    quantityReceived: '',
    unitPrice: '',
    batchNo: '',
    expiryDate: '',
    mrp: '',
    notes: '',
  });

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

  const grnStatusOpts = useMemo(() => {
    const allowed = new Set(['Draft', 'Completed']);
    return (statusMap.data ?? []).filter((o) => allowed.has(o.label));
  }, [statusMap.data]);

  const draftGrnStatusId = useMemo(() => grnStatusOpts.find((o) => o.label === 'Draft')?.value ?? '', [grnStatusOpts]);

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

  const supplierList = useQuery({
    queryKey: ['pharmacy', 'grn-supplier-opts'],
    queryFn: async () => {
      const acc: { value: string; label: string; name: string }[] = [];
      const ps = 200;
      let p = 1;
      for (;;) {
        const res = await getSupplierPaged({ page: p, pageSize: ps });
        if (!res.success || !res.data) break;
        const { items, totalCount } = res.data;
        for (const s of items as Row[]) {
          const id = Number(s.id);
          if (!Number.isFinite(id)) continue;
          const name = pickStr(s, 'supplierName', 'SupplierName');
          const code = pickStr(s, 'supplierCode', 'SupplierCode');
          acc.push({ value: String(id), label: code ? `${name} (${code})` : name, name });
        }
        if ((items as Row[]).length === 0) break;
        if (typeof totalCount === 'number' && acc.length >= totalCount) break;
        p += 1;
      }
      return acc;
    },
    staleTime: 120_000,
  });

  const supplierOpts = useMemo(() => {
    return (supplierList.data ?? []).map((r) => ({
      value: r.value,
      label: r.label,
    }));
  }, [supplierList.data]);

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

  const {
    control,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<HForm>({
    resolver: yupResolver(hSchema) as Resolver<HForm>,
    defaultValues: {
      goodsReceiptNo: '',
      mode: 'withPO',
      purchaseOrderId: '',
      supplierId: '',
      receivedOn: new Date().toISOString().slice(0, 10),
      statusReferenceValueId: '',
      discountAmount: '',
      gstPercent: '',
      otherTaxAmount: '',
      notes: '',
      items: [],
    },
  });

  const modeWatch = useWatch({ control, name: 'mode' });
  const poIdWatch = useWatch({ control, name: 'purchaseOrderId' });
  const itemsWatch = useWatch({ control, name: 'items' });

  const { fields: itemFields, append: appendItem, remove: removeItem, update: updateItem, replace: replaceItems } = useFieldArray({
    control,
    name: 'items',
  });

  const poDetail = useQuery({
    queryKey: ['pharmacy', 'grn-po-detail', poIdWatch],
    queryFn: () => getPurchaseOrderById(Number(poIdWatch)),
    enabled: modeWatch === 'withPO' && Boolean(poIdWatch && /^\d+$/.test(poIdWatch)),
    staleTime: 30_000,
  });

  useEffect(() => {
    if (!modal) return;
    if (modal.mode === 'create') {
      reset({
        goodsReceiptNo: '',
        mode: 'withPO',
        purchaseOrderId: '',
        supplierId: '',
        receivedOn: new Date().toISOString().slice(0, 10),
        statusReferenceValueId: draftGrnStatusId || statusMap.data?.[0]?.value || '',
        discountAmount: '',
        gstPercent: '',
        otherTaxAmount: '',
        notes: '',
        items: [],
      });
      replaceItems([]);
      return;
    }
    const d = editSeed.data;
    if (d?.success && d.data) {
      const r = d.data as Row;
      const poIdRaw = r.purchaseOrderId ?? r.PurchaseOrderId;
      const hasPo = poIdRaw != null && String(poIdRaw).trim() !== '' && Number(poIdRaw) > 0;
      reset({
        goodsReceiptNo: pickStr(r, 'goodsReceiptNo', 'GoodsReceiptNo'),
        mode: hasPo ? 'withPO' : 'withoutPO',
        purchaseOrderId: hasPo ? String(r.purchaseOrderId ?? r.PurchaseOrderId ?? '') : '',
        supplierId: hasPo ? String(r.supplierId ?? r.SupplierId ?? '') : String(r.supplierId ?? r.SupplierId ?? ''),
        receivedOn: pickStr(r, 'receivedOn', 'ReceivedOn').slice(0, 10),
        statusReferenceValueId: String(r.statusReferenceValueId ?? '') || draftGrnStatusId,
        discountAmount: r.discountAmount != null ? String(r.discountAmount) : '',
        gstPercent: r.gstPercent != null ? String(r.gstPercent) : '',
        otherTaxAmount: r.otherTaxAmount != null ? String(r.otherTaxAmount) : '',
        notes: pickStr(r, 'notes', 'Notes'),
        items: [],
      });

      const items = (Array.isArray((r as any).items) ? ((r as any).items as Row[]) : []) as Row[];
      replaceItems(
        items.map((x) => ({
          id: x.id != null ? Number(x.id) : undefined,
          purchaseOrderItemId: x.purchaseOrderItemId != null ? String(x.purchaseOrderItemId) : '',
          medicineId: String(x.medicineId ?? ''),
          orderedQuantity: x.orderedQuantity != null ? String(x.orderedQuantity) : '',
          quantityReceived: String(x.quantityReceived ?? ''),
          unitPrice: String(x.unitPrice ?? x.UnitPrice ?? ''),
          batchNo: String(x.batchNo ?? x.BatchNo ?? ''),
          expiryDate: String(x.expiryDate ?? x.ExpiryDate ?? '').slice(0, 10),
          mrp: String(x.mrp ?? x.MRP ?? ''),
          notes: pickStr(x, 'notes', 'Notes'),
        }))
      );
    }
  }, [modal, editSeed.data, reset, statusMap.data, draftGrnStatusId, replaceItems]);

  useEffect(() => {
    if (modeWatch !== 'withPO') return;
    if (!modal) return;
    if (!poIdWatch || !/^\d+$/.test(poIdWatch)) {
      replaceItems([]);
      return;
    }
    const res = poDetail.data;
    if (!res?.success || !res.data) return;
    const po = res.data as Row;
    const poItems = (Array.isArray((po as any).items) ? ((po as any).items as Row[]) : []) as Row[];
    replaceItems(
      poItems.map((it) => ({
        purchaseOrderItemId: String(it.id ?? ''),
        medicineId: String(it.medicineId ?? it.MedicineId ?? ''),
        orderedQuantity: String(it.quantityOrdered ?? it.QuantityOrdered ?? ''),
        quantityReceived: '',
        unitPrice: String(it.unitPrice ?? it.UnitPrice ?? ''),
        batchNo: '',
        expiryDate: '',
        mrp: '',
        notes: '',
      }))
    );
  }, [modeWatch, poIdWatch, poDetail.data, modal, replaceItems]);

  const saveMut = useMutation({
    mutationFn: async (args: { v: HForm; editId?: number }) => {
      const isWithPo = args.v.mode === 'withPO';
      const body = {
        goodsReceiptNo: args.v.goodsReceiptNo.trim(),
        purchaseOrderId: isWithPo && args.v.purchaseOrderId.trim() ? Number(args.v.purchaseOrderId) : null,
        supplierId: !isWithPo && args.v.supplierId.trim() ? Number(args.v.supplierId) : null,
        receivedOn: new Date(args.v.receivedOn).toISOString(),
        statusReferenceValueId: Number(args.v.statusReferenceValueId),
        discountAmount: args.v.discountAmount.trim() ? Number(args.v.discountAmount) : 0,
        gstPercent: args.v.gstPercent.trim() ? Number(args.v.gstPercent) : 0,
        otherTaxAmount: args.v.otherTaxAmount.trim() ? Number(args.v.otherTaxAmount) : 0,
        notes: args.v.notes.trim() || undefined,
        items: (args.v.items ?? []).map((x) => ({
          id: x.id ?? undefined,
          purchaseOrderItemId: isWithPo && x.purchaseOrderItemId?.trim() ? Number(x.purchaseOrderItemId) : null,
          medicineId: Number(x.medicineId),
          quantityReceived: Number(x.quantityReceived),
          unitPrice: Number(x.unitPrice),
          batchNo: x.batchNo.trim(),
          expiryDate: new Date(x.expiryDate).toISOString(),
          mrp: x.mrp.trim() ? Number(x.mrp) : undefined,
          notes: x.notes?.trim() || undefined,
        })),
      };
      if (args.editId != null) return updateGoodsReceipt(args.editId, body);
      return createGoodsReceipt(body);
    },
    onSuccess: (res) => {
      if (!res.success) {
        showToast(res.message ?? 'Save failed', 'error');
        return;
      }
      showToast('Goods receipt saved', 'success');
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'grn'] });
      setModal(null);
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

      <DetailDrawer
        open={drawerRow != null}
        onClose={() => setDrawerRow(null)}
        title={detailData ? pickStr(detailData, 'goodsReceiptNo', 'GoodsReceiptNo') : 'GRN'}
        subtitle={
          detailData
            ? Number(detailData.purchaseOrderId ?? detailData.PurchaseOrderId)
                ? poLabelMap.get(Number(detailData.purchaseOrderId ?? detailData.PurchaseOrderId))
                : detailData.supplierId != null || detailData.SupplierId != null
                  ? `Supplier ${String(detailData.supplierId ?? detailData.SupplierId)}`
                  : undefined
            : undefined
        }
      >
        {detailData ? (
          <Stack spacing={1}>
            <DetailKv label="GRN number" value={pickStr(detailData, 'goodsReceiptNo', 'GoodsReceiptNo')} />
            <DetailKv
              label="Purchase order"
              value={
                detailData.purchaseOrderId != null || detailData.PurchaseOrderId != null
                  ? poLabelMap.get(Number(detailData.purchaseOrderId ?? detailData.PurchaseOrderId)) ?? ''
                  : detailData.supplierId != null
                    ? `Supplier ${String(detailData.supplierId)}`
                    : detailData.SupplierId != null
                      ? `Supplier ${String(detailData.SupplierId)}`
                      : ''
              }
            />
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
                name="mode"
                control={control}
                render={({ field }) => (
                  <TriVitaTextField
                    {...field}
                    select
                    SelectProps={{ native: true }}
                    label="GRN mode"
                    required
                    error={Boolean(errors.mode)}
                    helperText={errors.mode?.message}
                  >
                    <option value="withPO">With PO</option>
                    <option value="withoutPO">Without PO</option>
                  </TriVitaTextField>
                )}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <Controller
                name="goodsReceiptNo"
                control={control}
                render={({ field }) => (
                  <TriVitaTextField {...field} label="GRN number" required error={Boolean(errors.goodsReceiptNo)} helperText={errors.goodsReceiptNo?.message} />
                )}
              />
            </Grid>
            {modeWatch === 'withPO' ? (
              <Grid item xs={12} md={6}>
                <Controller
                  name="purchaseOrderId"
                  control={control}
                  render={({ field }) => (
                    <TriVitaTextField
                      {...field}
                      select
                      SelectProps={{ native: true }}
                      label="Purchase order"
                      required
                      error={Boolean(errors.purchaseOrderId)}
                      helperText={errors.purchaseOrderId?.message}
                    >
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
            ) : (
              <Grid item xs={12} md={6}>
                <Controller
                  name="supplierId"
                  control={control}
                  render={({ field }) => (
                    <TriVitaTextField
                      {...field}
                      select
                      SelectProps={{ native: true }}
                      label="Supplier"
                      required
                      error={Boolean(errors.supplierId)}
                      helperText={errors.supplierId?.message}
                    >
                      <option value="">Select</option>
                      {supplierOpts.map((o) => (
                        <option key={o.value} value={o.value}>
                          {o.label}
                        </option>
                      ))}
                    </TriVitaTextField>
                  )}
                />
              </Grid>
            )}
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
              <Controller
                name="statusReferenceValueId"
                control={control}
                render={({ field }) => (
                  <TriVitaTextField
                    {...field}
                    select
                    SelectProps={{ native: true }}
                    label="Status"
                    required
                    error={Boolean(errors.statusReferenceValueId)}
                    helperText={errors.statusReferenceValueId?.message}
                  >
                    {(grnStatusOpts.length ? grnStatusOpts : [{ value: '', label: 'Draft' }]).map((o) => (
                      <option key={o.value || o.label} value={o.value}>
                        {o.label}
                      </option>
                    ))}
                  </TriVitaTextField>
                )}
              />
            </Grid>
          </FormGroup>

          <Box sx={{ mt: 2 }}>
            <Typography variant="subtitle2" sx={{ mb: 1 }}>
              Items
            </Typography>

            <Table size="small" sx={{ mb: 2 }}>
              <TableHead>
                <TableRow>
                  <TableCell>Medicine</TableCell>
                  <TableCell align="right">Ordered qty</TableCell>
                  <TableCell align="right">Received qty</TableCell>
                  <TableCell>Batch number</TableCell>
                  <TableCell>Expiry date</TableCell>
                  <TableCell align="right">Unit price</TableCell>
                  <TableCell align="right">Actions</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {itemFields.map((f, idx) => {
                  const w = (itemsWatch ?? [])[idx];
                  const isWithPo = modeWatch === 'withPO';
                  return (
                    <TableRow key={f.id} hover>
                      <TableCell>{medMap.get(Number(w?.medicineId)) ?? '—'}</TableCell>
                      <TableCell align="right">{w?.orderedQuantity ?? '—'}</TableCell>
                      <TableCell align="right" sx={{ width: 140 }}>
                        <TriVitaTextField
                          size="small"
                          value={w?.quantityReceived ?? ''}
                          onChange={(e) => updateItem(idx, { ...(w ?? {}), quantityReceived: e.target.value })}
                          sx={{ width: 120 }}
                        />
                      </TableCell>
                      <TableCell sx={{ minWidth: 160 }}>
                        <TriVitaTextField
                          size="small"
                          value={w?.batchNo ?? ''}
                          onChange={(e) => updateItem(idx, { ...(w ?? {}), batchNo: e.target.value })}
                        />
                      </TableCell>
                      <TableCell sx={{ width: 160 }}>
                        <TriVitaTextField
                          size="small"
                          type="date"
                          InputLabelProps={{ shrink: true }}
                          value={(w?.expiryDate ?? '').slice(0, 10)}
                          onChange={(e) => updateItem(idx, { ...(w ?? {}), expiryDate: e.target.value })}
                        />
                      </TableCell>
                      <TableCell align="right" sx={{ width: 160 }}>
                        <TriVitaTextField
                          size="small"
                          value={w?.unitPrice ?? ''}
                          onChange={(e) => updateItem(idx, { ...(w ?? {}), unitPrice: e.target.value })}
                          disabled={isWithPo}
                          sx={{ width: 120 }}
                        />
                      </TableCell>
                      <TableCell align="right">
                        <Stack direction="row" spacing={1} justifyContent="flex-end">
                          <IconButton size="small" onClick={() => removeItem(idx)} disabled={modeWatch === 'withPO'}>
                            <Delete fontSize="small" />
                          </IconButton>
                        </Stack>
                      </TableCell>
                    </TableRow>
                  );
                })}
                {itemFields.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={7}>
                      <Typography variant="body2" color="text.secondary">
                        {modeWatch === 'withPO' ? 'Select a purchase order to load items.' : 'No items yet. Add at least one item.'}
                      </Typography>
                    </TableCell>
                  </TableRow>
                ) : null}
              </TableBody>
            </Table>

            {modeWatch === 'withoutPO' ? (
              <>
                <Typography variant="subtitle2" sx={{ mb: 1 }}>
                  Add item
                </Typography>

                <TriVitaTextField
                  select
                  SelectProps={{ native: true }}
                  label="Medicine"
                  size="small"
                  value={lineDraft.medicineId}
                  onChange={(e) => setLineDraft((d) => ({ ...d, medicineId: e.target.value }))}
                  sx={{ mb: 2 }}
                >
                  <option value="">Select medicine</option>
                  {(medicines.data?.success && medicines.data.data ? (medicines.data.data.items as Row[]) : []).map((m) => (
                    <option key={String(m.id ?? '')} value={String(m.id ?? '')}>
                      {pickStr(m, 'medicineName', 'MedicineName')}
                    </option>
                  ))}
                </TriVitaTextField>

                <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} sx={{ mb: 2 }}>
                  <TriVitaTextField label="Qty received" size="small" value={lineDraft.quantityReceived} onChange={(e) => setLineDraft((d) => ({ ...d, quantityReceived: e.target.value }))} />
                  <TriVitaTextField label="Unit price" size="small" value={lineDraft.unitPrice} onChange={(e) => setLineDraft((d) => ({ ...d, unitPrice: e.target.value }))} />
                  <TriVitaTextField label="Batch number" size="small" value={lineDraft.batchNo} onChange={(e) => setLineDraft((d) => ({ ...d, batchNo: e.target.value }))} sx={{ flex: 1 }} />
                  <TriVitaTextField label="Expiry" type="date" size="small" InputLabelProps={{ shrink: true }} value={lineDraft.expiryDate} onChange={(e) => setLineDraft((d) => ({ ...d, expiryDate: e.target.value }))} />
                  <TriVitaTextField label="MRP" size="small" value={lineDraft.mrp} onChange={(e) => setLineDraft((d) => ({ ...d, mrp: e.target.value }))} />
                </Stack>
                <TriVitaTextField label="Notes" size="small" value={lineDraft.notes} onChange={(e) => setLineDraft((d) => ({ ...d, notes: e.target.value }))} sx={{ mb: 2 }} />

                <Box sx={{ display: 'flex', justifyContent: 'flex-end' }}>
                  <TriVitaButton
                    variant="contained"
                    startIcon={<Add />}
                    disabled={
                      !lineDraft.medicineId ||
                      !lineDraft.quantityReceived.trim() ||
                      Number.isNaN(Number(lineDraft.quantityReceived)) ||
                      Number(lineDraft.quantityReceived) <= 0 ||
                      !lineDraft.unitPrice.trim() ||
                      Number.isNaN(Number(lineDraft.unitPrice)) ||
                      Number(lineDraft.unitPrice) <= 0 ||
                      !lineDraft.batchNo.trim() ||
                      !lineDraft.expiryDate.trim()
                    }
                    onClick={() => {
                      appendItem({
                        medicineId: lineDraft.medicineId,
                        quantityReceived: lineDraft.quantityReceived,
                        unitPrice: lineDraft.unitPrice,
                        batchNo: lineDraft.batchNo,
                        expiryDate: lineDraft.expiryDate,
                        mrp: lineDraft.mrp,
                        notes: lineDraft.notes,
                      });
                      setLineDraft({
                        purchaseOrderItemId: '',
                        medicineId: '',
                        quantityReceived: '',
                        unitPrice: '',
                        batchNo: '',
                        expiryDate: '',
                        mrp: '',
                        notes: '',
                      });
                    }}
                  >
                    Add
                  </TriVitaButton>
                </Box>
              </>
            ) : null}
          </Box>

          <Box sx={{ mt: 2 }}>
            <Typography variant="subtitle2" sx={{ mb: 1 }}>
              Billing
            </Typography>
            <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
              <Controller name="discountAmount" control={control} render={({ field }) => <TriVitaTextField {...field} label="Discount amount" disabled={modeWatch === 'withPO'} />} />
              <Controller name="gstPercent" control={control} render={({ field }) => <TriVitaTextField {...field} label="GST %" disabled={modeWatch === 'withPO'} />} />
              <Controller name="otherTaxAmount" control={control} render={({ field }) => <TriVitaTextField {...field} label="Other tax amount" disabled={modeWatch === 'withPO'} />} />
            </Stack>
          </Box>

          <Box sx={{ mt: 2 }}>
            <Typography variant="subtitle2" sx={{ mb: 1 }}>
              Notes
            </Typography>
            <Controller name="notes" control={control} render={({ field }) => <TriVitaTextField {...field} label="Notes" multiline minRows={2} />} />
          </Box>
        </Box>
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
