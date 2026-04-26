import { useQuery } from '@tanstack/react-query';
import { Controller, type Control, type FieldPath, type FieldValues } from 'react-hook-form';
import { useMemo } from 'react';
import { TriVitaSelect, type TriVitaSelectOption } from '@/components/ds/TriVitaSelect';

export type LookupLoadContext = { editId: number | null };

export interface LookupSelectProps<T extends FieldValues> {
  name: FieldPath<T>;
  control: Control<T>;
  label: string;
  required?: boolean;
  disabled?: boolean;
  queryKey: readonly unknown[];
  loadOptions: (ctx: LookupLoadContext) => Promise<readonly TriVitaSelectOption[]>;
  editId: number | null;
  allowNone?: boolean;
  noneLabel?: string;
  noneValue?: string;
}

export function LookupSelect<T extends FieldValues>({
  name,
  control,
  label,
  required,
  disabled,
  queryKey,
  loadOptions,
  editId,
  allowNone,
  noneLabel,
  noneValue = '',
}: LookupSelectProps<T>) {
  const optsQuery = useQuery({
    queryKey: [...queryKey, editId],
    queryFn: () => loadOptions({ editId }),
    staleTime: 60_000,
  });

  const options: TriVitaSelectOption[] = useMemo(() => {
    const base = [...(optsQuery.data ?? [])];
    if (allowNone) {
      return [{ value: noneValue, label: noneLabel ?? '—' }, ...base];
    }
    return base;
  }, [optsQuery.data, allowNone, noneLabel, noneValue]);

  return (
    <Controller
      name={name}
      control={control}
      render={({ field, fieldState }) => (
        <TriVitaSelect
          label={label}
          required={required}
          value={field.value ?? ''}
          onChange={(e) => field.onChange(String(e.target.value))}
          options={options}
          disabled={disabled || optsQuery.isLoading}
          error={Boolean(fieldState.error)}
          helperText={
            fieldState.error?.message ?? (optsQuery.isLoading ? 'Loading…' : optsQuery.isError ? 'Could not load options' : undefined)
          }
        />
      )}
    />
  );
}
