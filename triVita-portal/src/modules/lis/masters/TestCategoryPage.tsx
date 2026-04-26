import * as Yup from 'yup';
import { Stack } from '@mui/material';
import { useQuery } from '@tanstack/react-query';
import { useMemo } from 'react';
import { DetailKv } from '@/components/masters/DetailKv';
import { MasterEntityShell } from '@/components/masters/MasterEntityShell';
import type { LookupLoadContext } from '@/components/common/LookupSelect';
import { getTestCategoryPaged } from '@/services/lisService';
import { fromDatetimeLocal, toDatetimeLocal } from '@/utils/dateFields';

type Row = Record<string, unknown> & { id?: number };

const schema = Yup.object({
  categoryCode: Yup.string().trim().required().max(80),
  categoryName: Yup.string().trim().required().max(200),
  parentCategoryId: Yup.string().trim().matches(/^$|^\d+$/, 'Invalid selection').default(''),
  effectiveFrom: Yup.string().default(''),
  effectiveTo: Yup.string().default(''),
});

const defaults = {
  categoryCode: '',
  categoryName: '',
  parentCategoryId: '',
  effectiveFrom: '',
  effectiveTo: '',
};

const fields = [
  { kind: 'text' as const, name: 'categoryCode', label: 'Category code', required: true, gridCols: 6 },
  { kind: 'text' as const, name: 'categoryName', label: 'Category name', required: true, gridCols: 6 },
  {
    kind: 'lookup' as const,
    name: 'parentCategoryId',
    label: 'Parent category',
    queryKey: ['lis', 'test-category', 'parent-options'],
    allowNone: true,
    noneLabel: 'None',
    loadOptions: async ({ editId }: LookupLoadContext) => {
      const res = await getTestCategoryPaged({ page: 1, pageSize: 500 });
      if (!res.success || !res.data) return [];
      const items = [...(res.data.items ?? [])] as Row[];
      return items
        .filter((r) => editId == null || Number(r.id) !== editId)
        .map((r) => ({
          value: String(r.id ?? ''),
          label: String(r.categoryName ?? r.categoryCode ?? ''),
        }));
    },
    gridCols: 6,
  },
  { kind: 'date' as const, name: 'effectiveFrom', label: 'Effective from', gridCols: 6 },
  { kind: 'date' as const, name: 'effectiveTo', label: 'Effective to', gridCols: 6 },
];

export function TestCategoryPage() {
  const parentNames = useQuery({
    queryKey: ['lis', 'test-category', 'name-map'],
    queryFn: async () => {
      const res = await getTestCategoryPaged({ page: 1, pageSize: 500 });
      const m = new Map<number, string>();
      if (res.success && res.data) {
        for (const r of res.data.items as Row[]) {
          const id = Number(r.id);
          if (Number.isFinite(id)) m.set(id, String(r.categoryName ?? r.categoryCode ?? ''));
        }
      }
      return m;
    },
    staleTime: 60_000,
  });

  const parentMap = parentNames.data ?? new Map<number, string>();

  const columns = useMemo(
    () => [
      { id: 'categoryCode', label: 'Code', minWidth: 120 },
      { id: 'categoryName', label: 'Name', minWidth: 200 },
      {
        id: 'parentCategoryId',
        label: 'Parent',
        minWidth: 180,
        format: (r: Row) => {
          const pid = r.parentCategoryId;
          if (pid == null) return '—';
          const id = Number(pid);
          return parentMap.get(id) ?? '—';
        },
      },
    ],
    [parentMap]
  );

  return (
    <MasterEntityShell<Row>
      module="lis"
      resourcePath="test-category"
      title="Test categories"
      schema={schema}
      defaultCreateValues={defaults}
      fields={fields}
      getDrawerTitle={(r) => String(r.categoryName ?? 'Test category')}
      getDrawerSubtitle={(r) => String(r.categoryCode ?? '')}
      columns={columns}
      rowToFormValues={(r) => ({
        categoryCode: String(r.categoryCode ?? ''),
        categoryName: String(r.categoryName ?? ''),
        parentCategoryId: r.parentCategoryId != null ? String(r.parentCategoryId) : '',
        effectiveFrom: toDatetimeLocal(r.effectiveFrom),
        effectiveTo: toDatetimeLocal(r.effectiveTo),
      })}
      toCreatePayload={(v) => ({
        categoryCode: v.categoryCode.trim(),
        categoryName: v.categoryName.trim(),
        parentCategoryId: v.parentCategoryId.trim() ? Number(v.parentCategoryId) : undefined,
        effectiveFrom: fromDatetimeLocal(v.effectiveFrom),
        effectiveTo: fromDatetimeLocal(v.effectiveTo),
      })}
      toUpdatePayload={(v) => ({
        categoryCode: v.categoryCode.trim(),
        categoryName: v.categoryName.trim(),
        parentCategoryId: v.parentCategoryId.trim() ? Number(v.parentCategoryId) : undefined,
        effectiveFrom: fromDatetimeLocal(v.effectiveFrom),
        effectiveTo: fromDatetimeLocal(v.effectiveTo),
      })}
      renderDetail={(r) => (
        <Stack spacing={1}>
          <DetailKv label="Code" value={String(r.categoryCode ?? '')} />
          <DetailKv label="Name" value={String(r.categoryName ?? '')} />
          <DetailKv
            label="Parent category"
            value={(() => {
              const pid = r.parentCategoryId;
              if (pid == null) return '';
              return parentMap.get(Number(pid)) ?? '';
            })()}
          />
          <DetailKv label="Effective from" value={r.effectiveFrom != null ? String(r.effectiveFrom) : ''} />
          <DetailKv label="Effective to" value={r.effectiveTo != null ? String(r.effectiveTo) : ''} />
        </Stack>
      )}
    />
  );
}
