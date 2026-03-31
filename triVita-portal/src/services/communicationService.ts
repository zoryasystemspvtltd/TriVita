import type { BaseResponse, PagedQueryParams, PagedResponse } from '@/types/api';
import { communicationClient } from './http/clients';

export async function getNotificationTemplatesPaged(params: PagedQueryParams) {
  const { data } = await communicationClient.get<BaseResponse<PagedResponse<Record<string, unknown>>>>(
    '/api/v1/notifications/templates',
    { params }
  );
  return data;
}

export async function getNotificationLogsPaged(params: PagedQueryParams, notificationChannelId?: number) {
  const { data } = await communicationClient.get<BaseResponse<PagedResponse<Record<string, unknown>>>>(
    '/api/v1/notifications/logs',
    { params: { ...params, notificationChannelId } }
  );
  return data;
}

export async function createNotification(body: Record<string, unknown>) {
  const { data } = await communicationClient.post<BaseResponse<Record<string, unknown>>>(
    '/api/v1/notifications',
    body
  );
  return data;
}
