import type { BaseResponse, PagedQueryParams, PagedResponse } from '@/types/api';
import { pharmacyClient } from './http/clients';

export async function getMedicinePaged(params: PagedQueryParams) {
  const { data } = await pharmacyClient.get<BaseResponse<PagedResponse<Record<string, unknown>>>>(
    '/api/v1/medicine',
    { params }
  );
  return data;
}

export async function getMedicineCategoryPaged(params: PagedQueryParams) {
  const { data } = await pharmacyClient.get<BaseResponse<PagedResponse<Record<string, unknown>>>>(
    '/api/v1/medicine-category',
    { params }
  );
  return data;
}

export async function getMedicineCategoryById(id: number) {
  const { data } = await pharmacyClient.get<BaseResponse<Record<string, unknown>>>(`/api/v1/medicine-category/${id}`);
  return data;
}

export async function createMedicineCategory(body: Record<string, unknown>) {
  const { data } = await pharmacyClient.post<BaseResponse<Record<string, unknown>>>('/api/v1/medicine-category', body);
  return data;
}

export async function updateMedicineCategory(id: number, body: Record<string, unknown>) {
  const { data } = await pharmacyClient.put<BaseResponse<Record<string, unknown>>>(
    `/api/v1/medicine-category/${id}`,
    body
  );
  return data;
}

export async function deleteMedicineCategory(id: number) {
  const { data } = await pharmacyClient.delete<BaseResponse<unknown>>(`/api/v1/medicine-category/${id}`);
  return data;
}

export async function getManufacturerPaged(params: PagedQueryParams) {
  const { data } = await pharmacyClient.get<BaseResponse<PagedResponse<Record<string, unknown>>>>(
    '/api/v1/manufacturer',
    { params }
  );
  return data;
}

export async function getManufacturerById(id: number) {
  const { data } = await pharmacyClient.get<BaseResponse<Record<string, unknown>>>(`/api/v1/manufacturer/${id}`);
  return data;
}

export async function createManufacturer(body: Record<string, unknown>) {
  const { data } = await pharmacyClient.post<BaseResponse<Record<string, unknown>>>('/api/v1/manufacturer', body);
  return data;
}

export async function updateManufacturer(id: number, body: Record<string, unknown>) {
  const { data } = await pharmacyClient.put<BaseResponse<Record<string, unknown>>>(
    `/api/v1/manufacturer/${id}`,
    body
  );
  return data;
}

export async function deleteManufacturer(id: number) {
  const { data } = await pharmacyClient.delete<BaseResponse<unknown>>(`/api/v1/manufacturer/${id}`);
  return data;
}

export async function getCompositionPaged(params: PagedQueryParams) {
  const { data } = await pharmacyClient.get<BaseResponse<PagedResponse<Record<string, unknown>>>>(
    '/api/v1/composition',
    { params }
  );
  return data;
}

export async function getCompositionById(id: number) {
  const { data } = await pharmacyClient.get<BaseResponse<Record<string, unknown>>>(`/api/v1/composition/${id}`);
  return data;
}

export async function createComposition(body: Record<string, unknown>) {
  const { data } = await pharmacyClient.post<BaseResponse<Record<string, unknown>>>('/api/v1/composition', body);
  return data;
}

export async function updateComposition(id: number, body: Record<string, unknown>) {
  const { data } = await pharmacyClient.put<BaseResponse<Record<string, unknown>>>(
    `/api/v1/composition/${id}`,
    body
  );
  return data;
}

export async function deleteComposition(id: number) {
  const { data } = await pharmacyClient.delete<BaseResponse<unknown>>(`/api/v1/composition/${id}`);
  return data;
}

export async function getMedicineById(id: number) {
  const { data } = await pharmacyClient.get<BaseResponse<Record<string, unknown>>>(`/api/v1/medicine/${id}`);
  return data;
}

export async function createMedicine(body: Record<string, unknown>) {
  const { data } = await pharmacyClient.post<BaseResponse<Record<string, unknown>>>('/api/v1/medicine', body);
  return data;
}

export async function updateMedicine(id: number, body: Record<string, unknown>) {
  const { data } = await pharmacyClient.put<BaseResponse<Record<string, unknown>>>(
    `/api/v1/medicine/${id}`,
    body
  );
  return data;
}

export async function deleteMedicine(id: number) {
  const { data } = await pharmacyClient.delete<BaseResponse<unknown>>(`/api/v1/medicine/${id}`);
  return data;
}

export async function getInventoryLocationsPaged(params: PagedQueryParams) {
  const { data } = await pharmacyClient.get<BaseResponse<PagedResponse<Record<string, unknown>>>>(
    '/api/v1/inventory-locations',
    { params }
  );
  return data;
}

export async function getPurchaseOrdersPaged(params: PagedQueryParams) {
  const { data } = await pharmacyClient.get<BaseResponse<PagedResponse<Record<string, unknown>>>>(
    '/api/v1/purchase-order',
    { params }
  );
  return data;
}

