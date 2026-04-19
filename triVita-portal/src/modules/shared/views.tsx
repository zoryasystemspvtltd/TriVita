import { Stack, TextField, Typography } from '@mui/material';
import { useQuery } from '@tanstack/react-query';
import { useMemo, useState } from 'react';
import { Link as RouterLink } from 'react-router-dom';
import { DataTable } from '@/components/common/DataTable';
import { PageHeader } from '@/components/layout/PageHeader';
import { DetailDrawer } from '@/components/layout/DetailDrawer';
import { SectionContainer } from '@/components/ds/SectionContainer';
import { TriVitaButton } from '@/components/ds/TriVitaButton';
import { getFacilityById, listFacilities } from '@/services/sharedService';
import { EnterpriseHierarchyExplorer } from '@/modules/shared/EnterpriseHierarchyExplorer';

export function SharedHierarchyView() {
  return <EnterpriseHierarchyExplorer />;
}

export function SharedFacilitiesView() {
  const [filterBu, setFilterBu] = useState('');
  const [search, setSearch] = useState('');
  const [searchApplied, setSearchApplied] = useState('');
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const [drawerId, setDrawerId] = useState<number | null>(null);

  const businessUnitId = filterBu.trim() === '' ? undefined : Number(filterBu);
  const q = useQuery({
    queryKey: ['shared', 'facilities-business', businessUnitId ?? 'all'],
    queryFn: () => listFacilities(businessUnitId != null && Number.isFinite(businessUnitId) ? businessUnitId : undefined),
  });

  const detail = useQuery({
    queryKey: ['shared', 'facility-detail', drawerId],
    queryFn: () => getFacilityById(drawerId!),
    enabled: drawerId != null,
  });

  const all = useMemo(() => {
    const raw = q.data?.success && q.data.data ? [...q.data.data] : [];
    const f = searchApplied.trim().toLowerCase();
    if (!f) return raw as Record<string, unknown>[];
    return raw.filter((r) => {
      const row = r as Record<string, unknown>;
      const blob = `${row.facilityCode ?? ''} ${row.facilityName ?? ''} ${row.id ?? ''}`.toLowerCase();
      return blob.includes(f);
    }) as Record<string, unknown>[];
  }, [q.data, searchApplied]);

  const slice = all.slice(page * pageSize, page * pageSize + pageSize);
  const total = all.length;
  const detailData = detail.data?.success ? detail.data.data : null;

  return (
    <Stack spacing={3}>
      <PageHeader
        title="Facilities"
        subtitle="Operational directory for the current tenant — filter by business unit, search locally, drill into a facility record."
        action={
          <TriVitaButton component={RouterLink} to="/shared/data-registry?resource=facilities" variant="outlined" size="small">
            API registry
          </TriVitaButton>
        }
      />
      <SectionContainer title="Directory">
        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} sx={{ mb: 2 }} alignItems={{ sm: 'flex-end' }}>
          <TextField
            label="Business unit ID (optional filter)"
            size="small"
            value={filterBu}
            onChange={(e) => setFilterBu(e.target.value)}
            sx={{ width: 260 }}
          />
          <TextField
            label="Search name / code / id"
            size="small"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && setSearchApplied(search)}
            sx={{ flex: 1, minWidth: 200 }}
          />
          <TriVitaButton variant="outlined" onClick={() => { setSearchApplied(search); setPage(0); }}>
            Apply
          </TriVitaButton>
          <TriVitaButton variant="outlined" onClick={() => void q.refetch()}>
            Refresh
          </TriVitaButton>
        </Stack>
        <Typography variant="caption" color="text.secondary" sx={{ display: 'block', mb: 1 }}>
          For full create/update flows open <strong>Enterprise admin</strong> from the Shared module menu.
        </Typography>
        <DataTable
          tableAriaLabel="Facilities"
          columns={[
            { id: 'id', label: 'ID' },
            { id: 'facilityCode', label: 'Code' },
            { id: 'facilityName', label: 'Name' },
            { id: 'facilityType', label: 'Type' },
            {
              id: '_a',
              label: '',
              format: (row) => (
                <TriVitaButton size="small" variant="outlined" onClick={() => setDrawerId(Number(row.id))}>
                  Profile
                </TriVitaButton>
              ),
            },
          ]}
          rows={slice}
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
        open={drawerId != null}
        onClose={() => setDrawerId(null)}
        title={detailData ? String(detailData.facilityName ?? 'Facility') : ''}
        subtitle={detailData ? String(detailData.facilityCode ?? '') : undefined}
      >
        {detail.isLoading ? <Typography color="text.secondary">Loading…</Typography> : null}
        {detailData ? (
          <Stack spacing={1.5}>
            {Object.entries(detailData).map(([k, v]) => (
              <div key={k}>
                <Typography variant="caption" color="text.secondary" display="block">
                  {k}
                </Typography>
                <Typography variant="body2">{v === null || v === undefined ? '—' : String(v)}</Typography>
              </div>
            ))}
          </Stack>
        ) : detail.data && !detail.data.success ? (
          <Typography color="error">{detail.data.message}</Typography>
        ) : null}
      </DetailDrawer>
    </Stack>
  );
}
