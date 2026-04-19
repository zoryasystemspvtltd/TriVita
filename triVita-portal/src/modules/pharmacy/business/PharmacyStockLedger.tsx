import { Box, Stack, TextField, Typography } from '@mui/material';
import { useQuery } from '@tanstack/react-query';
import { useMemo, useState } from 'react';
import { Link as RouterLink } from 'react-router-dom';
import { DataTable } from '@/components/common/DataTable';
import { PageHeader } from '@/components/layout/PageHeader';
import { SectionContainer } from '@/components/ds/SectionContainer';
import { TriVitaButton } from '@/components/ds/TriVitaButton';
import { getStockLedgerPaged } from '@/services/pharmacyService';

export function PharmacyStockLedger() {
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [search, setSearch] = useState('');
  const [searchApplied, setSearchApplied] = useState('');

  const q = useQuery({
    queryKey: ['pharmacy', 'ledger', page, pageSize, searchApplied],
    queryFn: () => getStockLedgerPaged({ page: page + 1, pageSize, search: searchApplied || undefined }),
  });

  const rows = useMemo(
    () => (q.data?.success && q.data.data ? [...q.data.data.items] : []) as Record<string, unknown>[],
    [q.data]
  );
  const total = q.data?.success && q.data.data ? q.data.data.totalCount : 0;

  return (
    <Stack spacing={3}>
      <PageHeader
        title="Stock ledger"
        subtitle="Chronological inventory movements — read-only operational view with server search."
        action={
          <TriVitaButton component={RouterLink} to="/pharmacy/data-registry?resource=stock-ledger" variant="outlined" size="small">
            API registry
          </TriVitaButton>
        }
      />
      <SectionContainer title="Movement register" subtitle="Each row reflects a quantity delta against batch stock.">
        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} sx={{ mb: 2 }} alignItems={{ sm: 'flex-end' }}>
          <TextField
            label="Search"
            size="small"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && setSearchApplied(search)}
            sx={{ flex: 1, minWidth: 200 }}
          />
          <TriVitaButton variant="outlined" onClick={() => setSearchApplied(search)}>
            Apply
          </TriVitaButton>
        </Stack>
        <Box
          sx={{
            '& .MuiTableRow-root:nth-of-type(even)': { bgcolor: 'action.hover' },
            '& .MuiTableCell-root': { fontFamily: 'ui-monospace, Consolas, monospace', fontSize: 13 },
          }}
        >
          <DataTable
            tableAriaLabel="Stock ledger"
            columns={[
              { id: 'id', label: 'Entry', minWidth: 72 },
              { id: 'batchStockId', label: 'Batch stock', minWidth: 100 },
              { id: 'quantityDelta', label: 'Δ Qty', align: 'right' },
              { id: 'transactionType', label: 'Type', minWidth: 120 },
              { id: 'referenceNumber', label: 'Ref #' },
              { id: 'transactionDate', label: 'When', minWidth: 160 },
            ]}
            rows={rows}
            rowKey={(r) => String(r.id ?? '')}
            totalCount={total}
            page={page}
            pageSize={pageSize}
            onPageChange={(p, ps) => {
              setPage(p);
              setPageSize(ps);
            }}
            loading={q.isLoading}
          />
        </Box>
        <Typography variant="caption" color="text.secondary" sx={{ mt: 1, display: 'block' }}>
          Tip: pair this view with batch stock in the API registry when reconciling physical counts.
        </Typography>
      </SectionContainer>
    </Stack>
  );
}
