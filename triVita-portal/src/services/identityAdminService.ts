import type { BaseResponse } from '@/types/api';
import { identityClient } from './http/clients';

export async function createUser(body: Record<string, unknown>) {
  const { data } = await identityClient.post<BaseResponse<number>>('/api/v1/identity-admin/users', body);
  return data;
}

export async function assignUserRoles(userId: number, roleIds: number[]) {
  const { data } = await identityClient.post<BaseResponse<unknown>>(
    `/api/v1/identity-admin/users/${userId}/roles`,
    { roleIds }
  );
  return data;
}

export async function assignUserFacilities(userId: number, grantFacilityIds: number[]) {
  const { data } = await identityClient.post<BaseResponse<unknown>>(
    `/api/v1/identity-admin/users/${userId}/facilities`,
    { grantFacilityIds }
  );
  return data;
}

export async function createRole(body: Record<string, unknown>) {
  const { data } = await identityClient.post<BaseResponse<number>>('/api/v1/identity-admin/roles', body);
  return data;
}

export async function assignRolePermissions(roleId: number, permissionIds: number[]) {
  const { data } = await identityClient.post<BaseResponse<unknown>>(
    `/api/v1/identity-admin/roles/${roleId}/permissions`,
    { permissionIds }
  );
  return data;
}

export async function createPermission(body: Record<string, unknown>) {
  const { data } = await identityClient.post<BaseResponse<number>>('/api/v1/identity-admin/permissions', body);
  return data;
}
