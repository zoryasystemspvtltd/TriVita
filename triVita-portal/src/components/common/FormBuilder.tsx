import { Stack } from '@mui/material';
import { FormikTextField } from './FormikTextField';

export type FormFieldType = 'text' | 'email' | 'password' | 'number';

export interface FormFieldConfig {
  name: string;
  label: string;
  type?: FormFieldType;
  multiline?: boolean;
  rows?: number;
  disabled?: boolean;
}

/** Renders a vertical stack of Formik+MUI fields from a small schema. */
export function FormBuilder({ fields }: { fields: FormFieldConfig[] }) {
  return (
    <Stack spacing={0}>
      {fields.map((f) => (
        <FormikTextField
          key={f.name}
          name={f.name}
          label={f.label}
          type={f.type ?? 'text'}
          multiline={f.multiline}
          rows={f.rows}
          disabled={f.disabled}
        />
      ))}
    </Stack>
  );
}
