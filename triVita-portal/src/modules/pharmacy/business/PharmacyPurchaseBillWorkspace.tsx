import { yupResolver } from '@hookform/resolvers/yup';
import {
  Box,
  Link,
  MenuItem,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  TextField,
  Typography,
} from '@mui/material';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useEffect, useMemo, useState } from 'react';
import { Controller, useFieldArray, useForm, type Resolver } from 'react-hook-form';
import * as Yup from 'yup';
import { DataTable } from '@/components/common/DataTable';
import { TriVitaButton } from '@/components/ds/TriVitaButton';
import { TriVitaModal } from '@/components/ds/TriVitaModal';
import { PageHeader } from '@/components/layout/PageHeader';
import { useToast } from '@/components/toast/ToastProvider';
import {
  createPurchaseBill,
  deletePurchaseBill,
  getGoodsReceiptById,
  getGoodsReceiptForPurchaseBill,
  getMedicineById,
  getPurchaseBillById,
  getPurchaseBillPaged,
  getPurchaseOrderById,
  getPurchaseOrdersPaged,
  postPurchaseBill,
  updatePurchaseBill,
} from '@/services/pharmacyService';
import { getApiErrorMessage } from '@/utils/errorMap';

type Row = Record<string, unknown> & { id?: number };

const SOURCE_PO = 1;
const SOURCE_DIRECT = 2;
const ST_DRAFT = 1;
const ST_POSTED = 2;

function pickStr(r: Record<string, unknown>, ...keys: string[]) {
  for (const k of keys) {
    const v = r[k];
    if (v != null && String(v).trim() !== '') return String(v);
  }
  return '';
}

function pickNum(r: Record<string, unknown>, ...keys: string[]) {
  for (const k of keys) {
    const v = r[k];
    const n = Number(v);
    if (Number.isFinite(n)) return n;
  }
  return 0;
}

type LineForm = {
  goodsReceiptItemId: string;
  medicineId: string;
  batchNo: string;
  quantity: string;
  rate: string;
};

type HForm = {
  mode: 'po' | 'direct';
  purchaseOrderId: string;
  goodsReceiptId: string;
  supplierId: string;
  invoiceNo: string;
  invoiceDate: string;
  discountAmount: string;
  gstPercent: string;
  otherTaxAmount: string;
  notes: string;
  lines: LineForm[];
};

const lineSchema = Yup.object({
  goodsReceiptItemId: Yup.string().required().matches(/^\d+$/),
  medicineId: Yup.string().required(),
  batchNo: Yup.string().default(''),
  quantity: Yup.string()
    .required()
    .test('q', 'Qty required', (v) => Number.isFinite(Number(v)) && Number(v) > 0),
  rate: Yup.string()
    .required()
    .test('r', 'Rate required', (v) => Number.isFinite(Number(v)) && Number(v) >= 0),
});

const hSchema = Yup.object({
  mode: Yup.mixed<'po' | 'direct'>().oneOf(['po', 'direct']).required(),
  purchaseOrderId: Yup.string().when('mode', {
    is: 'po',
    then: (s) => s.trim().required().matches(/^\d+$/),
    otherwise: (s) => s.trim().default(''),
  }),
  goodsReceiptId: Yup.string().trim().required().matches(/^\d+$/),
  supplierId: Yup.string().trim().required().matches(/^\d+$/),
  invoiceNo: Yup.string().trim().required().max(120),
  invoiceDate: Yup.string().required(),
  discountAmount: Yup.string()
    .default('0')
    .test('d', 'Invalid', (v) => Number.isFinite(Number(v ?? 0)) && Number(v ?? 0) >= 0),
  gstPercent: Yup.string()
    .default('0')
    .test('g', 'Invalid', (v) => Number.isFinite(Number(v ?? 0)) && Number(v ?? 0) >= 0),
  otherTaxAmount: Yup.string()
    .default('0')
    .test('o', 'Invalid', (v) => Number.isFinite(Number(v ?? 0)) && Number(v ?? 0) >= 0),
  notes: Yup.string().trim().max(1000).default(''),
  lines: Yup.array().of(lineSchema).min(1),
});

