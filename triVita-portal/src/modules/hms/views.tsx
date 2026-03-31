import { Alert, Button, Stack } from '@mui/material';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Form, Formik } from 'formik';
import * as Yup from 'yup';
import { useState } from 'react';
import { DataTable } from '@/components/common/DataTable';
import { FormikTextField } from '@/components/common/FormikTextField';
import { PageHeader } from '@/components/layout/PageHeader';
import {
  createAppointment,
  createPatientMaster,
  getAppointmentQueuePaged,
  getAppointmentsPaged,
  getBillingItemsPaged,
  searchPatientMasters,
} from '@/services/hmsService';
import { getApiErrorMessage } from '@/utils/errorMap';

export function HmsPatientRegistrationView() {
  const qc = useQueryClient();
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const q = useQuery({
    queryKey: ['hms', 'patients', page, pageSize],
    queryFn: () => searchPatientMasters({ page: page + 1, pageSize }),
  });
  const m = useMutation({
    mutationFn: createPatientMaster,
    onSuccess: () => qc.invalidateQueries({ queryKey: ['hms', 'patients'] }),
  });

  const rows = q.data?.success && q.data.data ? q.data.data.items : [];
  const total = q.data?.success && q.data.data ? q.data.data.totalCount : 0;

  return (
    <Stack spacing={2}>
      <PageHeader
        title="Patient registration"
        subtitle="Search and register patient masters (UPID). Uses GET/POST /api/v1/patient-masters."
      />
      {m.isError ? <Alert severity="error">{getApiErrorMessage(m.error)}</Alert> : null}
      {m.isSuccess && m.data?.success === false ? <Alert severity="warning">{m.data.message}</Alert> : null}
      {m.isSuccess && m.data?.success ? <Alert severity="success">Patient created.</Alert> : null}
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
      <DataTable
        columns={[
          { id: 'upid', label: 'UPID' },
          { id: 'fullName', label: 'Name' },
          { id: 'primaryPhone', label: 'Phone' },
          { id: 'primaryEmail', label: 'Email' },
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
        loading={q.isLoading}
      />
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
