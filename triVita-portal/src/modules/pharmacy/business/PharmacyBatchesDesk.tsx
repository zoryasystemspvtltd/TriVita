import { Stack, TextField, Typography } from '@mui/material';
import { useQuery } from '@tanstack/react-query';
import { useMemo, useState } from 'react';
import { Link as RouterLink } from 'react-router-dom';
import { DataTable } from '@/components/common/DataTable';
import { PageHeader } from '@/components/layout/PageHeader';
import { DetailDrawer } from '@/components/layout/DetailDrawer';
import { SectionContainer } from '@/components/ds/SectionContainer';
import { TriVitaButton } from '@/components/ds/TriVitaButton';
import { getMedicineBatchPaged } from '@/services/pharmacyService';

export function PharmacyBatchesDesk() {
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const [search, setSearch] = useState('');
  const [searchApplied, setSearchApplied] = useState('');
  const [drawer, setDrawer] = useState<Record<string, unknown> | null>(null);

  const q = useQuery({
    queryKey: ['pharmacy', 'batches-desk', page, pageSize, searchApplied],
    queryFn: () => getMedicineBatchPaged({ page: page + 1, pageSize, search: searchApplied || undefined }),
  });

  const rows = useMemo(
    () => (q.data?.success && q.data.data ? [...q.data.data.items] : []) as Record<string, unknown>[],
    [q.data]
  );
  const total = q.data?.success && q.data.data ? q.data.data.totalCount : 0;

  return (
    <Stack spacing={3}>
      <PageHeader
        title="Medicine batches"
        subtitle="Lot / expiry tracking for formulary items tied to inventory."
        action={
          <TriVitaButton component={RouterLink} to="/pharmacy/data-registry?resource=medicine-batch" variant="outlined" size="small">
            API registry
          </TriVitaButton>
        }
      />
      <SectionContainer title="Batch register">
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
        <DataTable
          tableAriaLabel="Medicine batches"
          columns={[
            { id: 'id', label: 'ID' },
            { id: 'medicineId', label: 'Medicine' },
            { id: 'batchNumber', label: 'Batch #' },
            { id: 'expiryDate', label: 'Expiry', minWidth: 120 },
            {
              id: '_a',
              label: '',
              format: (row) => (
                <TriVitaButton size="small" variant="outlined" onClick={() => setDrawer(row)}>
                  Lot detail
                </TriVitaButton>
              ),
            },
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
      </SectionContainer>

      <DetailDrawer
        open={Boolean(drawer)}
        onClose={() => setDrawer(null)}
        title={drawer ? `Batch ${String(drawer.batchNumber ?? '')}` : ''}
        subtitle={drawer ? `Medicine #${String(drawer.medicineId ?? '')}` : undefined}
      >
        {drawer ? (
          <Stack spacing={1.5}>
            {Object.entries(drawer).map(([k, v]) => (
              <div key={k}>
                <Typography variant="caption" color="text.secondary" display="block">
                  {k}
                </Typography>
                <Typography variant="body2">{v === null || v === undefined ? '—' : String(v)}</Typography>
              </div>
            ))}
          </Stack>
        ) : null}
      </DetailDrawer>
    </Stack>
  );
}
