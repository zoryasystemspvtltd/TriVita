import { Alert, Box, Stack, TextField, Typography } from '@mui/material';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Form, Formik } from 'formik';
import * as Yup from 'yup';
import { useMemo, useState } from 'react';
import { Link as RouterLink } from 'react-router-dom';
import { DataTable } from '@/components/common/DataTable';
import { FormikTextField } from '@/components/common/FormikTextField';
import { PageHeader } from '@/components/layout/PageHeader';
import { DetailDrawer } from '@/components/layout/DetailDrawer';
import { SectionContainer } from '@/components/ds/SectionContainer';
import { TriVitaButton } from '@/components/ds/TriVitaButton';
import { TriVitaModal } from '@/components/ds/TriVitaModal';
import { useToast } from '@/components/toast/ToastProvider';
import {
  createMedicine,
  deleteMedicine,
  getMedicineById,
  getMedicinePaged,
  updateMedicine,
} from '@/services/pharmacyService';
import { getApiErrorMessage } from '@/utils/errorMap';

const medicineSchema = Yup.object({
  medicineCode: Yup.string().trim().max(64).required('Code is required'),
  medicineName: Yup.string().trim().max(256).required('Name is required'),
  categoryId: Yup.string().required('Category ID is required').matches(/^\d+$/, 'Must be a whole number'),
  manufacturerId: Yup.string().matches(/^$|^\d+$/, 'Must be numeric or empty'),
  strength: Yup.string().max(120).nullable(),
  defaultUnitId: Yup.string().matches(/^$|^\d+$/, 'Must be numeric or empty'),
  formReferenceValueId: Yup.string().matches(/^$|^\d+$/, 'Must be numeric or empty'),
  notes: Yup.string().max(2000).nullable(),
});

function toOptLong(v: string) {
  const t = v.trim();
  if (!t) return undefined;
  const n = Number(t);
  return Number.isFinite(n) ? n : undefined;
}

type MedicineForm = {
  medicineCode: string;
  medicineName: string;
  categoryId: string;
  manufacturerId: string;
  strength: string;
  defaultUnitId: string;
  formReferenceValueId: string;
  notes: string;
};

const emptyForm: MedicineForm = {
  medicineCode: '',
  medicineName: '',
  categoryId: '',
  manufacturerId: '',
  strength: '',
  defaultUnitId: '',
  formReferenceValueId: '',
  notes: '',
};

function rowToForm(row: Record<string, unknown>): MedicineForm {
  return {
    medicineCode: String(row.medicineCode ?? ''),
    medicineName: String(row.medicineName ?? ''),
    categoryId: String(row.categoryId ?? ''),
    manufacturerId: row.manufacturerId != null ? String(row.manufacturerId) : '',
    strength: String(row.strength ?? ''),
    defaultUnitId: row.defaultUnitId != null ? String(row.defaultUnitId) : '',
    formReferenceValueId: row.formReferenceValueId != null ? String(row.formReferenceValueId) : '',
    notes: String(row.notes ?? ''),
  };
}

function buildBody(v: MedicineForm) {
  return {
    medicineCode: v.medicineCode.trim(),
    medicineName: v.medicineName.trim(),
    categoryId: Number(v.categoryId),
    manufacturerId: toOptLong(v.manufacturerId),
    strength: v.strength.trim() || undefined,
    defaultUnitId: toOptLong(v.defaultUnitId),
    formReferenceValueId: toOptLong(v.formReferenceValueId),
    notes: v.notes.trim() || undefined,
  };
}

