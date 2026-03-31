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
