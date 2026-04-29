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
import { Controller, useFieldArray, useForm, useWatch, type Resolver } from 'react-hook-form';
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
  deletePurchaseOrder,
  getMedicinePaged,
  getSupplierPaged,
  getPurchaseOrderById,
  getPurchaseOrdersPaged,
  updatePurchaseOrder,
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
  items: { id?: number; medicineId: string; quantityOrdered: string; unitPrice: string; notes: string }[];
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
  items: Yup.array()
    .of(
      Yup.object({
        id: Yup.number().notRequired(),
        medicineId: Yup.string().trim().required().matches(/^\d+$/, 'Select medicine'),
        quantityOrdered: Yup.string().trim().required(),
        unitPrice: Yup.string().trim().required(),
        notes: Yup.string().trim().default(''),
      })
    )
    .min(1, 'Add at least one item')
    .required(),
});

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
  // no separate edit modal; inline editing only

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

  const poStatusOpts = useMemo(() => {
    const allowed = new Set(['Draft', 'Approved', 'Closed']);
    return (statusOpts.data ?? []).filter((o) => allowed.has(o.label));
  }, [statusOpts.data]);

  const draftPoStatusId = useMemo(() => poStatusOpts.find((o) => o.label === 'Draft')?.value ?? '', [poStatusOpts]);

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
      items: [],
    },
  });

  const { fields: itemFields, append: appendItem, remove: removeItem, update: updateItem, replace: replaceItems } = useFieldArray({
    control,
    name: 'items',
  });

  const discountWatch = useWatch({ control, name: 'discountAmount' });
  const gstPercentWatch = useWatch({ control, name: 'gstPercent' });
  const otherTaxWatch = useWatch({ control, name: 'otherTaxAmount' });
  const itemsWatch = useWatch({ control, name: 'items' });

  const billing = useMemo(() => {
    const subTotal = (itemsWatch ?? []).reduce((acc, r) => {
      const q = Number(r.quantityOrdered) || 0;
      const p = Number(r.unitPrice) || 0;
      return acc + q * p;
    }, 0);
    const discount = Number(discountWatch) || 0;
    const gstPercent = Number(gstPercentWatch) || 0;
    const otherTax = Number(otherTaxWatch) || 0;
    const taxable = Math.max(0, subTotal - discount);
    const gstAmount = (taxable * gstPercent) / 100;
    const total = subTotal - discount + gstAmount + otherTax;
    return { subTotal, discount, gstPercent, gstAmount, otherTax, total };
  }, [itemsWatch, discountWatch, gstPercentWatch, otherTaxWatch]);

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
        statusReferenceValueId: draftPoStatusId || statusOpts.data?.[0]?.value || '',
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
      const supplierName = pickStr(r, 'supplierName', 'SupplierName').trim().toLowerCase();
      const sup = supplierOptions.data?.find((x) => x.name.trim().toLowerCase() === supplierName) ?? null;
      reset({
        purchaseOrderNo: pickStr(r, 'purchaseOrderNo', 'PurchaseOrderNo'),
        supplierId: sup?.value ?? '',
        orderDate: pickStr(r, 'orderDate', 'OrderDate').slice(0, 10),
        expectedOn: r.expectedOn != null ? String(r.expectedOn).slice(0, 10) : '',
        statusReferenceValueId: String(r.statusReferenceValueId ?? '') || draftPoStatusId,
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
          medicineId: String(x.medicineId ?? ''),
          quantityOrdered: String(x.quantityOrdered ?? ''),
          unitPrice: String(x.unitPrice ?? ''),
          notes: pickStr(x, 'notes', 'Notes'),
        }))
      );
    }
  }, [modal, editSeed.data, reset, statusOpts.data, supplierOptions.data, draftPoStatusId, replaceItems]);

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
        items: (args.v.items ?? []).map((x) => ({
          id: x.id ?? undefined,
          medicineId: Number(x.medicineId),
          quantityOrdered: Number(x.quantityOrdered),
          unitPrice: Number(x.unitPrice),
          notes: x.notes?.trim() || undefined,
        })),
      };
      if (args.editId != null) return updatePurchaseOrder(args.editId, body);
      return createPurchaseOrder(body);
    },
    onSuccess: (res) => {
      if (!res.success) {
        showToast(res.message ?? 'Save failed', 'error');
        return;
      }
      showToast('Purchase order saved', 'success');
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'po'] });
      setModal(null);
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

  // no draft row; grid is the source of truth

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
          emptyTitle="No purchase orders"
        />
      </FormSection>

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
                    {(poStatusOpts.length ? poStatusOpts : [{ value: '', label: 'Draft' }]).map((o) => (
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

            <Box sx={{ display: 'flex', justifyContent: 'flex-end', mb: 1 }}>
              <TriVitaButton
                size="small"
                variant="outlined"
                startIcon={<Add />}
                onClick={() => appendItem({ medicineId: '', quantityOrdered: '', unitPrice: '', notes: '' })}
              >
                Add row
              </TriVitaButton>
            </Box>

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
                {itemFields.map((f, idx) => {
                  const w = (itemsWatch ?? [])[idx];
                  const q = Number(w?.quantityOrdered) || 0;
                  const p = Number(w?.unitPrice) || 0;
                  const lineTotal = q * p;
                  return (
                    <TableRow key={f.id} hover>
                      <TableCell sx={{ minWidth: 220 }}>
                        <TriVitaTextField
                          size="small"
                          select
                          SelectProps={{ native: true }}
                          value={w?.medicineId ?? ''}
                          onChange={(e) => updateItem(idx, { ...(w ?? {}), medicineId: e.target.value })}
                          sx={{ minWidth: 220 }}
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
                      </TableCell>
                      <TableCell align="right" sx={{ width: 140 }}>
                        <TriVitaTextField
                          size="small"
                          value={w?.quantityOrdered ?? ''}
                          onChange={(e) => updateItem(idx, { ...(w ?? {}), quantityOrdered: e.target.value })}
                          sx={{ width: 120 }}
                        />
                      </TableCell>
                      <TableCell align="right" sx={{ width: 160 }}>
                        <TriVitaTextField
                          size="small"
                          value={w?.unitPrice ?? ''}
                          onChange={(e) => updateItem(idx, { ...(w ?? {}), unitPrice: e.target.value })}
                          sx={{ width: 120 }}
                        />
                      </TableCell>
                      <TableCell align="right">{lineTotal ? lineTotal.toFixed(2) : '0.00'}</TableCell>
                      <TableCell sx={{ minWidth: 160 }}>
                        <TriVitaTextField
                          size="small"
                          value={w?.notes ?? ''}
                          onChange={(e) => updateItem(idx, { ...(w ?? {}), notes: e.target.value })}
                        />
                      </TableCell>
                      <TableCell align="right">
                        <Stack direction="row" spacing={1} justifyContent="flex-end">
                          <IconButton size="small" aria-label="delete line" onClick={() => removeItem(idx)}>
                            <Delete fontSize="small" />
                          </IconButton>
                        </Stack>
                      </TableCell>
                    </TableRow>
                  );
                })}
                {itemFields.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={6}>
                      <Typography variant="body2" color="text.secondary">
                        No items yet. Add at least one item.
                      </Typography>
                    </TableCell>
                  </TableRow>
                ) : null}
              </TableBody>
            </Table>

            <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} sx={{ mb: 2 }} justifyContent="flex-end">
              <Typography variant="body2">Subtotal: {billing.subTotal.toFixed(2)}</Typography>
              <Typography variant="body2">GST: {billing.gstAmount.toFixed(2)}</Typography>
              <Typography variant="body2">Total: {billing.total.toFixed(2)}</Typography>
            </Stack>

          </Box>

          <Box sx={{ mt: 2 }}>
            <Typography variant="subtitle2" sx={{ mb: 1 }}>
              Billing
            </Typography>
            <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
              <Controller name="discountAmount" control={control} render={({ field }) => <TriVitaTextField {...field} label="Discount amount" />} />
              <Controller name="gstPercent" control={control} render={({ field }) => <TriVitaTextField {...field} label="GST %" />} />
              <Controller name="otherTaxAmount" control={control} render={({ field }) => <TriVitaTextField {...field} label="Other tax amount" />} />
            </Stack>
          </Box>

          <Box sx={{ mt: 2 }}>
            <Typography variant="subtitle2" sx={{ mb: 1 }}>
              Notes
            </Typography>
            <Controller
              name="notes"
              control={control}
              render={({ field }) => (
                <TriVitaTextField {...field} label="Notes" multiline minRows={2} error={Boolean(errors.notes)} helperText={errors.notes?.message} />
              )}
            />
          </Box>
        </Box>
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
