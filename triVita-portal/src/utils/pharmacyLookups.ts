import { getMedicinePaged } from '@/services/pharmacyService';

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

export async function loadPharmacyUnitOptionsFromMedicines(): Promise<{ value: string; label: string }[]> {
  const res = await getMedicinePaged({ page: 1, pageSize: 1000 });
  if (!res.success || !res.data) return [];
  const items = [...(res.data.items ?? [])] as Record<string, unknown>[];
  const map = buildGroupedLabels(items, 'defaultUnitId', (row) => String(row.medicineName ?? ''));
  return mapToSelectOptions(map);
}

export async function loadPharmacyFormRefOptionsFromMedicines(): Promise<{ value: string; label: string }[]> {
  const res = await getMedicinePaged({ page: 1, pageSize: 1000 });
  if (!res.success || !res.data) return [];
  const items = [...(res.data.items ?? [])] as Record<string, unknown>[];
  const map = buildGroupedLabels(items, 'formReferenceValueId', (row) => String(row.medicineName ?? ''));
  return mapToSelectOptions(map);
}
