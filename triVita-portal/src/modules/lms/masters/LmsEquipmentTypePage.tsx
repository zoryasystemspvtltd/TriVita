import * as Yup from 'yup';
import { Stack } from '@mui/material';
import { DetailKv } from '@/components/masters/DetailKv';
import { MasterEntityShell } from '@/components/masters/MasterEntityShell';

type Row = Record<string, unknown> & { id?: number };

const schema = Yup.object({
  typeCode: Yup.string().trim().max(80).default(''),
  typeName: Yup.string().trim().required().max(200),
  description: Yup.string().trim().max(2000).default(''),
  isActive: Yup.string().oneOf(['true', 'false']).default('true'),
});

const defaults = {
  typeCode: '',
  typeName: '',
  description: '',
  isActive: 'true',
};

const fields = [
  { kind: 'text' as const, name: 'typeCode', label: 'Type code', required: true, gridCols: 6, readOnlyOnEdit: true },
  { kind: 'text' as const, name: 'typeName', label: 'Type name', required: true, gridCols: 6 },
  { kind: 'textarea' as const, name: 'description', label: 'Description', gridCols: 12 },
  {
    kind: 'select' as const,
    name: 'isActive',
    label: 'Status',
    showOn: 'edit' as const,
    gridCols: 6,
    options: [
      { value: 'true', label: 'Active' },
      { value: 'false', label: 'Inactive' },
    ],
  },
];

export function LmsEquipmentTypePage() {
  return (
    <MasterEntityShell<Row>
      module="lms"
      resourcePath="workflow/equipment-types"
      title="Equipment types"
      schema={schema}
      defaultCreateValues={defaults}
      fields={fields}
      getDrawerTitle={(r) => String(r.typeName ?? 'Equipment type')}
      getDrawerSubtitle={(r) => String(r.typeCode ?? '')}
      beforeSave={(v, mode) => {
        if (mode === 'create' && !v.typeCode.trim()) return 'Type code is required.';
        return null;
      }}
      columns={[
        { id: 'typeCode', label: 'Code', minWidth: 120 },
        { id: 'typeName', label: 'Name', minWidth: 220 },
        { id: 'description', label: 'Description', minWidth: 200, format: (r) => String(r.description ?? '—') },
      ]}
      rowToFormValues={(r) => ({
        typeCode: String(r.typeCode ?? ''),
        typeName: String(r.typeName ?? ''),
        description: String(r.description ?? ''),
        isActive: r.isActive === false ? 'false' : 'true',
      })}
      toCreatePayload={(v) => ({
        typeCode: v.typeCode.trim(),
        typeName: v.typeName.trim(),
        description: v.description.trim() || undefined,
      })}
      toUpdatePayload={(v) => ({
        typeName: v.typeName.trim(),
        description: v.description.trim() || undefined,
        isActive: v.isActive === 'true',
      })}
      renderDetail={(r) => (
        <Stack spacing={1}>
          <DetailKv label="Code" value={String(r.typeCode ?? '')} />
          <DetailKv label="Name" value={String(r.typeName ?? '')} />
          <DetailKv label="Description" value={String(r.description ?? '')} />
        </Stack>
      )}
    />
  );
}
