import { Stack, Typography } from '@mui/material';
import { useQuery } from '@tanstack/react-query';
import { useMemo } from 'react';
import { DataTable } from '@/components/common/DataTable';
import { PageHeader } from '@/components/layout/PageHeader';
import { FormSection } from '@/components/forms/FormSection';
import { getMedicinePaged } from '@/services/pharmacyService';

type FormRow = { label: string; medicineCount: number; examples: string };

export function PharmacyFormMasterPage() {
  const meds = useQuery({
    queryKey: ['pharmacy', 'form-master', 'medicines'],
    queryFn: () => getMedicinePaged({ page: 1, pageSize: 1000 }),
    staleTime: 120_000,
  });

  const rows = useMemo<FormRow[]>(() => {
    if (!meds.data?.success || !meds.data.data) return [];
    const byForm = new Map<number, { count: number; examples: string[] }>();
    for (const raw of meds.data.data.items as Record<string, unknown>[]) {
      const fid = Number(raw.formReferenceValueId);
      if (!Number.isFinite(fid)) continue;
      const name = String(raw.medicineName ?? '').trim();
      const cur = byForm.get(fid) ?? { count: 0, examples: [] };
      cur.count += 1;
      if (name && cur.examples.length < 4) cur.examples.push(name);
      byForm.set(fid, cur);
    }
    const sorted = [...byForm.entries()].sort((a, b) => b[1].count - a[1].count);
    return sorted.map(([_, v], idx) => ({
      label: `Dosage form slot ${idx + 1}`,
      medicineCount: v.count,
      examples: v.examples.join(', '),
    }));
  }, [meds.data]);

  return (
    <Stack spacing={2}>
      <PageHeader title="Form master" subtitle="Distinct dosage forms referenced on medicines (no raw identifiers shown)." />
      <FormSection title="Forms in use">
        <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
          Reference values are managed in shared data. This directory is rebuilt from medicines so pharmacy can see which form configurations are used on the formulary.
        </Typography>
        <DataTable<FormRow>
          tableAriaLabel="Form master"
          columns={[
            { id: 'label', label: 'Form slot' },
            { id: 'medicineCount', label: 'Medicines', align: 'right', format: (r) => String(r.medicineCount) },
            { id: 'examples', label: 'Example medicines', format: (r) => r.examples || '—' },
          ]}
          rows={rows}
          rowKey={(r) => r.label}
          loading={meds.isLoading}
          emptyTitle="No form references on medicines yet"
        />
      </FormSection>
    </Stack>
  );
}
