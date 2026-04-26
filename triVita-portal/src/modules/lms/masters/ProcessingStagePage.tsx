import * as Yup from 'yup';
import { Stack } from '@mui/material';
import { DetailKv } from '@/components/masters/DetailKv';
import { MasterEntityShell } from '@/components/masters/MasterEntityShell';

type Row = Record<string, unknown> & { id?: number };

const schema = Yup.object({
  stageCode: Yup.string().trim().required().max(80),
  stageName: Yup.string().trim().required().max(200),
  sequenceNo: Yup.string().trim().required().matches(/^\d+$/, 'Enter a whole number'),
  stageNotes: Yup.string().trim().max(2000).default(''),
});

const defaults = {
  stageCode: '',
  stageName: '',
  sequenceNo: '1',
  stageNotes: '',
};

const fields = [
  { kind: 'text' as const, name: 'stageCode', label: 'Stage code', required: true, gridCols: 6 },
  { kind: 'text' as const, name: 'stageName', label: 'Stage name', required: true, gridCols: 6 },
  { kind: 'number' as const, name: 'sequenceNo', label: 'Sequence', required: true, integer: true, gridCols: 6 },
  { kind: 'textarea' as const, name: 'stageNotes', label: 'Notes', gridCols: 12 },
];

export function ProcessingStagePage() {
  return (
    <MasterEntityShell<Row>
      module="lms"
      resourcePath="processing-stage"
      title="Processing stages"
      schema={schema}
      defaultCreateValues={defaults}
      fields={fields}
      getDrawerTitle={(r) => String(r.stageName ?? 'Stage')}
      getDrawerSubtitle={(r) => String(r.stageCode ?? '')}
      columns={[
        { id: 'stageCode', label: 'Code', minWidth: 120 },
        { id: 'stageName', label: 'Name', minWidth: 200 },
        { id: 'sequenceNo', label: 'Sequence', minWidth: 100 },
      ]}
      rowToFormValues={(r) => ({
        stageCode: String(r.stageCode ?? ''),
        stageName: String(r.stageName ?? ''),
        sequenceNo: String(r.sequenceNo ?? '0'),
        stageNotes: String(r.stageNotes ?? ''),
      })}
      toCreatePayload={(v) => ({
        stageCode: v.stageCode.trim(),
        stageName: v.stageName.trim(),
        sequenceNo: Number(v.sequenceNo),
        stageNotes: v.stageNotes.trim() || undefined,
      })}
      toUpdatePayload={(v) => ({
        stageCode: v.stageCode.trim(),
        stageName: v.stageName.trim(),
        sequenceNo: Number(v.sequenceNo),
        stageNotes: v.stageNotes.trim() || undefined,
      })}
      renderDetail={(r) => (
        <Stack spacing={1}>
          <DetailKv label="Code" value={String(r.stageCode ?? '')} />
          <DetailKv label="Name" value={String(r.stageName ?? '')} />
          <DetailKv label="Sequence" value={String(r.sequenceNo ?? '')} />
          <DetailKv label="Notes" value={String(r.stageNotes ?? '')} />
        </Stack>
      )}
    />
  );
}