export async function getPharmacyInfo() {
  const { data } = await pharmacyClient.get<BaseResponse<Record<string, unknown>>>('/api/v1/info');
  return data;
}

export async function getPharmacySalesPaged(params: PagedQueryParams) {
  const { data } = await pharmacyClient.get<BaseResponse<PagedResponse<Record<string, unknown>>>>(
    '/api/v1/pharmacy-sale',
    { params }
  );
  return data;
}

export async function getMedicineBatchPaged(params: PagedQueryParams) {
  const { data } = await pharmacyClient.get<BaseResponse<PagedResponse<Record<string, unknown>>>>(
    '/api/v1/medicine-batch',
    { params }
  );
  return data;
}

export async function getGoodsReceiptPaged(params: PagedQueryParams) {
  const { data } = await pharmacyClient.get<BaseResponse<PagedResponse<Record<string, unknown>>>>(
    '/api/v1/goods-receipt',
    { params }
  );
  return data;
}

export async function getStockLedgerPaged(params: PagedQueryParams) {
  const { data } = await pharmacyClient.get<BaseResponse<PagedResponse<Record<string, unknown>>>>(
    '/api/v1/stock-ledger',
    { params }
  );
  return data;
}

export async function getStockLedgerById(id: number) {
  const { data } = await pharmacyClient.get<BaseResponse<Record<string, unknown>>>(`/api/v1/stock-ledger/${id}`);
  return data;
}

export async function createStockLedger(body: Record<string, unknown>) {
  const { data } = await pharmacyClient.post<BaseResponse<Record<string, unknown>>>('/api/v1/stock-ledger', body);
  return data;
}

export async function updateStockLedger(id: number, body: Record<string, unknown>) {
  const { data } = await pharmacyClient.put<BaseResponse<Record<string, unknown>>>(`/api/v1/stock-ledger/${id}`, body);
  return data;
}

export async function deleteStockLedger(id: number) {
  const { data } = await pharmacyClient.delete<BaseResponse<unknown>>(`/api/v1/stock-ledger/${id}`);
  return data;
}

export async function getBatchStockPaged(params: PagedQueryParams) {
  const { data } = await pharmacyClient.get<BaseResponse<PagedResponse<Record<string, unknown>>>>(
    '/api/v1/batch-stock',
    { params }
  );
  return data;
}

export async function getBatchStockById(id: number) {
  const { data } = await pharmacyClient.get<BaseResponse<Record<string, unknown>>>(`/api/v1/batch-stock/${id}`);
  return data;
}

export async function getPurchaseOrderById(id: number) {
  const { data } = await pharmacyClient.get<BaseResponse<Record<string, unknown>>>(`/api/v1/purchase-order/${id}`);
  return data;
}

export async function createPurchaseOrder(body: Record<string, unknown>) {
  const { data } = await pharmacyClient.post<BaseResponse<Record<string, unknown>>>('/api/v1/purchase-order', body);
  return data;
}

export async function updatePurchaseOrder(id: number, body: Record<string, unknown>) {
  const { data } = await pharmacyClient.put<BaseResponse<Record<string, unknown>>>(
    `/api/v1/purchase-order/${id}`,
    body
  );
  return data;
}

export async function deletePurchaseOrder(id: number) {
  const { data } = await pharmacyClient.delete<BaseResponse<unknown>>(`/api/v1/purchase-order/${id}`);
  return data;
}

export async function getPurchaseOrderItemsPaged(params: PagedQueryParams) {
  const { data } = await pharmacyClient.get<BaseResponse<PagedResponse<Record<string, unknown>>>>(
    '/api/v1/purchase-order-item',
    { params }
  );
  return data;
}

export async function getPurchaseOrderItemById(id: number) {
  const { data } = await pharmacyClient.get<BaseResponse<Record<string, unknown>>>(
    `/api/v1/purchase-order-item/${id}`
  );
  return data;
}

export async function createPurchaseOrderItem(body: Record<string, unknown>) {
  const { data } = await pharmacyClient.post<BaseResponse<Record<string, unknown>>>(
    '/api/v1/purchase-order-item',
    body
  );
  return data;
}

export async function updatePurchaseOrderItem(id: number, body: Record<string, unknown>) {
  const { data } = await pharmacyClient.put<BaseResponse<Record<string, unknown>>>(
    `/api/v1/purchase-order-item/${id}`,
    body
  );
  return data;
}

export async function deletePurchaseOrderItem(id: number) {
  const { data } = await pharmacyClient.delete<BaseResponse<unknown>>(`/api/v1/purchase-order-item/${id}`);
  return data;
}

export async function getGoodsReceiptById(id: number) {
  const { data } = await pharmacyClient.get<BaseResponse<Record<string, unknown>>>(`/api/v1/goods-receipt/${id}`);
  return data;
}

