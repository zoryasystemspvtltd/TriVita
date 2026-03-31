import { TextField, type TextFieldProps } from '@mui/material';
import { useField, type FieldHookConfig } from 'formik';

export function FormikTextField(props: TextFieldProps & FieldHookConfig<string>) {
  const [field, meta] = useField(props);
  const { name, label, fullWidth = true, margin = 'normal', ...rest } = props;
  return (
    <TextField
      {...field}
      {...rest}
      name={name}
      label={label}
      fullWidth={fullWidth}
      margin={margin}
      error={meta.touched && Boolean(meta.error)}
      helperText={meta.touched && meta.error ? meta.error : rest.helperText}
    />
  );
}
