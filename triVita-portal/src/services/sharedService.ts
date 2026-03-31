import type { BaseResponse } from '@/types/api';
import { sharedClient } from './http/clients';

export async function getEnterpriseHierarchy(enterpriseId: number) {
  const { data } = await sharedClient.get<BaseResponse<Record<string, unknown>>>(
    `/api/v1/enterprise-hierarchy/${enterpriseId}`
  );
  return data;
}

export async function listFacilities(businessUnitId?: number) {
  const { data } = await sharedClient.get<BaseResponse<readonly Record<string, unknown>[]>>(
    '/api/v1/facilities',
    { params: { businessUnitId } }
  );
  return data;
}

export async function getFacilityById(id: number) {
  const { data } = await sharedClient.get<BaseResponse<Record<string, unknown>>>(`/api/v1/facilities/${id}`);
  return data;
}
