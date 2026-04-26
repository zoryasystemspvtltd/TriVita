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
  createMedicineForm,
  deleteMedicineForm,
  getMedicineFormById,
  getMedicineFormPaged,
  updateMedicineForm,
} from '@/services/pharmacyService';
import { getApiErrorMessage } from '@/utils/errorMap';

export type MedicineFormRow = {
  id: number;
  formCode: string;
  formName: string;
  description?: string | null;
};

type FormValues = {
  formCode: string;
  formName: string;
  description: string;
};

const emptyForm: FormValues = {
  formCode: '',
  formName: '',
  description: '',
};

const schema = Yup.object({
  formName: Yup.string()
    .trim()
    .required('Name is required')
    .max(300)
    .matches(/[A-Za-z]/, 'Name must include a letter')
    .matches(/^[A-Za-z]/, 'Name must start with a letter')
    .test('no-long-digit-run', 'Name must not contain three or more consecutive digits', (v) => !v || !/\d{3,}/.test(v)),
  formCode: Yup.string()
    .trim()
    .required('Code is required')
    .max(80)
    .matches(/^[A-Z][A-Z0-9_-]*$/, 'Use uppercase letters, digits, hyphen or underscore (e.g. TAB, SYR)'),
  description: Yup.string().trim().max(1000, 'Max 1000 characters').default(''),
});

function toRow(raw: Record<string, unknown>): MedicineFormRow {
  return {
    id: Number(raw.id),
    formCode: String(raw.formCode ?? raw.FormCode ?? ''),
    formName: String(raw.formName ?? raw.FormName ?? ''),
    description: raw.description != null ? String(raw.description) : undefined,
  };
}

function toFormValues(row: MedicineFormRow | Record<string, unknown>): FormValues {
  const r =
    'formCode' in row && 'formName' in row ? (row as MedicineFormRow) : toRow(row as Record<string, unknown>);
  return {
    formCode: r.formCode,
    formName: r.formName,
    description: r.description != null ? String(r.description) : '',
  };
}

function toPayload(v: FormValues) {
  return {
    formCode: v.formCode.trim(),
    formName: v.formName.trim(),
    description: v.description.trim() || undefined,
  };
}

