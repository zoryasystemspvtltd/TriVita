import { Alert, Button, Stack, TextField, Typography } from '@mui/material';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Form, Formik } from 'formik';
import { useState } from 'react';
import { DataTable } from '@/components/common/DataTable';
import { FormikTextField } from '@/components/common/FormikTextField';
import { PageHeader } from '@/components/layout/PageHeader';
import {
  getEquipmentTypesPaged,
  getTestBookingsPaged,
  getTestMastersPaged,
  registerBarcode,
  resolveBarcode,
} from '@/services/lmsService';
import { getApiErrorMessage } from '@/utils/errorMap';

export function LmsTestMasterView() {
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const q = useQuery({
    queryKey: ['lms', 'test-masters', page, pageSize],
    queryFn: () => getTestMastersPaged({ page: page + 1, pageSize }),
  });
  const rows =
    q.data?.success && q.data.data
      ? (q.data.data.items as Record<string, unknown>[])
      : [];
  const total = q.data?.success && q.data.data ? q.data.data.totalCount : 0;

  return (
    <Stack spacing={2}>
      <PageHeader title="Test master" subtitle="/api/v1/workflow/test-masters" />
      <DataTable
        columns={[
          { id: 'id', label: 'ID' },
          { id: 'catalogTestCode', label: 'Code' },
          { id: 'catalogTestName', label: 'Name' },
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

export function LmsEquipmentMasterView() {
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const q = useQuery({
    queryKey: ['lms', 'equipment-types', page, pageSize],
    queryFn: () => getEquipmentTypesPaged({ page: page + 1, pageSize }),
  });
  const rows =
    q.data?.success && q.data.data
      ? (q.data.data.items as Record<string, unknown>[])
      : [];
  const total = q.data?.success && q.data.data ? q.data.data.totalCount : 0;

  return (
    <Stack spacing={2}>
      <PageHeader title="Equipment master" subtitle="/api/v1/workflow/equipment-types" />
      <DataTable
        columns={[
          { id: 'id', label: 'ID' },
          { id: 'equipmentTypeCode', label: 'Code' },
          { id: 'equipmentTypeName', label: 'Name' },
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

export function LmsTestBookingView() {
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const q = useQuery({
    queryKey: ['lms', 'bookings', page, pageSize],
    queryFn: () => getTestBookingsPaged({ page: page + 1, pageSize }),
  });
  const rows =
    q.data?.success && q.data.data
      ? (q.data.data.items as Record<string, unknown>[])
      : [];
  const total = q.data?.success && q.data.data ? q.data.data.totalCount : 0;

  return (
    <Stack spacing={2}>
      <PageHeader title="Test booking" subtitle="/api/v1/workflow/test-bookings" />
      <DataTable
        columns={[
          { id: 'id', label: 'ID' },
          { id: 'patientId', label: 'Patient' },
          { id: 'bookingNotes', label: 'Notes' },
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

export function LmsBarcodeView() {
  const qc = useQueryClient();
  const [lookup, setLookup] = useState('');
  const res = useQuery({
    queryKey: ['lms', 'barcode-resolve', lookup],
    queryFn: () => resolveBarcode(lookup),
    enabled: lookup.length > 2,
  });
  const m = useMutation({
    mutationFn: registerBarcode,
    onSuccess: () => qc.invalidateQueries({ queryKey: ['lms'] }),
  });

  return (
    <Stack spacing={2}>
      <PageHeader
        title="Barcode management"
        subtitle="Register samples and resolve workflow via LMS integration API."
      />
      {m.isError ? <Alert severity="error">{getApiErrorMessage(m.error)}</Alert> : null}
      <Formik
        initialValues={{
          barcodeValue: '',
          testBookingItemId: 1,
          barcodeStatusReferenceValueId: 1,
          registeredFromSystem: 'TriVitaPortal',
        }}
        onSubmit={async (v) => {
          await m.mutateAsync({
            barcodeValue: v.barcodeValue,
            testBookingItemId: Number(v.testBookingItemId),
            barcodeStatusReferenceValueId: Number(v.barcodeStatusReferenceValueId),
            registeredFromSystem: v.registeredFromSystem,
          });
        }}
      >
        {({ isSubmitting }) => (
          <Form>
            <Stack direction="row" flexWrap="wrap" gap={2}>
              <FormikTextField name="barcodeValue" label="Barcode value" sx={{ minWidth: 200 }} />
              <FormikTextField name="testBookingItemId" label="Booking item ID" type="number" sx={{ width: 160 }} />
              <FormikTextField name="barcodeStatusReferenceValueId" label="Status ref. ID" type="number" sx={{ width: 140 }} />
              <Button type="submit" variant="contained" disabled={isSubmitting || m.isPending}>
                Register
              </Button>
            </Stack>
          </Form>
        )}
      </Formik>
      <Typography variant="subtitle2">Resolve barcode</Typography>
      <TextField
        label="Barcode"
        size="small"
        value={lookup}
        onChange={(e) => setLookup(e.target.value)}
        sx={{ maxWidth: 320 }}
      />
      {lookup.length > 2 && res.data ? (
        <Alert severity={res.data.success ? 'info' : 'warning'}>
          {JSON.stringify(res.data.data ?? res.data.message)}
        </Alert>
      ) : null}
    </Stack>
  );
}

export function LmsWorkflowDashboardView() {
  const b = useQuery({
    queryKey: ['lms', 'bookings', 'dash', 1],
    queryFn: () => getTestBookingsPaged({ page: 1, pageSize: 5 }),
  });
  const t = useQuery({
    queryKey: ['lms', 'tests', 'dash', 1],
    queryFn: () => getTestMastersPaged({ page: 1, pageSize: 5 }),
  });
  const bc = b.data?.success && b.data.data ? b.data.data.totalCount : 0;
  const tc = t.data?.success && t.data.data ? t.data.data.totalCount : 0;

  return (
    <Stack spacing={2}>
      <PageHeader
        title="Sample workflow dashboard"
        subtitle="Snapshot counts from test bookings and catalog tests."
      />
      <Stack direction="row" gap={4}>
        <Typography>Test bookings (total): {b.isLoading ? '…' : bc}</Typography>
        <Typography>Catalog tests (total): {t.isLoading ? '…' : tc}</Typography>
      </Stack>
    </Stack>
  );
}
