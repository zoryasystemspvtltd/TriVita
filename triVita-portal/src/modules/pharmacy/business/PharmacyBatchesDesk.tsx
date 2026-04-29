import { Link, Stack, TextField } from '@mui/material';
import { useQuery } from '@tanstack/react-query';
import { useEffect, useMemo, useState } from 'react';
import { DataTable } from '@/components/common/DataTable';
import { DetailKv } from '@/components/masters/DetailKv';
import { DetailDrawer } from '@/components/layout/DetailDrawer';
import { PageHeader } from '@/components/layout/PageHeader';
import { FormSection } from '@/components/forms/FormSection';
import {
  getMedicineBatchById,
  getMedicineBatchPaged,
  getMedicinePaged,
} from '@/services/pharmacyService';

type Row = Record<string, unknown> & { id?: number };

function pickStr(r: Record<string, unknown>, ...keys: string[]) {
  for (const k of keys) {
    const v = r[k];
    if (v != null && String(v).trim() !== '') return String(v);
  }
  return '';
}

export function PharmacyBatchesDesk() {
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const [search, setSearch] = useState('');
  const [searchApplied, setSearchApplied] = useState('');
  const [drawerId, setDrawerId] = useState<number | null>(null);

  useEffect(() => {
    const t = window.setTimeout(() => setSearchApplied(search), 400);
    return () => window.clearTimeout(t);
  }, [search]);

  useEffect(() => {
    setPage(0);
  }, [searchApplied]);

  const list = useQuery({
    queryKey: ['pharmacy', 'batches', page, pageSize, searchApplied],
    queryFn: () => getMedicineBatchPaged({ page: page + 1, pageSize, search: searchApplied || undefined }),
  });

  const medicines = useQuery({
    queryKey: ['pharmacy', 'batches-med-map'],
    queryFn: () => getMedicinePaged({ page: 1, pageSize: 500 }),
    staleTime: 120_000,
  });

  const medMap = useMemo(() => {
    const m = new Map<number, string>();
    const d = medicines.data?.success ? medicines.data.data : null;
    for (const x of (d?.items ?? []) as Row[]) {
      const id = Number(x.id);
      if (Number.isFinite(id)) m.set(id, pickStr(x, 'medicineName', 'MedicineName'));
    }
    return m;
  }, [medicines.data]);

  const rows = useMemo(
    () => (list.data?.success && list.data.data ? [...list.data.data.items] : []) as Row[],
    [list.data]
  );
  const total = list.data?.success && list.data.data ? list.data.data.totalCount : 0;

  const detail = useQuery({
    queryKey: ['pharmacy', 'batch-detail', drawerId],
    queryFn: () => getMedicineBatchById(drawerId!),
    enabled: drawerId != null,
  });

  const detailData = (detail.data?.success ? detail.data.data : null) as Row | null;

  return (
    <Stack spacing={2}>
      <PageHeader title="Medicine Batches" subtitle="View-only. Batches are created and updated from GRN only." />

      <FormSection title="Search">
        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} alignItems={{ sm: 'center' }}>
          <TextField size="small" label="Search" value={search} onChange={(e) => setSearch(e.target.value)} sx={{ flex: 1, minWidth: 220 }} />
        </Stack>
      </FormSection>

      <FormSection title="Batch register">
        <DataTable<Row>
          tableAriaLabel="Medicine Batches"
          columns={[
            {
              id: 'medicine',
              label: 'Medicine',
              format: (r) => medMap.get(Number(r.medicineId ?? r.MedicineId)) ?? '—',
            },
            { id: 'batchNo', label: 'Batch #', format: (r) => pickStr(r, 'batchNo', 'BatchNo') },
            { id: 'expiryDate', label: 'Expiry', format: (r) => (r.expiryDate != null ? String(r.expiryDate).slice(0, 10) : '—') },
            { id: 'mrp', label: 'MRP', align: 'right', format: (r) => (r.mrp != null ? String(r.mrp) : '—') },
            {
              id: '_a',
              label: 'Actions',
              minWidth: 200,
              format: (r) => {
                const id = Number(r.id);
                return (
                  <Stack direction="row" spacing={1} flexWrap="wrap" useFlexGap>
                    <Link component="button" type="button" variant="body2" onClick={() => setDrawerId(id)}>
                      View
                    </Link>
                  </Stack>
                );
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
          loading={list.isLoading}
          emptyTitle="No batches"
        />
      </FormSection>

      <DetailDrawer
        open={drawerId != null}
        onClose={() => setDrawerId(null)}
        title={detailData ? pickStr(detailData, 'batchNo', 'BatchNo') : 'Batch'}
        subtitle={detailData ? medMap.get(Number(detailData.medicineId ?? detailData.MedicineId)) : undefined}
      >
        {detailData ? (
          <Stack spacing={1}>
            <DetailKv label="Medicine" value={medMap.get(Number(detailData.medicineId ?? detailData.MedicineId)) ?? ''} />
            <DetailKv label="Batch number" value={pickStr(detailData, 'batchNo', 'BatchNo')} />
            <DetailKv label="Expiry" value={detailData.expiryDate != null ? String(detailData.expiryDate).slice(0, 10) : ''} />
            <DetailKv label="MRP" value={detailData.mrp != null ? String(detailData.mrp) : ''} />
            <DetailKv label="Purchase rate" value={detailData.purchaseRate != null ? String(detailData.purchaseRate) : ''} />
            <DetailKv label="Available qty" value={detailData.availableQuantity != null ? String(detailData.availableQuantity) : '—'} />
            <DetailKv label="Created from GRN" value={detailData.createdFromGoodsReceiptId != null ? String(detailData.createdFromGoodsReceiptId) : '—'} />
            <DetailKv
              label="Manufacturing date"
              value={detailData.manufacturingDate != null ? String(detailData.manufacturingDate).slice(0, 10) : ''}
            />
          </Stack>
        ) : null}
      </DetailDrawer>
    </Stack>
  );
}
