export interface BaseResponse<T> {
  success: boolean;
  message?: string | null;
  data?: T | null;
  errors?: readonly string[] | null;
}

export interface PagedResponse<T> {
  items: readonly T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface PagedQueryParams {
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortDescending?: boolean;
  search?: string;
}

export interface TokenResponseDto {
  accessToken: string;
  tokenType: string;
  expiresInSeconds: number;
  refreshToken?: string | null;
  refreshExpiresInSeconds: number;
}

export interface UserSummaryDto {
  userId: number;
  email: string;
  tenantId: number;
  facilityId?: number | null;
  role: string;
  roles: string[];
  permissions: string[];
}
