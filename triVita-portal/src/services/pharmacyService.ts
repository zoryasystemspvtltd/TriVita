import type { BaseResponse, PagedQueryParams, PagedResponse } from '@/types/api';
import { pharmacyClient } from './http/clients';

export async function getMedicinePaged(params: PagedQueryParams) {
  const { data } = await pharmacyClient.get<BaseResponse<PagedResponse<Record<string, unknown>>>>(
    '/api/v1/medicine',
    { params }
  );
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
