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

export async function getEquipmentFacilityMappingsPaged(params: PagedQueryParams) {
  const { data } = await lmsClient.get<BaseResponse<PagedResponse<Record<string, unknown>>>>(
    '/api/v1/workflow/equipment-facility-mappings',
    { params }
  );
  return data;
}

export async function getWorkQueuePaged(params: PagedQueryParams) {
  const { data } = await lmsClient.get<BaseResponse<PagedResponse<Record<string, unknown>>>>(
    '/api/v1/work-queue',
    { params }
  );
  return data;
}

export async function getProcessingStagePaged(params: PagedQueryParams) {
  const { data } = await lmsClient.get<BaseResponse<PagedResponse<Record<string, unknown>>>>(
    '/api/v1/processing-stage',
    { params }
  );
  return data;
}

export async function getProcessingStageById(id: number) {
  const { data } = await lmsClient.get<BaseResponse<Record<string, unknown>>>(`/api/v1/processing-stage/${id}`);
  return data;
}

export async function createProcessingStage(body: Record<string, unknown>) {
  const { data } = await lmsClient.post<BaseResponse<Record<string, unknown>>>('/api/v1/processing-stage', body);
  return data;
}

export async function updateProcessingStage(id: number, body: Record<string, unknown>) {
  const { data } = await lmsClient.put<BaseResponse<Record<string, unknown>>>(
    `/api/v1/processing-stage/${id}`,
    body
  );
  return data;
}

export async function deleteProcessingStage(id: number) {
  const { data } = await lmsClient.delete<BaseResponse<unknown>>(`/api/v1/processing-stage/${id}`);
  return data;
}

export async function getEquipmentTypeById(id: number) {
  const { data } = await lmsClient.get<BaseResponse<Record<string, unknown>>>(
    `/api/v1/workflow/equipment-types/${id}`
  );
  return data;
}

export async function createEquipmentType(body: Record<string, unknown>) {
  const { data } = await lmsClient.post<BaseResponse<Record<string, unknown>>>(
    '/api/v1/workflow/equipment-types',
    body
  );
  return data;
}

export async function updateEquipmentType(id: number, body: Record<string, unknown>) {
  const { data } = await lmsClient.put<BaseResponse<Record<string, unknown>>>(
    `/api/v1/workflow/equipment-types/${id}`,
    body
  );
  return data;
}

export async function deleteEquipmentType(id: number) {
  const { data } = await lmsClient.delete<BaseResponse<unknown>>(`/api/v1/workflow/equipment-types/${id}`);
  return data;
}
