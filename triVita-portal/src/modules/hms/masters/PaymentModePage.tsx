import * as Yup from 'yup';
import { Stack } from '@mui/material';
import { DetailKv } from '@/components/masters/DetailKv';
import { MasterEntityShell } from '@/components/masters/MasterEntityShell';

type Row = Record<string, unknown> & { id?: number };

const schema = Yup.object({
  modeCode: Yup.string().trim().required().max(80),
  modeName: Yup.string().trim().required().max(200),
  sortOrder: Yup.string().trim().required().matches(/^\d+$/, 'Enter a whole number'),
});

const defaults = {
  modeCode: '',
  modeName: '',
  sortOrder: '0',
};

const fields = [
  { kind: 'text' as const, name: 'modeCode', label: 'Mode code', required: true, gridCols: 6 },
  { kind: 'text' as const, name: 'modeName', label: 'Mode name', required: true, gridCols: 6 },
  { kind: 'number' as const, name: 'sortOrder', label: 'Sort order', required: true, integer: true, gridCols: 6 },
];

export function PaymentModePage() {
  return (
    <MasterEntityShell<Row>
      module="hms"
      resourcePath="payment-modes"
      title="Payment modes"
      schema={schema}
      defaultCreateValues={defaults}
      fields={fields}
      getDrawerTitle={(r) => String(r.modeName ?? 'Payment mode')}
      getDrawerSubtitle={(r) => String(r.modeCode ?? '')}
      columns={[
        { id: 'modeCode', label: 'Code', minWidth: 120 },
        { id: 'modeName', label: 'Name', minWidth: 200 },
        { id: 'sortOrder', label: 'Sort', minWidth: 80 },
      ]}
      rowToFormValues={(r) => ({
        modeCode: String(r.modeCode ?? ''),
        modeName: String(r.modeName ?? ''),
        sortOrder: String(r.sortOrder ?? '0'),
      })}
      toCreatePayload={(v) => ({
        modeCode: v.modeCode.trim(),
        modeName: v.modeName.trim(),
        sortOrder: Number(v.sortOrder),
      })}
      toUpdatePayload={(v) => ({
        modeCode: v.modeCode.trim(),
        modeName: v.modeName.trim(),
        sortOrder: Number(v.sortOrder),
      })}
      renderDetail={(r) => (
        <Stack spacing={1}>
          <DetailKv label="Code" value={String(r.modeCode ?? '')} />
          <DetailKv label="Name" value={String(r.modeName ?? '')} />
          <DetailKv label="Sort order" value={String(r.sortOrder ?? '')} />
        </Stack>
      )}
    />
  );
}
