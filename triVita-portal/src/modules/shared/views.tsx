import { Alert, Button, Stack, TextField, Typography } from '@mui/material';
import { useQuery } from '@tanstack/react-query';
import { useState } from 'react';
import { DataTable } from '@/components/common/DataTable';
import { PageHeader } from '@/components/layout/PageHeader';
import { getEnterpriseHierarchy, listFacilities } from '@/services/sharedService';

export function SharedHierarchyView() {
  const [enterpriseId, setEnterpriseId] = useState('1');
  const id = Number(enterpriseId) || 1;
  const q = useQuery({
    queryKey: ['shared', 'hierarchy', id],
    queryFn: () => getEnterpriseHierarchy(id),
  });

  return (
    <Stack spacing={2}>
      <PageHeader
        title="Enterprise hierarchy"
        subtitle="GET /api/v1/enterprise-hierarchy/{enterpriseId}"
      />
      <Stack direction="row" gap={2} alignItems="center">
        <TextField
          label="Enterprise ID"
          size="small"
          value={enterpriseId}
          onChange={(e) => setEnterpriseId(e.target.value)}
          sx={{ width: 160 }}
        />
        <Button variant="outlined" onClick={() => void q.refetch()}>
          Load
        </Button>
      </Stack>
      {q.data ? (
        <Alert severity={q.data.success ? 'info' : 'warning'}>
          <Typography variant="body2" component="pre" sx={{ whiteSpace: 'pre-wrap', m: 0, maxHeight: 400, overflow: 'auto' }}>
            {JSON.stringify(q.data.data ?? q.data.message, null, 2)}
          </Typography>
        </Alert>
      ) : null}
    </Stack>
  );
}

export function SharedFacilitiesView() {
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const q = useQuery({
    queryKey: ['shared', 'facilities'],
    queryFn: () => listFacilities(),
  });
  const all = q.data?.success && q.data.data ? [...q.data.data] : [];
  const total = all.length;
  const slice = all.slice(page * pageSize, page * pageSize + pageSize);

  return (
    <Stack spacing={2}>
      <PageHeader title="Facility management" subtitle="GET /api/v1/facilities" />
      <DataTable
        columns={[
          { id: 'id', label: 'ID' },
          { id: 'facilityCode', label: 'Code' },
          { id: 'facilityName', label: 'Name' },
          { id: 'facilityType', label: 'Type' },
        ]}
        rows={slice as Record<string, unknown>[]}
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
