import { TextField, type TextFieldProps } from '@mui/material';
import { forwardRef } from 'react';

/** Form-aligned text field (8px grid via MUI spacing). */
export const TriVitaTextField = forwardRef<HTMLInputElement, TextFieldProps>(function TriVitaTextField(
  { size = 'small', margin = 'normal', fullWidth = true, ...rest },
  ref
) {
  return <TextField inputRef={ref} size={size} margin={margin} fullWidth={fullWidth} {...rest} />;
});
