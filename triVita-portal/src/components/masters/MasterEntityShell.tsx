import { yupResolver } from '@hookform/resolvers/yup';
import { Box, Grid, Link, Stack, TextField, Typography } from '@mui/material';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useEffect, useMemo, useRef, useState, type ReactNode } from 'react';
import { Controller, useForm, type Resolver } from 'react-hook-form';
import type { AnyObjectSchema } from 'yup';
import { DataTable } from '@/components/common/DataTable';
import { LookupSelect } from '@/components/common/LookupSelect';
import { DetailDrawer } from '@/components/layout/DetailDrawer';
import { PageHeader } from '@/components/layout/PageHeader';
import { FormGroup } from '@/components/ds/FormGroup';
import { TriVitaButton } from '@/components/ds/TriVitaButton';
import { TriVitaModal } from '@/components/ds/TriVitaModal';
import { TriVitaSelect } from '@/components/ds/TriVitaSelect';
import { TriVitaTextField } from '@/components/ds/TriVitaTextField';
import { useToast } from '@/components/toast/ToastProvider';
import { getAxiosForModule } from '@/services/moduleClients';
import type { BaseResponse, PagedQueryParams, PagedResponse } from '@/types/api';
import { getApiErrorMessage } from '@/utils/errorMap';

export type MasterFieldShowOn = 'create' | 'edit' | 'both';

type BaseField = {
  name: string;
  label: string;
  required?: boolean;
  gridCols?: number;
  showOn?: MasterFieldShowOn;
};

export type MasterSectionField = {
  kind: 'section';
  name: string;
  label: string;
  showOn?: MasterFieldShowOn;
};

export type MasterTextField = BaseField & { kind: 'text'; readOnlyOnEdit?: boolean };
export type MasterTextAreaField = BaseField & { kind: 'textarea'; minRows?: number };
export type MasterNumberField = BaseField & { kind: 'number'; integer?: boolean };
export type MasterDateField = BaseField & { kind: 'date' };
export type MasterSelectField = BaseField & {
  kind: 'select';
  options: readonly { value: string; label: string }[];
};
export type MasterLookupField = BaseField & {
  kind: 'lookup';
  queryKey: readonly unknown[];
  loadOptions: (ctx: { editId: number | null }) => Promise<readonly { value: string; label: string }[]>;
  allowNone?: boolean;
  noneLabel?: string;
};

export type MasterField =
  | MasterSectionField
  | MasterTextField
  | MasterTextAreaField
  | MasterNumberField
  | MasterDateField
  | MasterSelectField
  | MasterLookupField;

export type MasterTableColumn<T extends Record<string, unknown> = Record<string, unknown>> = {
  id: string;
  label: string;
  minWidth?: number;
  format?: (row: T) => ReactNode;
};

type ModalState = null | { mode: 'create' } | { mode: 'edit'; id: number };

function fieldVisible(f: MasterField, mode: 'create' | 'edit'): boolean {
  if (!f.showOn || f.showOn === 'both') return true;
  return f.showOn === mode;
}

