export { PharmacyBatchesDesk as PharmacyBatchesView } from '@/modules/pharmacy/business/PharmacyBatchesDesk';
export { PharmacyStockLedger as PharmacyStockLedgerView } from '@/modules/pharmacy/business/PharmacyStockLedger';
export { PharmacyProcurementHub as PharmacyPurchaseOrdersView } from '@/modules/pharmacy/business/PharmacyProcurementHub';
import { PharmacyProcurementHub } from '@/modules/pharmacy/business/PharmacyProcurementHub';

export function PharmacyGoodsReceiptView() {
  return <PharmacyProcurementHub initialTab={1} />;
}
