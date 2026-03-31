import axios from 'axios';
import type { BaseResponse, TokenResponseDto, UserSummaryDto } from '@/types/api';
import { envBase } from '@/services/http/createModuleClient';

const base = envBase.identity.replace(/\/$/, '');

/** Login / token without interceptors (no Bearer yet). */
export async function postToken(email: string, password: string, tenantId: number) {
  const { data } = await axios.post<BaseResponse<TokenResponseDto>>(`${base}/api/v1/auth/token`, {
    email,
    password,
    tenantId,
  });
  return data;
}

export async function postRefresh(refreshToken: string) {
  const { data } = await axios.post<BaseResponse<TokenResponseDto>>(`${base}/api/v1/auth/refresh-token`, {
    refreshToken,
  });
  return data;
}

export async function postLogout(refreshToken: string) {
  const { data } = await axios.post<BaseResponse<unknown>>(`${base}/api/v1/auth/logout`, {
    refreshToken,
  });
  return data;
}

export async function getMe(accessToken: string) {
  const { data } = await axios.get<BaseResponse<UserSummaryDto>>(`${base}/api/v1/auth/me`, {
    headers: { Authorization: `Bearer ${accessToken}` },
  });
  return data;
}
