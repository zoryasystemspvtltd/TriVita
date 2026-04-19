import { Stack, Tab, Tabs, TextField, Typography } from '@mui/material';
import { useQuery } from '@tanstack/react-query';
import { useMemo, useState } from 'react';
import { Link as RouterLink } from 'react-router-dom';
import { DataTable } from '@/components/common/DataTable';
import { PageHeader } from '@/components/layout/PageHeader';
import { DetailDrawer } from '@/components/layout/DetailDrawer';
import { SectionContainer } from '@/components/ds/SectionContainer';
import { TriVitaButton } from '@/components/ds/TriVitaButton';
import { getGoodsReceiptPaged, getPurchaseOrdersPaged } from '@/services/pharmacyService';

export function PharmacyProcurementHub({ initialTab = 0 }: { initialTab?: 0 | 1 }) {
  const [tab, setTab] = useState<0 | 1>(initialTab);
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const [search, setSearch] = useState('');
  const [searchApplied, setSearchApplied] = useState('');
  const [drawer, setDrawer] = useState<Record<string, unknown> | null>(null);

  const po = useQuery({
    queryKey: ['pharmacy', 'po-hub', page, pageSize, searchApplied, tab],
    queryFn: () => getPurchaseOrdersPaged({ page: page + 1, pageSize, search: searchApplied || undefined }),
    enabled: tab === 0,
  });
  const gr = useQuery({
    queryKey: ['pharmacy', 'gr-hub', page, pageSize, searchApplied, tab],
    queryFn: () => getGoodsReceiptPaged({ page: page + 1, pageSize, search: searchApplied || undefined }),
    enabled: tab === 1,
  });

  const active = tab === 0 ? po : gr;
  const rows = useMemo(
    () => (active.data?.success && active.data.data ? [...active.data.data.items] : []) as Record<string, unknown>[],
    [active.data]
  );
  const total = active.data?.success && active.data.data ? active.data.data.totalCount : 0;

  return (
    <Stack spacing={3}>
      <PageHeader
        title="Procurement"
        subtitle="Purchase orders and goods receipt notes — operational triage workspace."
        action={
          <TriVitaButton component={RouterLink} to="/pharmacy/data-registry" variant="outlined" size="small">
            API registry
          </TriVitaButton>
        }
      />
      <Tabs value={tab} onChange={(_, v) => { setTab(v); setPage(0); }} sx={{ borderBottom: 1, borderColor: 'divider' }}>
        <Tab label="Purchase orders" />
        <Tab label="Goods receipt" />
      </Tabs>
      <SectionContainer
        title={tab === 0 ? 'Open purchase orders' : 'Inbound receipts'}
        subtitle="Filter by document number or supplier reference depending on API search support."
      >
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
          tableAriaLabel={tab === 0 ? 'Purchase orders' : 'Goods receipts'}
          columns={
            tab === 0
              ? [
                  { id: 'id', label: 'ID' },
                  { id: 'poNumber', label: 'PO #' },
                  { id: 'supplierId', label: 'Supplier' },
                  { id: 'orderDate', label: 'Ordered' },
                  { id: 'status', label: 'Status' },
                  {
                    id: '_a',
                    label: '',
                    format: (row) => (
                      <TriVitaButton size="small" variant="outlined" onClick={() => setDrawer(row)}>
                        Detail
                      </TriVitaButton>
                    ),
                  },
                ]
              : [
                  { id: 'id', label: 'ID' },
                  { id: 'grnNumber', label: 'GRN #' },
                  { id: 'supplierId', label: 'Supplier' },
                  { id: 'receivedOn', label: 'Received' },
                  {
                    id: '_a',
                    label: '',
                    format: (row) => (
                      <TriVitaButton size="small" variant="outlined" onClick={() => setDrawer(row)}>
                        Detail
                      </TriVitaButton>
                    ),
                  },
                ]
          }
          rows={rows}
          rowKey={(r) => String(r.id ?? '')}
          totalCount={total}
          page={page}
          pageSize={pageSize}
          onPageChange={(p, ps) => {
            setPage(p);
            setPageSize(ps);
          }}
          loading={active.isLoading}
        />
      </SectionContainer>

      <DetailDrawer
        open={Boolean(drawer)}
        onClose={() => setDrawer(null)}
        title={drawer ? (tab === 0 ? `PO ${String(drawer.poNumber ?? '')}` : `GRN ${String(drawer.grnNumber ?? '')}`) : ''}
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
