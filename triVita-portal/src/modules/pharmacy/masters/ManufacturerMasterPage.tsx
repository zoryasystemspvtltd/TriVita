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
  createManufacturer,
  deleteManufacturer,
  getManufacturerById,
  getManufacturerPaged,
  updateManufacturer,
} from '@/services/pharmacyService';
import { getApiErrorMessage } from '@/utils/errorMap';

/** PhrManufacturer / ManufacturerResponseDto: ManufacturerCode?, ManufacturerName, CountryCode? */
export type ManufacturerRow = {
  id: number;
  manufacturerCode?: string | null;
  manufacturerName: string;
  countryCode?: string | null;
};

type FormValues = {
  manufacturerCode: string;
  manufacturerName: string;
  countryCode: string;
};

const emptyForm: FormValues = {
  manufacturerCode: '',
  manufacturerName: '',
  countryCode: '',
};

const schema = Yup.object({
  manufacturerName: Yup.string().trim().required('Name is required').max(250),
  manufacturerCode: Yup.string().trim().max(80, 'Max 80 characters').default(''),
  countryCode: Yup.string().trim().max(10, 'Max 10 characters').default(''),
});

function toRow(raw: Record<string, unknown>): ManufacturerRow {
  return {
    id: Number(raw.id),
    manufacturerCode: raw.manufacturerCode != null ? String(raw.manufacturerCode) : undefined,
    manufacturerName: String(raw.manufacturerName ?? ''),
    countryCode: raw.countryCode != null ? String(raw.countryCode) : undefined,
  };
}

function toFormValues(row: ManufacturerRow | Record<string, unknown>): FormValues {
  const r =
    'manufacturerName' in row
      ? (row as ManufacturerRow)
      : toRow(row as Record<string, unknown>);
  return {
    manufacturerCode: r.manufacturerCode != null ? String(r.manufacturerCode) : '',
    manufacturerName: r.manufacturerName,
    countryCode: r.countryCode != null ? String(r.countryCode) : '',
  };
}

function toPayload(v: FormValues) {
  return {
    manufacturerName: v.manufacturerName.trim(),
    manufacturerCode: v.manufacturerCode.trim() || undefined,
    countryCode: v.countryCode.trim() || undefined,
  };
}

