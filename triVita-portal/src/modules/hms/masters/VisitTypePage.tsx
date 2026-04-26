import * as Yup from 'yup';
import { Stack } from '@mui/material';
import { DetailKv } from '@/components/masters/DetailKv';
import { MasterEntityShell } from '@/components/masters/MasterEntityShell';
import { fromDatetimeLocal, toDatetimeLocal } from '@/utils/dateFields';

type Row = Record<string, unknown> & { id?: number };

const schema = Yup.object({
  visitTypeCode: Yup.string().trim().required().max(80),
  visitTypeName: Yup.string().trim().required().max(200),
  effectiveFrom: Yup.string().default(''),
  effectiveTo: Yup.string().default(''),
  isActive: Yup.string().oneOf(['true', 'false']).default('true'),
});

const defaults = {
  visitTypeCode: '',
  visitTypeName: '',
  effectiveFrom: '',
  effectiveTo: '',
  isActive: 'true',
};

const fields = [
  { kind: 'text' as const, name: 'visitTypeCode', label: 'Visit type code', required: true, gridCols: 6, readOnlyOnEdit: true },
  { kind: 'text' as const, name: 'visitTypeName', label: 'Visit type name', required: true, gridCols: 6 },
  { kind: 'date' as const, name: 'effectiveFrom', label: 'Effective from', gridCols: 6 },
  { kind: 'date' as const, name: 'effectiveTo', label: 'Effective to', gridCols: 6 },
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

export function VisitTypePage() {
  return (
    <MasterEntityShell<Row>
      module="hms"
      resourcePath="visit-types"
      title="Visit types"
      schema={schema}
      defaultCreateValues={defaults}
      fields={fields}
      beforeSave={(v, mode) => {
        if (mode === 'create' && !v.visitTypeCode.trim()) return 'Visit type code is required.';
        return null;
      }}
      getDrawerTitle={(r) => String(r.visitTypeName ?? 'Visit type')}
      getDrawerSubtitle={(r) => String(r.visitTypeCode ?? '')}
      columns={[
        { id: 'visitTypeCode', label: 'Code', minWidth: 120 },
        { id: 'visitTypeName', label: 'Name', minWidth: 200 },
      ]}
      rowToFormValues={(r) => ({
        visitTypeCode: String(r.visitTypeCode ?? ''),
        visitTypeName: String(r.visitTypeName ?? ''),
        effectiveFrom: toDatetimeLocal(r.effectiveFrom),
        effectiveTo: toDatetimeLocal(r.effectiveTo),
        isActive: r.isActive === false ? 'false' : 'true',
      })}
      toCreatePayload={(v) => ({
        visitTypeCode: v.visitTypeCode.trim(),
        visitTypeName: v.visitTypeName.trim(),
        effectiveFrom: fromDatetimeLocal(v.effectiveFrom),
        effectiveTo: fromDatetimeLocal(v.effectiveTo),
      })}
      toUpdatePayload={(v) => ({
        visitTypeCode: v.visitTypeCode.trim(),
        visitTypeName: v.visitTypeName.trim(),
        effectiveFrom: fromDatetimeLocal(v.effectiveFrom),
        effectiveTo: fromDatetimeLocal(v.effectiveTo),
        isActive: v.isActive === 'true',
      })}
      renderDetail={(r) => (
        <Stack spacing={1}>
          <DetailKv label="Code" value={String(r.visitTypeCode ?? '')} />
          <DetailKv label="Name" value={String(r.visitTypeName ?? '')} />
          <DetailKv label="Effective from" value={r.effectiveFrom != null ? String(r.effectiveFrom) : ''} />
          <DetailKv label="Effective to" value={r.effectiveTo != null ? String(r.effectiveTo) : ''} />
          <DetailKv label="Status" value={r.isActive === false ? 'Inactive' : 'Active'} />
        </Stack>
      )}
    />
  );
}
