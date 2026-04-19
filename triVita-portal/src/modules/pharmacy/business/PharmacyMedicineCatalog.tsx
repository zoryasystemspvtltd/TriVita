import { yupResolver } from '@hookform/resolvers/yup';
import { Box, Chip, Grid, Link, Stack, TextField, Typography } from '@mui/material';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useEffect, useMemo, useState } from 'react';
import { Controller, useForm, type Resolver } from 'react-hook-form';
import * as Yup from 'yup';
import { DataTable } from '@/components/common/DataTable';
import { DetailDrawer } from '@/components/layout/DetailDrawer';
import { PageHeader } from '@/components/layout/PageHeader';
import { FormGroup } from '@/components/ds/FormGroup';
import { TriVitaButton } from '@/components/ds/TriVitaButton';
import { TriVitaModal } from '@/components/ds/TriVitaModal';
import { TriVitaSelect } from '@/components/ds/TriVitaSelect';
import { TriVitaTextField } from '@/components/ds/TriVitaTextField';
import { useToast } from '@/components/toast/ToastProvider';
import {
  createMedicine,
  deleteMedicine,
  getMedicineById,
  getMedicineCategoryPaged,
  getMedicinePaged,
  updateMedicine,
} from '@/services/pharmacyService';
import { getApiErrorMessage } from '@/utils/errorMap';

type MedicineRow = Record<string, unknown> & { id?: number };

type MedicineFormValues = {
  medicineName: string;
  medicineCode: string;
  categoryId: string;
  defaultUnitId: string;
  description: string;
  status: 'active' | 'inactive';
};

const emptyForm: MedicineFormValues = {
  medicineName: '',
  medicineCode: '',
  categoryId: '',
  defaultUnitId: '',
  description: '',
  status: 'active',
};

const medicineSchema = Yup.object({
  medicineName: Yup.string().trim().max(256).required('Medicine name is required'),
  medicineCode: Yup.string().trim().max(64).required('Code is required'),
  categoryId: Yup.string().required('Category is required').matches(/^\d+$/, 'Select a category'),
  defaultUnitId: Yup.string().matches(/^$|^\d+$/, 'Unit must be numeric or empty'),
  description: Yup.string().max(2000),
  status: Yup.string().oneOf(['active', 'inactive']).required(),
});

function rowToFormValues(row: MedicineRow): MedicineFormValues {
  return {
    medicineName: String(row.medicineName ?? ''),
    medicineCode: String(row.medicineCode ?? ''),
    categoryId: row.categoryId != null ? String(row.categoryId) : '',
    defaultUnitId: row.defaultUnitId != null ? String(row.defaultUnitId) : '',
    description: String(row.notes ?? ''),
    status: row.isActive === false ? 'inactive' : 'active',
  };
}

function buildApiBody(v: MedicineFormValues) {
  const unit = v.defaultUnitId.trim();
  return {
    medicineCode: v.medicineCode.trim(),
    medicineName: v.medicineName.trim(),
    categoryId: Number(v.categoryId),
    defaultUnitId: unit ? Number(unit) : undefined,
    notes: v.description.trim() || undefined,
  };
}

function collectUnitIds(items: readonly MedicineRow[]): number[] {
  const s = new Set<number>();
  for (const r of items) {
    const u = r.defaultUnitId;
    if (u != null && Number.isFinite(Number(u))) s.add(Number(u));
  }
  return [...s].sort((a, b) => a - b);
}

function DetailField({ label, value }: { label: string; value: string }) {
  return (
    <Box sx={{ py: 0.5 }}>
      <Typography variant="caption" color="text.secondary" display="block" sx={{ mb: 0.5 }}>
        {label}
      </Typography>
      <Typography variant="body2">{value || '—'}</Typography>
    </Box>
  );
}

