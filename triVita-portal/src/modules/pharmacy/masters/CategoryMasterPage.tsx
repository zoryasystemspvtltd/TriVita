import { yupResolver } from '@hookform/resolvers/yup';
import { Box, Grid, Link, Stack, TextField, Typography } from '@mui/material';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useEffect, useMemo, useState } from 'react';
import { Controller, useForm, type Resolver } from 'react-hook-form';
import * as Yup from 'yup';
import { DataTable } from '@/components/common/DataTable';
import { DetailKv } from '@/components/masters/DetailKv';
import { DetailDrawer } from '@/components/layout/DetailDrawer';
import { PageHeader } from '@/components/layout/PageHeader';
import { FormGroup } from '@/components/ds/FormGroup';
import { TriVitaButton } from '@/components/ds/TriVitaButton';
import { TriVitaModal } from '@/components/ds/TriVitaModal';
import { TriVitaTextField } from '@/components/ds/TriVitaTextField';
import { SectionContainer } from '@/components/ds/SectionContainer';
import { useToast } from '@/components/toast/ToastProvider';
import {
  createMedicineCategory,
  deleteMedicineCategory,
  getMedicineCategoryById,
  getMedicineCategoryPaged,
  updateMedicineCategory,
} from '@/services/pharmacyService';
import { getApiErrorMessage } from '@/utils/errorMap';

/** Mirrors PharmacyService DTO/entity: CategoryCode, CategoryName, Description, EffectiveFrom, EffectiveTo */
export type CategoryRow = {
  id: number;
  categoryCode: string;
  categoryName: string;
  description?: string | null;
  effectiveFrom?: string | null;
  effectiveTo?: string | null;
};

type FormValues = {
  categoryCode: string;
  categoryName: string;
  description: string;
  effectiveFrom: string;
  effectiveTo: string;
};

const emptyForm: FormValues = {
  categoryCode: '',
  categoryName: '',
  description: '',
  effectiveFrom: '',
  effectiveTo: '',
};

function toDateInput(v: unknown): string {
  if (v == null || v === '') return '';
  const d = new Date(String(v));
  if (Number.isNaN(d.getTime())) return '';
  return d.toISOString().slice(0, 10);
}

function fromDateInput(v: string): string | undefined {
  const t = v.trim();
  if (!t) return undefined;
  const d = new Date(`${t}T00:00:00`);
  if (Number.isNaN(d.getTime())) return undefined;
  return d.toISOString();
}

function formatDateDisplay(v: unknown): string {
  if (v == null || v === '') return '—';
  const d = new Date(String(v));
  if (Number.isNaN(d.getTime())) return '—';
  return d.toLocaleDateString();
}

const schema = Yup.object({
  categoryCode: Yup.string().trim().required('Code is required').max(80),
  categoryName: Yup.string().trim().required('Name is required').max(250),
  description: Yup.string().trim().max(500, 'Max 500 characters').default(''),
  effectiveFrom: Yup.string().default(''),
  effectiveTo: Yup.string()
    .default('')
    .test('after-from', 'Effective to must be on or after effective from', function (val) {
      const from = (this.parent as FormValues).effectiveFrom?.trim();
      const to = val?.trim();
      if (!from || !to) return true;
      return to >= from;
    }),
});

function toRow(raw: Record<string, unknown>): CategoryRow {
  return {
    id: Number(raw.id),
    categoryCode: String(raw.categoryCode ?? ''),
    categoryName: String(raw.categoryName ?? ''),
    description: raw.description != null ? String(raw.description) : undefined,
    effectiveFrom: raw.effectiveFrom != null ? String(raw.effectiveFrom) : undefined,
    effectiveTo: raw.effectiveTo != null ? String(raw.effectiveTo) : undefined,
  };
}

function toFormValues(row: CategoryRow | Record<string, unknown>): FormValues {
  const r = 'categoryCode' in row && 'categoryName' in row ? (row as CategoryRow) : toRow(row as Record<string, unknown>);
  return {
    categoryCode: r.categoryCode,
    categoryName: r.categoryName,
    description: r.description != null ? String(r.description) : '',
    effectiveFrom: toDateInput(r.effectiveFrom),
    effectiveTo: toDateInput(r.effectiveTo),
  };
}

function toPayload(v: FormValues) {
  return {
    categoryCode: v.categoryCode.trim(),
    categoryName: v.categoryName.trim(),
    description: v.description.trim() || undefined,
    effectiveFrom: fromDateInput(v.effectiveFrom),
    effectiveTo: fromDateInput(v.effectiveTo),
  };
}

