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
  createMedicineUnit,
  deleteMedicineUnit,
  getMedicineUnitById,
  getMedicineUnitPaged,
  updateMedicineUnit,
} from '@/services/pharmacyService';
import { getApiErrorMessage } from '@/utils/errorMap';

export type UnitRow = {
  id: number;
  unitCode: string;
  unitName: string;
  unitSymbol: string;
  description?: string | null;
};

type FormValues = {
  unitCode: string;
  unitName: string;
  unitSymbol: string;
  description: string;
};

const emptyForm: FormValues = {
  unitCode: '',
  unitName: '',
  unitSymbol: '',
  description: '',
};

const schema = Yup.object({
  unitCode: Yup.string().trim().required('Code is required').max(80),
  unitName: Yup.string().trim().required('Name is required').max(200),
  unitSymbol: Yup.string().trim().required('Symbol is required').max(100),
  description: Yup.string().trim().max(500, 'Max 500 characters').default(''),
});

function toRow(raw: Record<string, unknown>): UnitRow {
  return {
    id: Number(raw.id),
    unitCode: String(raw.unitCode ?? raw.UnitCode ?? ''),
    unitName: String(raw.unitName ?? raw.UnitName ?? ''),
    unitSymbol: String(raw.unitSymbol ?? raw.UnitSymbol ?? ''),
    description: raw.description != null ? String(raw.description) : undefined,
  };
}

function toFormValues(row: UnitRow | Record<string, unknown>): FormValues {
  const r = 'unitCode' in row && 'unitName' in row ? (row as UnitRow) : toRow(row as Record<string, unknown>);
  return {
    unitCode: r.unitCode,
    unitName: r.unitName,
    unitSymbol: r.unitSymbol,
    description: r.description != null ? String(r.description) : '',
  };
}

function toPayload(v: FormValues) {
  return {
    unitCode: v.unitCode.trim(),
    unitName: v.unitName.trim(),
    unitSymbol: v.unitSymbol.trim(),
    description: v.description.trim() || undefined,
  };
}

