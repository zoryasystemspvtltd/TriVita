import { Alert, Button, Stack, TextField, Typography } from '@mui/material';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Form, Formik } from 'formik';
import * as Yup from 'yup';
import { useState } from 'react';
import { Link as RouterLink } from 'react-router-dom';
import { DataTable } from '@/components/common/DataTable';
import { FormikTextField } from '@/components/common/FormikTextField';
import { PageHeader } from '@/components/layout/PageHeader';
import { DetailDrawer } from '@/components/layout/DetailDrawer';
import { SectionContainer } from '@/components/ds/SectionContainer';
import { TriVitaButton } from '@/components/ds/TriVitaButton';
import {
  createAppointment,
  createPatientMaster,
  getAppointmentQueuePaged,
  getAppointmentsPaged,
  getBillingItemsPaged,
  getPatientMasterById,
  searchPatientMasters,
} from '@/services/hmsService';
import { getApiErrorMessage } from '@/utils/errorMap';

export function HmsPatientRegistrationView() {
  const qc = useQueryClient();
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const [search, setSearch] = useState('');
  const [searchApplied, setSearchApplied] = useState('');
  const [linkedFacilityId, setLinkedFacilityId] = useState('');
  const [drawerId, setDrawerId] = useState<number | null>(null);

  const fac = linkedFacilityId.trim() === '' ? undefined : Number(linkedFacilityId);
  const q = useQuery({
    queryKey: ['hms', 'patients', page, pageSize, searchApplied, fac ?? 'all'],
    queryFn: () =>
      searchPatientMasters({
        page: page + 1,
        pageSize,
        search: searchApplied || undefined,
        linkedFacilityId: fac != null && Number.isFinite(fac) ? fac : undefined,
      }),
  });

  const detail = useQuery({
    queryKey: ['hms', 'patient-detail', drawerId],
    queryFn: () => getPatientMasterById(drawerId!),
    enabled: drawerId != null,
  });

  const m = useMutation({
    mutationFn: createPatientMaster,
    onSuccess: () => qc.invalidateQueries({ queryKey: ['hms', 'patients'] }),
  });

  const rows = q.data?.success && q.data.data ? q.data.data.items : [];
  const total = q.data?.success && q.data.data ? q.data.data.totalCount : 0;
  const detailRow = detail.data?.success ? detail.data.data : null;

  return (
    <Stack spacing={3}>
      <PageHeader
        title="Patient registration"
        subtitle="Register new patient masters, search the MPI index, and review demographics in a clinical layout."
        action={
          <TriVitaButton component={RouterLink} to="/hms/data-registry?resource=patient-masters" variant="outlined" size="small">
            API registry
          </TriVitaButton>
        }
      />
      {m.isError ? <Alert severity="error">{getApiErrorMessage(m.error)}</Alert> : null}
      {m.isSuccess && m.data?.success === false ? <Alert severity="warning">{m.data.message}</Alert> : null}
      {m.isSuccess && m.data?.success ? <Alert severity="success">Patient created.</Alert> : null}

      <SectionContainer title="New registration" subtitle="Minimum demographics to mint a UPID for the active tenant.">
        <Formik
          initialValues={{
            fullName: '',
            primaryEmail: '',
            primaryPhone: '',
            dateOfBirth: '',
          }}
          validationSchema={Yup.object({
            fullName: Yup.string().max(250).required('Required'),
            primaryEmail: Yup.string().max(200).email(),
            primaryPhone: Yup.string().max(40),
          })}
          onSubmit={async (v, { resetForm }) => {
            await m.mutateAsync({
              fullName: v.fullName,
              primaryEmail: v.primaryEmail || undefined,
              primaryPhone: v.primaryPhone || undefined,
              dateOfBirth: v.dateOfBirth ? `${v.dateOfBirth}T00:00:00Z` : undefined,
            });
            resetForm();
          }}
        >
          {({ isSubmitting }) => (
            <Form>
              <Stack direction={{ xs: 'column', md: 'row' }} spacing={2} flexWrap="wrap">
                <FormikTextField name="fullName" label="Full name" sx={{ flex: 1, minWidth: 200 }} />
                <FormikTextField name="dateOfBirth" label="Date of birth" type="date" InputLabelProps={{ shrink: true }} />
                <FormikTextField name="primaryPhone" label="Phone" />
                <FormikTextField name="primaryEmail" label="Email" />
                <Button type="submit" variant="contained" disabled={isSubmitting || m.isPending} sx={{ mt: 2 }}>
                  Register
                </Button>
              </Stack>
            </Form>
          )}
        </Formik>
      </SectionContainer>

      <SectionContainer title="Master patient index" subtitle="Server search across UPID, name, and contact fields.">
        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} sx={{ mb: 2 }} alignItems={{ sm: 'flex-end' }}>
          <TextField
            label="Search"
            size="small"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && setSearchApplied(search)}
            sx={{ flex: 1, minWidth: 200 }}
          />
          <TextField
            label="Linked facility ID (optional)"
            size="small"
            value={linkedFacilityId}
            onChange={(e) => setLinkedFacilityId(e.target.value)}
            sx={{ width: 220 }}
          />
          <TriVitaButton variant="outlined" onClick={() => { setSearchApplied(search); setPage(0); }}>
            Apply
          </TriVitaButton>
        </Stack>
        <DataTable
          columns={[
            { id: 'upid', label: 'UPID' },
            { id: 'fullName', label: 'Name' },
            { id: 'primaryPhone', label: 'Phone' },
            { id: 'primaryEmail', label: 'Email' },
            {
              id: '_a',
              label: '',
              format: (row) => (
                <TriVitaButton size="small" variant="outlined" onClick={() => setDrawerId(Number(row.id))}>
                  Chart summary
                </TriVitaButton>
              ),
            },
          ]}
          rows={rows as unknown as Record<string, unknown>[]}
          rowKey={(r) => String(r.id)}
          totalCount={total}
          page={page}
          pageSize={pageSize}
          onPageChange={(p, ps) => {
            setPage(p);
            setPageSize(ps);
          }}
          loading={q.isLoading}
        />
      </SectionContainer>

      <DetailDrawer
        open={drawerId != null}
        onClose={() => setDrawerId(null)}
        title={detailRow?.fullName ?? 'Patient'}
        subtitle={detailRow?.upid ? `UPID ${detailRow.upid}` : undefined}
      >
        {detail.isLoading ? <Typography color="text.secondary">Loading…</Typography> : null}
        {detailRow ? (
          <Stack spacing={1.5}>
            {Object.entries(detailRow as unknown as Record<string, unknown>).map(([k, v]) => (
              <div key={k}>
                <Typography variant="caption" color="text.secondary" display="block">
                  {k}
                </Typography>
                <Typography variant="body2">{v === null || v === undefined ? '—' : String(v)}</Typography>
              </div>
            ))}
          </Stack>
        ) : detail.data && !detail.data.success ? (
          <Typography color="error">{detail.data.message}</Typography>
        ) : null}
      </DetailDrawer>
    </Stack>
  );
}

