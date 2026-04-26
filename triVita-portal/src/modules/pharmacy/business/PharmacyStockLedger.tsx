import { Box, Link, Stack, TextField } from '@mui/material';
import { useQuery } from '@tanstack/react-query';
import { useEffect, useMemo, useState } from 'react';
import { DataTable } from '@/components/common/DataTable';
import { DetailKv } from '@/components/masters/DetailKv';
import { DetailDrawer } from '@/components/layout/DetailDrawer';
import { PageHeader } from '@/components/layout/PageHeader';
import { FormSection } from '@/components/forms/FormSection';
import { getMedicineBatchById, getStockLedgerById, getStockLedgerPaged } from '@/services/pharmacyService';
import { buildPharmacyLedgerTypeOptions } from '@/utils/pharmacyStatusOptions';

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
  return NaN;
}

export function PharmacyStockLedger() {
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(25);
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

  const typeLabels = useQuery({
    queryKey: ['pharmacy', 'ledger-type-opts'],
    queryFn: buildPharmacyLedgerTypeOptions,
    staleTime: 120_000,
  });

  const tm = useMemo(() => {
    const m = new Map<string, string>();
    for (const o of typeLabels.data ?? []) m.set(o.value, o.label);
    return m;
  }, [typeLabels.data]);

  const q = useQuery({
    queryKey: ['pharmacy', 'ledger', page, pageSize, searchApplied],
    queryFn: () => getStockLedgerPaged({ page: page + 1, pageSize, search: searchApplied || undefined }),
  });

  const rows = useMemo(
    () => (q.data?.success && q.data.data ? [...q.data.data.items] : []) as Row[],
    [q.data]
  );
  const total = q.data?.success && q.data.data ? q.data.data.totalCount : 0;

  const detail = useQuery({
    queryKey: ['pharmacy', 'ledger-detail', drawerId],
    queryFn: () => getStockLedgerById(drawerId!),
    enabled: drawerId != null,
  });

  const detailData = (detail.data?.success ? detail.data.data : null) as Row | null;

  const batchIdForDetail = detailData ? num(detailData, 'medicineBatchId', 'MedicineBatchId') : NaN;
  const batchRow = useQuery({
    queryKey: ['pharmacy', 'ledger-batch', batchIdForDetail],
    queryFn: () => getMedicineBatchById(batchIdForDetail),
    enabled: Number.isFinite(batchIdForDetail),
  });

  const batchLabel =
    batchRow.data?.success && batchRow.data.data
      ? pickStr(batchRow.data.data as Row, 'batchNo', 'BatchNo')
      : '';

  return (
    <Stack spacing={2}>
      <PageHeader title="Stock ledger" subtitle="Inventory movements with running balance per entry." />

      <FormSection title="Search">
        <TextField size="small" label="Search" value={search} onChange={(e) => setSearch(e.target.value)} sx={{ maxWidth: 400 }} />
      </FormSection>

      <FormSection title="Movement register">
        <Box
          sx={{
            '& .MuiTableRow-root:nth-of-type(even)': { bgcolor: 'action.hover' },
          }}
        >
          <DataTable<Row>
            tableAriaLabel="Stock ledger"
            columns={[
              {
                id: 'transactionOn',
                label: 'Date',
                minWidth: 120,
                format: (r) => (r.transactionOn != null ? String(r.transactionOn).slice(0, 16).replace('T', ' ') : '—'),
              },
              {
                id: 'type',
                label: 'Transaction type',
                minWidth: 140,
                format: (r) => tm.get(String(r.ledgerTypeReferenceValueId ?? r.LedgerTypeReferenceValueId ?? '')) ?? '—',
              },
              {
                id: 'qty',
                label: 'Qty in / out',
                align: 'right',
                format: (r) => {
                  const d = num(r, 'quantityDelta', 'QuantityDelta');
                  if (!Number.isFinite(d)) return '—';
                  if (d > 0) return `+${d}`;
                  return String(d);
                },
              },
              {
                id: 'balance',
                label: 'Balance after',
                align: 'right',
                format: (r) => {
                  const a = num(r, 'afterQty', 'AfterQty');
                  return Number.isFinite(a) ? String(a) : '—';
                },
              },
              {
                id: 'source',
                label: 'Reference',
                format: (r) => pickStr(r, 'sourceReference', 'SourceReference') || '—',
              },
              {
                id: '_a',
                label: 'Actions',
                minWidth: 80,
                format: (r) => {
                  const id = Number(r.id);
                  return (
                    <Link component="button" type="button" variant="body2" onClick={() => setDrawerId(id)}>
                      View
                    </Link>
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
            loading={q.isLoading}
            emptyTitle="No ledger rows"
          />
        </Box>
      </FormSection>

      <DetailDrawer
        open={drawerId != null}
        onClose={() => setDrawerId(null)}
        title={detailData && detailData.transactionOn != null ? String(detailData.transactionOn).slice(0, 16).replace('T', ' ') : 'Movement'}
        subtitle={batchLabel || undefined}
      >
        {detailData ? (
          <Stack spacing={1}>
            <DetailKv label="Batch" value={batchLabel} />
            <DetailKv
              label="Transaction type"
              value={tm.get(String(detailData.ledgerTypeReferenceValueId ?? detailData.LedgerTypeReferenceValueId ?? '')) ?? ''}
            />
            <DetailKv
              label="Quantity change"
              value={(() => {
                const d = num(detailData, 'quantityDelta', 'QuantityDelta');
                return Number.isFinite(d) ? String(d) : '';
              })()}
            />
            <DetailKv
              label="Balance after"
              value={(() => {
                const a = num(detailData, 'afterQty', 'AfterQty');
                return Number.isFinite(a) ? String(a) : '';
              })()}
            />
            <DetailKv label="Reference" value={pickStr(detailData, 'sourceReference', 'SourceReference')} />
            <DetailKv label="Notes" value={pickStr(detailData, 'notes', 'Notes')} />
          </Stack>
        ) : null}
      </DetailDrawer>
    </Stack>
  );
}