export function PharmacyMedicineCatalog() {
  const qc = useQueryClient();
  const { showToast } = useToast();
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const [search, setSearch] = useState('');
  const [searchApplied, setSearchApplied] = useState('');
  const [drawerId, setDrawerId] = useState<number | null>(null);
  const [modal, setModal] = useState<null | { mode: 'create' } | { mode: 'edit'; id: number; seed?: MedicineForm }>(
    null
  );
  const [deleteId, setDeleteId] = useState<number | null>(null);

  const list = useQuery({
    queryKey: ['pharmacy', 'medicine', 'biz', page, pageSize, searchApplied],
    queryFn: () => getMedicinePaged({ page: page + 1, pageSize, search: searchApplied || undefined }),
  });

  const detail = useQuery({
    queryKey: ['pharmacy', 'medicine', 'detail', drawerId],
    queryFn: () => getMedicineById(drawerId!),
    enabled: drawerId != null,
  });

  const rows = useMemo(
    () => (list.data?.success && list.data.data ? [...list.data.data.items] : []) as Record<string, unknown>[],
    [list.data]
  );
  const total = list.data?.success && list.data.data ? list.data.data.totalCount : 0;

  const saveMut = useMutation({
    mutationFn: async (args: { values: MedicineForm; editId?: number }) => {
      const body = buildBody(args.values);
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

  const detailRecord = detail.data?.success ? detail.data.data : null;

  return (
    <Stack spacing={3}>
      <PageHeader
        title="Medicine catalog"
        subtitle="Formulary master data with validated forms — aligned to PharmacyService create/update DTOs."
        action={
          <TriVitaButton
            component={RouterLink}
            to="/pharmacy/data-registry?resource=medicine"
            variant="outlined"
            size="small"
          >
            API registry
          </TriVitaButton>
        }
      />

      <SectionContainer title="Catalog" subtitle="Search by code or name, manage lifecycle without raw JSON.">
        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} alignItems={{ sm: 'flex-end' }} sx={{ mb: 2 }}>
          <TextField
            label="Search"
            size="small"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            onKeyDown={(e) => {
              if (e.key === 'Enter') setSearchApplied(search);
            }}
            helperText="Server-side search. Press Enter or Apply."
            sx={{ flex: 1, minWidth: 200 }}
          />
          <TriVitaButton variant="outlined" onClick={() => setSearchApplied(search)}>
            Apply
          </TriVitaButton>
          <TriVitaButton variant="contained" onClick={() => setModal({ mode: 'create' })}>
            New medicine
          </TriVitaButton>
        </Stack>
        <Alert severity="info" sx={{ mb: 2 }}>
          Category, manufacturer, and unit fields reference tenant dictionary IDs (numeric).
        </Alert>
        <DataTable
          tableAriaLabel="Medicine catalog"
          columns={[
            { id: 'id', label: 'ID', minWidth: 72 },
            { id: 'medicineCode', label: 'Code', minWidth: 120 },
            { id: 'medicineName', label: 'Name', minWidth: 200 },
            { id: 'categoryId', label: 'Category' },
            { id: 'strength', label: 'Strength' },
            {
              id: '_a',
              label: '',
              minWidth: 220,
              format: (row) => (
                <Stack direction="row" spacing={1} flexWrap="wrap" useFlexGap>
                  <TriVitaButton size="small" variant="outlined" onClick={() => setDrawerId(Number(row.id))}>
                    Details
                  </TriVitaButton>
                  <TriVitaButton
                    size="small"
                    variant="contained"
                    onClick={() => setModal({ mode: 'edit', id: Number(row.id), seed: rowToForm(row) })}
                  >
                    Edit
                  </TriVitaButton>
                  <TriVitaButton size="small" color="error" variant="outlined" onClick={() => setDeleteId(Number(row.id))}>
                    Delete
                  </TriVitaButton>
                </Stack>
              ),
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
        />
      </SectionContainer>

      <DetailDrawer
        open={drawerId != null}
        onClose={() => setDrawerId(null)}
        title={detailRecord ? String(detailRecord.medicineName ?? 'Medicine') : 'Medicine'}
        subtitle={detailRecord ? `Code ${String(detailRecord.medicineCode ?? '')}` : undefined}
      >
        {detail.isLoading ? <Typography color="text.secondary">Loading…</Typography> : null}
        {detailRecord ? (
          <Stack spacing={1.5}>
            {Object.entries(detailRecord).map(([k, v]) => (
              <Box key={k}>
                <Typography variant="caption" color="text.secondary" display="block">
                  {k}
                </Typography>
                <Typography variant="body2">{v === null || v === undefined ? '—' : String(v)}</Typography>
              </Box>
            ))}
          </Stack>
        ) : detail.data && !detail.data.success ? (
          <Alert severity="warning">{detail.data.message}</Alert>
        ) : null}
      </DetailDrawer>

      <TriVitaModal
        open={Boolean(modal)}
        onClose={() => setModal(null)}
        title={modal?.mode === 'edit' ? 'Edit medicine' : 'New medicine'}
        actions={
          <>
            <TriVitaButton onClick={() => setModal(null)}>Cancel</TriVitaButton>
            <TriVitaButton type="submit" form="medicine-form" variant="contained" disabled={saveMut.isPending}>
              Save
            </TriVitaButton>
          </>
        }
      >
        <Formik
          enableReinitialize
          key={modal?.mode === 'edit' ? `e-${modal.id}` : 'c'}
          initialValues={modal?.mode === 'edit' && modal.seed ? modal.seed : emptyForm}
          validationSchema={medicineSchema}
          onSubmit={(values) => {
            const editId = modal?.mode === 'edit' ? modal.id : undefined;
            saveMut.mutate({ values, editId });
          }}
        >
          {({ isSubmitting }) => (
            <Form id="medicine-form">
              <Stack spacing={2} useFlexGap flexWrap="wrap" direction="row">
                <FormikTextField name="medicineCode" label="Medicine code" required sx={{ flex: '1 1 220px', minWidth: 200 }} />
                <FormikTextField name="medicineName" label="Medicine name" required sx={{ flex: '2 1 280px', minWidth: 220 }} />
                <FormikTextField name="categoryId" label="Category ID" required sx={{ flex: '1 1 140px', minWidth: 120 }} />
                <FormikTextField name="manufacturerId" label="Manufacturer ID (optional)" sx={{ flex: '1 1 180px', minWidth: 160 }} />
                <FormikTextField name="strength" label="Strength" sx={{ flex: '1 1 160px', minWidth: 140 }} />
                <FormikTextField name="defaultUnitId" label="Default unit ID (optional)" sx={{ flex: '1 1 180px', minWidth: 160 }} />
                <FormikTextField name="formReferenceValueId" label="Form reference value ID (optional)" sx={{ flex: '1 1 220px', minWidth: 200 }} />
                <FormikTextField name="notes" label="Notes" multiline minRows={2} sx={{ flex: '1 1 100%', minWidth: 280 }} />
              </Stack>
              {saveMut.isPending || isSubmitting ? (
                <Typography variant="caption" color="text.secondary" sx={{ mt: 1 }}>
                  Saving…
                </Typography>
              ) : null}
            </Form>
          )}
        </Formik>
      </TriVitaModal>

      <TriVitaModal
        open={deleteId != null}
        onClose={() => setDeleteId(null)}
        title="Delete medicine"
        actions={
          <>
            <TriVitaButton onClick={() => setDeleteId(null)}>Cancel</TriVitaButton>
            <TriVitaButton color="error" variant="contained" onClick={() => deleteId != null && delMut.mutate(deleteId)}>
              Delete
            </TriVitaButton>
          </>
        }
      >
        <Typography>Permanently delete medicine #{deleteId}? This cannot be undone.</Typography>
      </TriVitaModal>
    </Stack>
  );
}