export function PharmacyMedicineCatalog() {
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
    queryKey: ['pharmacy', 'medicine', 'biz', page, pageSize, searchApplied],
    queryFn: () => getMedicinePaged({ page: page + 1, pageSize, search: searchApplied || undefined }),
  });

  const categories = useQuery({
    queryKey: ['pharmacy', 'medicine-category', 'options'],
    queryFn: async () => {
      const res = await getMedicineCategoryPaged({ page: 1, pageSize: 500 });
      return res;
    },
    staleTime: 5 * 60_000,
  });

  const unitSource = useQuery({
    queryKey: ['pharmacy', 'medicine', 'unit-options'],
    queryFn: () => getMedicinePaged({ page: 1, pageSize: 500 }),
    staleTime: 60_000,
  });

  const detail = useQuery({
    queryKey: ['pharmacy', 'medicine', 'detail', drawerId],
    queryFn: () => getMedicineById(drawerId!),
    enabled: drawerId != null,
  });

  const editFormQuery = useQuery({
    queryKey: ['pharmacy', 'medicine', 'form-seed', modal?.mode === 'edit' ? modal.id : null],
    queryFn: () => {
      if (modal?.mode !== 'edit') throw new Error('Edit form query without edit modal');
      return getMedicineById(modal.id);
    },
    enabled: modal?.mode === 'edit',
  });

  const rows = useMemo(
    () => (list.data?.success && list.data.data ? [...list.data.data.items] : []) as MedicineRow[],
    [list.data]
  );
  const total = list.data?.success && list.data.data ? list.data.data.totalCount : 0;

  const categoryMap = useMemo(() => {
    const m = new Map<number, string>();
    const data = categories.data?.success ? categories.data.data : null;
    const items = data?.items ?? [];
    for (const c of items as MedicineRow[]) {
      const id = Number(c.id);
      const name = String(c.categoryName ?? c.categoryCode ?? id);
      if (Number.isFinite(id)) m.set(id, name);
    }
    return m;
  }, [categories.data]);

  const categoryOptions = useMemo(() => {
    const data = categories.data?.success ? categories.data.data : null;
    const items = (data?.items ?? []) as MedicineRow[];
    const opts = items.map((c) => ({
      value: String(c.id ?? ''),
      label: String(c.categoryName ?? c.categoryCode ?? c.id),
    }));
    return [{ value: '', label: 'Select category' }, ...opts];
  }, [categories.data]);

  const editSeedRow = (editFormQuery.data?.success ? editFormQuery.data.data : null) as MedicineRow | null;

  const unitIds = useMemo(() => {
    const fromList = collectUnitIds(rows);
    const extra = unitSource.data?.success && unitSource.data.data ? collectUnitIds(unitSource.data.data.items as MedicineRow[]) : [];
    const merged = new Set([...fromList, ...extra]);
    if (editSeedRow?.defaultUnitId != null) merged.add(Number(editSeedRow.defaultUnitId));
    return [...merged].sort((a, b) => a - b);
  }, [rows, unitSource.data, editSeedRow]);

  const unitOptions = useMemo(
    () => [{ value: '', label: 'None' }, ...unitIds.map((id) => ({ value: String(id), label: `Unit #${id}` }))],
    [unitIds]
  );

  const {
    control,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<MedicineFormValues>({
    resolver: yupResolver(medicineSchema) as Resolver<MedicineFormValues>,
    defaultValues: emptyForm,
  });

  useEffect(() => {
    if (!modal) return;
    if (modal.mode === 'create') {
      reset(emptyForm);
      return;
    }
    const d = editFormQuery.data;
    if (d?.success && d.data) {
      reset(rowToFormValues(d.data as MedicineRow));
    }
  }, [modal, editFormQuery.data, reset]);

  const saveMut = useMutation({
    mutationFn: async (args: { values: MedicineFormValues; editId?: number }) => {
      const body = buildApiBody(args.values);
      if (args.editId != null) return updateMedicine(args.editId, body);
      return createMedicine(body);
    },
    onSuccess: (res, vars) => {
      if (!res.success) {
        showToast(res.message ?? 'Save failed', 'error');
        return;
      }
      showToast(vars.editId != null ? 'Medicine updated' : 'Medicine created', 'success');
      setModal(null);
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'medicine'] });
    },
    onError: (e) => showToast(getApiErrorMessage(e), 'error'),
  });

  const delMut = useMutation({
    mutationFn: (id: number) => deleteMedicine(id),
    onSuccess: (res) => {
      if (!res.success) {
        showToast(res.message ?? 'Delete failed', 'error');
        return;
      }
      showToast('Medicine deleted', 'success');
      setDeleteId(null);
      setDrawerId(null);
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'medicine'] });
    },
    onError: (e) => showToast(getApiErrorMessage(e), 'error'),
  });

  const detailRecord = (detail.data?.success ? detail.data.data : null) as MedicineRow | null;

  const onSave = (values: MedicineFormValues) => {
    const editId = modal?.mode === 'edit' ? modal.id : undefined;
    saveMut.mutate({ values, editId });
  };

  const statusChip = (row: MedicineRow) => {
    const active = row.isActive !== false;
    return <Chip size="small" label={active ? 'Active' : 'Inactive'} color={active ? 'success' : 'default'} variant="outlined" />;
  };

  return (
    <Stack spacing={2}>
      <PageHeader title="Medicine Master" />

      <Stack
        direction={{ xs: 'column', sm: 'row' }}
        spacing={2}
        alignItems={{ sm: 'center' }}
        justifyContent="space-between"
      >
        <TextField
          size="small"
          label="Search (name / code)"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          sx={{ flex: 1, minWidth: 220, maxWidth: { sm: 480 } }}
        />
        <Box sx={{ display: 'flex', justifyContent: 'flex-end' }}>
          <TriVitaButton variant="contained" onClick={() => setModal({ mode: 'create' })}>
            Add Medicine
          </TriVitaButton>
        </Box>
      </Stack>

      <DataTable<MedicineRow>
        tableAriaLabel="Medicine master"
        columns={[
          {
            id: 'medicineName',
            label: 'Name',
            minWidth: 200,
            format: (row) => (
              <Typography variant="body2" fontWeight={500}>
                {String(row.medicineName ?? '—')}
              </Typography>
            ),
          },
          { id: 'medicineCode', label: 'Code', minWidth: 120 },
          {
            id: 'categoryId',
            label: 'Category',
            minWidth: 160,
            format: (row) => {
              const id = Number(row.categoryId);
              return categoryMap.get(id) ?? (Number.isFinite(id) ? `ID ${id}` : '—');
            },
          },
          {
            id: 'defaultUnitId',
            label: 'Unit',
            minWidth: 100,
            format: (row) => (row.defaultUnitId != null ? `Unit #${row.defaultUnitId}` : '—'),
          },
          {
            id: 'status',
            label: 'Status',
            minWidth: 100,
            format: (row) => statusChip(row),
          },
          {
            id: '_actions',
            label: 'Actions',
            minWidth: 200,
            format: (row) => {
              const id = Number(row.id);
              return (
                <Stack direction="row" spacing={1} alignItems="center" flexWrap="wrap" useFlexGap>
                  <Link
                    component="button"
                    type="button"
                    variant="body2"
                    onClick={(e) => {
                      e.stopPropagation();
                      setDrawerId(id);
                    }}
                    sx={{ cursor: 'pointer' }}
                  >
                    View
                  </Link>
                  <Typography variant="body2" color="text.disabled">
                    |
                  </Typography>
                  <Link
                    component="button"
                    type="button"
                    variant="body2"
                    onClick={(e) => {
                      e.stopPropagation();
                      setModal({ mode: 'edit', id });
                    }}
                    sx={{ cursor: 'pointer' }}
                  >
                    Edit
                  </Link>
                  <Typography variant="body2" color="text.disabled">
                    |
                  </Typography>
                  <Link
                    component="button"
                    type="button"
                    variant="body2"
                    color="error"
                    onClick={(e) => {
                      e.stopPropagation();
                      setDeleteId(id);
                    }}
                    sx={{ cursor: 'pointer' }}
                  >
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
        emptyTitle="No medicines found"
        onRowClick={(row) => {
          const id = Number(row.id);
          if (Number.isFinite(id)) setDrawerId(id);
        }}
      />

      <DetailDrawer
        open={drawerId != null}
        onClose={() => setDrawerId(null)}
        title={detailRecord ? String(detailRecord.medicineName ?? 'Medicine') : 'Medicine'}
        subtitle={detailRecord ? `Code ${String(detailRecord.medicineCode ?? '')}` : undefined}
      >
        {detail.isLoading ? (
          <Typography color="text.secondary">Loading…</Typography>
        ) : null}
        {detailRecord ? (
          <Stack spacing={2}>
            <DetailField label="Medicine name" value={String(detailRecord.medicineName ?? '')} />
            <DetailField label="Code" value={String(detailRecord.medicineCode ?? '')} />
            <DetailField
              label="Category"
              value={(() => {
                const id = Number(detailRecord.categoryId);
                return categoryMap.get(id) ?? (Number.isFinite(id) ? `ID ${id}` : '');
              })()}
            />
            <DetailField
              label="Unit"
              value={detailRecord.defaultUnitId != null ? `Unit #${detailRecord.defaultUnitId}` : ''}
            />
            <DetailField label="Strength" value={String(detailRecord.strength ?? '')} />
            <DetailField label="Manufacturer ID" value={detailRecord.manufacturerId != null ? String(detailRecord.manufacturerId) : ''} />
            <DetailField
              label="Form reference value ID"
              value={detailRecord.formReferenceValueId != null ? String(detailRecord.formReferenceValueId) : ''}
            />
            <DetailField label="Description" value={String(detailRecord.notes ?? '')} />
            <DetailField label="Status" value={detailRecord.isActive === false ? 'Inactive' : 'Active'} />
            <DetailField label="Record ID" value={String(detailRecord.id ?? '')} />
          </Stack>
        ) : detail.data && !detail.data.success ? (
          <Typography color="warning.main">{detail.data.message}</Typography>
        ) : null}
      </DetailDrawer>

      <TriVitaModal
        open={Boolean(modal)}
        onClose={() => setModal(null)}
        title={modal?.mode === 'edit' ? 'Edit medicine' : 'Add medicine'}
        actions={
          <Stack direction="row" spacing={2} justifyContent="flex-end" sx={{ width: '100%' }}>
            <TriVitaButton onClick={() => setModal(null)}>Cancel</TriVitaButton>
            <TriVitaButton
              type="submit"
              form="medicine-master-form"
              variant="contained"
              disabled={saveMut.isPending || (modal?.mode === 'edit' && editFormQuery.isLoading)}
            >
              Save
            </TriVitaButton>
          </Stack>
        }
      >
        <Box component="form" id="medicine-master-form" onSubmit={handleSubmit(onSave)} noValidate>
          {modal?.mode === 'edit' && editFormQuery.isLoading ? (
            <Typography color="text.secondary" sx={{ mb: 2 }}>
              Loading medicine…
            </Typography>
          ) : null}
          <FormGroup>
            <Grid item xs={12} md={6}>
              <Controller
                name="medicineName"
                control={control}
                render={({ field }) => (
                  <TriVitaTextField {...field} label="Medicine name" required error={Boolean(errors.medicineName)} helperText={errors.medicineName?.message} />
                )}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <Controller
                name="medicineCode"
                control={control}
                render={({ field }) => (
                  <TriVitaTextField {...field} label="Code" required error={Boolean(errors.medicineCode)} helperText={errors.medicineCode?.message} />
                )}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <Controller
                name="categoryId"
                control={control}
                render={({ field }) => (
                  <TriVitaSelect
                    label="Category"
                    required
                    value={field.value}
                    onChange={(e) => field.onChange(e.target.value)}
                    options={categoryOptions}
                    error={Boolean(errors.categoryId)}
                    helperText={errors.categoryId?.message ?? (categories.isLoading ? 'Loading categories…' : undefined)}
                    disabled={categories.isLoading}
                  />
                )}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <Controller
                name="defaultUnitId"
                control={control}
                render={({ field }) => (
                  <TriVitaSelect
                    label="Unit"
                    value={field.value}
                    onChange={(e) => field.onChange(String(e.target.value))}
                    options={unitOptions}
                    error={Boolean(errors.defaultUnitId)}
                    helperText={errors.defaultUnitId?.message}
                  />
                )}
              />
            </Grid>
            <Grid item xs={12}>
              <Controller
                name="description"
                control={control}
                render={({ field }) => (
                  <TriVitaTextField {...field} label="Description" multiline minRows={3} error={Boolean(errors.description)} helperText={errors.description?.message} />
                )}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <Controller
                name="status"
                control={control}
                render={({ field }) => (
                  <TriVitaSelect
                    label="Status"
                    value={field.value}
                    onChange={(e) => field.onChange(e.target.value)}
                    options={[
                      { value: 'active', label: 'Active' },
                      { value: 'inactive', label: 'Inactive' },
                    ]}
                    error={Boolean(errors.status)}
                    helperText={errors.status?.message}
                  />
                )}
              />
            </Grid>
          </FormGroup>
        </Box>
      </TriVitaModal>

      <TriVitaModal
        open={deleteId != null}
        onClose={() => setDeleteId(null)}
        title="Delete medicine"
        actions={
          <Stack direction="row" spacing={2} justifyContent="flex-end" sx={{ width: '100%' }}>
            <TriVitaButton onClick={() => setDeleteId(null)}>Cancel</TriVitaButton>
            <TriVitaButton color="error" variant="contained" onClick={() => deleteId != null && delMut.mutate(deleteId)} disabled={delMut.isPending}>
              Delete
            </TriVitaButton>
          </Stack>
        }
      >
        <Typography>Delete this medicine permanently? This cannot be undone.</Typography>
      </TriVitaModal>
    </Stack>
  );
}