export function MasterEntityShell<T extends Record<string, unknown> = Record<string, unknown>>({
  module,
  resourcePath,
  title,
  schema,
  defaultCreateValues,
  fields,
  columns,
  rowToFormValues,
  toCreatePayload,
  toUpdatePayload,
  renderDetail,
  getDrawerTitle,
  getDrawerSubtitle,
  beforeSave,
  clientListSearch,
  searchFieldLabel,
}: {
  module: 'pharmacy' | 'lis' | 'lms' | 'hms';
  resourcePath: string;
  title: string;
  schema: AnyObjectSchema;
  defaultCreateValues: Record<string, string>;
  fields: readonly MasterField[];
  columns: readonly MasterTableColumn<T>[];
  rowToFormValues: (row: T) => Record<string, string>;
  toCreatePayload: (values: Record<string, string>) => Record<string, unknown>;
  toUpdatePayload: (values: Record<string, string>) => Record<string, unknown>;
  renderDetail: (row: T) => ReactNode;
  getDrawerTitle: (row: T) => string;
  getDrawerSubtitle?: (row: T) => string | undefined;
  beforeSave?: (values: Record<string, string>, mode: 'create' | 'edit') => string | null;
  /** Pharmacy-style masters: fetch all pages while searching, filter client-side (no API `search` param). */
  clientListSearch?: (row: T, queryLower: string) => boolean;
  searchFieldLabel?: string;
}) {
  const client = getAxiosForModule(module);
  const basePath = `/api/v1/${resourcePath}`;
  const qc = useQueryClient();
  const { showToast } = useToast();
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const [search, setSearch] = useState('');
  const [searchApplied, setSearchApplied] = useState('');
  const [drawerRow, setDrawerRow] = useState<T | null>(null);
  const [modal, setModal] = useState<ModalState>(null);
  const [deleteId, setDeleteId] = useState<number | null>(null);

  const useClientListSearch = clientListSearch != null;
  const searchTrim = search.trim();
  const hasSearch = useClientListSearch && searchTrim.length > 0;

  const rowToFormValuesRef = useRef(rowToFormValues);
  rowToFormValuesRef.current = rowToFormValues;

  useEffect(() => {
    if (useClientListSearch) return;
    const t = window.setTimeout(() => setSearchApplied(search), 400);
    return () => window.clearTimeout(t);
  }, [search, useClientListSearch]);

  useEffect(() => {
    if (useClientListSearch) return;
    setPage(0);
  }, [searchApplied, useClientListSearch]);

  useEffect(() => {
    if (!useClientListSearch) return;
    setPage(0);
  }, [searchTrim, useClientListSearch]);

  const listQuery = useQuery({
    queryKey: useClientListSearch
      ? ['master', module, resourcePath, 'paged', page, pageSize]
      : ['master', module, resourcePath, page, pageSize, searchApplied],
    queryFn: async () => {
      const params: PagedQueryParams = {
        page: page + 1,
        pageSize,
        ...(useClientListSearch ? {} : { search: searchApplied.trim() || undefined }),
      };
      const { data } = await client.get<BaseResponse<PagedResponse<T>>>(basePath, { params });
      return data;
    },
    enabled: useClientListSearch ? !hasSearch : true,
  });

  const searchSource = useQuery({
    queryKey: ['master', module, resourcePath, 'search-source'],
    queryFn: async () => {
      const acc: T[] = [];
      const ps = 200;
      let p = 1;
      for (;;) {
        const { data } = await client.get<BaseResponse<PagedResponse<T>>>(basePath, {
          params: { page: p, pageSize: ps },
        });
        if (!data.success || !data.data) break;
        const { items, totalCount } = data.data;
        for (const item of items) acc.push(item);
        if (items.length === 0) break;
        if (typeof totalCount === 'number' && acc.length >= totalCount) break;
        p += 1;
      }
      return acc;
    },
    enabled: useClientListSearch && hasSearch,
  });

  const pagedRows = useMemo(
    () => (listQuery.data?.success && listQuery.data.data ? [...listQuery.data.data.items] : []) as T[],
    [listQuery.data]
  );

  const filteredData = useMemo(() => {
    if (!useClientListSearch || !hasSearch || !clientListSearch) return [] as T[];
    const base = searchSource.data ?? [];
    const q = searchTrim.toLowerCase();
    return base.filter((item) => clientListSearch(item, q));
  }, [useClientListSearch, hasSearch, clientListSearch, searchSource.data, searchTrim]);

  const displayRows = useClientListSearch && hasSearch ? filteredData : pagedRows;
  const total = listQuery.data?.success && listQuery.data.data ? listQuery.data.data.totalCount : 0;
  const listLoading = useClientListSearch && hasSearch ? searchSource.isLoading : listQuery.isLoading;

  const editId = modal?.mode === 'edit' ? modal.id : null;

  const {
    control,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<Record<string, string>>({
    resolver: yupResolver(schema) as Resolver<Record<string, string>>,
    defaultValues: defaultCreateValues,
  });

  useEffect(() => {
    if (!modal) return;
    if (modal.mode === 'create') {
      reset(defaultCreateValues);
      return;
    }
    const row = displayRows.find((r) => Number(r.id) === modal.id);
    if (row) reset(rowToFormValuesRef.current(row));
  }, [modal, displayRows, reset, defaultCreateValues]);

  const invalidate = () => void qc.invalidateQueries({ queryKey: ['master', module, resourcePath] });

  const saveMut = useMutation({
    mutationFn: async (args: { values: Record<string, string>; editId?: number }) => {
      const body = args.editId != null ? toUpdatePayload(args.values) : toCreatePayload(args.values);
      if (args.editId != null) {
        const { data } = await client.put<BaseResponse<unknown>>(`${basePath}/${args.editId}`, body);
        return data;
      }
      const { data } = await client.post<BaseResponse<unknown>>(basePath, body);
      return data;
    },
    onSuccess: (res, vars) => {
      if (!res.success) {
        showToast(res.message ?? 'Save failed', 'error');
        return;
      }
      showToast(vars.editId != null ? 'Updated' : 'Created', 'success');
      setModal(null);
      invalidate();
    },
    onError: (e) => showToast(getApiErrorMessage(e), 'error'),
  });

  const delMut = useMutation({
    mutationFn: async (id: number) => {
      const { data } = await client.delete<BaseResponse<unknown>>(`${basePath}/${id}`);
      return data;
    },
    onSuccess: (res) => {
      if (!res.success) {
        showToast(res.message ?? 'Delete failed', 'error');
        return;
      }
      showToast('Deleted', 'success');
      setDeleteId(null);
      setDrawerRow(null);
      invalidate();
    },
    onError: (e) => showToast(getApiErrorMessage(e), 'error'),
  });

  const mode = modal?.mode;
  const activeFields = useMemo(
    () => (mode ? fields.filter((f) => fieldVisible(f, mode)) : []),
    [fields, mode]
  );

  const onSave = (values: Record<string, string>) => {
    const m = modal?.mode === 'edit' ? 'edit' : 'create';
    const err = beforeSave?.(values, m);
    if (err) {
      showToast(err, 'warning');
      return;
    }
    const eid = modal?.mode === 'edit' ? modal.id : undefined;
    saveMut.mutate({ values, editId: eid });
  };

  const tableColumns = useMemo(
    () => [
      ...columns,
      {
        id: '_actions',
        label: 'Actions',
        minWidth: 200,
        format: (row: T) => {
          const id = Number(row.id);
          return (
            <Stack direction="row" spacing={1} alignItems="center" flexWrap="wrap" useFlexGap>
              <Link
                component="button"
                type="button"
                variant="body2"
                onClick={(e) => {
                  e.stopPropagation();
                  setDrawerRow(row);
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
                  if (Number.isFinite(id)) setModal({ mode: 'edit', id });
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
                  if (Number.isFinite(id)) setDeleteId(id);
                }}
                sx={{ cursor: 'pointer' }}
              >
                Delete
              </Link>
            </Stack>
          );
        },
      } as MasterTableColumn<T>,
    ],
    [columns]
  );

  const formId = `master-form-${resourcePath.replace(/\//g, '-')}`;

  return (
    <Stack spacing={3}>
      <PageHeader title={title} />

      <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} alignItems={{ sm: 'center' }} justifyContent="space-between" sx={{ mb: 0.5 }}>
        <TextField
          size="small"
          label={searchFieldLabel ?? 'Search (name / code)'}
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          sx={{ flex: 1, minWidth: 220, maxWidth: { sm: 480 } }}
        />
        <Box sx={{ display: 'flex', justifyContent: 'flex-end' }}>
          <TriVitaButton variant="contained" onClick={() => setModal({ mode: 'create' })}>
            Add
          </TriVitaButton>
        </Box>
      </Stack>

      <DataTable<T>
        key={
          useClientListSearch && hasSearch
            ? `shell-search-${searchTrim}`
            : useClientListSearch
              ? `shell-paged-${page}-${pageSize}`
              : `shell-${page}-${pageSize}-${searchApplied}`
        }
        tableAriaLabel={title}
        columns={tableColumns}
        rows={displayRows}
        rowKey={(r) => String(r.id ?? '')}
        hidePagination={useClientListSearch && hasSearch}
        {...(useClientListSearch && hasSearch
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
        emptyTitle="No records"
        onRowClick={(row) => setDrawerRow(row)}
      />

      <DetailDrawer
        open={drawerRow != null}
        onClose={() => setDrawerRow(null)}
        title={drawerRow ? getDrawerTitle(drawerRow) : title}
        subtitle={drawerRow && getDrawerSubtitle ? getDrawerSubtitle(drawerRow) : undefined}
      >
        {drawerRow ? renderDetail(drawerRow) : null}
      </DetailDrawer>

      <TriVitaModal
        open={Boolean(modal)}
        onClose={() => setModal(null)}
        title={modal?.mode === 'edit' ? `Edit ${title}` : `Add ${title}`}
        actions={
          <Stack direction="row" spacing={2} justifyContent="flex-end" sx={{ width: '100%' }}>
            <TriVitaButton onClick={() => setModal(null)}>Cancel</TriVitaButton>
            <TriVitaButton type="submit" form={formId} variant="contained" disabled={saveMut.isPending}>
              Save
            </TriVitaButton>
          </Stack>
        }
      >
        <Box component="form" id={formId} onSubmit={handleSubmit(onSave)} noValidate>
          <FormGroup>
            {activeFields.map((f) => {
              if (f.kind === 'section') {
                return (
                  <Grid key={f.name} item xs={12}>
                    <Typography variant="subtitle2" sx={{ mt: 1 }}>
                      {f.label}
                    </Typography>
                  </Grid>
                );
              }

              const md = f.gridCols ?? 6;
              return (
                <Grid key={f.name} item xs={12} md={md}>
                  {f.kind === 'text' ? (
                    <Controller
                      name={f.name}
                      control={control}
                      render={({ field }) => (
                        <TriVitaTextField
                          {...field}
                          label={f.label}
                          required={f.required}
                          error={Boolean(errors[f.name])}
                          helperText={errors[f.name]?.message}
                          InputProps={{
                            readOnly: modal?.mode === 'edit' && Boolean(f.readOnlyOnEdit),
                          }}
                        />
                      )}
                    />
                  ) : null}
                  {f.kind === 'textarea' ? (
                    <Controller
                      name={f.name}
                      control={control}
                      render={({ field }) => (
                        <TriVitaTextField
                          {...field}
                          label={f.label}
                          required={f.required}
                          multiline
                          minRows={f.minRows ?? 3}
                          error={Boolean(errors[f.name])}
                          helperText={errors[f.name]?.message}
                        />
                      )}
                    />
                  ) : null}
                  {f.kind === 'number' ? (
                    <Controller
                      name={f.name}
                      control={control}
                      render={({ field }) => (
                        <TriVitaTextField
                          {...field}
                          label={f.label}
                          required={f.required}
                          type="number"
                          inputProps={f.integer ? { step: 1 } : undefined}
                          error={Boolean(errors[f.name])}
                          helperText={errors[f.name]?.message}
                        />
                      )}
                    />
                  ) : null}
                  {f.kind === 'date' ? (
                    <Controller
                      name={f.name}
                      control={control}
                      render={({ field }) => (
                        <TriVitaTextField
                          {...field}
                          label={f.label}
                          type="datetime-local"
                          InputLabelProps={{ shrink: true }}
                          required={f.required}
                          error={Boolean(errors[f.name])}
                          helperText={errors[f.name]?.message}
                        />
                      )}
                    />
                  ) : null}
                  {f.kind === 'select' ? (
                    <Controller
                      name={f.name}
                      control={control}
                      render={({ field }) => (
                        <TriVitaSelect
                          label={f.label}
                          required={f.required}
                          value={field.value}
                          onChange={(e) => field.onChange(String(e.target.value))}
                          options={[...f.options]}
                          error={Boolean(errors[f.name])}
                          helperText={errors[f.name]?.message}
                        />
                      )}
                    />
                  ) : null}
                  {f.kind === 'lookup' ? (
                    <LookupSelect<Record<string, string>>
                      name={f.name}
                      control={control}
                      label={f.label}
                      required={f.required}
                      queryKey={f.queryKey}
                      loadOptions={f.loadOptions}
                      editId={editId}
                      allowNone={f.allowNone}
                      noneLabel={f.noneLabel}
                    />
                  ) : null}
                </Grid>
              );
            })}
          </FormGroup>
        </Box>
      </TriVitaModal>

      <TriVitaModal
        open={deleteId != null}
        onClose={() => setDeleteId(null)}
        title="Confirm delete"
        actions={
          <Stack direction="row" spacing={2} justifyContent="flex-end" sx={{ width: '100%' }}>
            <TriVitaButton onClick={() => setDeleteId(null)}>Cancel</TriVitaButton>
            <TriVitaButton color="error" variant="contained" onClick={() => deleteId != null && delMut.mutate(deleteId)} disabled={delMut.isPending}>
              Delete
            </TriVitaButton>
          </Stack>
        }
      >
        <Typography>Delete this record permanently? This cannot be undone.</Typography>
      </TriVitaModal>
    </Stack>
  );
}
