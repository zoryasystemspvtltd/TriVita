import { yupResolver } from '@hookform/resolvers/yup';
import {
  Box,
  FormControl,
  FormHelperText,
  Grid,
  InputLabel,
  Link,
  MenuItem,
  Select,
  Stack,
  TextField,
  Typography,
} from '@mui/material';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useEffect, useMemo, useState } from 'react';
import { Controller, useForm, type Resolver } from 'react-hook-form';
import * as Yup from 'yup';
import { DataTable } from '@/components/common/DataTable';
import { LookupSelect } from '@/components/common/LookupSelect';
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
  createMedicine,
  deleteMedicine,
  getManufacturerPaged,
  getMedicineById,
  getMedicineCategoryPaged,
  getMedicinePaged,
  updateMedicine,
} from '@/services/pharmacyService';
import {
  getPharmacyCompositionLabelMap,
  getPharmacyFormLabelMap,
  getPharmacyUnitLabelMap,
  loadPharmacyCompositionMasterOptions,
  loadPharmacyFormMasterOptions,
  loadPharmacyUnitMasterOptions,
} from '@/utils/pharmacyLookups';
import { getApiErrorMessage } from '@/utils/errorMap';

type MedicineRow = Record<string, unknown> & { id?: number };

type MedicineFormValues = {
  medicineName: string;
  medicineCode: string;
  categoryId: string;
  manufacturerId: string;
  primaryCompositionId: string;
  formId: string;
  defaultUnitId: string;
  strength: string;
  notes: string;
  isActive: boolean;
};

const emptyForm: MedicineFormValues = {
  medicineName: '',
  medicineCode: '',
  categoryId: '',
  manufacturerId: '',
  primaryCompositionId: '',
  formId: '',
  defaultUnitId: '',
  strength: '',
  notes: '',
  isActive: true,
};

const medicineSchema = Yup.object({
  medicineName: Yup.string().trim().max(256).required('Medicine name is required'),
  medicineCode: Yup.string().trim().max(64).required('Code is required'),
  categoryId: Yup.string().required('Category is required').matches(/^\d+$/, 'Select a category'),
  manufacturerId: Yup.string().required('Manufacturer is required').matches(/^\d+$/, 'Select a manufacturer'),
  primaryCompositionId: Yup.string().required('Composition is required').matches(/^\d+$/, 'Select a composition'),
  formId: Yup.string().required('Form is required').matches(/^\d+$/, 'Select a form'),
  defaultUnitId: Yup.string().trim().matches(/^$|^\d+$/, 'Invalid unit'),
  strength: Yup.string().trim().max(120).default(''),
  notes: Yup.string().trim().max(1000).default(''),
  isActive: Yup.boolean().required(),
});

function rowActive(row: MedicineRow): boolean {
  const v = row.isActive;
  if (v === false || v === 'false' || v === 0) return false;
  return true;
}

function rowToFormValues(row: MedicineRow): MedicineFormValues {
  const pc = row.primaryCompositionId ?? row.PrimaryCompositionId;
  return {
    medicineName: String(row.medicineName ?? ''),
    medicineCode: String(row.medicineCode ?? ''),
    categoryId: row.categoryId != null ? String(row.categoryId) : '',
    manufacturerId: row.manufacturerId != null ? String(row.manufacturerId) : '',
    primaryCompositionId: pc != null ? String(pc) : '',
    formId: row.formId != null ? String(row.formId) : '',
    defaultUnitId: row.defaultUnitId != null ? String(row.defaultUnitId) : '',
    strength: String(row.strength ?? ''),
    notes: String(row.notes ?? ''),
    isActive: rowActive(row),
  };
}

function buildApiBody(v: MedicineFormValues) {
  return {
    medicineCode: v.medicineCode.trim(),
    medicineName: v.medicineName.trim(),
    categoryId: Number(v.categoryId),
    manufacturerId: Number(v.manufacturerId),
    primaryCompositionId: Number(v.primaryCompositionId),
    formId: Number(v.formId),
    defaultUnitId: v.defaultUnitId.trim() ? Number(v.defaultUnitId) : undefined,
    strength: v.strength.trim() || undefined,
    notes: v.notes.trim() || undefined,
    isActive: v.isActive,
  };
}

function formatCompositionCell(
  row: MedicineRow,
  compositionMap: Map<number, string>,
  unitLabelMap: Map<number, string>
): string {
  const cid = Number(row.primaryCompositionId ?? row.PrimaryCompositionId);
  const name = Number.isFinite(cid) ? compositionMap.get(cid) ?? '' : '';
  const strength = String(row.strength ?? '').trim();
  const uid = Number(row.defaultUnitId);
  const unit = Number.isFinite(uid) ? unitLabelMap.get(uid) ?? '' : '';
  const parts = [name, strength, unit].filter(Boolean);
  return parts.length ? parts.join(' · ') : '—';
}

