import { PagedResourceView } from '@/components/resource/PagedResourceView';
import { usePagedList } from '@/hooks/usePagedList';
import { getEquipmentFacilityMappingsPaged, getWorkQueuePaged } from '@/services/lmsService';

export function LmsEquipmentMappingsView() {
  const list = usePagedList<Record<string, unknown>>(
    ['lms', 'equipment-facility-mappings'],
    (page, pageSize) => getEquipmentFacilityMappingsPaged({ page, pageSize })
  );
  return (
    <PagedResourceView
      title="Equipment ↔ facility mappings"
      subtitle="GET /api/v1/workflow/equipment-facility-mappings"
      tableLabel="Equipment facility mappings"
      list={list}
      columns={[
        { id: 'id', label: 'ID' },
        { id: 'equipmentTypeId', label: 'Equipment type' },
        { id: 'facilityId', label: 'Facility' },
        { id: 'isActive', label: 'Active' },
      ]}
    />
  );
}

export function LmsWorkQueueView() {
  const list = usePagedList<Record<string, unknown>>(['lms', 'work-queue'], (page, pageSize) =>
    getWorkQueuePaged({ page, pageSize })
  );
  return (
    <PagedResourceView
      title="Lab work queue"
      subtitle="GET /api/v1/work-queue — operational queue for lab processing."
      tableLabel="Work queue"
      list={list}
      columns={[
        { id: 'id', label: 'ID' },
        { id: 'labOrderItemId', label: 'Order item' },
        { id: 'stageId', label: 'Stage' },
        { id: 'queueStatusReferenceValueId', label: 'Queue status ref.' },
        { id: 'queuedOn', label: 'Queued' },
      ]}
    />
  );
}
