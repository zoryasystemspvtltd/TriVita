import { Alert, Button, Stack } from '@mui/material';
import { DataTable, type Column } from '@/components/common/DataTable';
import { PageHeader } from '@/components/layout/PageHeader';
import { QueryStateBar } from '@/components/common/QueryState';
import type { PagedListState } from '@/hooks/usePagedList';

export function PagedResourceView({
  title,
  subtitle,
  tableLabel,
  list,
  columns,
  showPageHeader = true,
}: {
  title?: string;
  subtitle?: string;
  tableLabel: string;
  list: PagedListState<Record<string, unknown>>;
  columns: Column<Record<string, unknown>>[];
  showPageHeader?: boolean;
}) {
  return (
    <Stack spacing={2}>
      {showPageHeader && title ? <PageHeader title={title} subtitle={subtitle} /> : null}
      <Button size="small" variant="outlined" onClick={() => void list.query.refetch()} sx={{ alignSelf: 'flex-start' }}>
        Refresh
      </Button>
      <QueryStateBar
        isLoading={list.query.isFetching}
        isError={list.query.isError}
        error={list.query.error}
      />
      {list.query.data && list.query.data.success === false ? (
        <Alert severity="warning">{list.query.data.message ?? 'Request failed'}</Alert>
      ) : null}
      <DataTable
        tableAriaLabel={tableLabel}
        columns={columns}
        rows={list.rows}
        rowKey={(r) => {
          const o = r as Record<string, unknown>;
          return String(o.id ?? o.labInvoiceHeaderId ?? o.sampleBarcodeId ?? '');
        }}
        totalCount={list.totalCount}
        page={list.page}
        pageSize={list.pageSize}
        onPageChange={list.onPageChange}
        loading={list.query.isLoading}
      />
    </Stack>
  );
}
