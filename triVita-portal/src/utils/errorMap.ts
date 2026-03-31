import axios from 'axios';
import type { BaseResponse } from '@/types/api';

export function getApiErrorMessage(err: unknown, fallback = 'Something went wrong.'): string {
  if (axios.isAxiosError(err)) {
    const data = err.response?.data as BaseResponse<unknown> | undefined;
    if (data?.message) return data.message;
    if (data?.errors?.length) return data.errors.join(' ');
    if (err.response?.status === 401) return 'Session expired or not authorized.';
    if (err.response?.status === 403) return 'You do not have permission for this action.';
    if (err.response?.status === 404) return 'Resource not found.';
    if (!err.response) return 'Network error. Check your connection and API availability.';
  }
  if (err instanceof Error) return err.message;
  return fallback;
}
