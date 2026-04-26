import { Stack, Typography } from '@mui/material';
import { useQuery } from '@tanstack/react-query';
import { useMemo } from 'react';
import { DataTable } from '@/components/common/DataTable';
import { PageHeader } from '@/components/layout/PageHeader';
import { FormSection } from '@/components/forms/FormSection';
import { getMedicinePaged } from '@/services/pharmacyService';

type UnitRow = { label: string; medicineCount: number; examples: string };

export function PharmacyUnitMasterPage() {
  const meds = useQuery({
    queryKey: ['pharmacy', 'unit-master', 'medicines'],
    queryFn: () => getMedicinePaged({ page: 1, pageSize: 1000 }),
    staleTime: 120_000,
  });

  const rows = useMemo<UnitRow[]>(() => {
    if (!meds.data?.success || !meds.data.data) return [];
    const byUnit = new Map<number, { count: number; examples: string[] }>();
    for (const raw of meds.data.data.items as Record<string, unknown>[]) {
      const uid = Number(raw.defaultUnitId);
      if (!Number.isFinite(uid)) continue;
      const name = String(raw.medicineName ?? '').trim();
      const cur = byUnit.get(uid) ?? { count: 0, examples: [] };
      cur.count += 1;
      if (name && cur.examples.length < 4) cur.examples.push(name);
      byUnit.set(uid, cur);
    }
    const sorted = [...byUnit.entries()].sort((a, b) => b[1].count - a[1].count);
    return sorted.map(([_, v], idx) => ({
      label: `Formulary unit slot ${idx + 1}`,
      medicineCount: v.count,
      examples: v.examples.join(', '),
    }));
  }, [meds.data]);

  return (
    <Stack spacing={2}>
      <PageHeader title="Unit master" subtitle="Distinct dose or pack units currently referenced on medicines (no raw identifiers shown)." />
      <FormSection title="Units in use">
        <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
          Enterprise reference labels are managed centrally. This list is derived from the medicine formulary so you can see which unit configurations are in active use.
        </Typography>
        <DataTable<UnitRow>
          tableAriaLabel="Unit master"
          columns={[
            { id: 'label', label: 'Unit slot' },
            { id: 'medicineCount', label: 'Medicines', align: 'right', format: (r) => String(r.medicineCount) },
            { id: 'examples', label: 'Example medicines', format: (r) => r.examples || '—' },
          ]}
          rows={rows}
          rowKey={(r) => r.label}
          loading={meds.isLoading}
          emptyTitle="No unit references on medicines yet"
        />
      </FormSection>
    </Stack>
  );
}
