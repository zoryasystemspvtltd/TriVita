import type { BaseResponse, PagedQueryParams, PagedResponse } from '@/types/api';
import { lisClient } from './http/clients';

export async function analyzerQueryTest(barcode: string) {
  const { data } = await lisClient.get<BaseResponse<Record<string, unknown>>>(
    '/api/v1/analyzer/query-test',
    { params: { barcode } }
  );
  return data;
}

export async function getLabResultsPaged(params: PagedQueryParams) {
  const { data } = await lisClient.get<BaseResponse<PagedResponse<Record<string, unknown>>>>(
    '/api/v1/lab-result',
    { params }
  );
  return data;
}

export async function getResultApprovalsPaged(params: PagedQueryParams) {
  const { data } = await lisClient.get<BaseResponse<PagedResponse<Record<string, unknown>>>>(
    '/api/v1/result-approval',
    { params }
  );
  return data;
}

export async function getLabOrdersPaged(params: PagedQueryParams) {
  const { data } = await lisClient.get<BaseResponse<PagedResponse<Record<string, unknown>>>>(
    '/api/v1/lab-order',
    { params }
  );
  return data;
}

export async function getSampleTrackingPaged(params: PagedQueryParams) {
  const { data } = await lisClient.get<BaseResponse<PagedResponse<Record<string, unknown>>>>(
    '/api/v1/sample-tracking',
    { params }
  );
  return data;
}

export async function getResultHistoryPaged(params: PagedQueryParams) {
  const { data } = await lisClient.get<BaseResponse<PagedResponse<Record<string, unknown>>>>(
    '/api/v1/result-history',
    { params }
  );
  return data;
}

export async function getSampleTypePaged(params: PagedQueryParams) {
  const { data } = await lisClient.get<BaseResponse<PagedResponse<Record<string, unknown>>>>(
    '/api/v1/sample-type',
    { params }
  );
  return data;
}

export async function getSampleTypeById(id: number) {
  const { data } = await lisClient.get<BaseResponse<Record<string, unknown>>>(`/api/v1/sample-type/${id}`);
  return data;
}

export async function createSampleType(body: Record<string, unknown>) {
  const { data } = await lisClient.post<BaseResponse<Record<string, unknown>>>('/api/v1/sample-type', body);
  return data;
}

export async function updateSampleType(id: number, body: Record<string, unknown>) {
  const { data } = await lisClient.put<BaseResponse<Record<string, unknown>>>(`/api/v1/sample-type/${id}`, body);
  return data;
}

export async function deleteSampleType(id: number) {
  const { data } = await lisClient.delete<BaseResponse<unknown>>(`/api/v1/sample-type/${id}`);
  return data;
}

export async function getTestCategoryPaged(params: PagedQueryParams) {
  const { data } = await lisClient.get<BaseResponse<PagedResponse<Record<string, unknown>>>>(
    '/api/v1/test-category',
    { params }
  );
  return data;
}

export async function getTestCategoryById(id: number) {
  const { data } = await lisClient.get<BaseResponse<Record<string, unknown>>>(`/api/v1/test-category/${id}`);
  return data;
}

export async function createTestCategory(body: Record<string, unknown>) {
  const { data } = await lisClient.post<BaseResponse<Record<string, unknown>>>('/api/v1/test-category', body);
  return data;
}

export async function updateTestCategory(id: number, body: Record<string, unknown>) {
  const { data } = await lisClient.put<BaseResponse<Record<string, unknown>>>(`/api/v1/test-category/${id}`, body);
  return data;
}

export async function deleteTestCategory(id: number) {
  const { data } = await lisClient.delete<BaseResponse<unknown>>(`/api/v1/test-category/${id}`);
  return data;
}