export function PharmacyPurchaseBillWorkspace() {
  const qc = useQueryClient();
  const { showToast } = useToast();
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const [search, setSearch] = useState('');
  const [searchApplied, setSearchApplied] = useState('');
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
    queryKey: ['pharmacy', 'purchase-bill', page, pageSize, searchApplied],
    queryFn: () => getPurchaseBillPaged({ page: page + 1, pageSize, search: searchApplied || undefined }),
  });

  const rows = useMemo(
    () => (list.data?.success && list.data.data ? [...list.data.data.items] : []) as Row[],
    [list.data]
  );
  const total = list.data?.success && list.data.data ? list.data.data.totalCount : 0;

  const poOptions = useQuery({
    queryKey: ['pharmacy', 'pb-pos'],
    queryFn: async () => {
      const acc: { value: string; label: string }[] = [];
      for (let p = 1; p <= 15; p++) {
        const res = await getPurchaseOrdersPaged({ page: p, pageSize: 50 });
        if (!res.success || !res.data) break;
        for (const r of res.data.items as Row[]) {
          const id = pickNum(r, 'id', 'Id');
          if (!id) continue;
          const no = pickStr(r, 'purchaseOrderNo', 'PurchaseOrderNo');
          acc.push({ value: String(id), label: no || `PO #${id}` });
        }
        if (res.data.items.length < 50) break;
      }
      return acc;
    },
    staleTime: 60_000,
  });

  const form = useForm<HForm>({
    resolver: yupResolver(hSchema) as Resolver<HForm>,
    defaultValues: {
      mode: 'direct',
      purchaseOrderId: '',
      goodsReceiptId: '',
      supplierId: '',
      invoiceNo: '',
      invoiceDate: new Date().toISOString().slice(0, 10),
      discountAmount: '0',
      gstPercent: '0',
      otherTaxAmount: '0',
      notes: '',
      lines: [],
    },
  });

  const { fields, replace } = useFieldArray({ control: form.control, name: 'lines' });

  const mode = form.watch('mode');
  const goodsReceiptIdW = form.watch('goodsReceiptId');
  const poIdW = form.watch('purchaseOrderId');

  const grnPickList = useQuery({
    queryKey: ['pharmacy', 'pb-grn-pick', mode, mode === 'po' ? poIdW : 'x'],
    queryFn: async () => {
      const poNum = mode === 'po' ? Number(poIdW) : NaN;
      const res = await getGoodsReceiptForPurchaseBill(
        mode === 'po' && Number.isFinite(poNum) && poNum > 0 ? poNum : undefined
      );
      if (!res.success || !res.data) return [] as Row[];
      return [...res.data] as Row[];
    },
    enabled: modal != null && (mode === 'direct' || (mode === 'po' && /^\d+$/.test(poIdW))),
  });

  const watchedLines = form.watch('lines');

  const medNameForLines = useQuery({
    queryKey: ['pharmacy', 'pb-med', watchedLines.map((l) => l.medicineId).join(',')],
    queryFn: async () => {
      const ids = [...new Set(watchedLines.map((l) => l.medicineId).filter(Boolean))].map((x) => Number(x));
      const m = new Map<number, string>();
      for (const id of ids) {
        if (!id) continue;
        const r = await getMedicineById(id);
        if (r.success && r.data) m.set(id, pickStr(r.data as Row, 'medicineName', 'MedicineName'));
      }
      return m;
    },
    enabled: modal != null && watchedLines.length > 0,
  });

  useEffect(() => {
    if (modal?.mode !== 'create' || mode !== 'po') return;
    form.setValue('goodsReceiptId', '');
    replace([]);
  }, [mode, poIdW, modal?.mode, form, replace]);

  useEffect(() => {
    if (modal?.mode !== 'create' || mode !== 'po') return;
    const pid = Number(poIdW);
    if (!Number.isFinite(pid) || pid <= 0) return;
    void (async () => {
      const r = await getPurchaseOrderById(pid);
      if (!r.success || !r.data) return;
      const p = r.data as Row;
      form.setValue('discountAmount', String(pickNum(p, 'discountAmount', 'DiscountAmount')));
      form.setValue('gstPercent', String(pickNum(p, 'gstPercent', 'GstPercent')));
      form.setValue('otherTaxAmount', String(pickNum(p, 'otherTaxAmount', 'OtherTaxAmount')));
    })();
    // eslint-disable-next-line react-hooks/exhaustive-deps -- sync billing when PO changes
  }, [poIdW, mode, modal?.mode]);

  const loadGrnIntoForm = async (grnId: number) => {
    const gr = await getGoodsReceiptById(grnId);
    if (!gr.success || !gr.data) {
      showToast('Could not load GRN.', 'error');
      return;
    }
    const g = gr.data as Row;
    const sup = pickNum(g, 'supplierId', 'SupplierId');
    form.setValue('supplierId', sup ? String(sup) : '');
    const currentMode = form.getValues('mode');
    if (currentMode === 'po') {
      const pid = Number(form.getValues('purchaseOrderId'));
      if (Number.isFinite(pid) && pid > 0) {
        const pr = await getPurchaseOrderById(pid);
        if (pr.success && pr.data) {
          const p = pr.data as Row;
          form.setValue('discountAmount', String(pickNum(p, 'discountAmount', 'DiscountAmount')));
          form.setValue('gstPercent', String(pickNum(p, 'gstPercent', 'GstPercent')));
          form.setValue('otherTaxAmount', String(pickNum(p, 'otherTaxAmount', 'OtherTaxAmount')));
        }
      }
    } else {
      form.setValue('discountAmount', String(pickNum(g, 'discountAmount', 'DiscountAmount')));
      form.setValue('gstPercent', String(pickNum(g, 'gstPercent', 'GstPercent')));
      form.setValue('otherTaxAmount', String(pickNum(g, 'otherTaxAmount', 'OtherTaxAmount')));
    }
    const items = (g.items ?? g.Items) as Row[] | undefined;
    if (!items?.length) {
      replace([]);
      showToast('GRN has no lines.', 'error');
      return;
    }
    const lines: LineForm[] = items.map((it) => {
      const id = pickNum(it, 'id', 'Id');
      const mid = pickNum(it, 'medicineId', 'MedicineId');
      const qty = pickNum(it, 'quantityReceived', 'QuantityReceived');
      const rate = pickNum(it, 'unitPrice', 'UnitPrice');
      return {
        goodsReceiptItemId: String(id),
        medicineId: String(mid),
        batchNo: pickStr(it, 'batchNo', 'BatchNo'),
        quantity: String(qty || 0),
        rate: String(rate || 0),
      };
    });
    replace(lines);
  };

  useEffect(() => {
    if (!modal || modal.mode !== 'create') return;
    const id = Number(goodsReceiptIdW);
    if (!Number.isFinite(id) || id <= 0) return;
    void loadGrnIntoForm(id);
    // eslint-disable-next-line react-hooks/exhaustive-deps -- load when user selects GRN or switches direct/PO
  }, [goodsReceiptIdW, modal, mode]);

  const openCreate = () => {
    form.reset({
      mode: 'direct',
      purchaseOrderId: '',
      goodsReceiptId: '',
      supplierId: '',
      invoiceNo: '',
      invoiceDate: new Date().toISOString().slice(0, 10),
      discountAmount: '0',
      gstPercent: '0',
      otherTaxAmount: '0',
      notes: '',
      lines: [],
    });
    setModal({ mode: 'create' });
  };

  const openEdit = async (id: number) => {
    const r = await getPurchaseBillById(id);
    if (!r.success || !r.data) {
      showToast(getApiErrorMessage(r) ?? 'Load failed', 'error');
      return;
    }
    const b = r.data as Row;
    const poId = pickNum(b, 'purchaseOrderId', 'PurchaseOrderId');
    const sm = pickNum(b, 'sourceMode', 'SourceMode');
    form.reset({
      mode: sm === SOURCE_PO || poId ? 'po' : 'direct',
      purchaseOrderId: poId ? String(poId) : '',
      goodsReceiptId: String(pickNum(b, 'goodsReceiptId', 'GoodsReceiptId')),
      supplierId: String(pickNum(b, 'supplierId', 'SupplierId')),
      invoiceNo: pickStr(b, 'invoiceNo', 'InvoiceNo'),
      invoiceDate: pickStr(b, 'invoiceDate', 'InvoiceDate').slice(0, 10),
      discountAmount: String(pickNum(b, 'discountAmount', 'DiscountAmount')),
      gstPercent: String(pickNum(b, 'gstPercent', 'GstPercent')),
      otherTaxAmount: String(pickNum(b, 'otherTaxAmount', 'OtherTaxAmount')),
      notes: pickStr(b, 'notes', 'Notes'),
      lines: [],
    });
    const items = (b.items ?? b.Items) as Row[] | undefined;
    if (items?.length) {
      replace(
        items.map((it) => ({
          goodsReceiptItemId: String(pickNum(it, 'goodsReceiptItemId', 'GoodsReceiptItemId')),
          medicineId: String(pickNum(it, 'medicineId', 'MedicineId')),
          batchNo: pickStr(it, 'batchNo', 'BatchNo'),
          quantity: String(pickNum(it, 'quantity', 'Quantity')),
          rate: String(pickNum(it, 'rate', 'Rate')),
        }))
      );
    }
    if ((sm === SOURCE_PO || poId) && poId) {
      const pr = await getPurchaseOrderById(poId);
      if (pr.success && pr.data) {
        const p = pr.data as Row;
        form.setValue('discountAmount', String(pickNum(p, 'discountAmount', 'DiscountAmount')));
        form.setValue('gstPercent', String(pickNum(p, 'gstPercent', 'GstPercent')));
        form.setValue('otherTaxAmount', String(pickNum(p, 'otherTaxAmount', 'OtherTaxAmount')));
      }
    }
    setModal({ mode: 'edit', id });
  };

  const saveMut = useMutation({
    mutationFn: async (v: HForm) => {
      const lines = v.lines.map((l) => ({
        goodsReceiptItemId: Number(l.goodsReceiptItemId),
        quantity: Number(l.quantity),
        rate: Number(l.rate),
      }));
      const body = {
        invoiceNo: v.invoiceNo.trim(),
        invoiceDate: new Date(v.invoiceDate).toISOString(),
        discountAmount: Number(v.discountAmount || 0),
        gstPercent: Number(v.gstPercent || 0),
        otherTaxAmount: Number(v.otherTaxAmount || 0),
        notes: v.notes.trim() || null,
        items: lines,
      };
      if (modal?.mode === 'create') {
        return createPurchaseBill({
          sourceMode: v.mode === 'po' ? SOURCE_PO : SOURCE_DIRECT,
          purchaseOrderId: v.mode === 'po' ? Number(v.purchaseOrderId) : null,
          goodsReceiptId: Number(v.goodsReceiptId),
          supplierId: Number(v.supplierId),
          ...body,
        });
      }
      if (modal?.mode === 'edit') return updatePurchaseBill(modal.id, body);
      throw new Error('No modal');
    },
    onSuccess: (res) => {
      if (!res.success) {
        showToast(getApiErrorMessage(res) ?? 'Save failed', 'error');
        return;
      }
      showToast('Saved.', 'success');
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'purchase-bill'] });
      setModal(null);
    },
    onError: (e) => showToast(getApiErrorMessage(e) ?? 'Save failed', 'error'),
  });

  const postMut = useMutation({
    mutationFn: async (id: number) => postPurchaseBill(id),
    onSuccess: (res, id) => {
      if (!res.success) {
        showToast(getApiErrorMessage(res) ?? 'Post failed', 'error');
        return;
      }
      showToast('Posted.', 'success');
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'purchase-bill'] });
      if (modal?.mode === 'edit' && modal.id === id) setModal(null);
    },
    onError: (e) => showToast(getApiErrorMessage(e) ?? 'Post failed', 'error'),
  });

  const delMut = useMutation({
    mutationFn: async (id: number) => deletePurchaseBill(id),
    onSuccess: (res) => {
      if (!res.success) {
        showToast(getApiErrorMessage(res) ?? 'Delete failed', 'error');
        return;
      }
      showToast('Deleted.', 'success');
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'purchase-bill'] });
      setDeleteId(null);
    },
    onError: (e) => showToast(getApiErrorMessage(e) ?? 'Delete failed', 'error'),
  });

  const onSubmit = form.handleSubmit((v) => saveMut.mutate(v));

  const statusLabel = (s: number) => (s === ST_POSTED ? 'Posted' : 'Draft');

  const preview = useMemo(() => {
    const lines = watchedLines;
    const d = Number(form.watch('discountAmount') || 0);
    const g = Number(form.watch('gstPercent') || 0);
    const o = Number(form.watch('otherTaxAmount') || 0);
    let sub = 0;
    for (const l of lines) {
      const q = Number(l.quantity);
      const r = Number(l.rate);
      if (Number.isFinite(q) && Number.isFinite(r)) sub += Math.round(q * r * 10000) / 10000;
    }
    const taxable = Math.max(0, sub - d);
    const gst = Math.round(taxable * (g / 100) * 10000) / 10000;
    const total = Math.round((sub - d + gst + o) * 10000) / 10000;
    return { sub, gst, other: o, total };
  }, [watchedLines, form.watch('discountAmount'), form.watch('gstPercent'), form.watch('otherTaxAmount')]);

  const editId = modal?.mode === 'edit' ? modal.id : null;
  const billDetail = useQuery({
    queryKey: ['pharmacy', 'purchase-bill', editId],
    queryFn: () => getPurchaseBillById(editId!),
    enabled: editId != null,
  });
  const posted =
    billDetail.data?.success && billDetail.data.data
      ? pickNum(billDetail.data.data as Row, 'status', 'Status') === ST_POSTED
      : false;

  const billingTermsEditable = mode === 'direct' && !posted;

  return (
    <Box sx={{ p: 2 }}>
      <PageHeader title="Purchase Bill" subtitle="Financial document from GRN (no stock impact)." />

      <Stack direction="row" spacing={1} sx={{ mb: 2 }} alignItems="center">
        <TextField
          size="small"
          label="Search"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          sx={{ minWidth: 220 }}
        />
        <TriVitaButton variant="contained" onClick={openCreate}>
          New bill
        </TriVitaButton>
      </Stack>

      <Box
        sx={{
          '& .MuiTableRow-root:nth-of-type(even)': { bgcolor: 'action.hover' },
        }}
      >
        <DataTable<Row>
          tableAriaLabel="Purchase bills"
          loading={list.isFetching}
          columns={[
            {
              id: 'bill',
              label: 'Bill No',
              minWidth: 120,
              format: (r) => pickStr(r, 'billNo', 'BillNo'),
            },
            {
              id: 'inv',
              label: 'Invoice',
              minWidth: 120,
              format: (r) => pickStr(r, 'invoiceNo', 'InvoiceNo'),
            },
            {
              id: 'st',
              label: 'Status',
              minWidth: 90,
              format: (r) => statusLabel(pickNum(r, 'status', 'Status')),
            },
            {
              id: 'net',
              label: 'Total',
              minWidth: 100,
              align: 'right',
              format: (r) => String(pickNum(r, 'netAmount', 'NetAmount')),
            },
            {
              id: 'grn',
              label: 'GRN',
              minWidth: 72,
              format: (r) => String(pickNum(r, 'goodsReceiptId', 'GoodsReceiptId')),
            },
            {
              id: '_a',
              label: 'Actions',
              minWidth: 200,
              format: (r) => {
                const id = pickNum(r, 'id', 'Id');
                const st = pickNum(r, 'status', 'Status');
                return (
                  <Stack direction="row" spacing={1} alignItems="center" flexWrap="wrap" useFlexGap>
                    <Link component="button" type="button" variant="body2" onClick={() => void openEdit(id)}>
                      Open
                    </Link>
                    {st === ST_DRAFT ? (
                      <>
                        <Link component="button" type="button" variant="body2" onClick={() => postMut.mutate(id)}>
                          Post
                        </Link>
                        <Link component="button" type="button" variant="body2" color="error" onClick={() => setDeleteId(id)}>
                          Delete
                        </Link>
                      </>
                    ) : null}
                  </Stack>
                );
              },
            },
          ]}
          rows={rows}
          rowKey={(r) => String(pickNum(r, 'id', 'Id'))}
          totalCount={total}
          page={page}
          pageSize={pageSize}
          onPageChange={(p, ps) => {
            setPage(p);
            setPageSize(ps);
          }}
        />
      </Box>

      <TriVitaModal
        open={modal != null}
        onClose={() => setModal(null)}
        title={modal?.mode === 'create' ? 'New purchase bill' : 'Edit purchase bill'}
        maxWidth="md"
        fullWidth
        actions={
          <Stack direction="row" spacing={1} flexWrap="wrap" useFlexGap>
            <TriVitaButton onClick={() => setModal(null)}>Close</TriVitaButton>
            {modal && !posted && (
              <TriVitaButton variant="contained" onClick={() => void onSubmit()} disabled={saveMut.isPending}>
                Save draft
              </TriVitaButton>
            )}
            {modal?.mode === 'edit' && !posted && (
              <TriVitaButton variant="contained" color="secondary" onClick={() => postMut.mutate(modal.id)}>
                Post
              </TriVitaButton>
            )}
          </Stack>
        }
      >
        {modal && (
          <Stack spacing={2} sx={{ mt: 1 }}>
            {modal.mode === 'create' && (
              <>
                <Controller
                  name="mode"
                  control={form.control}
                  render={({ field }) => (
                    <TextField select label="Mode" fullWidth {...field}>
                      <MenuItem value="po">PO-linked (GRN from purchase order)</MenuItem>
                      <MenuItem value="direct">Direct GRN (no PO)</MenuItem>
                    </TextField>
                  )}
                />
                {mode === 'po' && (
                  <Controller
                    name="purchaseOrderId"
                    control={form.control}
                    render={({ field, fieldState }) => (
                      <TextField
                        select
                        label="Purchase order"
                        fullWidth
                        {...field}
                        error={!!fieldState.error}
                        helperText={fieldState.error?.message}
                        disabled={poOptions.isLoading}
                      >
                        {(poOptions.data ?? []).map((o) => (
                          <MenuItem key={o.value} value={o.value}>
                            {o.label}
                          </MenuItem>
                        ))}
                      </TextField>
                    )}
                  />
                )}
                <Controller
                  name="goodsReceiptId"
                  control={form.control}
                  render={({ field, fieldState }) => (
                    <TextField
                      select
                      label="Goods receipt (GRN)"
                      fullWidth
                      {...field}
                      error={!!fieldState.error}
                      helperText={fieldState.error?.message}
                      disabled={grnPickList.isLoading}
                    >
                      {(grnPickList.data ?? []).map((g) => (
                        <MenuItem key={pickNum(g, 'id', 'Id')} value={String(pickNum(g, 'id', 'Id'))}>
                          {pickStr(g, 'goodsReceiptNo', 'GoodsReceiptNo')} —{' '}
                          {pickStr(g, 'receivedOn', 'ReceivedOn').slice(0, 10)}
                        </MenuItem>
                      ))}
                    </TextField>
                  )}
                />
              </>
            )}
            {modal.mode === 'edit' && (
              <Typography variant="body2" color="text.secondary">
                GRN and mode are fixed for this bill. Edit invoice details and line quantities/rates only.
                {mode === 'po' ? ' Billing terms follow the purchase order (read-only).' : ''}
              </Typography>
            )}
            <Controller
              name="supplierId"
              control={form.control}
              render={({ field, fieldState }) => (
                <TextField
                  label="Supplier ID"
                  fullWidth
                  {...field}
                  disabled
                  error={!!fieldState.error}
                  helperText={fieldState.error?.message ?? 'From goods receipt'}
                />
              )}
            />
            <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
              <Controller
                name="invoiceNo"
                control={form.control}
                render={({ field, fieldState }) => (
                  <TextField
                    label="Invoice no"
                    fullWidth
                    {...field}
                    disabled={posted}
                    error={!!fieldState.error}
                    helperText={fieldState.error?.message}
                  />
                )}
              />
              <Controller
                name="invoiceDate"
                control={form.control}
                render={({ field, fieldState }) => (
                  <TextField
                    label="Invoice date"
                    type="date"
                    fullWidth
                    InputLabelProps={{ shrink: true }}
                    {...field}
                    disabled={posted}
                    error={!!fieldState.error}
                    helperText={fieldState.error?.message}
                  />
                )}
              />
            </Stack>
            <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
              <Controller
                name="discountAmount"
                control={form.control}
                render={({ field, fieldState }) => (
                  <TextField
                    label="Discount"
                    fullWidth
                    {...field}
                    disabled={!billingTermsEditable}
                    error={!!fieldState.error}
                    helperText={
                      billingTermsEditable ? undefined : mode === 'po' ? 'From purchase order' : 'Posted'
                    }
                  />
                )}
              />
              <Controller
                name="gstPercent"
                control={form.control}
                render={({ field, fieldState }) => (
                  <TextField
                    label="GST %"
                    fullWidth
                    {...field}
                    disabled={!billingTermsEditable}
                    error={!!fieldState.error}
                    helperText={
                      billingTermsEditable ? undefined : mode === 'po' ? 'From purchase order' : 'Posted'
                    }
                  />
                )}
              />
              <Controller
                name="otherTaxAmount"
                control={form.control}
                render={({ field, fieldState }) => (
                  <TextField
                    label="Other tax"
                    fullWidth
                    {...field}
                    disabled={!billingTermsEditable}
                    error={!!fieldState.error}
                    helperText={
                      billingTermsEditable
                        ? 'Editable for direct GRN bills'
                        : mode === 'po'
                          ? 'From purchase order'
                          : 'Posted'
                    }
                  />
                )}
              />
            </Stack>
            <Controller
              name="notes"
              control={form.control}
              render={({ field }) => <TextField label="Notes" fullWidth multiline minRows={2} {...field} disabled={posted} />}
            />

            <Typography variant="subtitle2">Lines (from GRN)</Typography>
            <Table size="small">
              <TableHead>
                <TableRow>
                  <TableCell>Medicine</TableCell>
                  <TableCell>Batch</TableCell>
                  <TableCell align="right">Qty</TableCell>
                  <TableCell align="right">Rate</TableCell>
                  <TableCell align="right">Amount</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {fields.map((f, idx) => {
                  const mid = Number(form.watch(`lines.${idx}.medicineId`));
                  const mname = medNameForLines.data?.get(mid) ?? `Medicine #${mid}`;
                  const q = Number(form.watch(`lines.${idx}.quantity`));
                  const rt = Number(form.watch(`lines.${idx}.rate`));
                  const amt = Number.isFinite(q) && Number.isFinite(rt) ? Math.round(q * rt * 10000) / 10000 : 0;
                  return (
                    <TableRow key={f.id}>
                      <TableCell>{mname}</TableCell>
                      <TableCell>{form.watch(`lines.${idx}.batchNo`)}</TableCell>
                      <TableCell align="right">
                        <Controller
                          name={`lines.${idx}.quantity`}
                          control={form.control}
                          render={({ field: lf }) => (
                            <TextField size="small" type="number" inputProps={{ step: 'any' }} {...lf} disabled={posted} />
                          )}
                        />
                      </TableCell>
                      <TableCell align="right">
                        <Controller
                          name={`lines.${idx}.rate`}
                          control={form.control}
                          render={({ field: lf }) => (
                            <TextField size="small" type="number" inputProps={{ step: 'any' }} {...lf} disabled={posted} />
                          )}
                        />
                      </TableCell>
                      <TableCell align="right">{amt}</TableCell>
                    </TableRow>
                  );
                })}
              </TableBody>
            </Table>

            <Box>
              <Typography variant="subtitle2" sx={{ mb: 0.5 }}>
                Totals
              </Typography>
              <Typography variant="body2">Subtotal: {preview.sub.toFixed(4)}</Typography>
              <Typography variant="body2">GST: {preview.gst.toFixed(4)}</Typography>
              <Typography variant="body2">Other tax: {preview.other.toFixed(4)}</Typography>
              <Typography variant="body2">Total: {preview.total.toFixed(4)}</Typography>
            </Box>
          </Stack>
        )}
      </TriVitaModal>

      <TriVitaModal
        open={deleteId != null}
        onClose={() => setDeleteId(null)}
        title="Delete draft bill?"
        actions={
          <>
            <TriVitaButton onClick={() => setDeleteId(null)}>Cancel</TriVitaButton>
            <TriVitaButton color="error" variant="contained" onClick={() => deleteId && delMut.mutate(deleteId)}>
              Delete
            </TriVitaButton>
          </>
        }
      >
        <Typography>This removes the draft purchase bill only. Stock is unchanged.</Typography>
      </TriVitaModal>
    </Box>
  );
}
