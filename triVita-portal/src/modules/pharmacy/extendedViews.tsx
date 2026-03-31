import { PagedResourceView } from '@/components/resource/PagedResourceView';
import { usePagedList } from '@/hooks/usePagedList';
import { getGoodsReceiptPaged, getMedicineBatchPaged, getStockLedgerPaged } from '@/services/pharmacyService';

export function PharmacyBatchesView() {
  const list = usePagedList<Record<string, unknown>>(['pharmacy', 'medicine-batch'], (page, pageSize) =>
    getMedicineBatchPaged({ page, pageSize })
  );
  return (
    <PagedResourceView
      title="Medicine batches"
      subtitle="GET /api/v1/medicine-batch"
      tableLabel="Medicine batches"
      list={list}
      columns={[
        { id: 'id', label: 'ID' },
        { id: 'medicineId', label: 'Medicine' },
        { id: 'batchNumber', label: 'Batch #' },
        { id: 'expiryDate', label: 'Expiry' },
      ]}
    />
  );
}

export function PharmacyGoodsReceiptView() {
  const list = usePagedList<Record<string, unknown>>(['pharmacy', 'goods-receipt'], (page, pageSize) =>
    getGoodsReceiptPaged({ page, pageSize })
  );
  return (
    <PagedResourceView
      title="Goods receipt (procurement)"
      subtitle="GET /api/v1/goods-receipt — inbound stock from suppliers."
      tableLabel="Goods receipts"
      list={list}
      columns={[
        { id: 'id', label: 'ID' },
        { id: 'grnNumber', label: 'GRN #' },
        { id: 'supplierId', label: 'Supplier' },
        { id: 'receivedOn', label: 'Received' },
      ]}
    />
  );
}

export function PharmacyStockLedgerView() {
  const list = usePagedList<Record<string, unknown>>(['pharmacy', 'stock-ledger'], (page, pageSize) =>
    getStockLedgerPaged({ page, pageSize })
  );
  return (
    <PagedResourceView
      title="Stock ledger"
      subtitle="GET /api/v1/stock-ledger — inventory movements."
      tableLabel="Stock ledger"
      list={list}
      columns={[
        { id: 'id', label: 'ID' },
        { id: 'batchStockId', label: 'Batch stock' },
        { id: 'quantityDelta', label: 'Δ Qty' },
        { id: 'transactionType', label: 'Type' },
      ]}
    />
  );
}
