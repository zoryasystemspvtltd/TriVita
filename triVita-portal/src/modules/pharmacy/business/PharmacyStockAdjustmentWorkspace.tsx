import { yupResolver } from '@hookform/resolvers/yup';
import { Add, Delete } from '@mui/icons-material';
import {
  Box,
  IconButton,
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
import { FormGroup } from '@/components/ds/FormGroup';
import { TriVitaButton } from '@/components/ds/TriVitaButton';
import { TriVitaModal } from '@/components/ds/TriVitaModal';
import { PageHeader } from '@/components/layout/PageHeader';
import { useToast } from '@/components/toast/ToastProvider';
import {
  createStockAdjustment,
  createStockAdjustmentItem,
  deleteStockAdjustment,
  deleteStockAdjustmentItem,
  getBatchStockPaged,
  getMedicineBatchById,
  getMedicineBatchPaged,
  getMedicinePaged,
  getStockAdjustmentById,
  getStockAdjustmentItemsByAdjustment,
  getStockAdjustmentPaged,
  postStockAdjustment,
  updateStockAdjustment,
  updateStockAdjustmentItem,
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

function pickNum(r: Record<string, unknown>, ...keys: string[]) {
  for (const k of keys) {
    const v = r[k];
    const n = Number(v);
    if (Number.isFinite(n)) return n;
  }
  return 0;
}

type LineForm = {
  clientKey: string;
  id?: number;
  medicineId: string;
  medicineBatchId: string;
  quantityDelta: string;
  notes: string;
};

type HForm = {
  adjustmentNo: string;
  adjustmentOn: string;
  reasonNotes: string;
  items: LineForm[];
};

const lineSchema = Yup.object({
  clientKey: Yup.string().required(),
  id: Yup.number().notRequired(),
  medicineId: Yup.string().trim().required().matches(/^\d+$/),
  medicineBatchId: Yup.string().trim().required().matches(/^\d+$/),
  quantityDelta: Yup.string()
    .trim()
    .required()
    .test('nz', 'Adjust qty cannot be zero', (v) => {
      const n = Number(v);
      return Number.isFinite(n) && n !== 0;
    }),
  notes: Yup.string().trim().default(''),
});

const hSchema = Yup.object({
  adjustmentNo: Yup.string().trim().default(''),
  adjustmentOn: Yup.string().required(),
  reasonNotes: Yup.string().trim().max(1000).default(''),
  items: Yup.array().of(lineSchema).default([]),
});

async function loadBatchesForMedicine(medicineId: number): Promise<{ value: string; label: string }[]> {
  const acc: { value: string; label: string }[] = [];
  const seen = new Set<string>();
  for (let p = 1; p <= 30; p++) {
    const res = await getMedicineBatchPaged({ page: p, pageSize: 100 });
    if (!res.success || !res.data) break;
    for (const b of res.data.items as Row[]) {
      const mid = pickNum(b, 'medicineId', 'MedicineId');
      if (mid !== medicineId) continue;
      const id = pickNum(b, 'id', 'Id');
      if (!id) continue;
      const key = String(id);
      if (seen.has(key)) continue;
      seen.add(key);
      const batchNo = pickStr(b, 'batchNo', 'BatchNo');
      acc.push({ value: key, label: batchNo || `Batch #${key}` });
    }
    if (res.data.items.length < 100) break;
  }
  return acc.sort((a, b) => a.label.localeCompare(b.label));
}

async function fetchExpiryAndQty(medicineBatchId: number): Promise<{ expiry: string; currentQty: string }> {
  const br = await getMedicineBatchById(medicineBatchId);
  const b = br.success && br.data ? (br.data as Row) : null;
  const exp = b ? pickStr(b, 'expiryDate', 'ExpiryDate') : '';
  for (let p = 1; p <= 25; p++) {
    const res = await getBatchStockPaged({ page: p, pageSize: 100 });
    if (!res.success || !res.data) break;
    for (const s of res.data.items as Row[]) {
      const bid = pickNum(s, 'medicineBatchId', 'MedicineBatchId');
      if (bid === medicineBatchId) {
        return { expiry: exp, currentQty: String(pickNum(s, 'currentQty', 'CurrentQty')) };
      }
    }
    if (res.data.items.length < 100) break;
  }
  return { expiry: exp, currentQty: '0' };
}

export function PharmacyStockAdjustmentWorkspace() {
  const qc = useQueryClient();
  const { showToast } = useToast();
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const [search, setSearch] = useState('');
  const [searchApplied, setSearchApplied] = useState('');
  const [modal, setModal] = useState<null | { mode: 'create' } | { mode: 'edit'; id: number }>(null);
  const [headerStatus, setHeaderStatus] = useState<1 | 2 | null>(null);
  const [deleteId, setDeleteId] = useState<number | null>(null);
  const [batchOptions, setBatchOptions] = useState<Record<string, { value: string; label: string }[]>>({});
  const [lineMeta, setLineMeta] = useState<Record<string, { expiry: string; currentQty: string }>>({});

  useEffect(() => {
    const t = window.setTimeout(() => setSearchApplied(search), 400);
    return () => window.clearTimeout(t);
  }, [search]);

  useEffect(() => {
    setPage(0);
  }, [searchApplied]);

  const list = useQuery({
    queryKey: ['pharmacy', 'stock-adj', page, pageSize, searchApplied],
    queryFn: () => getStockAdjustmentPaged({ page: page + 1, pageSize, search: searchApplied || undefined }),
  });

  const rows = useMemo(
    () => (list.data?.success && list.data.data ? [...list.data.data.items] : []) as Row[],
    [list.data]
  );
  const total = list.data?.success && list.data.data ? list.data.data.totalCount : 0;

  const medicineOpts = useQuery({
    queryKey: ['pharmacy', 'sa-medicines'],
    queryFn: async () => {
      const acc: { value: string; label: string }[] = [];
      for (let p = 1; p <= 10; p++) {
        const res = await getMedicinePaged({ page: p, pageSize: 100 });
        if (!res.success || !res.data) break;
        for (const m of res.data.items as Row[]) {
          const id = pickNum(m, 'id', 'Id');
          if (!id) continue;
          acc.push({ value: String(id), label: pickStr(m, 'medicineName', 'MedicineName') });
        }
        if (res.data.items.length < 100) break;
      }
      return acc;
    },
    staleTime: 120_000,
  });

  const form = useForm<HForm>({
    resolver: yupResolver(hSchema) as Resolver<HForm>,
    defaultValues: {
      adjustmentNo: '',
      adjustmentOn: new Date().toISOString().slice(0, 10),
      reasonNotes: '',
      items: [],
    },
  });

  const { fields, append, remove, replace } = useFieldArray({ control: form.control, name: 'items' });
  const watchedItems = form.watch('items');
  const locked = headerStatus === 2;

  useEffect(() => {
    if (!modal) return;
    let cancelled = false;
    (async () => {
      const next: Record<string, { expiry: string; currentQty: string }> = {};
      for (const line of watchedItems) {
        const bid = Number(line.medicineBatchId);
        if (!Number.isFinite(bid) || bid <= 0) continue;
        const d = await fetchExpiryAndQty(bid);
        if (!cancelled) next[line.clientKey] = d;
      }
      if (!cancelled) setLineMeta(next);
    })();
    return () => {
      cancelled = true;
    };
  }, [watchedItems, modal]);

  useEffect(() => {
    if (!modal || locked) return;
    let cancelled = false;
    (async () => {
      const mids = [
        ...new Set(
          watchedItems.map((l) => Number(l.medicineId)).filter((n) => Number.isFinite(n) && n > 0)
        ),
      ];
      const pairs: [string, { value: string; label: string }[]][] = [];
      for (const mid of mids) {
        pairs.push([String(mid), await loadBatchesForMedicine(mid)]);
      }
      if (cancelled) return;
      setBatchOptions((prev) => {
        const next = { ...prev };
        for (const [k, v] of pairs) next[k] = v;
        return next;
      });
    })();
    return () => {
      cancelled = true;
    };
  }, [watchedItems, modal, locked]);

  const openCreate = () => {
    setHeaderStatus(1);
    setBatchOptions({});
    setLineMeta({});
    form.reset({
      adjustmentNo: '',
      adjustmentOn: new Date().toISOString().slice(0, 10),
      reasonNotes: '',
      items: [
        {
          clientKey: `n-${Date.now()}`,
          medicineId: '',
          medicineBatchId: '',
          quantityDelta: '',
          notes: '',
        },
      ],
    });
    setModal({ mode: 'create' });
  };

  const openEdit = async (id: number) => {
    setBatchOptions({});
    setLineMeta({});
    const h = await getStockAdjustmentById(id);
    if (!h.success || !h.data) {
      showToast(getApiErrorMessage(h) || 'Failed to load', 'error');
      return;
    }
    const hd = h.data as Row;
    const st = pickNum(hd, 'status', 'Status') === 2 ? 2 : 1;
    setHeaderStatus(st);

    const itemsRes = await getStockAdjustmentItemsByAdjustment(id);
    const rawLines = itemsRes.success && itemsRes.data ? [...(itemsRes.data as Row[])] : [];
    const lines: LineForm[] = rawLines.map((r) => ({
      clientKey: `e-${pickNum(r, 'id', 'Id')}`,
      id: pickNum(r, 'id', 'Id'),
      medicineId: String(pickNum(r, 'medicineId', 'MedicineId')),
      medicineBatchId: String(pickNum(r, 'medicineBatchId', 'MedicineBatchId')),
      quantityDelta: String(pickNum(r, 'quantityDelta', 'QuantityDelta')),
      notes: pickStr(r, 'notes', 'Notes'),
    }));

    const items =
      lines.length > 0
        ? lines
        : st === 1
          ? [
              {
                clientKey: `n-${Date.now()}`,
                medicineId: '',
                medicineBatchId: '',
                quantityDelta: '',
                notes: '',
              },
            ]
          : [];

    form.reset({
      adjustmentNo: pickStr(hd, 'adjustmentNo', 'AdjustmentNo'),
      adjustmentOn: pickStr(hd, 'adjustmentOn', 'AdjustmentOn').slice(0, 10),
      reasonNotes: pickStr(hd, 'reasonNotes', 'ReasonNotes'),
      items,
    });
    replace(items);
    setModal({ mode: 'edit', id });
  };

  const saveMut = useMutation({
    mutationFn: async (mode: 'draft' | 'post') => {
      if (locked) throw new Error('Posted adjustment is locked.');
      const v = await form.trigger();
      if (!v) throw new Error('Fix validation errors');
      const vals = form.getValues();
      const validLines = vals.items.filter((line) => {
        const mid = Number(line.medicineId);
        const bid = Number(line.medicineBatchId);
        const qd = Number(line.quantityDelta);
        return Number.isFinite(mid) && mid > 0 && Number.isFinite(bid) && bid > 0 && Number.isFinite(qd) && qd !== 0;
      });
      if (validLines.length === 0) throw new Error('Add at least one complete line (medicine, batch, non-zero adjust qty).');
      const headerBody: Record<string, unknown> = {
        adjustmentOn: new Date(vals.adjustmentOn + 'T12:00:00').toISOString(),
        reasonNotes: vals.reasonNotes?.trim() ? vals.reasonNotes.trim() : null,
        adjustmentTypeReferenceValueId: null,
        performedByDoctorId: null,
      };

      let adjId = modal?.mode === 'edit' ? modal.id : 0;
      if (modal?.mode === 'create') {
        headerBody.adjustmentNo = '';
        const cr = await createStockAdjustment(headerBody);
        if (!cr.success || !cr.data) throw new Error(getApiErrorMessage(cr) || 'Create failed');
        adjId = pickNum(cr.data as Row, 'id', 'Id');
      } else if (modal?.mode === 'edit') {
        headerBody.adjustmentNo = vals.adjustmentNo?.trim() || '';
        const up = await updateStockAdjustment(adjId, headerBody);
        if (!up.success) throw new Error(getApiErrorMessage(up) || 'Update failed');
      }

      const existingIds = new Set<number>();
      if (modal?.mode === 'edit') {
        const cur = await getStockAdjustmentItemsByAdjustment(adjId);
        if (cur.success && cur.data) {
          for (const r of cur.data as Row[]) existingIds.add(pickNum(r, 'id', 'Id'));
        }
      }

      const keepIds = new Set<number>();
      for (const line of validLines) {
        const mid = Number(line.medicineId);
        const bid = Number(line.medicineBatchId);
        const qd = Number(line.quantityDelta);

        if (line.id) {
          keepIds.add(line.id);
          const ur = await updateStockAdjustmentItem(line.id, {
            stockAdjustmentId: adjId,
            medicineId: mid,
            lineNum: 0,
            medicineBatchId: bid,
            quantityDelta: qd,
            unitCost: null,
            notes: line.notes?.trim() ? line.notes.trim() : null,
          });
          if (!ur.success) throw new Error(getApiErrorMessage(ur) || 'Line update failed');
        } else {
          const ir = await createStockAdjustmentItem({
            stockAdjustmentId: adjId,
            medicineId: mid,
            lineNum: 0,
            medicineBatchId: bid,
            quantityDelta: qd,
            unitCost: null,
            notes: line.notes?.trim() ? line.notes.trim() : null,
          });
          if (!ir.success) throw new Error(getApiErrorMessage(ir) || 'Line create failed');
        }
      }

      if (modal?.mode === 'edit') {
        for (const oldId of existingIds) {
          if (!keepIds.has(oldId)) {
            const dr = await deleteStockAdjustmentItem(oldId);
            if (!dr.success) throw new Error(getApiErrorMessage(dr) || 'Line delete failed');
          }
        }
      }

      if (mode === 'post') {
        const pr = await postStockAdjustment(adjId);
        if (!pr.success) throw new Error(getApiErrorMessage(pr) || 'Post failed');
      }
    },
    onSuccess: (_, mode) => {
      showToast(mode === 'post' ? 'Posted.' : 'Saved draft.', 'success');
      qc.invalidateQueries({ queryKey: ['pharmacy', 'stock-adj'] });
      setModal(null);
      setHeaderStatus(null);
    },
    onError: (e: Error) => showToast(e.message || 'Failed', 'error'),
  });

  const delMut = useMutation({
    mutationFn: async (id: number) => {
      const r = await deleteStockAdjustment(id);
      if (!r.success) throw new Error(getApiErrorMessage(r) || 'Delete failed');
    },
    onSuccess: () => {
      showToast('Deleted.', 'success');
      qc.invalidateQueries({ queryKey: ['pharmacy', 'stock-adj'] });
      setDeleteId(null);
    },
    onError: (e: Error) => showToast(e.message || 'Failed', 'error'),
  });

  return (
    <Stack spacing={2}>
      <PageHeader title="Stock adjustment" subtitle="Pharmacy → Transactions" />

      <Stack direction="row" justifyContent="space-between" flexWrap="wrap" gap={1}>
        <TextField size="small" label="Search" value={search} onChange={(e) => setSearch(e.target.value)} sx={{ minWidth: 220 }} />
        <TriVitaButton variant="contained" onClick={openCreate}>
          New adjustment
        </TriVitaButton>
      </Stack>

      <Box
        sx={{
          '& .MuiTableRow-root:nth-of-type(even)': { bgcolor: 'action.hover' },
        }}
      >
        <DataTable<Row>
          tableAriaLabel="Stock adjustments"
          loading={list.isFetching}
          columns={[
            {
              id: 'no',
              label: 'No.',
              minWidth: 120,
              format: (r) => pickStr(r, 'adjustmentNo', 'AdjustmentNo'),
            },
            {
              id: 'dt',
              label: 'Date',
              minWidth: 110,
              format: (r) => pickStr(r, 'adjustmentOn', 'AdjustmentOn').slice(0, 10),
            },
            {
              id: 'st',
              label: 'Status',
              minWidth: 90,
              format: (r) => (pickNum(r, 'status', 'Status') === 2 ? 'Posted' : 'Draft'),
            },
            {
              id: 'rm',
              label: 'Remarks',
              minWidth: 160,
              format: (r) => pickStr(r, 'reasonNotes', 'ReasonNotes') || '—',
            },
            {
              id: '_a',
              label: 'Actions',
              minWidth: 140,
              format: (r) => {
                const id = pickNum(r, 'id', 'Id');
                const st = pickNum(r, 'status', 'Status');
                return (
                  <Stack direction="row" spacing={1} alignItems="center">
                    <Link component="button" type="button" variant="body2" onClick={() => void openEdit(id)}>
                      Open
                    </Link>
                    {st !== 2 ? (
                      <Link component="button" type="button" variant="body2" color="error" onClick={() => setDeleteId(id)}>
                        Delete
                      </Link>
                    ) : null}
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
        />
      </Box>

      <TriVitaModal
        open={!!modal}
        onClose={() => {
          if (!saveMut.isPending) {
            setModal(null);
            setHeaderStatus(null);
          }
        }}
        title={modal?.mode === 'create' ? 'New stock adjustment' : 'Stock adjustment'}
        maxWidth="lg"
        actions={
          <Stack direction="row" spacing={1} flexWrap="wrap" useFlexGap>
            <TriVitaButton onClick={() => !saveMut.isPending && (setModal(null), setHeaderStatus(null))}>Close</TriVitaButton>
            <TriVitaButton variant="contained" color="secondary" disabled={saveMut.isPending || locked} onClick={() => saveMut.mutate('draft')}>
              Save draft
            </TriVitaButton>
            <TriVitaButton variant="contained" disabled={saveMut.isPending || locked} onClick={() => saveMut.mutate('post')}>
              Post
            </TriVitaButton>
          </Stack>
        }
      >
        <Stack spacing={2} sx={{ mt: 1 }}>
          <Typography variant="body2" color="text.secondary">
            Status: {locked ? 'Posted (locked)' : 'Draft'}
          </Typography>
          <FormGroup columns={2}>
            <TextField label="Adjustment no." value={form.watch('adjustmentNo')} disabled fullWidth size="small" />
            <Controller
              name="adjustmentOn"
              control={form.control}
              render={({ field, fieldState }) => (
                <TextField
                  {...field}
                  type="date"
                  label="Date"
                  size="small"
                  fullWidth
                  disabled={locked}
                  error={!!fieldState.error}
                  helperText={fieldState.error?.message}
                  InputLabelProps={{ shrink: true }}
                />
              )}
            />
            <Box sx={{ gridColumn: '1 / -1' }}>
              <Controller
                name="reasonNotes"
                control={form.control}
                render={({ field, fieldState }) => (
                  <TextField
                    {...field}
                    label="Remarks"
                    size="small"
                    fullWidth
                    multiline
                    minRows={2}
                    disabled={locked}
                    error={!!fieldState.error}
                    helperText={fieldState.error?.message}
                  />
                )}
              />
            </Box>
          </FormGroup>

          <Stack direction="row" alignItems="center" justifyContent="space-between">
            <Typography variant="subtitle1">Lines</Typography>
            {!locked ? (
              <TriVitaButton
                size="small"
                startIcon={<Add />}
                onClick={() =>
                  append({
                    clientKey: `n-${Date.now()}`,
                    medicineId: '',
                    medicineBatchId: '',
                    quantityDelta: '',
                    notes: '',
                  })
                }
              >
                Add row
              </TriVitaButton>
            ) : null}
          </Stack>

          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Medicine</TableCell>
                <TableCell>Batch</TableCell>
                <TableCell>Expiry</TableCell>
                <TableCell align="right">Current qty</TableCell>
                <TableCell align="right">Adjust qty</TableCell>
                <TableCell>Reason</TableCell>
                <TableCell width={48} />
              </TableRow>
            </TableHead>
            <TableBody>
              {fields.length === 0 && locked ? (
                <TableRow>
                  <TableCell colSpan={7}>
                    <Typography variant="body2" color="text.secondary">
                      No lines.
                    </Typography>
                  </TableCell>
                </TableRow>
              ) : null}
              {fields.map((f, idx) => {
                const line = watchedItems[idx];
                const mid = line?.medicineId;
                const opts = mid ? batchOptions[mid] ?? [] : [];
                const meta = lineMeta[line?.clientKey ?? ''] ?? { expiry: '', currentQty: '' };
                return (
                  <TableRow key={f.id}>
                    <TableCell sx={{ minWidth: 180 }}>
                      <Controller
                        name={`items.${idx}.medicineId`}
                        control={form.control}
                        render={({ field: fld, fieldState }) => (
                          <TextField
                            select
                            size="small"
                            fullWidth
                            {...fld}
                            disabled={locked}
                            onChange={(e) => {
                              fld.onChange(e);
                              form.setValue(`items.${idx}.medicineBatchId`, '');
                            }}
                            error={!!fieldState.error}
                            helperText={fieldState.error?.message}
                          >
                            {(medicineOpts.data ?? []).map((o) => (
                              <MenuItem key={o.value} value={o.value}>
                                {o.label}
                              </MenuItem>
                            ))}
                          </TextField>
                        )}
                      />
                    </TableCell>
                    <TableCell sx={{ minWidth: 140 }}>
                      <Controller
                        name={`items.${idx}.medicineBatchId`}
                        control={form.control}
                        render={({ field: fld, fieldState }) => (
                          <TextField
                            select
                            size="small"
                            fullWidth
                            {...fld}
                            disabled={locked || !mid}
                            error={!!fieldState.error}
                            helperText={fieldState.error?.message}
                          >
                            {opts.map((o) => (
                              <MenuItem key={o.value} value={o.value}>
                                {o.label}
                              </MenuItem>
                            ))}
                          </TextField>
                        )}
                      />
                    </TableCell>
                    <TableCell>{meta.expiry || '—'}</TableCell>
                    <TableCell align="right">{meta.currentQty || '—'}</TableCell>
                    <TableCell align="right" sx={{ maxWidth: 120 }}>
                      <Controller
                        name={`items.${idx}.quantityDelta`}
                        control={form.control}
                        render={({ field: fld, fieldState }) => (
                          <TextField
                            size="small"
                            type="number"
                            fullWidth
                            {...fld}
                            disabled={locked}
                            error={!!fieldState.error}
                            helperText={fieldState.error?.message}
                          />
                        )}
                      />
                    </TableCell>
                    <TableCell>
                      <Controller
                        name={`items.${idx}.notes`}
                        control={form.control}
                        render={({ field: fld }) => <TextField size="small" fullWidth {...fld} disabled={locked} />}
                      />
                    </TableCell>
                    <TableCell>
                      {!locked && fields.length > 1 ? (
                        <IconButton size="small" color="error" onClick={() => remove(idx)}>
                          <Delete fontSize="small" />
                        </IconButton>
                      ) : null}
                    </TableCell>
                  </TableRow>
                );
              })}
            </TableBody>
          </Table>
          {form.formState.errors.items && typeof form.formState.errors.items.message === 'string' ? (
            <Typography color="error" variant="caption">
              {form.formState.errors.items.message}
            </Typography>
          ) : null}
        </Stack>
      </TriVitaModal>

      <TriVitaModal
        open={deleteId != null}
        onClose={() => setDeleteId(null)}
        title="Delete adjustment?"
        actions={
          <>
            <TriVitaButton onClick={() => setDeleteId(null)}>Cancel</TriVitaButton>
            <TriVitaButton color="error" variant="contained" disabled={delMut.isPending} onClick={() => deleteId && delMut.mutate(deleteId)}>
              Delete
            </TriVitaButton>
          </>
        }
      >
        <Typography variant="body2">This removes the draft and its lines.</Typography>
      </TriVitaModal>
    </Stack>
  );
}
