import type { BaseResponse, PagedQueryParams, PagedResponse } from '@/types/api';
import { hmsClient } from './http/clients';

export interface PatientMasterRow {
  id: number;
  upid: string;
  fullName: string;
  dateOfBirth?: string | null;
  primaryPhone?: string | null;
  primaryEmail?: string | null;
}

export interface CreatePatientMasterPayload {
  sharedPatientId?: number | null;
  fullName: string;
  dateOfBirth?: string | null;
  genderReferenceValueId?: number | null;
  primaryPhone?: string | null;
  primaryEmail?: string | null;
}

export async function searchPatientMasters(
  params: PagedQueryParams & { search?: string; linkedFacilityId?: number }
) {
  const { data } = await hmsClient.get<BaseResponse<PagedResponse<PatientMasterRow>>>(
    '/api/v1/patient-masters',
    { params }
  );
  return data;
}

export async function createPatientMaster(body: CreatePatientMasterPayload) {
  const { data } = await hmsClient.post<BaseResponse<PatientMasterRow>>('/api/v1/patient-masters', body);
  return data;
}

export async function getAppointmentsPaged(
  params: PagedQueryParams & {
    patientId?: number;
    doctorId?: number;
    scheduledFrom?: string;
    scheduledTo?: string;
  }
) {
  const { data } = await hmsClient.get<BaseResponse<PagedResponse<Record<string, unknown>>>>(
    '/api/v1/appointments',
    { params }
  );
  return data;
}

export async function createAppointment(body: Record<string, unknown>) {
  const { data } = await hmsClient.post<BaseResponse<Record<string, unknown>>>('/api/v1/appointments', body);
  return data;
}

export async function getAppointmentQueuePaged(params: PagedQueryParams) {
  const { data } = await hmsClient.get<BaseResponse<PagedResponse<Record<string, unknown>>>>(
    '/api/v1/appointment-queue',
    { params }
  );
  return data;
}

export async function getBillingItemsPaged(params: PagedQueryParams) {
  const { data } = await hmsClient.get<BaseResponse<PagedResponse<Record<string, unknown>>>>(
    '/api/v1/billing-items',
    { params }
  );
  return data;
}
