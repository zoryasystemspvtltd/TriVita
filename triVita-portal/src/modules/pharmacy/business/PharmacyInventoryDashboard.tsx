import { Card, CardContent, Stack, TextField, Typography } from '@mui/material';
import { useQuery } from '@tanstack/react-query';
import { useEffect, useMemo, useState } from 'react';
import { DataTable } from '@/components/common/DataTable';
import { PageHeader } from '@/components/layout/PageHeader';
import { FormSection } from '@/components/forms/FormSection';
import { getBatchStockPaged, getMedicineBatchPaged, getMedicinePaged } from '@/services/pharmacyService';

type Row = Record<string, unknown> & { id?: number };

function pickStr(r: Record<string, unknown>, ...keys: string[]) {
  for (const k of keys) {
    const v = r[k];
    if (v != null && String(v).trim() !== '') return String(v);
  }
  return '';
}

function num(r: Row, ...keys: string[]) {
  for (const k of keys) {
    const v = r[k];
    const n = Number(v);
    if (Number.isFinite(n)) return n;
  }
  return 0;
}

type MedAgg = { medicineName: string; totalQty: number };

export function PharmacyInventoryDashboard() {
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const [search, setSearch] = useState('');
  const [searchApplied, setSearchApplied] = useState('');

  useEffect(() => {
    const t = window.setTimeout(() => setSearchApplied(search), 400);
    return () => window.clearTimeout(t);
  }, [search]);

  useEffect(() => {
    setPage(0);
  }, [searchApplied]);

  const kpiStock = useQuery({
    queryKey: ['pharmacy', 'inv-kpi'],
    queryFn: () => getBatchStockPaged({ page: 1, pageSize: 2000 }),
    staleTime: 60_000,
  });

  const stock = useQuery({
    queryKey: ['pharmacy', 'inv-stock', page, pageSize, searchApplied],
    queryFn: () => getBatchStockPaged({ page: page + 1, pageSize, search: searchApplied || undefined }),
  });

  const batches = useQuery({
    queryKey: ['pharmacy', 'inv-batches-map'],
    queryFn: () => getMedicineBatchPaged({ page: 1, pageSize: 3000 }),
    staleTime: 60_000,
  });

  const medicines = useQuery({
    queryKey: ['pharmacy', 'inv-meds'],
    queryFn: () => getMedicinePaged({ page: 1, pageSize: 2000 }),
    staleTime: 120_000,
  });

  const batchById = useMemo(() => {
    const m = new Map<number, Row>();
    const d = batches.data?.success ? batches.data.data : null;
    for (const b of (d?.items ?? []) as Row[]) {
      const id = Number(b.id);
      if (Number.isFinite(id)) m.set(id, b);
    }
    return m;
  }, [batches.data]);

  const medMap = useMemo(() => {
    const m = new Map<number, string>();
    const d = medicines.data?.success ? medicines.data.data : null;
    for (const x of (d?.items ?? []) as Row[]) {
      const id = Number(x.id);
      if (Number.isFinite(id)) m.set(id, pickStr(x, 'medicineName', 'MedicineName'));
    }
    return m;
  }, [medicines.data]);

  const kpiItems = useMemo(
    () => (kpiStock.data?.success && kpiStock.data.data ? [...kpiStock.data.data.items] : []) as Row[],
    [kpiStock.data]
  );

  const kpis = useMemo(() => {
    let total = 0;
    let low = 0;
    for (const r of kpiItems) {
      const cur = num(r, 'currentQty', 'CurrentQty');
      const avail = num(r, 'availableQty', 'AvailableQty');
      const reorder = num(r, 'reorderLevelQty', 'ReorderLevelQty');
      total += cur;
      const threshold = reorder > 0 ? reorder : 10;
      if (avail < threshold) low += 1;
    }
    return { total, low, lines: kpiItems.length };
  }, [kpiItems]);

  const byMedicine = useMemo(() => {
    const map = new Map<number, MedAgg>();
    for (const r of kpiItems) {
      const batchId = num(r, 'medicineBatchId', 'MedicineBatchId');
      const batch = batchById.get(batchId);
      if (!batch) continue;
      const medId = Number(batch.medicineId ?? batch.MedicineId);
      if (!Number.isFinite(medId)) continue;
      const name = medMap.get(medId) ?? 'Medicine';
      const cur = num(r, 'currentQty', 'CurrentQty');
      const curAgg = map.get(medId) ?? { medicineName: name, totalQty: 0 };
      curAgg.totalQty += cur;
      map.set(medId, curAgg);
    }
    return [...map.values()].sort((a, b) => b.totalQty - a.totalQty);
  }, [kpiItems, batchById, medMap]);

  const rows = useMemo(
    () => (stock.data?.success && stock.data.data ? [...stock.data.data.items] : []) as Row[],
    [stock.data]
  );
  const total = stock.data?.success && stock.data.data ? stock.data.data.totalCount : 0;

  return (
    <Stack spacing={2}>
      <PageHeader title="Inventory" subtitle="Batch-level stock with medicine rollups and low-stock highlights." />

      <Stack direction={{ xs: 'column', md: 'row' }} spacing={2} useFlexGap>
        <Card variant="outlined" sx={{ borderRadius: 2, flex: '1 1 200px' }}>
          <CardContent>
            <Typography variant="overline" color="text.secondary">
              Total on-hand (sample)
            </Typography>
            <Typography variant="h4">{kpis.total.toFixed(0)}</Typography>
            <Typography variant="body2" color="text.secondary">
              Sum of current quantity across the latest inventory sample (up to 2000 batch lines).
            </Typography>
          </CardContent>
        </Card>
        <Card variant="outlined" sx={{ borderRadius: 2, flex: '1 1 200px' }}>
          <CardContent>
            <Typography variant="overline" color="text.secondary">
              Low stock lines
            </Typography>
            <Typography variant="h4">{kpis.low}</Typography>
            <Typography variant="body2" color="text.secondary">
              Available below reorder level (or below 10 when reorder is unset), within the same sample.
            </Typography>
          </CardContent>
        </Card>
        <Card variant="outlined" sx={{ borderRadius: 2, flex: '1 1 200px' }}>
          <CardContent>
            <Typography variant="overline" color="text.secondary">
              Batch lines tracked
            </Typography>
            <Typography variant="h4">{kpis.lines}</Typography>
            <Typography variant="body2" color="text.secondary">
              Rows included in the sample window used for KPIs.
            </Typography>
          </CardContent>
        </Card>
      </Stack>

      <FormSection title="Search">
        <TextField size="small" label="Search" value={search} onChange={(e) => setSearch(e.target.value)} sx={{ maxWidth: 400 }} />
      </FormSection>

      <FormSection title="Stock by medicine (sample)">
        <DataTable<MedAgg>
          tableAriaLabel="Medicine stock totals"
          columns={[
            { id: 'medicineName', label: 'Medicine' },
            { id: 'totalQty', label: 'On-hand (sample)', align: 'right', format: (r) => r.totalQty.toFixed(2) },
          ]}
          rows={byMedicine.slice(0, 50)}
          rowKey={(r) => r.medicineName}
          loading={kpiStock.isLoading || batches.isLoading}
          emptyTitle="No stock rows in sample"
        />
      </FormSection>

      <FormSection title="Batch stock">
        <DataTable<Row>
          tableAriaLabel="Batch stock"
          columns={[
            {
              id: 'medicine',
              label: 'Medicine',
              format: (r) => {
                const bid = num(r, 'medicineBatchId', 'MedicineBatchId');
                const b = batchById.get(bid);
                const mid = b ? Number(b.medicineId ?? b.MedicineId) : NaN;
                return Number.isFinite(mid) ? medMap.get(mid) ?? '—' : '—';
              },
            },
            {
              id: 'batch',
              label: 'Batch',
              format: (r) => {
                const bid = num(r, 'medicineBatchId', 'MedicineBatchId');
                const b = batchById.get(bid);
                return b ? pickStr(b, 'batchNo', 'BatchNo') : '—';
              },
            },
            {
              id: 'expiry',
              label: 'Expiry',
              format: (r) => {
                const bid = num(r, 'medicineBatchId', 'MedicineBatchId');
                const b = batchById.get(bid);
                return b?.expiryDate != null ? String(b.expiryDate).slice(0, 10) : '—';
              },
            },
            { id: 'currentQty', label: 'Current qty', align: 'right', format: (r) => String(num(r, 'currentQty', 'CurrentQty')) },
            { id: 'availableQty', label: 'Available', align: 'right', format: (r) => String(num(r, 'availableQty', 'AvailableQty')) },
            {
              id: 'reorderLevelQty',
              label: 'Reorder level',
              align: 'right',
              format: (r) => {
                const v = r.reorderLevelQty ?? r.ReorderLevelQty;
                return v != null && String(v).trim() !== '' ? String(v) : '—';
              },
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
          loading={stock.isLoading}
          emptyTitle="No batch stock"
        />
      </FormSection>
    </Stack>
  );
}
