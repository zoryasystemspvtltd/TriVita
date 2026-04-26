import { PagedResourceView } from '@/components/resource/PagedResourceView';
import { usePagedList } from '@/hooks/usePagedList';
import { getLabOrdersPaged, getResultHistoryPaged, getSampleTrackingPaged } from '@/services/lisService';

export function LisLabOrdersView() {
  const list = usePagedList<Record<string, unknown>>(['lis', 'lab-orders'], (page, pageSize) =>
    getLabOrdersPaged({ page, pageSize })
  );
  return (
    <PagedResourceView
      title="Lab orders (ingestion context)"
      subtitle="Laboratory orders driving accession, processing, and results."
      tableLabel="Lab orders"
      list={list}
      columns={[
        { id: 'id', label: 'ID' },
        { id: 'orderNumber', label: 'Order #' },
        { id: 'patientId', label: 'Patient' },
        { id: 'orderStatusReferenceValueId', label: 'Status ref.' },
      ]}
    />
  );
}

export function LisSampleTrackingView() {
  const list = usePagedList<Record<string, unknown>>(['lis', 'sample-tracking'], (page, pageSize) =>
    getSampleTrackingPaged({ page, pageSize })
  );
  return (
    <PagedResourceView
      title="Sample tracking"
      subtitle="Sample status from collection through processing."
      tableLabel="Sample tracking"
      list={list}
      columns={[
        { id: 'id', label: 'ID' },
        { id: 'sampleBarcodeId', label: 'Barcode' },
        { id: 'currentStatusReferenceValueId', label: 'Status ref.' },
        { id: 'lastEventAt', label: 'Last event' },
      ]}
    />
  );
}

export function LisResultHistoryView() {
  const list = usePagedList<Record<string, unknown>>(['lis', 'result-history'], (page, pageSize) =>
    getResultHistoryPaged({ page, pageSize })
  );
  return (
    <PagedResourceView
      title="Result history"
      subtitle="Audit history of result edits and sign-off events."
      tableLabel="Result history"
      list={list}
      columns={[
        { id: 'id', label: 'ID' },
        { id: 'labResultId', label: 'Result' },
        { id: 'changeType', label: 'Change' },
        { id: 'changedAt', label: 'When' },
      ]}
    />
  );
}
