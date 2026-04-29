import { yupResolver } from '@hookform/resolvers/yup';
import {
  Box,
  Grid,
  IconButton,
  Link,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  TextField,
  Typography,
} from '@mui/material';
import { Add, Delete } from '@mui/icons-material';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useEffect, useMemo, useState } from 'react';
import { Controller, useForm, useWatch, type Resolver } from 'react-hook-form';
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
  createPurchaseOrder,
  createPurchaseOrderItem,
  deletePurchaseOrder,
  deletePurchaseOrderItem,
  getMedicinePaged,
  getSupplierPaged,
  getPurchaseOrderById,
  getPurchaseOrderItemsPaged,
  getPurchaseOrdersPaged,
  updatePurchaseOrder,
  updatePurchaseOrderItem,
} from '@/services/pharmacyService';
import { buildPharmacyReferenceStatusOptions } from '@/utils/pharmacyStatusOptions';
import { getApiErrorMessage } from '@/utils/errorMap';

type Row = Record<string, unknown> & { id?: number };

type HeaderForm = {
  purchaseOrderNo: string;
  supplierId: string;
  orderDate: string;
  expectedOn: string;
  statusReferenceValueId: string;
  discountAmount: string;
  gstPercent: string;
  otherTaxAmount: string;
  notes: string;
};

const headerSchema = Yup.object({
  purchaseOrderNo: Yup.string().trim().max(80),
  supplierId: Yup.string().trim().required().matches(/^\d+$/, 'Select a supplier'),
  orderDate: Yup.string().required(),
  expectedOn: Yup.string().default(''),
  statusReferenceValueId: Yup.string().trim().required().matches(/^\d+$/),
  discountAmount: Yup.string().trim().default(''),
  gstPercent: Yup.string().trim().default(''),
  otherTaxAmount: Yup.string().trim().default(''),
  notes: Yup.string().trim().max(2000).default(''),
});

type LineForm = { medicineId: string; quantityOrdered: string; unitPrice: string; notes: string };

function pickStr(r: Record<string, unknown>, ...keys: string[]) {
  for (const k of keys) {
    const v = r[k];
    if (v != null && String(v).trim() !== '') return String(v);
  }
  return '';
}

