/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_IDENTITY_BASE: string;
  readonly VITE_HMS_BASE: string;
  readonly VITE_LIS_BASE: string;
  readonly VITE_LMS_BASE: string;
  readonly VITE_PHARMACY_BASE: string;
  readonly VITE_SHARED_BASE: string;
  readonly VITE_COMMUNICATION_BASE: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}
