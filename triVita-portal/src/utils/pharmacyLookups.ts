import { getMedicinePaged, getMedicineUnitPaged } from '@/services/pharmacyService';

function buildGroupedLabels(
  items: readonly Record<string, unknown>[],
  idKey: string,
  labelFromRow: (row: Record<string, unknown>) => string
): Map<number, string> {
  const groups = new Map<number, string[]>();
  for (const row of items) {
    const raw = row[idKey];
    if (raw == null) continue;
    const id = Number(raw);
    if (!Number.isFinite(id)) continue;
    const lbl = labelFromRow(row).trim();
    if (!lbl) continue;
    if (!groups.has(id)) groups.set(id, []);
    const arr = groups.get(id)!;
    if (!arr.includes(lbl)) arr.push(lbl);
  }
  const result = new Map<number, string>();
  for (const [id, names] of groups) {
    result.set(id, names.slice(0, 2).join(', ') + (names.length > 2 ? '…' : ''));
  }
  return result;
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

export async function loadPharmacyFormRefOptionsFromMedicines(): Promise<{ value: string; label: string }[]> {
  const res = await getMedicinePaged({ page: 1, pageSize: 1000 });
  if (!res.success || !res.data) return [];
  const items = [...(res.data.items ?? [])] as Record<string, unknown>[];
  const map = buildGroupedLabels(items, 'formReferenceValueId', (row) => String(row.medicineName ?? ''));
  return mapToSelectOptions(map);
}