export function UnitMasterPage() {
  const qc = useQueryClient();
  const { showToast } = useToast();
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const [search, setSearch] = useState('');
  const [drawerId, setDrawerId] = useState<number | null>(null);
  const [modal, setModal] = useState<null | { mode: 'create' } | { mode: 'edit'; id: number }>(null);
  const [deleteId, setDeleteId] = useState<number | null>(null);

  const searchTrim = search.trim();
  const hasSearch = searchTrim.length > 0;

  useEffect(() => {
    setPage(0);
  }, [searchTrim]);

  const list = useQuery({
    queryKey: ['pharmacy', 'unit-master', 'paged', page, pageSize],
    queryFn: () => getMedicineUnitPaged({ page: page + 1, pageSize }),
    enabled: !hasSearch,
  });

  const searchSource = useQuery({
    queryKey: ['pharmacy', 'unit-master', 'search-source'],
    queryFn: async () => {
      const acc: UnitRow[] = [];
      const ps = 200;
      let p = 1;
      for (;;) {
        const res = await getMedicineUnitPaged({ page: p, pageSize: ps });
        if (!res.success || !res.data) break;
        const { items, totalCount } = res.data;
        for (const r of items) acc.push(toRow(r as Record<string, unknown>));
        if (items.length === 0) break;
        if (typeof totalCount === 'number' && acc.length >= totalCount) break;
        p += 1;
      }
      return acc;
    },
    enabled: hasSearch,
  });

  const detail = useQuery({
    queryKey: ['pharmacy', 'unit-master', 'detail', drawerId],
    queryFn: () => getMedicineUnitById(drawerId!),
    enabled: drawerId != null,
  });

  const editFormQuery = useQuery({
    queryKey: ['pharmacy', 'unit-master', 'form-seed', modal != null && modal.mode === 'edit' ? modal.id : null],
    queryFn: () => {
      if (modal?.mode !== 'edit') throw new Error('No edit');
      return getMedicineUnitById(modal.id);
    },
    enabled: modal?.mode === 'edit',
  });

  const pagedRows = useMemo(() => {
    if (!list.data?.success || !list.data.data) return [] as UnitRow[];
    return list.data.data.items.map((r) => toRow(r as Record<string, unknown>));
  }, [list.data]);

  const filteredData = useMemo(() => {
    if (!hasSearch) return [] as UnitRow[];
    const base = searchSource.data ?? [];
    const q = searchTrim.toLowerCase();
    return base.filter((item) => {
      const name = item.unitName.toLowerCase();
      const code = item.unitCode.toLowerCase();
      const sym = item.unitSymbol.toLowerCase();
      const desc = (item.description ?? '').toLowerCase();
      return name.includes(q) || code.includes(q) || sym.includes(q) || desc.includes(q);
    });
  }, [hasSearch, searchSource.data, searchTrim]);

  const tableRows = hasSearch ? filteredData : pagedRows;
  const total = list.data?.success && list.data.data ? list.data.data.totalCount : 0;
  const listLoading = hasSearch ? searchSource.isLoading : list.isLoading;

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
      reset(toFormValues(toRow(d.data as Record<string, unknown>)));
    }
  }, [modal, editFormQuery.data, reset]);

  const saveMut = useMutation({
    mutationFn: async (args: { values: FormValues; editId?: number }) => {
      const body = toPayload(args.values);
      if (args.editId != null) return updateMedicineUnit(args.editId, body);
      return createMedicineUnit(body);
    },
    onSuccess: (res, vars) => {
      if (!res.success) {
        showToast(res.message ?? 'Save failed', 'error');
        return;
      }
      showToast(vars.editId != null ? 'Unit updated' : 'Unit created', 'success');
      setModal(null);
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'unit-master'] });
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'medicine-unit'] });
    },
    onError: (e) => showToast(getApiErrorMessage(e), 'error'),
  });

  const delMut = useMutation({
    mutationFn: (id: number) => deleteMedicineUnit(id),
    onSuccess: (res) => {
      if (!res.success) {
        showToast(res.message ?? 'Delete failed', 'error');
        return;
      }
      showToast('Unit deleted', 'success');
      setDeleteId(null);
      setDrawerId(null);
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'unit-master'] });
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'medicine-unit'] });
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
      <PageHeader title="Unit Master" />

      <SectionContainer title="Units">
        <Stack spacing={2.5}>
          <Stack
            direction={{ xs: 'column', sm: 'row' }}
            spacing={2}
            alignItems={{ sm: 'center' }}
            justifyContent="space-between"
          >
            <TextField
              size="small"
              label="Search (name / code / symbol / description)"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              sx={{ flex: 1, minWidth: 220, maxWidth: { sm: 480 } }}
            />
            <Box sx={{ display: 'flex', justifyContent: 'flex-end' }}>
              <TriVitaButton variant="contained" onClick={() => setModal({ mode: 'create' })}>
                Add Unit
              </TriVitaButton>
            </Box>
          </Stack>

          <DataTable<UnitRow>
            key={hasSearch ? `unit-search-${searchTrim}` : `unit-paged-${page}-${pageSize}`}
            tableAriaLabel="Unit master"
            columns={[
              {
                id: 'unitName',
                label: 'Name',
                minWidth: 200,
                format: (row) => (
                  <Typography variant="body2" fontWeight={500}>
                    {row.unitName || '—'}
                  </Typography>
                ),
              },
              { id: 'unitCode', label: 'Code', minWidth: 120 },
              {
                id: 'unitSymbol',
                label: 'Symbol',
                minWidth: 100,
                format: (row) => (row.unitSymbol?.trim() ? row.unitSymbol : '—'),
              },
              {
                id: 'description',
                label: 'Description',
                minWidth: 220,
                format: (row) => (row.description?.trim() ? row.description : '—'),
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
            rows={tableRows}
            rowKey={(r) => r.id}
            hidePagination={hasSearch}
            {...(hasSearch
              ? {}
              : {
                  totalCount: total,
                  page,
                  pageSize,
                  onPageChange: (p: number, ps: number) => {
                    setPage(p);
                    setPageSize(ps);
                  },
                })}
            loading={listLoading}
            emptyTitle="No units found"
            onRowClick={(row) => {
              if (Number.isFinite(row.id)) setDrawerId(row.id);
            }}
          />
        </Stack>
      </SectionContainer>

      <DetailDrawer
        open={drawerId != null}
        onClose={() => setDrawerId(null)}
        title={detailData ? detailData.unitName : 'Unit'}
        subtitle={detailData ? detailData.unitCode : undefined}
      >
        {detail.isLoading ? <Typography color="text.secondary">Loading…</Typography> : null}
        {detailData ? (
          <Stack spacing={1}>
            <DetailKv label="Code" value={detailData.unitCode} />
            <DetailKv label="Name" value={detailData.unitName} />
            <DetailKv label="Symbol" value={detailData.unitSymbol?.trim() ? detailData.unitSymbol : '—'} />
            <DetailKv label="Description" value={detailData.description?.trim() ? detailData.description : '—'} />
          </Stack>
        ) : detail.data && !detail.data.success ? (
          <Typography color="warning.main">{detail.data.message}</Typography>
        ) : null}
      </DetailDrawer>

      <TriVitaModal
        open={Boolean(modal)}
        onClose={() => setModal(null)}
        title={modal?.mode === 'edit' ? 'Edit unit' : 'Add unit'}
        actions={
          <Stack direction="row" spacing={2} justifyContent="flex-end" sx={{ width: '100%' }}>
            <TriVitaButton onClick={() => setModal(null)}>Cancel</TriVitaButton>
            <TriVitaButton
              type="submit"
              form="unit-master-form"
              variant="contained"
              disabled={saveMut.isPending || (modal?.mode === 'edit' && editFormQuery.isLoading)}
            >
              Save
            </TriVitaButton>
          </Stack>
        }
      >
        <Box component="form" id="unit-master-form" onSubmit={handleSubmit(onSave)} noValidate>
          {modal?.mode === 'edit' && editFormQuery.isLoading ? (
            <Typography color="text.secondary" sx={{ mb: 2 }}>
              Loading unit…
            </Typography>
          ) : null}
          <FormGroup>
            <Grid item xs={12} md={6}>
              <Controller
                name="unitName"
                control={control}
                render={({ field }) => (
                  <TriVitaTextField
                    {...field}
                    label="Name"
                    required
                    error={Boolean(errors.unitName)}
                    helperText={errors.unitName?.message}
                  />
                )}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <Controller
                name="unitCode"
                control={control}
                render={({ field }) => (
                  <TriVitaTextField
                    {...field}
                    label="Code"
                    required
                    error={Boolean(errors.unitCode)}
                    helperText={errors.unitCode?.message}
                  />
                )}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <Controller
                name="unitSymbol"
                control={control}
                render={({ field }) => (
                  <TriVitaTextField
                    {...field}
                    label="Symbol"
                    required
                    placeholder="e.g. mg, ml, tab"
                    error={Boolean(errors.unitSymbol)}
                    helperText={errors.unitSymbol?.message}
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
          </FormGroup>
        </Box>
      </TriVitaModal>

      <TriVitaModal
        open={deleteId != null}
        onClose={() => setDeleteId(null)}
        title="Delete unit"
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
        <Typography>Delete this unit permanently? This cannot be undone.</Typography>
      </TriVitaModal>
    </Stack>
  );
}
