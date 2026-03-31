import { Stack } from '@mui/material';
import { useQuery } from '@tanstack/react-query';
import { useState } from 'react';
import { DataTable } from '@/components/common/DataTable';
import { PageHeader } from '@/components/layout/PageHeader';
import { getNotificationLogsPaged, getNotificationTemplatesPaged } from '@/services/communicationService';

export function CommunicationNotificationsView() {
  const [tPage, setTPage] = useState(0);
  const [tSize, setTSize] = useState(20);
  const [lPage, setLPage] = useState(0);
  const [lSize, setLSize] = useState(20);

  const templates = useQuery({
    queryKey: ['comm', 'templates', tPage, tSize],
    queryFn: () => getNotificationTemplatesPaged({ page: tPage + 1, pageSize: tSize }),
  });
  const logs = useQuery({
    queryKey: ['comm', 'logs', lPage, lSize],
    queryFn: () => getNotificationLogsPaged({ page: lPage + 1, pageSize: lSize }),
  });

  const tRows =
    templates.data?.success && templates.data.data
      ? (templates.data.data.items as Record<string, unknown>[])
      : [];
  const tTotal = templates.data?.success && templates.data.data ? templates.data.data.totalCount : 0;

  const lRows =
    logs.data?.success && logs.data.data ? (logs.data.data.items as Record<string, unknown>[]) : [];
  const lTotal = logs.data?.success && logs.data.data ? logs.data.data.totalCount : 0;

  return (
    <Stack spacing={4}>
      <PageHeader
        title="Notifications & templates"
        subtitle="Communication service: templates and delivery logs."
      />
      <Stack spacing={1}>
        <PageHeader title="Templates" />
        <DataTable
          columns={[
            { id: 'id', label: 'ID' },
            { id: 'templateCode', label: 'Code' },
            { id: 'channelType', label: 'Channel' },
          ]}
          rows={tRows}
          rowKey={(r) => String(r.id ?? Math.random())}
          totalCount={tTotal}
          page={tPage}
          pageSize={tSize}
          onPageChange={(p, ps) => {
            setTPage(p);
            setTSize(ps);
          }}
          loading={templates.isLoading}
        />
      </Stack>
      <Stack spacing={1}>
        <PageHeader title="Delivery logs" />
        <DataTable
          columns={[
            { id: 'id', label: 'ID' },
            { id: 'notificationId', label: 'Notification' },
            { id: 'deliveryStatus', label: 'Status' },
          ]}
          rows={lRows}
          rowKey={(r) => String(r.id ?? Math.random())}
          totalCount={lTotal}
          page={lPage}
          pageSize={lSize}
          onPageChange={(p, ps) => {
            setLPage(p);
            setLSize(ps);
          }}
          loading={logs.isLoading}
        />
      </Stack>
    </Stack>
  );
}
