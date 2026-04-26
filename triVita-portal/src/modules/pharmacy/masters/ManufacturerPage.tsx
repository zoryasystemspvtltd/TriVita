import * as Yup from 'yup';
import { Stack } from '@mui/material';
import { DetailKv } from '@/components/masters/DetailKv';
import { MasterEntityShell } from '@/components/masters/MasterEntityShell';

type Row = Record<string, unknown> & { id?: number };

const schema = Yup.object({
  manufacturerCode: Yup.string().trim().max(80).default(''),
  manufacturerName: Yup.string().trim().required().max(200),
  countryCode: Yup.string().trim().max(8).default(''),
});

const defaults = {
  manufacturerCode: '',
  manufacturerName: '',
  countryCode: '',
};

const fields = [
  { kind: 'text' as const, name: 'manufacturerCode', label: 'Manufacturer code', gridCols: 6 },
  { kind: 'text' as const, name: 'manufacturerName', label: 'Manufacturer name', required: true, gridCols: 6 },
  { kind: 'text' as const, name: 'countryCode', label: 'Country code', gridCols: 6 },
];

export function ManufacturerPage() {
  return (
    <MasterEntityShell<Row>
      module="pharmacy"
      resourcePath="manufacturer"
      title="Manufacturers"
      schema={schema}
      defaultCreateValues={defaults}
      fields={fields}
      getDrawerTitle={(r) => String(r.manufacturerName ?? 'Manufacturer')}
      getDrawerSubtitle={(r) => String(r.manufacturerCode ?? '')}
      columns={[
        { id: 'manufacturerCode', label: 'Code', minWidth: 120 },
        { id: 'manufacturerName', label: 'Name', minWidth: 220 },
        { id: 'countryCode', label: 'Country', minWidth: 100 },
      ]}
      rowToFormValues={(r) => ({
        manufacturerCode: String(r.manufacturerCode ?? ''),
        manufacturerName: String(r.manufacturerName ?? ''),
        countryCode: String(r.countryCode ?? ''),
      })}
      toCreatePayload={(v) => ({
        manufacturerCode: v.manufacturerCode.trim() || undefined,
        manufacturerName: v.manufacturerName.trim(),
        countryCode: v.countryCode.trim() || undefined,
      })}
      toUpdatePayload={(v) => ({
        manufacturerCode: v.manufacturerCode.trim() || undefined,
        manufacturerName: v.manufacturerName.trim(),
        countryCode: v.countryCode.trim() || undefined,
      })}
      renderDetail={(r) => (
        <Stack spacing={1}>
          <DetailKv label="Code" value={String(r.manufacturerCode ?? '')} />
          <DetailKv label="Name" value={String(r.manufacturerName ?? '')} />
          <DetailKv label="Country code" value={String(r.countryCode ?? '')} />
        </Stack>
      )}
    />
  );
}
