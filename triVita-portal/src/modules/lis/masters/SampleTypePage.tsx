import * as Yup from 'yup';
import { Stack } from '@mui/material';
import { DetailKv } from '@/components/masters/DetailKv';
import { MasterEntityShell } from '@/components/masters/MasterEntityShell';
import { fromDatetimeLocal, toDatetimeLocal } from '@/utils/dateFields';

type Row = Record<string, unknown> & { id?: number };

const schema = Yup.object({
  sampleTypeCode: Yup.string().trim().required().max(80),
  sampleTypeName: Yup.string().trim().required().max(200),
  description: Yup.string().trim().max(2000).default(''),
  effectiveFrom: Yup.string().default(''),
  effectiveTo: Yup.string().default(''),
});

const defaults = {
  sampleTypeCode: '',
  sampleTypeName: '',
  description: '',
  effectiveFrom: '',
  effectiveTo: '',
};

const fields = [
  { kind: 'text' as const, name: 'sampleTypeCode', label: 'Sample type code', required: true, gridCols: 6 },
  { kind: 'text' as const, name: 'sampleTypeName', label: 'Sample type name', required: true, gridCols: 6 },
  { kind: 'textarea' as const, name: 'description', label: 'Description', gridCols: 12 },
  { kind: 'date' as const, name: 'effectiveFrom', label: 'Effective from', gridCols: 6 },
  { kind: 'date' as const, name: 'effectiveTo', label: 'Effective to', gridCols: 6 },
];

export function SampleTypePage() {
  return (
    <MasterEntityShell<Row>
      module="lis"
      resourcePath="sample-type"
      title="Sample types"
      schema={schema}
      defaultCreateValues={defaults}
      fields={fields}
      getDrawerTitle={(r) => String(r.sampleTypeName ?? 'Sample type')}
      getDrawerSubtitle={(r) => String(r.sampleTypeCode ?? '')}
      columns={[
        { id: 'sampleTypeCode', label: 'Code', minWidth: 120 },
        { id: 'sampleTypeName', label: 'Name', minWidth: 200 },
        { id: 'description', label: 'Description', minWidth: 200, format: (r) => String(r.description ?? '—') },
      ]}
      rowToFormValues={(r) => ({
        sampleTypeCode: String(r.sampleTypeCode ?? ''),
        sampleTypeName: String(r.sampleTypeName ?? ''),
        description: String(r.description ?? ''),
        effectiveFrom: toDatetimeLocal(r.effectiveFrom),
        effectiveTo: toDatetimeLocal(r.effectiveTo),
      })}
      toCreatePayload={(v) => ({
        sampleTypeCode: v.sampleTypeCode.trim(),
        sampleTypeName: v.sampleTypeName.trim(),
        description: v.description.trim() || undefined,
        effectiveFrom: fromDatetimeLocal(v.effectiveFrom),
        effectiveTo: fromDatetimeLocal(v.effectiveTo),
      })}
      toUpdatePayload={(v) => ({
        sampleTypeCode: v.sampleTypeCode.trim(),
        sampleTypeName: v.sampleTypeName.trim(),
        description: v.description.trim() || undefined,
        effectiveFrom: fromDatetimeLocal(v.effectiveFrom),
        effectiveTo: fromDatetimeLocal(v.effectiveTo),
      })}
      renderDetail={(r) => (
        <Stack spacing={1}>
          <DetailKv label="Code" value={String(r.sampleTypeCode ?? '')} />
          <DetailKv label="Name" value={String(r.sampleTypeName ?? '')} />
          <DetailKv label="Description" value={String(r.description ?? '')} />
          <DetailKv label="Effective from" value={r.effectiveFrom != null ? String(r.effectiveFrom) : ''} />
          <DetailKv label="Effective to" value={r.effectiveTo != null ? String(r.effectiveTo) : ''} />
        </Stack>
      )}
    />
  );
}
