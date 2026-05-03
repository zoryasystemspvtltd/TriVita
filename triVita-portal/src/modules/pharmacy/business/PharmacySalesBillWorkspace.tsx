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
  createSalesBill,
  deleteSalesBill,
  getCustomerPaged,
  getMedicinePaged,
  getSalesBillById,
  getSalesBillPaged,
  postSalesBill,
  updateSalesBill,
} from '@/services/pharmacyService';
import { getApiErrorMessage } from '@/utils/errorMap';

type Row = Record<string, unknown> & { id?: number };

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

type LineForm = { medicineId: string; quantity: string; unitPrice: string };

type HForm = {
  buyer: 'customer' | 'patient';
  customerId: string;
  patientId: string;
  salesDate: string;
  discountAmount: string;
  gstPercent: string;
  otherTaxAmount: string;
  notes: string;
  lines: LineForm[];
};

const lineSchema = Yup.object({
  medicineId: Yup.string().required().matches(/^\d+$/),
  quantity: Yup.string()
    .required()
    .test('q', 'Qty', (v) => Number.isFinite(Number(v)) && Number(v) > 0),
  unitPrice: Yup.string()
    .required()
    .test('p', 'Price', (v) => Number.isFinite(Number(v)) && Number(v) >= 0),
});

const hSchema = Yup.object({
  buyer: Yup.mixed<'customer' | 'patient'>().oneOf(['customer', 'patient']).required(),
  customerId: Yup.string().when('buyer', {
    is: 'customer',
    then: (s) => s.trim().required().matches(/^\d+$/),
    otherwise: (s) => s.trim().default(''),
  }),
  patientId: Yup.string().when('buyer', {
    is: 'patient',
    then: (s) => s.trim().required().matches(/^\d+$/),
    otherwise: (s) => s.trim().default(''),
  }),
  salesDate: Yup.string().required(),
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

export function PharmacySalesBillWorkspace() {
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
    queryKey: ['pharmacy', 'sales-bill', page, pageSize, searchApplied],
    queryFn: () => getSalesBillPaged({ page: page + 1, pageSize, search: searchApplied || undefined }),
  });

  const rows = useMemo(
    () => (list.data?.success && list.data.data ? [...list.data.data.items] : []) as Row[],
    [list.data]
  );
  const total = list.data?.success && list.data.data ? list.data.data.totalCount : 0;

  const customerOptions = useQuery({
    queryKey: ['pharmacy', 'sb-customers'],
    queryFn: async () => {
      const acc: { value: string; label: string }[] = [];
      for (let p = 1; p <= 10; p++) {
        const res = await getCustomerPaged({ page: p, pageSize: 50 });
        if (!res.success || !res.data) break;
        for (const r of res.data.items as Row[]) {
          const id = pickNum(r, 'id', 'Id');
          if (!id) continue;
          const name = pickStr(r, 'customerName', 'CustomerName');
          acc.push({ value: String(id), label: name || `Customer #${id}` });
        }
        if (res.data.items.length < 50) break;
      }
      return acc;
    },
    staleTime: 60_000,
  });

  const medicineOptions = useQuery({
    queryKey: ['pharmacy', 'sb-medicines'],
    queryFn: async () => {
      const acc: { value: string; label: string }[] = [];
      for (let p = 1; p <= 15; p++) {
        const res = await getMedicinePaged({ page: p, pageSize: 50 });
        if (!res.success || !res.data) break;
        for (const r of res.data.items as Row[]) {
          const id = pickNum(r, 'id', 'Id');
          if (!id) continue;
          const name = pickStr(r, 'medicineName', 'MedicineName');
          acc.push({ value: String(id), label: name || `Med #${id}` });
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
      buyer: 'customer',
      customerId: '',
      patientId: '',
      salesDate: new Date().toISOString().slice(0, 10),
      discountAmount: '0',
      gstPercent: '0',
      otherTaxAmount: '0',
      notes: '',
      lines: [{ medicineId: '', quantity: '1', unitPrice: '0' }],
    },
  });

  const { fields, append, remove, replace } = useFieldArray({ control: form.control, name: 'lines' });
  const buyer = form.watch('buyer');
  const watchedLines = form.watch('lines');

  useEffect(() => {
    if (buyer === 'customer') form.setValue('patientId', '');
    else form.setValue('customerId', '');
  }, [buyer, form]);

  const preview = useMemo(() => {
    const d = Number(form.watch('discountAmount') || 0);
    const g = Number(form.watch('gstPercent') || 0);
    const o = Number(form.watch('otherTaxAmount') || 0);
    let sub = 0;
    for (const l of watchedLines) {
      const q = Number(l.quantity);
      const p = Number(l.unitPrice);
      if (Number.isFinite(q) && Number.isFinite(p)) sub += Math.round(q * p * 10000) / 10000;
    }
    const taxable = Math.max(0, sub - d);
    const gst = Math.round(taxable * (g / 100) * 10000) / 10000;
    const net = Math.round((sub - d + gst + o) * 10000) / 10000;
    return { sub, gst, other: o, net };
  }, [watchedLines, form.watch('discountAmount'), form.watch('gstPercent'), form.watch('otherTaxAmount')]);

  const openCreate = () => {
    form.reset({
      buyer: 'customer',
      customerId: '',
      patientId: '',
      salesDate: new Date().toISOString().slice(0, 10),
      discountAmount: '0',
      gstPercent: '0',
      otherTaxAmount: '0',
      notes: '',
      lines: [{ medicineId: '', quantity: '1', unitPrice: '0' }],
    });
    setModal({ mode: 'create' });
  };

  const openEdit = async (id: number) => {
    const r = await getSalesBillById(id);
    if (!r.success || !r.data) {
      showToast(getApiErrorMessage(r) ?? 'Load failed', 'error');
      return;
    }
    const b = r.data as Row;
    const cid = pickNum(b, 'customerId', 'CustomerId');
    const pid = pickNum(b, 'patientId', 'PatientId');
    form.reset({
      buyer: cid ? 'customer' : 'patient',
      customerId: cid ? String(cid) : '',
      patientId: pid ? String(pid) : '',
      salesDate: pickStr(b, 'salesDate', 'SalesDate').slice(0, 10),
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
          medicineId: String(pickNum(it, 'medicineId', 'MedicineId')),
          quantity: String(pickNum(it, 'quantity', 'Quantity')),
          unitPrice: String(pickNum(it, 'unitPrice', 'UnitPrice')),
        }))
      );
    } else {
      replace([{ medicineId: '', quantity: '1', unitPrice: '0' }]);
    }
    setModal({ mode: 'edit', id });
  };

  const saveMut = useMutation({
    mutationFn: async (v: HForm) => {
      const lines = v.lines.map((l) => ({
        medicineId: Number(l.medicineId),
        quantity: Number(l.quantity),
        unitPrice: Number(l.unitPrice),
      }));
      const body = {
        customerId: v.buyer === 'customer' ? Number(v.customerId) : null,
        patientId: v.buyer === 'patient' ? Number(v.patientId) : null,
        salesDate: new Date(v.salesDate).toISOString(),
        discountAmount: Number(v.discountAmount || 0),
        gstPercent: Number(v.gstPercent || 0),
        otherTaxAmount: Number(v.otherTaxAmount || 0),
        notes: v.notes.trim() || null,
        items: lines,
      };
      if (modal?.mode === 'create') return createSalesBill(body);
      if (modal?.mode === 'edit') return updateSalesBill(modal.id, body);
      throw new Error('No modal');
    },
    onSuccess: (res) => {
      if (!res.success) {
        showToast(getApiErrorMessage(res) ?? 'Save failed', 'error');
        return;
      }
      showToast('Saved.', 'success');
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'sales-bill'] });
      setModal(null);
    },
    onError: (e) => showToast(getApiErrorMessage(e) ?? 'Save failed', 'error'),
  });

  const postMut = useMutation({
    mutationFn: async (id: number) => postSalesBill(id),
    onSuccess: (res, id) => {
      if (!res.success) {
        showToast(getApiErrorMessage(res) ?? 'Post failed', 'error');
        return;
      }
      showToast('Posted. Stock deducted (FEFO).', 'success');
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'sales-bill'] });
      if (modal?.mode === 'edit' && modal.id === id) setModal(null);
    },
    onError: (e) => showToast(getApiErrorMessage(e) ?? 'Post failed', 'error'),
  });

  const delMut = useMutation({
    mutationFn: async (id: number) => deleteSalesBill(id),
    onSuccess: (res) => {
      if (!res.success) {
        showToast(getApiErrorMessage(res) ?? 'Delete failed', 'error');
        return;
      }
      showToast('Deleted.', 'success');
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'sales-bill'] });
      setDeleteId(null);
    },
    onError: (e) => showToast(getApiErrorMessage(e) ?? 'Delete failed', 'error'),
  });

  const onSubmit = form.handleSubmit((v) => saveMut.mutate(v));

  const statusLabel = (s: number) => (s === ST_POSTED ? 'Posted' : 'Draft');

  const editId = modal?.mode === 'edit' ? modal.id : null;
  const billDetail = useQuery({
    queryKey: ['pharmacy', 'sales-bill', editId],
    queryFn: () => getSalesBillById(editId!),
    enabled: editId != null,
  });
  const posted =
    billDetail.data?.success && billDetail.data.data
      ? pickNum(billDetail.data.data as Row, 'status', 'Status') === ST_POSTED
      : false;

  return (
    <Box sx={{ p: 2 }}>
      <PageHeader title="Sales Bill" subtitle="Draft → Post: FEFO batch deduction & stock ledger." />

      <Stack direction="row" spacing={1} sx={{ mb: 2 }} alignItems="center">
        <TextField
          size="small"
          label="Search"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          sx={{ minWidth: 220 }}
        />
        <TriVitaButton variant="contained" onClick={openCreate}>
          New sales bill
        </TriVitaButton>
      </Stack>

      <Box
        sx={{
          '& .MuiTableRow-root:nth-of-type(even)': { bgcolor: 'action.hover' },
        }}
      >
        <DataTable<Row>
          tableAriaLabel="Sales bills"
          loading={list.isFetching}
          columns={[
            {
              id: 'bill',
              label: 'Bill No',
              minWidth: 120,
              format: (r) => pickStr(r, 'billNo', 'BillNo'),
            },
            {
              id: 'st',
              label: 'Status',
              minWidth: 90,
              format: (r) => statusLabel(pickNum(r, 'status', 'Status')),
            },
            {
              id: 'net',
              label: 'Net',
              minWidth: 100,
              align: 'right',
              format: (r) => String(pickNum(r, 'netAmount', 'NetAmount')),
            },
            {
              id: 'party',
              label: 'Party',
              minWidth: 140,
              format: (r) => {
                const c = pickNum(r, 'customerId', 'CustomerId');
                const p = pickNum(r, 'patientId', 'PatientId');
                if (c) return `Customer #${c}`;
                if (p) return `Patient #${p}`;
                return '—';
              },
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
        title={modal?.mode === 'create' ? 'New sales bill' : 'Edit sales bill'}
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
                Post (deduct stock)
              </TriVitaButton>
            )}
          </Stack>
        }
      >
        {modal && (
          <Stack spacing={2} sx={{ mt: 1 }}>
            <Controller
              name="buyer"
              control={form.control}
              render={({ field }) => (
                <TextField select label="Sold to" fullWidth {...field} disabled={posted}>
                  <MenuItem value="customer">Customer</MenuItem>
                  <MenuItem value="patient">Registered patient (HMS)</MenuItem>
                </TextField>
              )}
            />
            {buyer === 'customer' && (
              <Controller
                name="customerId"
                control={form.control}
                render={({ field, fieldState }) => (
                  <TextField
                    select
                    label="Customer"
                    fullWidth
                    {...field}
                    disabled={posted || customerOptions.isLoading}
                    error={!!fieldState.error}
                    helperText={fieldState.error?.message}
                  >
                    {(customerOptions.data ?? []).map((o) => (
                      <MenuItem key={o.value} value={o.value}>
                        {o.label}
                      </MenuItem>
                    ))}
                  </TextField>
                )}
              />
            )}
            {buyer === 'patient' && (
              <Controller
                name="patientId"
                control={form.control}
                render={({ field, fieldState }) => (
                  <TextField
                    label="Patient ID (HMS_PatientMaster)"
                    fullWidth
                    {...field}
                    disabled={posted}
                    error={!!fieldState.error}
                    helperText={fieldState.error?.message}
                  />
                )}
              />
            )}
            <Controller
              name="salesDate"
              control={form.control}
              render={({ field, fieldState }) => (
                <TextField
                  label="Sales date"
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
            <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
              <Controller
                name="discountAmount"
                control={form.control}
                render={({ field }) => (
                  <TextField label="Discount" fullWidth {...field} disabled={posted} />
                )}
              />
              <Controller
                name="gstPercent"
                control={form.control}
                render={({ field }) => (
                  <TextField label="GST %" fullWidth {...field} disabled={posted} />
                )}
              />
              <Controller
                name="otherTaxAmount"
                control={form.control}
                render={({ field }) => (
                  <TextField label="Other tax" fullWidth {...field} disabled={posted} />
                )}
              />
            </Stack>
            <Controller
              name="notes"
              control={form.control}
              render={({ field }) => <TextField label="Notes" fullWidth multiline minRows={2} {...field} disabled={posted} />}
            />

            <Typography variant="subtitle2">Lines (medicine / qty / price — batches assigned on Post via FEFO)</Typography>
            <Table size="small">
              <TableHead>
                <TableRow>
                  <TableCell>Medicine</TableCell>
                  <TableCell align="right">Qty</TableCell>
                  <TableCell align="right">Unit price</TableCell>
                  <TableCell align="right">Amount</TableCell>
                  <TableCell />
                </TableRow>
              </TableHead>
              <TableBody>
                {fields.map((f, idx) => {
                  const q = Number(form.watch(`lines.${idx}.quantity`));
                  const pr = Number(form.watch(`lines.${idx}.unitPrice`));
                  const amt = Number.isFinite(q) && Number.isFinite(pr) ? Math.round(q * pr * 10000) / 10000 : 0;
                  return (
                    <TableRow key={f.id}>
                      <TableCell sx={{ minWidth: 200 }}>
                        <Controller
                          name={`lines.${idx}.medicineId`}
                          control={form.control}
                          render={({ field: lf }) => (
                            <TextField select size="small" fullWidth {...lf} disabled={posted}>
                              {(medicineOptions.data ?? []).map((o) => (
                                <MenuItem key={o.value} value={o.value}>
                                  {o.label}
                                </MenuItem>
                              ))}
                            </TextField>
                          )}
                        />
                      </TableCell>
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
                          name={`lines.${idx}.unitPrice`}
                          control={form.control}
                          render={({ field: lf }) => (
                            <TextField size="small" type="number" inputProps={{ step: 'any' }} {...lf} disabled={posted} />
                          )}
                        />
                      </TableCell>
                      <TableCell align="right">{amt}</TableCell>
                      <TableCell>
                        {!posted && fields.length > 1 ? (
                          <Link component="button" type="button" variant="body2" onClick={() => remove(idx)}>
                            Remove
                          </Link>
                        ) : null}
                      </TableCell>
                    </TableRow>
                  );
                })}
              </TableBody>
            </Table>
            {!posted ? (
              <TriVitaButton variant="outlined" onClick={() => append({ medicineId: '', quantity: '1', unitPrice: '0' })}>
                Add line
              </TriVitaButton>
            ) : null}

            <Box>
              <Typography variant="subtitle2" sx={{ mb: 0.5 }}>
                Totals
              </Typography>
              <Typography variant="body2">Subtotal: {preview.sub.toFixed(4)}</Typography>
              <Typography variant="body2">GST: {preview.gst.toFixed(4)}</Typography>
              <Typography variant="body2">Other tax: {preview.other.toFixed(4)}</Typography>
              <Typography variant="body2">Net: {preview.net.toFixed(4)}</Typography>
            </Box>
          </Stack>
        )}
      </TriVitaModal>

      <TriVitaModal
        open={deleteId != null}
        onClose={() => setDeleteId(null)}
        title="Delete draft sales bill?"
        actions={
          <>
            <TriVitaButton onClick={() => setDeleteId(null)}>Cancel</TriVitaButton>
            <TriVitaButton color="error" variant="contained" onClick={() => deleteId && delMut.mutate(deleteId)}>
              Delete
            </TriVitaButton>
          </>
        }
      >
        <Typography>Draft only. Posted bills cannot be deleted here.</Typography>
      </TriVitaModal>
    </Box>
  );
}
