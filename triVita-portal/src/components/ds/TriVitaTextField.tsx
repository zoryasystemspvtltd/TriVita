import { TextField, type TextFieldProps } from '@mui/material';
import { forwardRef } from 'react';

/** Form text field with design-system defaults (compact, label above). */
export const TriVitaTextField = forwardRef<HTMLInputElement, TextFieldProps>(function TriVitaTextField(
  { size = 'small', margin = 'dense', fullWidth = true, ...rest },
  ref
) {
  return <TextField inputRef={ref} size={size} margin={margin} fullWidth={fullWidth} {...rest} />;
});