export function HmsAppointmentsView() {
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const q = useQuery({
    queryKey: ['hms', 'appointments', page, pageSize],
    queryFn: () => getAppointmentsPaged({ page: page + 1, pageSize }),
  });
  const rows =
    q.data?.success && q.data.data
      ? (q.data.data.items as Record<string, unknown>[])
      : [];
  const total = q.data?.success && q.data.data ? q.data.data.totalCount : 0;

  return (
    <Stack spacing={2}>
      <PageHeader title="Appointment booking" subtitle="Paged list from /api/v1/appointments." />
      <Formik
        initialValues={{
          patientId: 1,
          doctorId: 1,
          departmentId: 1,
          appointmentStatusValueId: 1,
          scheduledStartOn: new Date().toISOString().slice(0, 16),
          reason: '',
        }}
        onSubmit={async (v, { resetForm }) => {
          await createAppointment({
            patientId: Number(v.patientId),
            doctorId: Number(v.doctorId),
            departmentId: Number(v.departmentId),
            appointmentStatusValueId: Number(v.appointmentStatusValueId),
            scheduledStartOn: new Date(v.scheduledStartOn).toISOString(),
            reason: v.reason || undefined,
          });
          resetForm();
          void q.refetch();
        }}
      >
        {({ isSubmitting }) => (
          <Form>
            <Stack direction="row" flexWrap="wrap" gap={2} alignItems="flex-start">
              <FormikTextField name="patientId" label="Patient ID" type="number" sx={{ width: 120 }} />
              <FormikTextField name="doctorId" label="Doctor ID" type="number" sx={{ width: 120 }} />
              <FormikTextField name="departmentId" label="Department ID" type="number" sx={{ width: 140 }} />
              <FormikTextField name="appointmentStatusValueId" label="Status ref. ID" type="number" sx={{ width: 160 }} />
              <FormikTextField
                name="scheduledStartOn"
                label="Scheduled start"
                type="datetime-local"
                InputLabelProps={{ shrink: true }}
              />
              <FormikTextField name="reason" label="Reason" sx={{ minWidth: 200 }} />
              <Button type="submit" variant="outlined" disabled={isSubmitting} sx={{ mt: 2 }}>
                Book
              </Button>
            </Stack>
          </Form>
        )}
      </Formik>
      <DataTable
        columns={[
          { id: 'id', label: 'ID' },
          { id: 'scheduledStartOn', label: 'Start' },
          { id: 'patientId', label: 'Patient' },
          { id: 'doctorId', label: 'Doctor' },
        ]}
        rows={rows}
        rowKey={(r) => String(r.id ?? Math.random())}
        totalCount={total}
        page={page}
        pageSize={pageSize}
        onPageChange={(p, ps) => {
          setPage(p);
          setPageSize(ps);
        }}
        loading={q.isLoading}
      />
    </Stack>
  );
}

