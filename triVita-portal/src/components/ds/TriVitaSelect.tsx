import { FormControl, FormHelperText, InputLabel, MenuItem, Select, type SelectProps } from '@mui/material';

export interface TriVitaSelectOption {
  value: string | number;
  label: string;
}

export interface TriVitaSelectProps extends Omit<SelectProps, 'children' | 'variant'> {
  label: string;
  options: readonly TriVitaSelectOption[];
  helperText?: string;
}

/** Labeled select with helper text slot. */
export function TriVitaSelect({
  label,
  options,
  helperText,
  fullWidth = true,
  size = 'small',
  id,
  error,
  ...rest
}: TriVitaSelectProps) {
  const lid = id ?? label.replace(/\s+/g, '-').toLowerCase();
  return (
    <FormControl fullWidth={fullWidth} size={size} margin="normal" error={error}>
      <InputLabel id={`${lid}-label`}>{label}</InputLabel>
      <Select labelId={`${lid}-label`} label={label} id={lid} {...rest}>
        {options.map((o) => (
          <MenuItem key={String(o.value)} value={o.value}>
            {o.label}
          </MenuItem>
        ))}
      </Select>
      {helperText ? <FormHelperText>{helperText}</FormHelperText> : null}
    </FormControl>
  );
}
