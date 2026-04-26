export function toDatetimeLocal(v: unknown): string {
  if (v == null || v === '') return '';
  const s = String(v);
  if (!s) return '';
  const d = new Date(s);
  if (Number.isNaN(d.getTime())) return '';
  return d.toISOString().slice(0, 16);
}

export function fromDatetimeLocal(v: string): string | undefined {
  const t = v.trim();
  if (!t) return undefined;
  const d = new Date(t);
  if (Number.isNaN(d.getTime())) return undefined;
  return d.toISOString();
}
