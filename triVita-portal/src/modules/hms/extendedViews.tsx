import { Stack, Tab, Tabs } from '@mui/material';
import { useCallback, useState } from 'react';
import { PageHeader } from '@/components/layout/PageHeader';
import { PagedResourceView } from '@/components/resource/PagedResourceView';
import { usePagedList } from '@/hooks/usePagedList';
import {
  getAdmissionsPaged,
  getBillingHeadersPaged,
  getBillingItemsPaged,
  getIpdBedsPaged,
  getIpdWardsPaged,
  getLabBillingInvoicesPaged,
  getPaymentTransactionsPaged,
  getPrescriptionsPaged,
  getVisitsPaged,
} from '@/services/hmsService';

export function HmsIpdHubView() {
  const [tab, setTab] = useState(0);
  const fetchPage = useCallback(
    (page: number, pageSize: number) => {
      const p = { page, pageSize };
      if (tab === 0) return getIpdWardsPaged(p);
      if (tab === 1) return getIpdBedsPaged(p);
      return getAdmissionsPaged(p);
    },
    [tab]
  );
  const list = usePagedList<Record<string, unknown>>(['hms', 'ipd', tab], fetchPage);

  const columns =
    tab === 0
      ? [
          { id: 'id', label: 'ID' },
          { id: 'wardCode', label: 'Code' },
          { id: 'wardName', label: 'Name' },
          { id: 'facilityId', label: 'Facility' },
        ]
      : tab === 1
        ? [
            { id: 'id', label: 'ID' },
            { id: 'bedCode', label: 'Bed' },
            { id: 'wardId', label: 'Ward' },
            { id: 'bedOperationalStatusReferenceValueId', label: 'Status ref.' },
          ]
        : [
            { id: 'id', label: 'ID' },
            { id: 'admissionNo', label: 'Admission #' },
            { id: 'patientMasterId', label: 'Patient' },
            { id: 'bedId', label: 'Bed' },
          ];

  return (
    <Stack spacing={2}>
      <PageHeader
        title="IPD — wards, beds & admissions"
        subtitle="GET /api/v1/ipd/wards, /ipd/beds, /admissions"
      />
      <Tabs
        value={tab}
        onChange={(_, v) => {
          setTab(v);
          list.setPage(0);
        }}
        aria-label="IPD data category"
      >
        <Tab label="Wards" id="ipd-tab-0" aria-controls="ipd-panel-0" />
        <Tab label="Beds" id="ipd-tab-1" aria-controls="ipd-panel-1" />
        <Tab label="Admissions" id="ipd-tab-2" aria-controls="ipd-panel-2" />
      </Tabs>
      <div role="tabpanel" id={`ipd-panel-${tab}`} aria-labelledby={`ipd-tab-${tab}`}>
        <PagedResourceView
          showPageHeader={false}
          tableLabel={tab === 0 ? 'IPD wards' : tab === 1 ? 'IPD beds' : 'Admissions'}
          list={list}
          columns={columns}
        />
      </div>
    </Stack>
  );
}

export function HmsPrescriptionsView() {
  const list = usePagedList<Record<string, unknown>>(['hms', 'prescriptions'], (page, pageSize) =>
    getPrescriptionsPaged({ page, pageSize })
  );
  return (
    <PagedResourceView
      title="Prescriptions"
      subtitle="GET /api/v1/prescriptions"
      tableLabel="Prescriptions"
      list={list}
      columns={[
        { id: 'id', label: 'ID' },
        { id: 'visitId', label: 'Visit' },
        { id: 'patientId', label: 'Patient' },
        { id: 'prescriptionStatusReferenceValueId', label: 'Status ref.' },
      ]}
    />
  );
}

export function HmsVisitsView() {
  const list = usePagedList<Record<string, unknown>>(['hms', 'visits'], (page, pageSize) =>
    getVisitsPaged({ page, pageSize })
  );
  return (
    <PagedResourceView
      title="OPD / visits"
      subtitle="GET /api/v1/visits"
      tableLabel="Clinical visits"
      list={list}
      columns={[
        { id: 'id', label: 'ID' },
        { id: 'patientId', label: 'Patient' },
        { id: 'visitTypeId', label: 'Visit type' },
        { id: 'scheduledStartOn', label: 'Start' },
      ]}
    />
  );
}

export function HmsBillingHubView() {
  const [tab, setTab] = useState(0);
  const fetchPage = useCallback(
    (page: number, pageSize: number) => {
      const p = { page, pageSize };
      if (tab === 0) return getBillingHeadersPaged(p);
      if (tab === 1) return getBillingItemsPaged(p);
      if (tab === 2) return getPaymentTransactionsPaged(p);
      return getLabBillingInvoicesPaged(p);
    },
    [tab]
  );
  const list = usePagedList<Record<string, unknown>>(['hms', 'billing-hub', tab], fetchPage);

  const columns =
    tab === 0
      ? [
          { id: 'id', label: 'ID' },
          { id: 'billNo', label: 'Bill #' },
          { id: 'patientId', label: 'Patient' },
          { id: 'visitId', label: 'Visit' },
          { id: 'grandTotal', label: 'Total' },
        ]
      : tab === 1
        ? [
            { id: 'id', label: 'ID' },
            { id: 'billingHeaderId', label: 'Header' },
            { id: 'description', label: 'Description' },
            { id: 'quantity', label: 'Qty' },
          ]
        : tab === 2
          ? [
              { id: 'id', label: 'ID' },
              { id: 'billingHeaderId', label: 'Billing' },
              { id: 'amount', label: 'Amount' },
              { id: 'paymentModeId', label: 'Mode' },
            ]
          : [
              { id: 'id', label: 'ID' },
              { id: 'invoiceNo', label: 'Invoice #' },
              { id: 'patientId', label: 'Patient' },
              { id: 'labOrderId', label: 'Lab order' },
              { id: 'grandTotal', label: 'Total' },
              { id: 'balanceDue', label: 'Balance' },
            ];

  return (
    <Stack spacing={2}>
      <PageHeader
        title="Billing & payments"
        subtitle="Headers, line items, payments, and LMS lab invoices via HMS integration."
      />
      <Tabs
        value={tab}
        onChange={(_, v) => {
          setTab(v);
          list.setPage(0);
        }}
        aria-label="Billing views"
        variant="scrollable"
        scrollButtons="auto"
      >
        <Tab label="Headers" />
        <Tab label="Line items" />
        <Tab label="Payments" />
        <Tab label="Lab invoices (LMS)" />
      </Tabs>
      <PagedResourceView showPageHeader={false} tableLabel="Billing data" list={list} columns={columns} />
    </Stack>
  );
}
