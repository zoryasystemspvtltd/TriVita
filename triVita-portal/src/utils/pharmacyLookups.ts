import { getCompositionPaged, getMedicineFormPaged, getMedicineUnitPaged } from '@/services/pharmacyService';

export function formatCompositionOptionLabel(compositionName: string, compositionCode: string): string {
  const n = compositionName.trim();
  const c = compositionCode.trim();
  if (!n) return c || '—';
  return c ? `${n} (${c})` : n;
}

export function mapToSelectOptions(map: Map<number, string>): { value: string; label: string }[] {
  return [...map.entries()]
    .sort((a, b) => a[0] - b[0])
    .map(([id, label]) => ({ value: String(id), label }));
}

/** Display label for unit dropdowns: "Milligram (mg)". */
export function formatPharmacyUnitOptionLabel(unitName: string, unitSymbol: string): string {
  const n = unitName.trim();
  const s = unitSymbol.trim();
  if (!n) return s || '—';
  return s ? `${n} (${s})` : n;
}

async function fetchAllMedicineUnits(): Promise<readonly Record<string, unknown>[]> {
  const acc: Record<string, unknown>[] = [];
  const ps = 200;
  let p = 1;
  for (;;) {
    const res = await getMedicineUnitPaged({ page: p, pageSize: ps });
    if (!res.success || !res.data) break;
    const { items, totalCount } = res.data;
    for (const r of items) acc.push(r as Record<string, unknown>);
    if (items.length === 0) break;
    if (typeof totalCount === 'number' && acc.length >= totalCount) break;
    p += 1;
  }
  return acc;
}

async function fetchAllCompositions(): Promise<readonly Record<string, unknown>[]> {
  const acc: Record<string, unknown>[] = [];
  const ps = 200;
  let p = 1;
  for (;;) {
    const res = await getCompositionPaged({ page: p, pageSize: ps });
    if (!res.success || !res.data) break;
    const { items, totalCount } = res.data;
    for (const r of items) acc.push(r as Record<string, unknown>);
    if (items.length === 0) break;
    if (typeof totalCount === 'number' && acc.length >= totalCount) break;
    p += 1;
  }
  return acc;
}

async function fetchAllMedicineForms(): Promise<readonly Record<string, unknown>[]> {
  const acc: Record<string, unknown>[] = [];
  const ps = 200;
  let p = 1;
  for (;;) {
    const res = await getMedicineFormPaged({ page: p, pageSize: ps });
    if (!res.success || !res.data) break;
    const { items, totalCount } = res.data;
    for (const r of items) acc.push(r as Record<string, unknown>);
    if (items.length === 0) break;
    if (typeof totalCount === 'number' && acc.length >= totalCount) break;
    p += 1;
  }
  return acc;
}

/** Id → display label for grids (name + symbol). */
export async function getPharmacyUnitLabelMap(): Promise<Map<number, string>> {
  const items = await fetchAllMedicineUnits();
  const m = new Map<number, string>();
  for (const raw of items) {
    const id = Number(raw.id);
    if (!Number.isFinite(id)) continue;
    m.set(id, formatPharmacyUnitOptionLabel(String(raw.unitName ?? ''), String(raw.unitSymbol ?? '')));
  }
  return m;
}

/** Unit master API — use with LookupSelect (value = id, label = name + symbol). */
export async function loadPharmacyUnitMasterOptions(): Promise<{ value: string; label: string }[]> {
  const items = await fetchAllMedicineUnits();
  return items
    .map((raw) => {
      const id = Number(raw.id);
      const unitName = String(raw.unitName ?? '');
      const unitSymbol = String(raw.unitSymbol ?? '');
      return {
        id,
        label: formatPharmacyUnitOptionLabel(unitName, unitSymbol),
      };
    })
    .filter((x) => Number.isFinite(x.id))
    .sort((a, b) => a.label.localeCompare(b.label, undefined, { sensitivity: 'base' }))
    .map((x) => ({ value: String(x.id), label: x.label }));
}

/** @deprecated Use {@link loadPharmacyUnitMasterOptions} (unit master API). */
export async function loadPharmacyUnitOptionsFromMedicines(): Promise<{ value: string; label: string }[]> {
  return loadPharmacyUnitMasterOptions();
}

/** Id → form display name for medicine grids (FK label = form name only). */
export async function getPharmacyFormLabelMap(): Promise<Map<number, string>> {
  const items = await fetchAllMedicineForms();
  const m = new Map<number, string>();
  for (const raw of items) {
    const id = Number(raw.id);
    if (!Number.isFinite(id)) continue;
    const name = String(raw.formName ?? raw.FormName ?? '').trim();
    m.set(id, name || '—');
  }
  return m;
}

/** Form master API — LookupSelect options (value = reference value id, label = form name). */
export async function loadPharmacyFormMasterOptions(): Promise<{ value: string; label: string }[]> {
  const items = await fetchAllMedicineForms();
  return items
    .map((raw) => {
      const id = Number(raw.id);
      const formName = String(raw.formName ?? raw.FormName ?? '').trim();
      return { id, label: formName || '—' };
    })
    .filter((x) => Number.isFinite(x.id))
    .sort((a, b) => a.label.localeCompare(b.label, undefined, { sensitivity: 'base' }))
    .map((x) => ({ value: String(x.id), label: x.label }));
}

/** @deprecated Use {@link loadPharmacyFormMasterOptions}. */
export async function loadPharmacyFormRefOptionsFromMedicines(): Promise<{ value: string; label: string }[]> {
  return loadPharmacyFormMasterOptions();
}

/** Id → composition display name for medicine grids. */
export async function getPharmacyCompositionLabelMap(): Promise<Map<number, string>> {
  const items = await fetchAllCompositions();
  const m = new Map<number, string>();
  for (const raw of items) {
    const id = Number(raw.id);
    if (!Number.isFinite(id)) continue;
    m.set(
      id,
      formatCompositionOptionLabel(String(raw.compositionName ?? ''), String(raw.compositionCode ?? ''))
    );
  }
  return m;
}

/** Composition master — LookupSelect options. */
export async function loadPharmacyCompositionMasterOptions(): Promise<{ value: string; label: string }[]> {
  const items = await fetchAllCompositions();
  return items
    .map((raw) => {
      const id = Number(raw.id);
      const label = formatCompositionOptionLabel(
        String(raw.compositionName ?? ''),
        String(raw.compositionCode ?? '')
      );
      return { id, label };
    })
    .filter((x) => Number.isFinite(x.id))
    .sort((a, b) => a.label.localeCompare(b.label, undefined, { sensitivity: 'base' }))
    .map((x) => ({ value: String(x.id), label: x.label }));
}
