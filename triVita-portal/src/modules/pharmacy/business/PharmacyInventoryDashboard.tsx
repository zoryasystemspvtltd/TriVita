import { Card, CardContent, Stack, TextField, Typography } from '@mui/material';
import { useQuery } from '@tanstack/react-query';
import { useMemo, useState } from 'react';
import { Link as RouterLink } from 'react-router-dom';
import { DataTable } from '@/components/common/DataTable';
import { PageHeader } from '@/components/layout/PageHeader';
import { DetailDrawer } from '@/components/layout/DetailDrawer';
import { SectionContainer } from '@/components/ds/SectionContainer';
import { TriVitaButton } from '@/components/ds/TriVitaButton';
import { getInventoryLocationsPaged } from '@/services/pharmacyService';

export function PharmacyInventoryDashboard() {
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const [search, setSearch] = useState('');
  const [searchApplied, setSearchApplied] = useState('');
  const [drawer, setDrawer] = useState<Record<string, unknown> | null>(null);

  const q = useQuery({
    queryKey: ['pharmacy', 'inv-dash', page, pageSize, searchApplied],
    queryFn: () => getInventoryLocationsPaged({ page: page + 1, pageSize, search: searchApplied || undefined }),
  });

  const rows = useMemo(
    () => (q.data?.success && q.data.data ? [...q.data.data.items] : []) as Record<string, unknown>[],
    [q.data]
  );
  const total = q.data?.success && q.data.data ? q.data.data.totalCount : 0;

  const kpis = useMemo(() => {
    const active = rows.filter((r) => String(r.isActive ?? 'true') !== 'false').length;
    return { onPage: rows.length, activeOnPage: active };
  }, [rows]);

  return (
    <Stack spacing={3}>
      <PageHeader
        title="Inventory locations"
        subtitle="Warehouse, store, and dispensing locations with capacity to drill into each row."
        action={
          <TriVitaButton component={RouterLink} to="/pharmacy/data-registry?resource=inventory-locations" variant="outlined" size="small">
            API registry
          </TriVitaButton>
        }
      />
      <Stack direction={{ xs: 'column', md: 'row' }} spacing={2} useFlexGap>
        <Card variant="outlined" sx={{ borderRadius: 2, flex: '1 1 200px' }}>
          <CardContent>
            <Typography variant="overline" color="text.secondary">
              This page
            </Typography>
            <Typography variant="h4">{kpis.onPage}</Typography>
            <Typography variant="body2" color="text.secondary">
              Locations loaded in the current result window
            </Typography>
          </CardContent>
        </Card>
        <Card variant="outlined" sx={{ borderRadius: 2, flex: '1 1 200px' }}>
          <CardContent>
            <Typography variant="overline" color="text.secondary">
              Active (sample)
            </Typography>
            <Typography variant="h4">{kpis.activeOnPage}</Typography>
            <Typography variant="body2" color="text.secondary">
              Active flags among visible rows
            </Typography>
          </CardContent>
        </Card>
        <Card variant="outlined" sx={{ borderRadius: 2, flex: '1 1 200px' }}>
          <CardContent>
            <Typography variant="overline" color="text.secondary">
              Total (server)
            </Typography>
            <Typography variant="h4">{total}</Typography>
            <Typography variant="body2" color="text.secondary">
              Paged total from API
            </Typography>
          </CardContent>
        </Card>
      </Stack>

      <SectionContainer title="Locations" subtitle="Search by code or name; open a row for structured detail.">
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
          tableAriaLabel="Inventory locations"
          columns={[
            { id: 'id', label: 'ID' },
            { id: 'locationCode', label: 'Code' },
            { id: 'locationName', label: 'Name' },
            { id: 'locationType', label: 'Type' },
            {
              id: '_a',
              label: '',
              format: (row) => (
                <TriVitaButton size="small" variant="outlined" onClick={() => setDrawer(row)}>
                  View
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
        title={drawer ? String(drawer.locationName ?? 'Location') : ''}
        subtitle={drawer ? String(drawer.locationCode ?? '') : undefined}
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
