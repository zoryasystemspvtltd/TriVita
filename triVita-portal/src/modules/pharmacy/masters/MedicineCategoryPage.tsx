import * as Yup from 'yup';
import { Stack } from '@mui/material';
import { DetailKv } from '@/components/masters/DetailKv';
import { MasterEntityShell } from '@/components/masters/MasterEntityShell';
import { fromDatetimeLocal, toDatetimeLocal } from '@/utils/dateFields';

type Row = Record<string, unknown> & { id?: number };

const schema = Yup.object({
  categoryCode: Yup.string().trim().required().max(80),
  categoryName: Yup.string().trim().required().max(200),
  description: Yup.string().trim().max(2000).default(''),
  effectiveFrom: Yup.string().default(''),
  effectiveTo: Yup.string().default(''),
});

const defaults = {
  categoryCode: '',
  categoryName: '',
  description: '',
  effectiveFrom: '',
  effectiveTo: '',
};

const fields = [
  { kind: 'text' as const, name: 'categoryCode', label: 'Category code', required: true, gridCols: 6 },
  { kind: 'text' as const, name: 'categoryName', label: 'Category name', required: true, gridCols: 6 },
  { kind: 'textarea' as const, name: 'description', label: 'Description', gridCols: 12 },
  { kind: 'date' as const, name: 'effectiveFrom', label: 'Effective from', gridCols: 6 },
  { kind: 'date' as const, name: 'effectiveTo', label: 'Effective to', gridCols: 6 },
];

export function MedicineCategoryPage() {
  return (
    <MasterEntityShell<Row>
      module="pharmacy"
      resourcePath="medicine-category"
      title="Medicine categories"
      schema={schema}
      defaultCreateValues={defaults}
      fields={fields}
      getDrawerTitle={(r) => String(r.categoryName ?? 'Category')}
      getDrawerSubtitle={(r) => String(r.categoryCode ?? '')}
      columns={[
        { id: 'categoryCode', label: 'Code', minWidth: 120 },
        { id: 'categoryName', label: 'Name', minWidth: 200 },
        {
          id: 'description',
          label: 'Description',
          minWidth: 220,
          format: (r) => String(r.description ?? '—'),
        },
      ]}
      rowToFormValues={(r) => ({
        categoryCode: String(r.categoryCode ?? ''),
        categoryName: String(r.categoryName ?? ''),
        description: String(r.description ?? ''),
        effectiveFrom: toDatetimeLocal(r.effectiveFrom),
        effectiveTo: toDatetimeLocal(r.effectiveTo),
      })}
      toCreatePayload={(v) => ({
        categoryCode: v.categoryCode.trim(),
        categoryName: v.categoryName.trim(),
        description: v.description.trim() || undefined,
        effectiveFrom: fromDatetimeLocal(v.effectiveFrom),
        effectiveTo: fromDatetimeLocal(v.effectiveTo),
      })}
      toUpdatePayload={(v) => ({
        categoryCode: v.categoryCode.trim(),
        categoryName: v.categoryName.trim(),
        description: v.description.trim() || undefined,
        effectiveFrom: fromDatetimeLocal(v.effectiveFrom),
        effectiveTo: fromDatetimeLocal(v.effectiveTo),
      })}
      renderDetail={(r) => (
        <Stack spacing={1}>
          <DetailKv label="Code" value={String(r.categoryCode ?? '')} />
          <DetailKv label="Name" value={String(r.categoryName ?? '')} />
          <DetailKv label="Description" value={String(r.description ?? '')} />
          <DetailKv label="Effective from" value={r.effectiveFrom != null ? String(r.effectiveFrom) : ''} />
          <DetailKv label="Effective to" value={r.effectiveTo != null ? String(r.effectiveTo) : ''} />
        </Stack>
      )}
    />
  );
}