export function HmsOpdDashboardView() {
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const q = useQuery({
    queryKey: ['hms', 'queue', page, pageSize],
    queryFn: () => getAppointmentQueuePaged({ page: page + 1, pageSize }),
  });
  const rows =
    q.data?.success && q.data.data
      ? (q.data.data.items as Record<string, unknown>[])
      : [];
  const total = q.data?.success && q.data.data ? q.data.data.totalCount : 0;

  return (
    <Stack spacing={2}>
      <PageHeader title="OPD dashboard" subtitle="Appointment queue from /api/v1/appointment-queue." />
      <DataTable
        columns={[
          { id: 'id', label: 'ID' },
          { id: 'appointmentId', label: 'Appointment' },
          { id: 'queuePosition', label: 'Position' },
        ]}
        rows={rows}
        rowKey={(r) => String(r.id ?? Math.random())}
        totalCount={total}
        page={page}
        pageSize={pageSize}
        onPageChange={(p, ps) => {
          setPage(p);
          setPageSize(ps);
        }}
        loading={q.isLoading}
      />
    </Stack>
  );
}

export function HmsBillingView() {
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const q = useQuery({
    queryKey: ['hms', 'billing-items', page, pageSize],
    queryFn: () => getBillingItemsPaged({ page: page + 1, pageSize }),
  });
  const rows =
    q.data?.success && q.data.data
      ? (q.data.data.items as Record<string, unknown>[])
      : [];
  const total = q.data?.success && q.data.data ? q.data.data.totalCount : 0;

  return (
    <Stack spacing={2}>
      <PageHeader title="HMS billing" subtitle="Billing line items from /api/v1/billing-items." />
      <DataTable
        columns={[
          { id: 'id', label: 'ID' },
          { id: 'description', label: 'Description' },
          { id: 'quantity', label: 'Qty' },
        ]}
        rows={rows}
        rowKey={(r) => String(r.id ?? Math.random())}
        totalCount={total}
        page={page}
        pageSize={pageSize}
        onPageChange={(p, ps) => {
          setPage(p);
          setPageSize(ps);
        }}
        loading={q.isLoading}
      />
    </Stack>
  );
}
