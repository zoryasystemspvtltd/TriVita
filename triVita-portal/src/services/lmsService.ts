import type { BaseResponse, PagedQueryParams, PagedResponse } from '@/types/api';
import { lmsClient } from './http/clients';

export async function getTestMastersPaged(params: PagedQueryParams) {
  const { data } = await lmsClient.get<BaseResponse<PagedResponse<Record<string, unknown>>>>(
    '/api/v1/workflow/test-masters',
    { params }
  );
  return data;
}

export async function getEquipmentTypesPaged(params: PagedQueryParams) {
  const { data } = await lmsClient.get<BaseResponse<PagedResponse<Record<string, unknown>>>>(
    '/api/v1/workflow/equipment-types',
    { params }
  );
  return data;
}

export async function getTestBookingsPaged(params: PagedQueryParams) {
  const { data } = await lmsClient.get<BaseResponse<PagedResponse<Record<string, unknown>>>>(
    '/api/v1/workflow/test-bookings',
    { params }
  );
  return data;
}

export async function registerBarcode(body: Record<string, unknown>) {
  const { data } = await lmsClient.post<BaseResponse<Record<string, unknown>>>(
    '/api/v1/workflow/barcodes/register',
    body
  );
  return data;
}

export async function resolveBarcode(barcodeValue: string) {
  const { data } = await lmsClient.get<BaseResponse<Record<string, unknown>>>(
    `/api/v1/workflow/integration/barcode/${encodeURIComponent(barcodeValue)}`
  );
  return data;
}