export async function createGoodsReceipt(body: Record<string, unknown>) {
  const { data } = await pharmacyClient.post<BaseResponse<Record<string, unknown>>>('/api/v1/goods-receipt', body);
  return data;
}

export async function updateGoodsReceipt(id: number, body: Record<string, unknown>) {
  const { data } = await pharmacyClient.put<BaseResponse<Record<string, unknown>>>(
    `/api/v1/goods-receipt/${id}`,
    body
  );
  return data;
}

export async function deleteGoodsReceipt(id: number) {
  const { data } = await pharmacyClient.delete<BaseResponse<unknown>>(`/api/v1/goods-receipt/${id}`);
  return data;
}

export async function getGoodsReceiptItemsPaged(params: PagedQueryParams) {
  const { data } = await pharmacyClient.get<BaseResponse<PagedResponse<Record<string, unknown>>>>(
    '/api/v1/goods-receipt-item',
    { params }
  );
  return data;
}

export async function getGoodsReceiptItemById(id: number) {
  const { data } = await pharmacyClient.get<BaseResponse<Record<string, unknown>>>(
    `/api/v1/goods-receipt-item/${id}`
  );
  return data;
}

export async function createGoodsReceiptItem(body: Record<string, unknown>) {
  const { data } = await pharmacyClient.post<BaseResponse<Record<string, unknown>>>(
    '/api/v1/goods-receipt-item',
    body
  );
  return data;
}

export async function updateGoodsReceiptItem(id: number, body: Record<string, unknown>) {
  const { data } = await pharmacyClient.put<BaseResponse<Record<string, unknown>>>(
    `/api/v1/goods-receipt-item/${id}`,
    body
  );
  return data;
}

export async function deleteGoodsReceiptItem(id: number) {
  const { data } = await pharmacyClient.delete<BaseResponse<unknown>>(`/api/v1/goods-receipt-item/${id}`);
  return data;
}

export async function getPharmacySaleById(id: number) {
  const { data } = await pharmacyClient.get<BaseResponse<Record<string, unknown>>>(`/api/v1/pharmacy-sale/${id}`);
  return data;
}

export async function createPharmacySale(body: Record<string, unknown>) {
  const { data } = await pharmacyClient.post<BaseResponse<Record<string, unknown>>>('/api/v1/pharmacy-sale', body);
  return data;
}

export async function updatePharmacySale(id: number, body: Record<string, unknown>) {
  const { data } = await pharmacyClient.put<BaseResponse<Record<string, unknown>>>(
    `/api/v1/pharmacy-sale/${id}`,
    body
  );
  return data;
}

export async function deletePharmacySale(id: number) {
  const { data } = await pharmacyClient.delete<BaseResponse<unknown>>(`/api/v1/pharmacy-sale/${id}`);
  return data;
}

export async function getPharmacySalesItemsPaged(params: PagedQueryParams) {
  const { data } = await pharmacyClient.get<BaseResponse<PagedResponse<Record<string, unknown>>>>(
    '/api/v1/pharmacy-sales-item',
    { params }
  );
  return data;
}

export async function getPharmacySalesItemById(id: number) {
  const { data } = await pharmacyClient.get<BaseResponse<Record<string, unknown>>>(
    `/api/v1/pharmacy-sales-item/${id}`
  );
  return data;
}

export async function createPharmacySalesItem(body: Record<string, unknown>) {
  const { data } = await pharmacyClient.post<BaseResponse<Record<string, unknown>>>(
    '/api/v1/pharmacy-sales-item',
    body
  );
  return data;
}

export async function updatePharmacySalesItem(id: number, body: Record<string, unknown>) {
  const { data } = await pharmacyClient.put<BaseResponse<Record<string, unknown>>>(
    `/api/v1/pharmacy-sales-item/${id}`,
    body
  );
  return data;
}

export async function deletePharmacySalesItem(id: number) {
  const { data } = await pharmacyClient.delete<BaseResponse<unknown>>(`/api/v1/pharmacy-sales-item/${id}`);
  return data;
}

export async function getMedicineBatchById(id: number) {
  const { data } = await pharmacyClient.get<BaseResponse<Record<string, unknown>>>(`/api/v1/medicine-batch/${id}`);
  return data;
}

export async function createMedicineBatch(body: Record<string, unknown>) {
  const { data } = await pharmacyClient.post<BaseResponse<Record<string, unknown>>>('/api/v1/medicine-batch', body);
  return data;
}

export async function updateMedicineBatch(id: number, body: Record<string, unknown>) {
  const { data } = await pharmacyClient.put<BaseResponse<Record<string, unknown>>>(
    `/api/v1/medicine-batch/${id}`,
    body
  );
  return data;
}

export async function deleteMedicineBatch(id: number) {
  const { data } = await pharmacyClient.delete<BaseResponse<unknown>>(`/api/v1/medicine-batch/${id}`);
  return data;
}
