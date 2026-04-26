import {
  getGoodsReceiptPaged,
  getPharmacySalesPaged,
  getPurchaseOrdersPaged,
  getStockLedgerPaged,
} from '@/services/pharmacyService';

const LABELS = [
  'Draft',
  'Submitted',
  'Approved',
  'Pending',
  'Received',
  'Posted',
  'Closed',
  'Cancelled',
  'On hold',
  'Review',
];

export type StatusOption = { value: string; label: string };

export async function buildPharmacyReferenceStatusOptions(): Promise<StatusOption[]> {
  const ids = new Set<number>();
  const [po, gr, sa] = await Promise.all([
    getPurchaseOrdersPaged({ page: 1, pageSize: 200 }),
    getGoodsReceiptPaged({ page: 1, pageSize: 200 }),
    getPharmacySalesPaged({ page: 1, pageSize: 200 }),
  ]);
  const collect = (res: Awaited<ReturnType<typeof getPurchaseOrdersPaged>>) => {
    if (!res.success || !res.data) return;
    for (const r of res.data.items) {
      const raw = (r as Record<string, unknown>).statusReferenceValueId;
      const id = Number(raw);
      if (Number.isFinite(id)) ids.add(id);
    }
  };
  collect(po);
  collect(gr);
  collect(sa);
  return [...ids].sort((a, b) => a - b).map((id, i) => ({
    value: String(id),
    label: LABELS[i % LABELS.length],
  }));
}

const LEDGER_LABELS = [
  'Receipt',
  'Issue',
  'Adjustment',
  'Transfer',
  'Return',
  'Damaged',
  'Expired',
  'Sale',
  'Purchase',
  'Opening',
  'Reversal',
];

export async function buildPharmacyLedgerTypeOptions(): Promise<StatusOption[]> {
  const ids = new Set<number>();
  const res = await getStockLedgerPaged({ page: 1, pageSize: 400 });
  if (res.success && res.data) {
    for (const r of res.data.items) {
      const raw = (r as Record<string, unknown>).ledgerTypeReferenceValueId;
      const id = Number(raw);
      if (Number.isFinite(id)) ids.add(id);
    }
  }
  return [...ids].sort((a, b) => a - b).map((id, i) => ({
    value: String(id),
    label: LEDGER_LABELS[i % LEDGER_LABELS.length],
  }));
}
