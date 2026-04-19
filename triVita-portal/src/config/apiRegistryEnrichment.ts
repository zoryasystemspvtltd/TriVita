import type { ApiRegistryEntry } from '@/generated/apiRegistry';

export interface RegistryEnrichment {
  /** Additional GET endpoints that return paged lists (e.g. notifications/logs). */
  secondaryPagedGets?: readonly { relPath: string; label: string }[];
}

const ENRICH: Partial<Record<string, RegistryEnrichment>> = {
  'communication:notifications': {
    secondaryPagedGets: [
      { relPath: 'logs', label: 'Delivery logs' },
      { relPath: 'templates', label: 'Templates' },
    ],
  },
};

export function enrichApiRegistryEntry(entry: ApiRegistryEntry): ApiRegistryEntry & RegistryEnrichment {
  const key = `${entry.module}:${entry.path}`;
  return { ...entry, ...(ENRICH[key] ?? {}) };
}