export function ManufacturerMasterPage() {
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
    queryKey: ['pharmacy', 'manufacturer-master', page, pageSize, searchApplied],
    queryFn: () => getManufacturerPaged({ page: page + 1, pageSize, search: searchApplied || undefined }),
  });

  const detail = useQuery({
    queryKey: ['pharmacy', 'manufacturer-master', 'detail', drawerId],
    queryFn: () => getManufacturerById(drawerId!),
    enabled: drawerId != null,
  });

  const editFormQuery = useQuery({
    queryKey: [
      'pharmacy',
      'manufacturer-master',
      'form-seed',
      modal != null && modal.mode === 'edit' ? modal.id : null,
    ],
    queryFn: () => {
      if (modal?.mode !== 'edit') throw new Error('No edit');
      return getManufacturerById(modal.id);
    },
    enabled: modal?.mode === 'edit',
  });

  const rows = useMemo(() => {
    if (!list.data?.success || !list.data.data) return [] as ManufacturerRow[];
    return list.data.data.items.map((r) => toRow(r as Record<string, unknown>));
  }, [list.data]);
  const total = list.data?.success && list.data.data ? list.data.data.totalCount : 0;

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
      if (args.editId != null) return updateManufacturer(args.editId, body);
      return createManufacturer(body);
    },
    onSuccess: (res, vars) => {
      if (!res.success) {
        showToast(res.message ?? 'Save failed', 'error');
        return;
      }
      showToast(vars.editId != null ? 'Manufacturer updated' : 'Manufacturer created', 'success');
      setModal(null);
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'manufacturer-master'] });
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'manufacturer'] });
    },
    onError: (e) => showToast(getApiErrorMessage(e), 'error'),
  });

  const delMut = useMutation({
    mutationFn: (id: number) => deleteManufacturer(id),
    onSuccess: (res) => {
      if (!res.success) {
        showToast(res.message ?? 'Delete failed', 'error');
        return;
      }
      showToast('Manufacturer deleted', 'success');
      setDeleteId(null);
      setDrawerId(null);
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'manufacturer-master'] });
      void qc.invalidateQueries({ queryKey: ['pharmacy', 'manufacturer'] });
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
      <PageHeader title="Manufacturer Master" />

      <SectionContainer title="Manufacturers">
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
                Add Manufacturer
              </TriVitaButton>
            </Box>
          </Stack>

          <DataTable<ManufacturerRow>
            tableAriaLabel="Manufacturer master"
            columns={[
              {
                id: 'manufacturerName',
                label: 'Name',
                minWidth: 200,
                format: (row) => (
                  <Typography variant="body2" fontWeight={500}>
                    {row.manufacturerName || '—'}
                  </Typography>
                ),
              },
              { id: 'manufacturerCode', label: 'Code', minWidth: 120, format: (r) => r.manufacturerCode?.trim() || '—' },
              { id: 'countryCode', label: 'Country', minWidth: 100, format: (r) => r.countryCode?.trim() || '—' },
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
            totalCount={total}
            page={page}
            pageSize={pageSize}
            onPageChange={(p, ps) => {
              setPage(p);
              setPageSize(ps);
            }}
            loading={list.isLoading}
            emptyTitle="No manufacturers found"
            onRowClick={(row) => {
              if (Number.isFinite(row.id)) setDrawerId(row.id);
            }}
          />
        </Stack>
      </SectionContainer>

      <DetailDrawer
        open={drawerId != null}
        onClose={() => setDrawerId(null)}
        title={detailData ? detailData.manufacturerName : 'Manufacturer'}
        subtitle={detailData ? detailData.manufacturerCode?.trim() || undefined : undefined}
      >
        {detail.isLoading ? <Typography color="text.secondary">Loading…</Typography> : null}
        {detailData ? (
          <Stack spacing={1}>
            <DetailKv label="Name" value={detailData.manufacturerName} />
            <DetailKv label="Code" value={detailData.manufacturerCode?.trim() ? detailData.manufacturerCode : '—'} />
            <DetailKv label="Country code" value={detailData.countryCode?.trim() ? detailData.countryCode : '—'} />
          </Stack>
        ) : detail.data && !detail.data.success ? (
          <Typography color="warning.main">{detail.data.message}</Typography>
        ) : null}
      </DetailDrawer>

      <TriVitaModal
        open={Boolean(modal)}
        onClose={() => setModal(null)}
        title={modal?.mode === 'edit' ? 'Edit manufacturer' : 'Add manufacturer'}
        actions={
          <Stack direction="row" spacing={2} justifyContent="flex-end" sx={{ width: '100%' }}>
            <TriVitaButton onClick={() => setModal(null)}>Cancel</TriVitaButton>
            <TriVitaButton
              type="submit"
              form="manufacturer-master-form"
              variant="contained"
              disabled={saveMut.isPending || (modal?.mode === 'edit' && editFormQuery.isLoading)}
            >
              Save
            </TriVitaButton>
          </Stack>
        }
      >
        <Box component="form" id="manufacturer-master-form" onSubmit={handleSubmit(onSave)} noValidate>
          {modal?.mode === 'edit' && editFormQuery.isLoading ? (
            <Typography color="text.secondary" sx={{ mb: 2 }}>
              Loading manufacturer…
            </Typography>
          ) : null}
          <FormGroup>
            <Grid item xs={12} md={6}>
              <Controller
                name="manufacturerName"
                control={control}
                render={({ field }) => (
                  <TriVitaTextField
                    {...field}
                    label="Name"
                    required
                    error={Boolean(errors.manufacturerName)}
                    helperText={errors.manufacturerName?.message}
                  />
                )}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <Controller
                name="manufacturerCode"
                control={control}
                render={({ field }) => (
                  <TriVitaTextField
                    {...field}
                    label="Code"
                    error={Boolean(errors.manufacturerCode)}
                    helperText={errors.manufacturerCode?.message}
                  />
                )}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <Controller
                name="countryCode"
                control={control}
                render={({ field }) => (
                  <TriVitaTextField
                    {...field}
                    label="Country code"
                    error={Boolean(errors.countryCode)}
                    helperText={errors.countryCode?.message}
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
        title="Delete manufacturer"
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
        <Typography>Delete this manufacturer permanently? This cannot be undone.</Typography>
      </TriVitaModal>
    </Stack>
  );
}
