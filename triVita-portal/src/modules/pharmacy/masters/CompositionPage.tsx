import * as Yup from 'yup';
import { Stack } from '@mui/material';
import { DetailKv } from '@/components/masters/DetailKv';
import { MasterEntityShell } from '@/components/masters/MasterEntityShell';

type Row = Record<string, unknown> & { id?: number };

const schema = Yup.object({
  compositionName: Yup.string().trim().required().max(200),
  compositionCode: Yup.string().trim().max(80).default(''),
  notes: Yup.string().trim().max(2000).default(''),
});

const defaults = {
  compositionName: '',
  compositionCode: '',
  notes: '',
};

const fields = [
  { kind: 'text' as const, name: 'compositionName', label: 'Composition name', required: true, gridCols: 6 },
  { kind: 'text' as const, name: 'compositionCode', label: 'Composition code', gridCols: 6 },
  { kind: 'textarea' as const, name: 'notes', label: 'Notes', gridCols: 12 },
];

export function CompositionPage() {
  return (
    <MasterEntityShell<Row>
      module="pharmacy"
      resourcePath="composition"
      title="Compositions"
      schema={schema}
      defaultCreateValues={defaults}
      fields={fields}
      getDrawerTitle={(r) => String(r.compositionName ?? 'Composition')}
      getDrawerSubtitle={(r) => String(r.compositionCode ?? '')}
      columns={[
        { id: 'compositionCode', label: 'Code', minWidth: 120 },
        { id: 'compositionName', label: 'Name', minWidth: 220 },
        {
          id: 'notes',
          label: 'Notes',
          minWidth: 200,
          format: (r) => String(r.notes ?? '—'),
        },
      ]}
      rowToFormValues={(r) => ({
        compositionName: String(r.compositionName ?? ''),
        compositionCode: String(r.compositionCode ?? ''),
        notes: String(r.notes ?? ''),
      })}
      toCreatePayload={(v) => ({
        compositionName: v.compositionName.trim(),
        compositionCode: v.compositionCode.trim() || undefined,
        notes: v.notes.trim() || undefined,
      })}
      toUpdatePayload={(v) => ({
        compositionName: v.compositionName.trim(),
        compositionCode: v.compositionCode.trim() || undefined,
        notes: v.notes.trim() || undefined,
      })}
      renderDetail={(r) => (
        <Stack spacing={1}>
          <DetailKv label="Name" value={String(r.compositionName ?? '')} />
          <DetailKv label="Code" value={String(r.compositionCode ?? '')} />
          <DetailKv label="Notes" value={String(r.notes ?? '')} />
        </Stack>
      )}
    />
  );
}