export function MedicineMasterPage() {
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
    queryFn: () => getMedicineCategoryPaged({ page: 1, pageSize: 500 }),
    staleTime: 5 * 60_000,
  });

  const manufacturers = useQuery({
    queryKey: ['pharmacy', 'manufacturer', 'options'],
    queryFn: () => getManufacturerPaged({ page: 1, pageSize: 500 }),
    staleTime: 5 * 60_000,
  });

  const unitLabelQuery = useQuery({
    queryKey: ['pharmacy', 'medicine-unit', 'label-map'],
    queryFn: getPharmacyUnitLabelMap,
    staleTime: 60_000,
  });

  const formLabelQuery = useQuery({
    queryKey: ['pharmacy', 'medicine-form', 'label-map'],
    queryFn: getPharmacyFormLabelMap,
    staleTime: 60_000,
  });

  const compositionLabelQuery = useQuery({
    queryKey: ['pharmacy', 'composition', 'label-map'],
    queryFn: getPharmacyCompositionLabelMap,
    staleTime: 60_000,
  });

  const detail = useQuery({
    queryKey: ['pharmacy', 'medicine', 'detail', drawerId],
    queryFn: () => getMedicineById(drawerId!),
    enabled: drawerId != null,
  });

  const editFormQuery = useQuery({
    queryKey: ['pharmacy', 'medicine', 'form-seed', modal != null && modal.mode === 'edit' ? modal.id : null],
    queryFn: () => {
      if (modal?.mode !== 'edit') throw new Error('No edit');
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
    for (const c of (data?.items ?? []) as MedicineRow[]) {
      const id = Number(c.id);
      if (Number.isFinite(id)) m.set(id, String(c.categoryName ?? c.categoryCode ?? ''));
    }
    return m;
  }, [categories.data]);

  const manufacturerMap = useMemo(() => {
    const m = new Map<number, string>();
    const data = manufacturers.data?.success ? manufacturers.data.data : null;
    for (const x of (data?.items ?? []) as MedicineRow[]) {
      const id = Number(x.id);
      if (Number.isFinite(id)) m.set(id, String(x.manufacturerName ?? x.manufacturerCode ?? ''));
    }
    return m;
  }, [manufacturers.data]);

  const unitLabelMap = unitLabelQuery.data ?? new Map<number, string>();
  const formLabelMap = formLabelQuery.data ?? new Map<number, string>();
  const compositionMap = compositionLabelQuery.data ?? new Map<number, string>();

  const editId = modal != null && modal.mode === 'edit' ? modal.id : null;

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
    if (d?.success && d.data) reset(rowToFormValues(d.data as MedicineRow));
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
    const eid = modal != null && modal.mode === 'edit' ? modal.id : undefined;
    saveMut.mutate({ values, editId: eid });
  };

  return (
    <Stack spacing={3}>
      <PageHeader title="Medicine Master" />

      <SectionContainer title="Medicines">
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
                Add Medicine
              </TriVitaButton>
            </Box>
          </Stack>

          <DataTable<MedicineRow>
            tableAriaLabel="Medicine master"
            columns={[
              {
                id: 'medicineName',
                label: 'Medicine name',
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
                format: (row) => categoryMap.get(Number(row.categoryId)) ?? '—',
              },
              {
                id: 'manufacturerId',
                label: 'Manufacturer',
                minWidth: 160,
                format: (row) => {
                  const id = Number(row.manufacturerId);
                  return Number.isFinite(id) ? manufacturerMap.get(id) ?? '—' : '—';
                },
              },
              {
                id: 'composition',
                label: 'Composition',
                minWidth: 220,
                format: (row) => formatCompositionCell(row, compositionMap, unitLabelMap),
              },
              {
                id: 'formId',
                label: 'Form',
                minWidth: 140,
                format: (row) => {
                  const id = Number(row.formId);
                  return Number.isFinite(id) ? formLabelMap.get(id) ?? '—' : '—';
                },
              },
              {
                id: 'isActive',
                label: 'Status',
                minWidth: 100,
                format: (row) => (rowActive(row) ? 'Active' : 'Inactive'),
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
        </Stack>
      </SectionContainer>

      <DetailDrawer
        open={drawerId != null}
        onClose={() => setDrawerId(null)}
        title={detailRecord ? String(detailRecord.medicineName ?? 'Medicine') : 'Medicine'}
        subtitle={detailRecord ? String(detailRecord.medicineCode ?? '') : undefined}
      >
        {detail.isLoading ? <Typography color="text.secondary">Loading…</Typography> : null}
        {detailRecord ? (
          <Stack spacing={1}>
            <DetailKv label="Medicine name" value={String(detailRecord.medicineName ?? '')} />
            <DetailKv label="Code" value={String(detailRecord.medicineCode ?? '')} />
            <DetailKv label="Category" value={categoryMap.get(Number(detailRecord.categoryId)) ?? '—'} />
            <DetailKv
              label="Manufacturer"
              value={(() => {
                const id = Number(detailRecord.manufacturerId);
                return Number.isFinite(id) ? manufacturerMap.get(id) ?? '—' : '—';
              })()}
            />
            <DetailKv
              label="Composition"
              value={formatCompositionCell(detailRecord, compositionMap, unitLabelMap)}
            />
            <DetailKv label="Strength" value={String(detailRecord.strength ?? '').trim() || '—'} />
            <DetailKv
              label="Display unit"
              value={(() => {
                const id = Number(detailRecord.defaultUnitId);
                return Number.isFinite(id) ? unitLabelMap.get(id) ?? '—' : '—';
              })()}
            />
            <DetailKv
              label="Form"
              value={(() => {
                const id = Number(detailRecord.formId);
                return Number.isFinite(id) ? formLabelMap.get(id) ?? '—' : '—';
              })()}
            />
            <DetailKv label="Status" value={rowActive(detailRecord) ? 'Active' : 'Inactive'} />
            <DetailKv label="Description" value={String(detailRecord.notes ?? '').trim() || '—'} />
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
                  <TriVitaTextField
                    {...field}
                    label="Name"
                    required
                    error={Boolean(errors.medicineName)}
                    helperText={errors.medicineName?.message}
                  />
                )}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <Controller
                name="medicineCode"
                control={control}
                render={({ field }) => (
                  <TriVitaTextField
                    {...field}
                    label="Code"
                    required
                    error={Boolean(errors.medicineCode)}
                    helperText={errors.medicineCode?.message}
                  />
                )}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <LookupSelect<MedicineFormValues>
                name="categoryId"
                control={control}
                label="Category"
                required
                editId={editId}
                queryKey={['pharmacy', 'medicine-master', 'lookup', 'category']}
                loadOptions={async () => {
                  const res = await getMedicineCategoryPaged({ page: 1, pageSize: 500 });
                  if (!res.success || !res.data) return [];
                  return (res.data.items as MedicineRow[]).map((c) => ({
                    value: String(c.id ?? ''),
                    label: String(c.categoryName ?? c.categoryCode ?? ''),
                  }));
                }}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <LookupSelect<MedicineFormValues>
                name="manufacturerId"
                control={control}
                label="Manufacturer"
                required
                editId={editId}
                queryKey={['pharmacy', 'medicine-master', 'lookup', 'manufacturer']}
                loadOptions={async () => {
                  const res = await getManufacturerPaged({ page: 1, pageSize: 500 });
                  if (!res.success || !res.data) return [];
                  return (res.data.items as MedicineRow[]).map((c) => ({
                    value: String(c.id ?? ''),
                    label: String(c.manufacturerName ?? c.manufacturerCode ?? ''),
                  }));
                }}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <LookupSelect<MedicineFormValues>
                name="primaryCompositionId"
                control={control}
                label="Composition"
                required
                editId={editId}
                queryKey={['pharmacy', 'medicine-master', 'lookup', 'composition']}
                loadOptions={() => loadPharmacyCompositionMasterOptions()}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <LookupSelect<MedicineFormValues>
                name="formId"
                control={control}
                label="Form"
                required
                editId={editId}
                queryKey={['pharmacy', 'medicine-form', 'lookup']}
                loadOptions={() => loadPharmacyFormMasterOptions()}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <Controller
                name="strength"
                control={control}
                render={({ field }) => (
                  <TriVitaTextField
                    {...field}
                    label="Strength"
                    error={Boolean(errors.strength)}
                    helperText={errors.strength?.message}
                  />
                )}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <LookupSelect<MedicineFormValues>
                name="defaultUnitId"
                control={control}
                label="Strength unit"
                editId={editId}
                allowNone
                noneLabel="None"
                queryKey={['pharmacy', 'medicine-unit', 'lookup-options']}
                loadOptions={() => loadPharmacyUnitMasterOptions()}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <Controller
                name="isActive"
                control={control}
                render={({ field }) => (
                  <FormControl fullWidth error={Boolean(errors.isActive)} size="small">
                    <InputLabel id="medicine-status-label">Status</InputLabel>
                    <Select
                      labelId="medicine-status-label"
                      label="Status"
                      value={field.value ? 'active' : 'inactive'}
                      onChange={(e) => field.onChange(e.target.value === 'active')}
                    >
                      <MenuItem value="active">Active</MenuItem>
                      <MenuItem value="inactive">Inactive</MenuItem>
                    </Select>
                    {errors.isActive ? <FormHelperText>{errors.isActive.message}</FormHelperText> : null}
                  </FormControl>
                )}
              />
            </Grid>
            <Grid item xs={12}>
              <Controller
                name="notes"
                control={control}
                render={({ field }) => (
                  <TriVitaTextField
                    {...field}
                    label="Description"
                    multiline
                    minRows={3}
                    error={Boolean(errors.notes)}
                    helperText={errors.notes?.message}
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
        <Typography>Delete this medicine permanently? This cannot be undone.</Typography>
      </TriVitaModal>
    </Stack>
  );
}
