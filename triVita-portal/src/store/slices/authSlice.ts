import { createSlice, type PayloadAction } from '@reduxjs/toolkit';
import type { UserSummaryDto } from '@/types/api';
import { STORAGE_KEYS } from '@/utils/storageKeys';

export interface AuthState {
  user: UserSummaryDto | null;
  hydrated: boolean;
}

const initialState: AuthState = {
  user: null,
  hydrated: false,
};

function readSessionUser(): UserSummaryDto | null {
  const raw = sessionStorage.getItem('trivita_user');
  if (!raw) return null;
  try {
    return JSON.parse(raw) as UserSummaryDto;
  } catch {
    return null;
  }
}

export const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    hydrateFromStorage(state) {
      state.user = readSessionUser();
      state.hydrated = true;
    },
    setSession(
      state,
      action: PayloadAction<{
        user: UserSummaryDto;
        accessToken: string;
        refreshToken: string;
        tenantId: number;
        facilityId: number | null;
      }>
    ) {
      const { user, accessToken, refreshToken, tenantId, facilityId } = action.payload;
      state.user = user;
      sessionStorage.setItem(STORAGE_KEYS.accessToken, accessToken);
      sessionStorage.setItem(STORAGE_KEYS.refreshToken, refreshToken);
      sessionStorage.setItem(STORAGE_KEYS.tenantId, String(tenantId));
      if (facilityId != null) {
        sessionStorage.setItem(STORAGE_KEYS.facilityId, String(facilityId));
      } else {
        sessionStorage.removeItem(STORAGE_KEYS.facilityId);
      }
      sessionStorage.setItem('trivita_user', JSON.stringify(user));
    },
    setFacilityId(state, action: PayloadAction<number | null>) {
      const id = action.payload;
      if (state.user) {
        state.user = { ...state.user, facilityId: id ?? undefined };
        sessionStorage.setItem('trivita_user', JSON.stringify(state.user));
      }
      if (id != null) sessionStorage.setItem(STORAGE_KEYS.facilityId, String(id));
      else sessionStorage.removeItem(STORAGE_KEYS.facilityId);
    },
    clearSession(state) {
      state.user = null;
      sessionStorage.removeItem(STORAGE_KEYS.accessToken);
      sessionStorage.removeItem(STORAGE_KEYS.refreshToken);
      sessionStorage.removeItem(STORAGE_KEYS.tenantId);
      sessionStorage.removeItem(STORAGE_KEYS.facilityId);
      sessionStorage.removeItem('trivita_user');
    },
  },
});

export const { hydrateFromStorage, setSession, setFacilityId, clearSession } = authSlice.actions;