export function PharmacyPurchaseOrderWorkspace() {
  const qc = useQueryClient();
  const { showToast } = useToast();
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const [search, setSearch] = useState('');
  const [searchApplied, setSearchApplied] = useState('');
  const [drawerRow, setDrawerRow] = useState<Row | null>(null);
  const [modal, setModal] = useState<null | { mode: 'create' } | { mode: 'edit'; id: number }>(null);
  const [deleteId, setDeleteId] = useState<number | null>(null);
  const [linePoId, setLinePoId] = useState<number | null>(null);
  const [lineRows, setLineRows] = useState<
    { id?: number; medicineId: string; quantityOrdered: string; unitPrice: string; lineTotal: string; notes: string; lineNum: number }[]
  >([]);
  const [editLine, setEditLine] = useState<null | { id: number; medicineId: string; quantityOrdered: string; unitPrice: string; notes: string; lineNum: number }>(null);

  useEffect(() => {
    const t = window.setTimeout(() => setSearchApplied(search), 400);
    return () => window.clearTimeout(t);
  }, [search]);

  const list = useQuery({
    queryKey: ['pharmacy', 'po', page, pageSize, searchApplied],
    queryFn: () => getPurchaseOrdersPaged({ page: page + 1, pageSize, search: searchApplied || undefined }),
  });

  const statusOpts = useQuery({
    queryKey: ['pharmacy', 'status-opts'],
    queryFn: buildPharmacyReferenceStatusOptions,
    staleTime: 120_000,
  });

  const detail = useQuery({
    queryKey: ['pharmacy', 'po-detail', drawerRow?.id],
    queryFn: () => getPurchaseOrderById(Number(drawerRow!.id)),
    enabled: Boolean(drawerRow?.id),
  });

  const editSeed = useQuery({
    queryKey: ['pharmacy', 'po-edit', modal != null && modal.mode === 'edit' ? modal.id : null],
    queryFn: () => getPurchaseOrderById((modal as { mode: 'edit'; id: number }).id),
    enabled: modal != null && modal.mode === 'edit',
  });

  const itemsForLines = useQuery({
    queryKey: ['pharmacy', 'po-items', linePoId],
    queryFn: () => getPurchaseOrderItemsPaged({ page: 1, pageSize: 500 }),
    enabled: linePoId != null,
  });

  const rows = useMemo(
    () => (list.data?.success && list.data.data ? [...list.data.data.items] : []) as Row[],
    [list.data]
  );
  const total = list.data?.success && list.data.data ? list.data.data.totalCount : 0;

  const medicineNameMap = useQuery({
    queryKey: ['pharmacy', 'medicine-names'],
    queryFn: () => getMedicinePaged({ page: 1, pageSize: 500 }),
    staleTime: 120_000,
  });

  const medMap = useMemo(() => {
    const m = new Map<number, string>();
    const d = medicineNameMap.data?.success ? medicineNameMap.data.data : null;
    for (const x of (d?.items ?? []) as Row[]) {
      const id = Number(x.id);
      if (Number.isFinite(id)) m.set(id, pickStr(x, 'medicineName', 'MedicineName'));
    }
    return m;
  }, [medicineNameMap.data]);

  const statusMap = useMemo(() => {
    const m = new Map<string, string>();
    for (const o of statusOpts.data ?? []) m.set(o.value, o.label);
    return m;
  }, [statusOpts.data]);

  const {
    control,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<HeaderForm>({
    resolver: yupResolver(headerSchema) as Resolver<HeaderForm>,
    defaultValues: {
      purchaseOrderNo: '',
      supplierId: '',
      orderDate: new Date().toISOString().slice(0, 10),
      expectedOn: '',
      statusReferenceValueId: '',
      discountAmount: '',
      gstPercent: '',
      otherTaxAmount: '',
      notes: '',
    },
  });

  const discountWatch = useWatch({ control, name: 'discountAmount' });
  const gstPercentWatch = useWatch({ control, name: 'gstPercent' });
  const otherTaxWatch = useWatch({ control, name: 'otherTaxAmount' });

  const billing = useMemo(() => {
    const subTotal = lineRows.reduce((acc, r) => acc + (Number(r.lineTotal) || 0), 0);
    const discount = Number(discountWatch) || 0;
    const gstPercent = Number(gstPercentWatch) || 0;
    const otherTax = Number(otherTaxWatch) || 0;
    const taxable = Math.max(0, subTotal - discount);
    const gstAmount = (taxable * gstPercent) / 100;
    const total = subTotal - discount + gstAmount + otherTax;
    return { subTotal, discount, gstPercent, gstAmount, otherTax, total };
  }, [lineRows, discountWatch, gstPercentWatch, otherTaxWatch]);

  const supplierOptions = useQuery({
    queryKey: ['pharmacy', 'supplier', 'opts'],
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

  useEffect(() => {
    if (!modal) return;
    if (modal.mode === 'create') {
      reset({
        purchaseOrderNo: '',
        supplierId: '',
        orderDate: new Date().toISOString().slice(0, 10),
        expectedOn: '',
        statusReferenceValueId: statusOpts.data?.[0]?.value ?? '',
        discountAmount: '',
        gstPercent: '',
        otherTaxAmount: '',
        notes: '',
      });
      return;
    }
    const d = editSeed.data;
    if (d?.success && d.data) {
      const r = d.data as Row;
      const supplierName = pickStr(r, 'supplierName', 'SupplierName').trim().toLowerCase();
      const sup = supplierOptions.data?.find((x) => x.name.trim().toLowerCase() === supplierName) ?? null;
      reset({
        purchaseOrderNo: pickStr(r, 'purchaseOrderNo', 'PurchaseOrderNo'),
        supplierId: sup?.value ?? '',
        orderDate: pickStr(r, 'orderDate', 'OrderDate').slice(0, 10),
        expectedOn: r.expectedOn != null ? String(r.expectedOn).slice(0, 10) : '',
        statusReferenceValueId: String(r.statusReferenceValueId ?? ''),
        discountAmount: r.discountAmount != null ? String(r.discountAmount) : '',
        gstPercent: r.gstPercent != null ? String(r.gstPercent) : '',
        otherTaxAmount: r.otherTaxAmount != null ? String(r.otherTaxAmount) : '',
        notes: pickStr(r, 'notes', 'Notes'),
      });
    }
  }, [modal, editSeed.data, reset, statusOpts.data, supplierOptions.data]);

  useEffect(() => {
    if (linePoId == null || !itemsForLines.data?.success || !itemsForLines.data.data) {
      setLineRows([]);
      return;
    }
    const all = [...itemsForLines.data.data.items] as Row[];
    const mine = all.filter((x) => Number(x.purchaseOrderId ?? x.PurchaseOrderId) === linePoId);
    setLineRows(
      mine.map((x, i) => ({
        id: Number(x.id),
        medicineId: String(x.medicineId ?? ''),
        quantityOrdered: String(x.quantityOrdered ?? ''),
        unitPrice: String(x.unitPrice ?? ''),
        lineTotal: String(x.lineTotal ?? ''),
        notes: pickStr(x, 'notes', 'Notes'),
        lineNum: Number(x.lineNum ?? i + 1),
      }))
    );
  }, [linePoId, itemsForLines.data]);

  const saveHeader = useMutation({
    mutationFn: async (args: { v: HeaderForm; editId?: number }) => {
      const sup = supplierOptions.data?.find((o) => o.value === args.v.supplierId) ?? null;
      const body = {
        purchaseOrderNo: args.v.purchaseOrderNo.trim(), // can be blank -> server generates PO/YYYY/XXXX
        supplierName: (sup?.name ?? '').trim(),
        orderDate: new Date(args.v.orderDate).toISOString(),
        expectedOn: args.v.expectedOn.trim() ? new Date(args.v.expectedOn).toISOString() : undefined,
        statusReferenceValueId: Number(args.v.statusReferenceValueId),
        discountAmount: args.v.discountAmount.trim() ? Number(args.v.discountAmount) : 0,
        gstPercent: args.v.gstPercent.trim() ? Number(args.v.gstPercent) : 0,
        otherTaxAmount: args.v.otherTaxAmount.trim() ? Number(args.v.otherTaxAmount) : 0,
        notes: args.v.notes.trim() || undefined,
      };
      if (args.editId != null) return updatePurchaseOrder(args.editId, body);
      return createPurchaseOrder(body);
    },
    onSuccess: (res, vars) => {
      if (!res.success) {
        showToast(res.message ?? 'Save failed', 'error');
        return;
      }
      showToast('Purchase order saved', 'success');
      const created = res.data as Row | undefined;
      const newId =
        vars.editId ?? (created != null && created.id != null ? Number(created.id) : NaN);
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'po'] });
      setModal(null);
      if (Number.isFinite(newId)) {
        setLinePoId(newId);
      }
    },
    onError: (e) => showToast(getApiErrorMessage(e), 'error'),
  });

  const delMut = useMutation({
    mutationFn: (id: number) => deletePurchaseOrder(id),
    onSuccess: (res) => {
      if (!res.success) {
        showToast(res.message ?? 'Delete failed', 'error');
        return;
      }
      showToast('Deleted', 'success');
      setDeleteId(null);
      setDrawerRow(null);
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'po'] });
    },
    onError: (e) => showToast(getApiErrorMessage(e), 'error'),
  });

  const [lineDraft, setLineDraft] = useState<LineForm>({
    medicineId: '',
    quantityOrdered: '',
    unitPrice: '',
    notes: '',
  });

  const addLineMut = useMutation({
    mutationFn: async () => {
      if (linePoId == null) throw new Error('No PO');
      const nextLine =
        lineRows.length === 0 ? 1 : Math.max(...lineRows.map((r) => r.lineNum)) + 1;
      const body = {
        purchaseOrderId: linePoId,
        lineNum: nextLine,
        medicineId: Number(lineDraft.medicineId),
        quantityOrdered: Number(lineDraft.quantityOrdered),
        unitPrice: Number(lineDraft.unitPrice),
        notes: lineDraft.notes.trim() || undefined,
      };
      return createPurchaseOrderItem(body);
    },
    onSuccess: (res) => {
      if (!res.success) {
        showToast(res.message ?? 'Line add failed', 'error');
        return;
      }
      showToast('Line added', 'success');
      setLineDraft({ medicineId: '', quantityOrdered: '', unitPrice: '', notes: '' });
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'po-items'] });
    },
    onError: (e) => showToast(getApiErrorMessage(e), 'error'),
  });

  const delLineMut = useMutation({
    mutationFn: (id: number) => deletePurchaseOrderItem(id),
    onSuccess: (res) => {
      if (!res.success) {
        showToast(res.message ?? 'Delete failed', 'error');
        return;
      }
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'po-items'] });
    },
    onError: (e) => showToast(getApiErrorMessage(e), 'error'),
  });

  const updLineMut = useMutation({
    mutationFn: async (args: { id: number; row: typeof editLine }) => {
      if (args.row == null || linePoId == null) throw new Error('No row');
      return updatePurchaseOrderItem(args.id, {
        purchaseOrderId: linePoId,
        lineNum: args.row.lineNum,
        medicineId: Number(args.row.medicineId),
        quantityOrdered: Number(args.row.quantityOrdered),
        unitPrice: Number(args.row.unitPrice),
        notes: args.row.notes.trim() || undefined,
      });
    },
    onSuccess: (res) => {
      if (!res.success) {
        showToast(res.message ?? 'Update failed', 'error');
        return;
      }
      showToast('Line updated', 'success');
      setEditLine(null);
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'po-items'] });
    },
    onError: (e) => showToast(getApiErrorMessage(e), 'error'),
  });

  const onHeaderSave = (v: HeaderForm) => {
    const eid = modal != null && modal.mode === 'edit' ? modal.id : undefined;
    saveHeader.mutate({ v, editId: eid });
  };

  const detailData = (detail.data?.success ? detail.data.data : null) as Row | null;

  return (
    <Stack spacing={2}>
      <PageHeader title="Purchase orders" />

      <FormSection title="Search" subtitle="Filter by PO number or supplier name.">
        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} alignItems={{ sm: 'center' }} justifyContent="space-between">
          <TextField
            size="small"
            label="Search"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            sx={{ flex: 1, minWidth: 220, maxWidth: { sm: 480 } }}
          />
          <Box sx={{ display: 'flex', justifyContent: 'flex-end', gap: 1 }}>
            <TriVitaButton variant="contained" onClick={() => setModal({ mode: 'create' })}>
              New purchase order
            </TriVitaButton>
          </Box>
        </Stack>
      </FormSection>

      <FormSection title="Purchase order register">
        <DataTable<Row>
          tableAriaLabel="Purchase orders"
          columns={[
            {
              id: 'purchaseOrderNo',
              label: 'PO number',
              minWidth: 140,
              format: (r) => pickStr(r, 'purchaseOrderNo', 'PurchaseOrderNo') || '—',
            },
            {
              id: 'supplierName',
              label: 'Supplier',
              minWidth: 180,
              format: (r) => pickStr(r, 'supplierName', 'SupplierName'),
            },
            {
              id: 'orderDate',
              label: 'Order date',
              minWidth: 120,
              format: (r) => (r.orderDate != null ? String(r.orderDate).slice(0, 10) : '—'),
            },
            {
              id: 'statusReferenceValueId',
              label: 'Status',
              minWidth: 120,
              format: (r) => statusMap.get(String(r.statusReferenceValueId ?? '')) ?? '—',
            },
            {
              id: '_actions',
              label: 'Actions',
              minWidth: 220,
              format: (r) => {
                const id = Number(r.id);
                return (
                  <Stack direction="row" spacing={1} alignItems="center" flexWrap="wrap" useFlexGap>
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
                        setLinePoId(id);
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
                        setLinePoId(id);
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
          emptyTitle="No purchase orders"
        />
      </FormSection>

      {linePoId != null ? (
        <FormSection
          title="Line items"
          subtitle="Add medicines, quantities, and purchase rates for the selected PO."
          action={
            <TriVitaButton size="small" variant="outlined" onClick={() => setLinePoId(null)}>
              Close lines
            </TriVitaButton>
          }
        >
          <Table size="small" sx={{ mb: 2 }}>
            <TableHead>
              <TableRow>
                <TableCell>Medicine</TableCell>
                <TableCell align="right">Qty</TableCell>
                <TableCell align="right">Unit price</TableCell>
                <TableCell align="right">Line total</TableCell>
                <TableCell>Notes</TableCell>
                <TableCell align="right">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {lineRows.map((lr) => (
                <TableRow key={lr.id ?? `${lr.lineNum}`} hover>
                  <TableCell>{medMap.get(Number(lr.medicineId)) ?? '—'}</TableCell>
                  <TableCell align="right">{lr.quantityOrdered}</TableCell>
                  <TableCell align="right">{lr.unitPrice}</TableCell>
                  <TableCell align="right">{lr.lineTotal}</TableCell>
                  <TableCell>{lr.notes}</TableCell>
                  <TableCell align="right">
                    {lr.id != null ? (
                      <Stack direction="row" spacing={1} justifyContent="flex-end">
                        <IconButton size="small" aria-label="edit line" onClick={() => setEditLine({ id: lr.id!, medicineId: lr.medicineId, quantityOrdered: lr.quantityOrdered, unitPrice: lr.unitPrice, notes: lr.notes, lineNum: lr.lineNum })}>
                          <Add fontSize="small" />
                        </IconButton>
                        <IconButton
                          size="small"
                          aria-label="delete line"
                          onClick={() => delLineMut.mutate(lr.id!)}
                          disabled={delLineMut.isPending}
                        >
                          <Delete fontSize="small" />
                        </IconButton>
                      </Stack>
                    ) : null}
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
          <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} sx={{ mb: 2 }} justifyContent="flex-end">
            <Typography variant="body2">Subtotal: {billing.subTotal.toFixed(2)}</Typography>
            <Typography variant="body2">GST: {billing.gstAmount.toFixed(2)}</Typography>
            <Typography variant="body2">Total: {billing.total.toFixed(2)}</Typography>
          </Stack>
          <Typography variant="subtitle2" sx={{ mb: 1 }}>
            Add line
          </Typography>
          <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} sx={{ mt: 1 }} alignItems={{ sm: 'flex-end' }}>
            <TriVitaTextField
              label="Medicine"
              size="small"
              select
              SelectProps={{ native: true }}
              value={lineDraft.medicineId}
              onChange={(e) => setLineDraft((d) => ({ ...d, medicineId: e.target.value }))}
              sx={{ minWidth: 220, flex: 1 }}
            >
              <option value="">Select medicine</option>
              {[...medMap.entries()]
                .sort((a, b) => a[1].localeCompare(b[1]))
                .map(([id, name]) => (
                  <option key={id} value={String(id)}>
                    {name}
                  </option>
                ))}
            </TriVitaTextField>
            <TriVitaTextField
              label="Quantity"
              size="small"
              value={lineDraft.quantityOrdered}
              onChange={(e) => setLineDraft((d) => ({ ...d, quantityOrdered: e.target.value }))}
              sx={{ width: 120 }}
            />
            <TriVitaTextField
              label="Unit price"
              size="small"
              value={lineDraft.unitPrice}
              onChange={(e) => setLineDraft((d) => ({ ...d, unitPrice: e.target.value }))}
              sx={{ width: 140 }}
            />
            <TriVitaTextField
              label="Notes"
              size="small"
              value={lineDraft.notes}
              onChange={(e) => setLineDraft((d) => ({ ...d, notes: e.target.value }))}
              sx={{ flex: 1, minWidth: 160 }}
            />
            <TriVitaButton
              variant="contained"
              startIcon={<Add />}
              disabled={
                addLineMut.isPending ||
                !lineDraft.medicineId ||
                !lineDraft.quantityOrdered.trim() ||
                Number.isNaN(Number(lineDraft.quantityOrdered)) ||
                !lineDraft.unitPrice.trim() ||
                Number.isNaN(Number(lineDraft.unitPrice))
              }
              onClick={() => addLineMut.mutate()}
            >
              Add line
            </TriVitaButton>
          </Stack>
        </FormSection>
      ) : null}

      <DetailDrawer
        open={drawerRow != null}
        onClose={() => setDrawerRow(null)}
        title={detailData ? pickStr(detailData, 'purchaseOrderNo', 'PurchaseOrderNo') : 'Purchase order'}
        subtitle={detailData ? pickStr(detailData, 'supplierName', 'SupplierName') : undefined}
      >
        {detail.isLoading ? <Typography color="text.secondary">Loading…</Typography> : null}
        {detailData ? (
          <Stack spacing={1}>
            <DetailKv label="PO number" value={pickStr(detailData, 'purchaseOrderNo', 'PurchaseOrderNo')} />
            <DetailKv label="Supplier" value={pickStr(detailData, 'supplierName', 'SupplierName')} />
            <DetailKv label="Order date" value={detailData.orderDate != null ? String(detailData.orderDate).slice(0, 10) : ''} />
            <DetailKv label="Expected on" value={detailData.expectedOn != null ? String(detailData.expectedOn).slice(0, 10) : ''} />
            <DetailKv
              label="Status"
              value={statusMap.get(String(detailData.statusReferenceValueId ?? '')) ?? ''}
            />
            <DetailKv label="Notes" value={pickStr(detailData, 'notes', 'Notes')} />
          </Stack>
        ) : null}
      </DetailDrawer>

      <TriVitaModal
        open={Boolean(modal)}
        onClose={() => setModal(null)}
        title={modal?.mode === 'edit' ? 'Edit purchase order' : 'New purchase order'}
        actions={
          <Stack direction="row" spacing={2} justifyContent="flex-end" sx={{ width: '100%' }}>
            <TriVitaButton onClick={() => setModal(null)}>Cancel</TriVitaButton>
            <TriVitaButton type="submit" form="po-header-form" variant="contained" disabled={saveHeader.isPending}>
              Save
            </TriVitaButton>
          </Stack>
        }
      >
        <Box component="form" id="po-header-form" onSubmit={handleSubmit(onHeaderSave)} noValidate>
          <FormGroup>
            <Grid item xs={12} md={6}>
              <Controller
                name="purchaseOrderNo"
                control={control}
                render={({ field }) => (
                  <TriVitaTextField {...field} label="PO number" required error={Boolean(errors.purchaseOrderNo)} helperText={errors.purchaseOrderNo?.message} />
                )}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <LookupSelect<Record<string, string>>
                name="supplierId"
                control={control as never}
                label="Supplier"
                required
                editId={null}
                queryKey={['pharmacy', 'supplier', 'lookup']}
                loadOptions={async () =>
                  (supplierOptions.data ?? []).map((o) => ({ value: o.value, label: o.label }))
                }
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <Controller
                name="orderDate"
                control={control}
                render={({ field }) => (
                  <TriVitaTextField {...field} label="Order date" type="date" InputLabelProps={{ shrink: true }} required error={Boolean(errors.orderDate)} helperText={errors.orderDate?.message} />
                )}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <Controller
                name="expectedOn"
                control={control}
                render={({ field }) => (
                  <TriVitaTextField {...field} label="Expected on" type="date" InputLabelProps={{ shrink: true }} error={Boolean(errors.expectedOn)} helperText={errors.expectedOn?.message} />
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
                queryKey={['pharmacy', 'po-status']}
                loadOptions={async () => {
                  const o = await buildPharmacyReferenceStatusOptions();
                  return o.length ? o : [{ value: '1', label: 'Default' }];
                }}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <Controller name="discountAmount" control={control} render={({ field }) => <TriVitaTextField {...field} label="Discount amount" />} />
            </Grid>
            <Grid item xs={12} md={6}>
              <Controller name="gstPercent" control={control} render={({ field }) => <TriVitaTextField {...field} label="GST %" />} />
            </Grid>
            <Grid item xs={12} md={6}>
              <Controller name="otherTaxAmount" control={control} render={({ field }) => <TriVitaTextField {...field} label="Other tax amount" />} />
            </Grid>
            <Grid item xs={12}>
              <Controller
                name="notes"
                control={control}
                render={({ field }) => (
                  <TriVitaTextField {...field} label="Notes" multiline minRows={2} error={Boolean(errors.notes)} helperText={errors.notes?.message} />
                )}
              />
            </Grid>
          </FormGroup>
        </Box>
      </TriVitaModal>

      <TriVitaModal
        open={editLine != null}
        onClose={() => setEditLine(null)}
        title="Edit PO line"
        actions={
          <Stack direction="row" spacing={2} justifyContent="flex-end" sx={{ width: '100%' }}>
            <TriVitaButton onClick={() => setEditLine(null)}>Cancel</TriVitaButton>
            <TriVitaButton variant="contained" onClick={() => editLine != null && updLineMut.mutate({ id: editLine.id, row: editLine })} disabled={updLineMut.isPending}>
              Save
            </TriVitaButton>
          </Stack>
        }
      >
        {editLine ? (
          <Stack spacing={2}>
            <TriVitaTextField label="Quantity" value={editLine.quantityOrdered} onChange={(e) => setEditLine({ ...editLine, quantityOrdered: e.target.value })} />
            <TriVitaTextField label="Unit price" value={editLine.unitPrice} onChange={(e) => setEditLine({ ...editLine, unitPrice: e.target.value })} />
            <TriVitaTextField label="Notes" value={editLine.notes} onChange={(e) => setEditLine({ ...editLine, notes: e.target.value })} />
          </Stack>
        ) : null}
      </TriVitaModal>

      <TriVitaModal
        open={deleteId != null}
        onClose={() => setDeleteId(null)}
        title="Delete purchase order"
        actions={
          <Stack direction="row" spacing={2} justifyContent="flex-end" sx={{ width: '100%' }}>
            <TriVitaButton onClick={() => setDeleteId(null)}>Cancel</TriVitaButton>
            <TriVitaButton color="error" variant="contained" onClick={() => deleteId != null && delMut.mutate(deleteId)} disabled={delMut.isPending}>
              Delete
            </TriVitaButton>
          </Stack>
        }
      >
        <Typography>Delete this purchase order permanently?</Typography>
      </TriVitaModal>
    </Stack>
  );
}
