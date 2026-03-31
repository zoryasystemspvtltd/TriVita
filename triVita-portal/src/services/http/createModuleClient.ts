import axios, { type AxiosError, type InternalAxiosRequestConfig } from 'axios';
import type { BaseResponse, TokenResponseDto } from '@/types/api';
import { STORAGE_KEYS } from '@/utils/storageKeys';

/**
 * Axios client for a TriVita microservice: JWT, tenant/facility headers, refresh on 401.
 */
export function createModuleClient(serviceBaseUrl: string, identityBaseUrl: string) {
  const client = axios.create({
    baseURL: serviceBaseUrl.replace(/\/$/, ''),
    timeout: 120_000,
    headers: { 'Content-Type': 'application/json' },
  });

  const attachHeaders = (config: InternalAxiosRequestConfig) => {
    const token = sessionStorage.getItem(STORAGE_KEYS.accessToken);
    const tenantId = sessionStorage.getItem(STORAGE_KEYS.tenantId);
    const facilityId = sessionStorage.getItem(STORAGE_KEYS.facilityId);
    if (token) config.headers.Authorization = `Bearer ${token}`;
    if (tenantId) config.headers['X-Tenant-Id'] = tenantId;
    if (facilityId) config.headers['X-Facility-Id'] = facilityId;
    return config;
  };

  client.interceptors.request.use(attachHeaders);

  let refreshChain: Promise<string> | null = null;

  const refreshAccess = async (): Promise<string> => {
    const rt = sessionStorage.getItem(STORAGE_KEYS.refreshToken);
    if (!rt) throw new Error('No refresh token');
    const url = `${identityBaseUrl.replace(/\/$/, '')}/api/v1/auth/refresh-token`;
    const { data } = await axios.post<BaseResponse<TokenResponseDto>>(url, { refreshToken: rt });
    if (!data.success || !data.data?.accessToken) {
      throw new Error(data.message ?? 'Refresh failed');
    }
    sessionStorage.setItem(STORAGE_KEYS.accessToken, data.data.accessToken);
    if (data.data.refreshToken) {
      sessionStorage.setItem(STORAGE_KEYS.refreshToken, data.data.refreshToken);
    }
    return data.data.accessToken;
  };

  const getRefreshedToken = () => {
    if (!refreshChain) {
      refreshChain = refreshAccess().finally(() => {
        refreshChain = null;
      });
    }
    return refreshChain;
  };

  client.interceptors.response.use(
    (r) => r,
    async (error: AxiosError) => {
      const original = error.config as InternalAxiosRequestConfig & { _retry?: boolean };
      if (!original || original._retry) return Promise.reject(error);
      if (error.response?.status !== 401) return Promise.reject(error);
      if (original.url?.includes('/auth/refresh-token')) return Promise.reject(error);

      original._retry = true;
      try {
        const token = await getRefreshedToken();
        original.headers.Authorization = `Bearer ${token}`;
        return client(original);
      } catch (e) {
        sessionStorage.removeItem(STORAGE_KEYS.accessToken);
        sessionStorage.removeItem(STORAGE_KEYS.refreshToken);
        if (typeof window !== 'undefined' && !window.location.pathname.startsWith('/login')) {
          window.location.assign('/login');
        }
        return Promise.reject(e);
      }
    }
  );

  return client;
}

export const envBase = {
  identity: import.meta.env.VITE_IDENTITY_BASE,
  hms: import.meta.env.VITE_HMS_BASE,
  lis: import.meta.env.VITE_LIS_BASE,
  lms: import.meta.env.VITE_LMS_BASE,
  pharmacy: import.meta.env.VITE_PHARMACY_BASE,
  shared: import.meta.env.VITE_SHARED_BASE,
  communication: import.meta.env.VITE_COMMUNICATION_BASE,
};
