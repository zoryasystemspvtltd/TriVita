import type { BaseResponse, PagedQueryParams, PagedResponse } from '@/types/api';
import { pharmacyClient } from './http/clients';

export async function getMedicinePaged(params: PagedQueryParams) {
  const { data } = await pharmacyClient.get<BaseResponse<PagedResponse<Record<string, unknown>>>>(
    '/api/v1/medicine',
    { params }
  );
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