export function CategoryMasterPage() {
  const qc = useQueryClient();
  const { showToast } = useToast();
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const [search, setSearch] = useState('');
  const [searchApplied, setSearchApplied] = useState('');
  const [drawerId, setDrawerId] = useState<number | null>(null);
  const [modal, setModal] = useState<null | { mode: 'create' } | { mode: 'edit'; id: number }>(null);
  const [deleteId, setDeleteId] = useState<number | null>(null);

  const searchQ = searchApplied.trim();
  const hasSearch = searchQ.length > 0;

  useEffect(() => {
    const t = window.setTimeout(() => setSearchApplied(search), 400);
    return () => window.clearTimeout(t);
  }, [search]);

  useEffect(() => {
    setPage(0);
  }, [searchApplied]);

  const list = useQuery({
    queryKey: ['pharmacy', 'category-master', 'paged', page, pageSize],
    queryFn: () => getMedicineCategoryPaged({ page: page + 1, pageSize }),
    enabled: !hasSearch,
  });

  const allForFilter = useQuery({
    queryKey: ['pharmacy', 'category-master', 'all-for-search'],
    queryFn: async () => {
      const acc: CategoryRow[] = [];
      const ps = 200;
      let p = 1;
      for (;;) {
        const res = await getMedicineCategoryPaged({ page: p, pageSize: ps });
        if (!res.success || !res.data) break;
        const { items, totalCount } = res.data;
        for (const r of items) acc.push(toRow(r as Record<string, unknown>));
        if (acc.length >= totalCount || items.length === 0) break;
        p += 1;
      }
      return acc;
    },
    enabled: hasSearch,
  });

  const detail = useQuery({
    queryKey: ['pharmacy', 'category-master', 'detail', drawerId],
    queryFn: () => getMedicineCategoryById(drawerId!),
    enabled: drawerId != null,
  });

  const editFormQuery = useQuery({
    queryKey: [
      'pharmacy',
      'category-master',
      'form-seed',
      modal != null && modal.mode === 'edit' ? modal.id : null,
    ],
    queryFn: () => {
      if (modal?.mode !== 'edit') throw new Error('No edit');
      return getMedicineCategoryById(modal.id);
    },
    enabled: modal?.mode === 'edit',
  });

  const rows = useMemo(() => {
    if (hasSearch) {
      const all = allForFilter.data;
      if (!all) return [] as CategoryRow[];
      const q = searchQ.toLowerCase();
      return all.filter(
        (r) =>
          (r.categoryName && r.categoryName.toLowerCase().includes(q)) ||
          (r.categoryCode && r.categoryCode.toLowerCase().includes(q))
      );
    }
    if (!list.data?.success || !list.data.data) return [] as CategoryRow[];
    return list.data.data.items.map((r) => toRow(r as Record<string, unknown>));
  }, [hasSearch, allForFilter.data, searchQ, list.data]);
  const total = hasSearch
    ? undefined
    : list.data?.success && list.data.data
      ? list.data.data.totalCount
      : 0;
  const listLoading = hasSearch ? allForFilter.isLoading : list.isLoading;

  const {
    control,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<FormValues>({
    resolver: yupResolver(schema) as Resolver<FormValues>,
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
      const row = toRow(d.data as Record<string, unknown>);
      reset(toFormValues(row));
    }
  }, [modal, editFormQuery.data, reset]);

  const saveMut = useMutation({
    mutationFn: async (args: { values: FormValues; editId?: number }) => {
      const body = toPayload(args.values);
      if (args.editId != null) return updateMedicineCategory(args.editId, body);
      return createMedicineCategory(body);
    },
    onSuccess: (res, vars) => {
      if (!res.success) {
        showToast(res.message ?? 'Save failed', 'error');
        return;
      }
      showToast(vars.editId != null ? 'Category updated' : 'Category created', 'success');
      setModal(null);
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'category-master'] });
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'medicine-category'] });
    },
    onError: (e) => showToast(getApiErrorMessage(e), 'error'),
  });

  const delMut = useMutation({
    mutationFn: (id: number) => deleteMedicineCategory(id),
    onSuccess: (res) => {
      if (!res.success) {
        showToast(res.message ?? 'Delete failed', 'error');
        return;
      }
      showToast('Category deleted', 'success');
      setDeleteId(null);
      setDrawerId(null);
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'category-master'] });
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'medicine-category'] });
    },
    onError: (e) => showToast(getApiErrorMessage(e), 'error'),
  });

  const detailData = detail.data?.success && detail.data.data ? toRow(detail.data.data as Record<string, unknown>) : null;
  const onSave = (values: FormValues) => {
    const eid = modal != null && modal.mode === 'edit' ? modal.id : undefined;
    saveMut.mutate({ values, editId: eid });
  };

  return (
    <Stack spacing={3}>
      <PageHeader title="Category Master" />

      <SectionContainer title="Categories">
        <Stack spacing={2.5}>
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
                Add Category
              </TriVitaButton>
            </Box>
          </Stack>

          <DataTable<CategoryRow>
            key={hasSearch ? 'category-search' : 'category-paged'}
            tableAriaLabel="Category master"
            columns={[
              {
                id: 'categoryName',
                label: 'Name',
                minWidth: 200,
                format: (row) => (
                  <Typography variant="body2" fontWeight={500}>
                    {row.categoryName || '—'}
                  </Typography>
                ),
              },
              { id: 'categoryCode', label: 'Code', minWidth: 120 },
              {
                id: 'description',
                label: 'Description',
                minWidth: 220,
                format: (row) => (row.description?.trim() ? row.description : '—'),
              },
              {
                id: 'effectiveFrom',
                label: 'Effective from',
                minWidth: 120,
                format: (row) => formatDateDisplay(row.effectiveFrom),
              },
              {
                id: 'effectiveTo',
                label: 'Effective to',
                minWidth: 120,
                format: (row) => formatDateDisplay(row.effectiveTo),
              },
              {
                id: '_actions',
                label: 'Actions',
                minWidth: 200,
                format: (row) => {
                  const id = row.id;
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
            rowKey={(r) => r.id}
            {...(hasSearch
              ? {}
              : {
                  totalCount: total as number,
                  page,
                  pageSize,
                  onPageChange: (p: number, ps: number) => {
                    setPage(p);
                    setPageSize(ps);
                  },
                })}
            loading={listLoading}
            emptyTitle="No categories found"
            onRowClick={(row) => {
              if (Number.isFinite(row.id)) setDrawerId(row.id);
            }}
          />
        </Stack>
      </SectionContainer>

      <DetailDrawer
        open={drawerId != null}
        onClose={() => setDrawerId(null)}
        title={detailData ? detailData.categoryName : 'Category'}
        subtitle={detailData ? detailData.categoryCode : undefined}
      >
        {detail.isLoading ? <Typography color="text.secondary">Loading…</Typography> : null}
        {detailData ? (
          <Stack spacing={1}>
            <DetailKv label="Code" value={detailData.categoryCode} />
            <DetailKv label="Name" value={detailData.categoryName} />
            <DetailKv label="Description" value={detailData.description?.trim() ? detailData.description : '—'} />
            <DetailKv label="Effective from" value={formatDateDisplay(detailData.effectiveFrom)} />
            <DetailKv label="Effective to" value={formatDateDisplay(detailData.effectiveTo)} />
          </Stack>
        ) : detail.data && !detail.data.success ? (
          <Typography color="warning.main">{detail.data.message}</Typography>
        ) : null}
      </DetailDrawer>

      <TriVitaModal
        open={Boolean(modal)}
        onClose={() => setModal(null)}
        title={modal?.mode === 'edit' ? 'Edit category' : 'Add category'}
        actions={
          <Stack direction="row" spacing={2} justifyContent="flex-end" sx={{ width: '100%' }}>
            <TriVitaButton onClick={() => setModal(null)}>Cancel</TriVitaButton>
            <TriVitaButton
              type="submit"
              form="category-master-form"
              variant="contained"
              disabled={saveMut.isPending || (modal?.mode === 'edit' && editFormQuery.isLoading)}
            >
              Save
            </TriVitaButton>
          </Stack>
        }
      >
        <Box component="form" id="category-master-form" onSubmit={handleSubmit(onSave)} noValidate>
          {modal?.mode === 'edit' && editFormQuery.isLoading ? (
            <Typography color="text.secondary" sx={{ mb: 2 }}>
              Loading category…
            </Typography>
          ) : null}
          <FormGroup>
            <Grid item xs={12} md={6}>
              <Controller
                name="categoryName"
                control={control}
                render={({ field }) => (
                  <TriVitaTextField
                    {...field}
                    label="Name"
                    required
                    error={Boolean(errors.categoryName)}
                    helperText={errors.categoryName?.message}
                  />
                )}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <Controller
                name="categoryCode"
                control={control}
                render={({ field }) => (
                  <TriVitaTextField
                    {...field}
                    label="Code"
                    required
                    error={Boolean(errors.categoryCode)}
                    helperText={errors.categoryCode?.message}
                  />
                )}
              />
            </Grid>
            <Grid item xs={12}>
              <Controller
                name="description"
                control={control}
                render={({ field }) => (
                  <TriVitaTextField
                    {...field}
                    label="Description"
                    multiline
                    minRows={3}
                    error={Boolean(errors.description)}
                    helperText={errors.description?.message}
                  />
                )}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <Controller
                name="effectiveFrom"
                control={control}
                render={({ field }) => (
                  <TriVitaTextField
                    {...field}
                    label="Effective from"
                    type="date"
                    InputLabelProps={{ shrink: true }}
                    error={Boolean(errors.effectiveFrom)}
                    helperText={errors.effectiveFrom?.message}
                  />
                )}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <Controller
                name="effectiveTo"
                control={control}
                render={({ field }) => (
                  <TriVitaTextField
                    {...field}
                    label="Effective to"
                    type="date"
                    InputLabelProps={{ shrink: true }}
                    error={Boolean(errors.effectiveTo)}
                    helperText={errors.effectiveTo?.message}
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
        title="Delete category"
        actions={
          <Stack direction="row" spacing={2} justifyContent="flex-end" sx={{ width: '100%' }}>
            <TriVitaButton onClick={() => setDeleteId(null)}>Cancel</TriVitaButton>
            <TriVitaButton
              color="error"
              variant="contained"
              onClick={() => deleteId != null && delMut.mutate(deleteId)}
              disabled={delMut.isPending}
            >
              Delete
            </TriVitaButton>
          </Stack>
        }
      >
        <Typography>Delete this category permanently? This cannot be undone.</Typography>
      </TriVitaModal>
    </Stack>
  );
}
