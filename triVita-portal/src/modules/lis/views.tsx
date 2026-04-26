import { Alert, Button, Stack, TextField, Typography } from '@mui/material';
import { useQuery } from '@tanstack/react-query';
import { useState } from 'react';
import { DataTable } from '@/components/common/DataTable';
import { PageHeader } from '@/components/layout/PageHeader';
import { analyzerQueryTest, getLabResultsPaged, getResultApprovalsPaged } from '@/services/lisService';

export function LisAnalyzerView() {
  const [barcode, setBarcode] = useState('');
  const q = useQuery({
    queryKey: ['lis', 'analyzer', barcode],
    queryFn: () => analyzerQueryTest(barcode),
    enabled: barcode.length > 2,
  });

  return (
    <Stack spacing={3}>
      <PageHeader title="Analyzer monitoring" subtitle="LIS analyzer query-test (barcode → equipment codes)." />
      <TextField
        label="Barcode"
        size="small"
        value={barcode}
        onChange={(e) => setBarcode(e.target.value)}
        sx={{ maxWidth: 360 }}
      />
      {barcode.length > 2 && q.data ? (
        <Alert severity={q.data.success ? 'success' : 'warning'}>
          <Typography variant="body2" component="pre" sx={{ whiteSpace: 'pre-wrap', m: 0 }}>
            {JSON.stringify(q.data.data ?? q.data.message, null, 2)}
          </Typography>
        </Alert>
      ) : null}
    </Stack>
  );
}

export function LisResultsView() {
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const q = useQuery({
    queryKey: ['lis', 'results', page, pageSize],
    queryFn: () => getLabResultsPaged({ page: page + 1, pageSize }),
  });
  const rows =
    q.data?.success && q.data.data
      ? (q.data.data.items as Record<string, unknown>[])
      : [];
  const total = q.data?.success && q.data.data ? q.data.data.totalCount : 0;

  return (
    <Stack spacing={3}>
      <PageHeader title="Result viewer" subtitle="Review and explore verified laboratory results." />
      <DataTable
        columns={[
          { id: 'id', label: 'ID' },
          { id: 'labOrderItemId', label: 'Order item' },
          { id: 'resultValue', label: 'Value' },
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

export function LisVerificationView() {
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const q = useQuery({
    queryKey: ['lis', 'verification', page, pageSize],
    queryFn: () => getResultApprovalsPaged({ page: page + 1, pageSize }),
  });
  const rows =
    q.data?.success && q.data.data
      ? (q.data.data.items as Record<string, unknown>[])
      : [];
  const total = q.data?.success && q.data.data ? q.data.data.totalCount : 0;

  return (
    <Stack spacing={3}>
      <PageHeader title="Result verification" subtitle="Approve or release results through the lab workflow." />
      <Button size="small" variant="outlined" onClick={() => void q.refetch()}>
        Refresh
      </Button>
      <DataTable
        columns={[
          { id: 'id', label: 'ID' },
          { id: 'labResultId', label: 'Result' },
          { id: 'approvalStatusReferenceValueId', label: 'Status ref.' },
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
