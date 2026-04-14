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

/* —— Enterprise hierarchy CRUD (SharedService) —— */

export async function listEnterprises() {
  const { data } = await sharedClient.get<BaseResponse<readonly Record<string, unknown>[]>>('/api/v1/enterprises');
  return data;
}

export async function getEnterprise(id: number) {
  const { data } = await sharedClient.get<BaseResponse<Record<string, unknown>>>(`/api/v1/enterprises/${id}`);
  return data;
}

export async function createEnterprise(body: Record<string, unknown>) {
  const { data } = await sharedClient.post<BaseResponse<Record<string, unknown>>>('/api/v1/enterprises', body);
  return data;
}

export async function updateEnterprise(id: number, body: Record<string, unknown>) {
  const { data } = await sharedClient.put<BaseResponse<Record<string, unknown>>>(
    `/api/v1/enterprises/${id}`,
    body
  );
  return data;
}

export async function deleteEnterprise(id: number) {
  const { data } = await sharedClient.delete<BaseResponse<unknown>>(`/api/v1/enterprises/${id}`);
  return data;
}

export async function listCompanies(enterpriseId?: number) {
  const { data } = await sharedClient.get<BaseResponse<readonly Record<string, unknown>[]>>(
    '/api/v1/companies',
    { params: enterpriseId != null ? { enterpriseId } : undefined }
  );
  return data;
}

export async function getCompany(id: number) {
  const { data } = await sharedClient.get<BaseResponse<Record<string, unknown>>>(`/api/v1/companies/${id}`);
  return data;
}

export async function createCompany(body: Record<string, unknown>) {
  const { data } = await sharedClient.post<BaseResponse<Record<string, unknown>>>('/api/v1/companies', body);
  return data;
}

export async function updateCompany(id: number, body: Record<string, unknown>) {
  const { data } = await sharedClient.put<BaseResponse<Record<string, unknown>>>(
    `/api/v1/companies/${id}`,
    body
  );
  return data;
}

export async function deleteCompany(id: number) {
  const { data } = await sharedClient.delete<BaseResponse<unknown>>(`/api/v1/companies/${id}`);
  return data;
}

export async function listBusinessUnits(companyId?: number) {
  const { data } = await sharedClient.get<BaseResponse<readonly Record<string, unknown>[]>>(
    '/api/v1/business-units',
    { params: companyId != null ? { companyId } : undefined }
  );
  return data;
}

export async function getBusinessUnit(id: number) {
  const { data } = await sharedClient.get<BaseResponse<Record<string, unknown>>>(
    `/api/v1/business-units/${id}`
  );
  return data;
}

export async function createBusinessUnit(body: Record<string, unknown>) {
  const { data } = await sharedClient.post<BaseResponse<Record<string, unknown>>>(
    '/api/v1/business-units',
    body
  );
  return data;
}

export async function updateBusinessUnit(id: number, body: Record<string, unknown>) {
  const { data } = await sharedClient.put<BaseResponse<Record<string, unknown>>>(
    `/api/v1/business-units/${id}`,
    body
  );
  return data;
}

export async function deleteBusinessUnit(id: number) {
  const { data } = await sharedClient.delete<BaseResponse<unknown>>(`/api/v1/business-units/${id}`);
  return data;
}

export async function listDepartments(facilityId?: number) {
  const { data } = await sharedClient.get<BaseResponse<readonly Record<string, unknown>[]>>(
    '/api/v1/departments',
    { params: facilityId != null ? { facilityId } : undefined }
  );
  return data;
}

export async function getDepartment(id: number) {
  const { data } = await sharedClient.get<BaseResponse<Record<string, unknown>>>(`/api/v1/departments/${id}`);
  return data;
}

export async function createDepartment(body: Record<string, unknown>) {
  const { data } = await sharedClient.post<BaseResponse<Record<string, unknown>>>('/api/v1/departments', body);
  return data;
}

export async function updateDepartment(id: number, body: Record<string, unknown>) {
  const { data } = await sharedClient.put<BaseResponse<Record<string, unknown>>>(
    `/api/v1/departments/${id}`,
    body
  );
  return data;
}

export async function deleteDepartment(id: number) {
  const { data } = await sharedClient.delete<BaseResponse<unknown>>(`/api/v1/departments/${id}`);
  return data;
}

export async function createFacility(body: Record<string, unknown>) {
  const { data } = await sharedClient.post<BaseResponse<Record<string, unknown>>>('/api/v1/facilities', body);
  return data;
}

export async function updateFacility(id: number, body: Record<string, unknown>) {
  const { data } = await sharedClient.put<BaseResponse<Record<string, unknown>>>(
    `/api/v1/facilities/${id}`,
    body
  );
  return data;
}

export async function deleteFacility(id: number) {
  const { data } = await sharedClient.delete<BaseResponse<unknown>>(`/api/v1/facilities/${id}`);
  return data;
}