export function FormMasterPage() {
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
    queryKey: ['pharmacy', 'form-master', 'paged', page, pageSize],
    queryFn: () => getMedicineFormPaged({ page: page + 1, pageSize }),
    enabled: !hasSearch,
  });

  const searchSource = useQuery({
    queryKey: ['pharmacy', 'form-master', 'search-source'],
    queryFn: async () => {
      const acc: MedicineFormRow[] = [];
      const ps = 200;
      let p = 1;
      for (;;) {
        const res = await getMedicineFormPaged({ page: p, pageSize: ps });
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
    queryKey: ['pharmacy', 'form-master', 'detail', drawerId],
    queryFn: () => getMedicineFormById(drawerId!),
    enabled: drawerId != null,
  });

  const editFormQuery = useQuery({
    queryKey: ['pharmacy', 'form-master', 'form-seed', modal != null && modal.mode === 'edit' ? modal.id : null],
    queryFn: () => {
      if (modal?.mode !== 'edit') throw new Error('No edit');
      return getMedicineFormById(modal.id);
    },
    enabled: modal?.mode === 'edit',
  });

  const pagedRows = useMemo(() => {
    if (!list.data?.success || !list.data.data) return [] as MedicineFormRow[];
    return list.data.data.items.map((r) => toRow(r as Record<string, unknown>));
  }, [list.data]);

  const filteredData = useMemo(() => {
    if (!hasSearch) return [] as MedicineFormRow[];
    const base = searchSource.data ?? [];
    const q = searchTrim.toLowerCase();
    return base.filter((item) => {
      const name = item.formName.toLowerCase();
      const code = item.formCode.toLowerCase();
      return name.includes(q) || code.includes(q);
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
      if (args.editId != null) return updateMedicineForm(args.editId, body);
      return createMedicineForm(body);
    },
    onSuccess: (res, vars) => {
      if (!res.success) {
        showToast(res.message ?? 'Save failed', 'error');
        return;
      }
      showToast(vars.editId != null ? 'Form updated' : 'Form created', 'success');
      setModal(null);
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'form-master'] });
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'medicine-form'] });
    },
    onError: (e) => showToast(getApiErrorMessage(e), 'error'),
  });

  const delMut = useMutation({
    mutationFn: (id: number) => deleteMedicineForm(id),
    onSuccess: (res) => {
      if (!res.success) {
        showToast(res.message ?? 'Delete failed', 'error');
        return;
      }
      showToast('Form deleted', 'success');
      setDeleteId(null);
      setDrawerId(null);
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'form-master'] });
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'medicine-form'] });
    },
    onError: (e) => showToast(getApiErrorMessage(e), 'error'),
  });

  const detailData =
    detail.data?.success && detail.data.data ? toRow(detail.data.data as Record<string, unknown>) : null;
  const onSave = (values: FormValues) => {
    const eid = modal != null && modal.mode === 'edit' ? modal.id : undefined;
    saveMut.mutate({ values, editId: eid });
  };

  return (
    <Stack spacing={3}>
      <PageHeader title="Form Master" />

      <SectionContainer title="Dosage forms">
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
                Add Form
              </TriVitaButton>
            </Box>
          </Stack>

          <DataTable<MedicineFormRow>
            key={hasSearch ? `form-search-${searchTrim}` : `form-paged-${page}-${pageSize}`}
            tableAriaLabel="Form master"
            columns={[
              {
                id: 'formName',
                label: 'Form name',
                minWidth: 200,
                format: (row) => (
                  <Typography variant="body2" fontWeight={500}>
                    {row.formName || '—'}
                  </Typography>
                ),
              },
              { id: 'formCode', label: 'Code', minWidth: 120 },
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
            emptyTitle="No forms found"
            onRowClick={(row) => {
              if (Number.isFinite(row.id)) setDrawerId(row.id);
            }}
          />
        </Stack>
      </SectionContainer>

      <DetailDrawer
        open={drawerId != null}
        onClose={() => setDrawerId(null)}
        title={detailData ? detailData.formName : 'Form'}
        subtitle={detailData ? detailData.formCode : undefined}
      >
        {detail.isLoading ? <Typography color="text.secondary">Loading…</Typography> : null}
        {detailData ? (
          <Stack spacing={1}>
            <DetailKv label="Name" value={detailData.formName} />
            <DetailKv label="Code" value={detailData.formCode} />
            <DetailKv label="Description" value={detailData.description?.trim() ? detailData.description : '—'} />
          </Stack>
        ) : detail.data && !detail.data.success ? (
          <Typography color="warning.main">{detail.data.message}</Typography>
        ) : null}
      </DetailDrawer>

      <TriVitaModal
        open={Boolean(modal)}
        onClose={() => setModal(null)}
        title={modal?.mode === 'edit' ? 'Edit form' : 'Add form'}
        actions={
          <Stack direction="row" spacing={2} justifyContent="flex-end" sx={{ width: '100%' }}>
            <TriVitaButton onClick={() => setModal(null)}>Cancel</TriVitaButton>
            <TriVitaButton
              type="submit"
              form="form-master-form"
              variant="contained"
              disabled={saveMut.isPending || (modal?.mode === 'edit' && editFormQuery.isLoading)}
            >
              Save
            </TriVitaButton>
          </Stack>
        }
      >
        <Box component="form" id="form-master-form" onSubmit={handleSubmit(onSave)} noValidate>
          {modal?.mode === 'edit' && editFormQuery.isLoading ? (
            <Typography color="text.secondary" sx={{ mb: 2 }}>
              Loading form…
            </Typography>
          ) : null}
          <FormGroup>
            <Grid item xs={12} md={6}>
              <Controller
                name="formName"
                control={control}
                render={({ field }) => (
                  <TriVitaTextField
                    {...field}
                    label="Name"
                    required
                    placeholder="e.g. Tablet, Syrup"
                    error={Boolean(errors.formName)}
                    helperText={errors.formName?.message}
                  />
                )}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <Controller
                name="formCode"
                control={control}
                render={({ field }) => (
                  <TriVitaTextField
                    {...field}
                    label="Code"
                    required
                    placeholder="e.g. TAB, SYR"
                    error={Boolean(errors.formCode)}
                    helperText={errors.formCode?.message}
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
        title="Delete form"
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
        <Typography>Delete this form permanently? Medicines referencing it may need to be updated first.</Typography>
      </TriVitaModal>
    </Stack>
  );
}
