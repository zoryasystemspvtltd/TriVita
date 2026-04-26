import { Button, type ButtonProps } from '@mui/material';
import { forwardRef } from 'react';

/** MUI Button with TriVita defaults. `to` is accepted for polymorphic `component={RouterLink}` usage. */
export type TriVitaButtonProps = ButtonProps & { to?: string };

export const TriVitaButton = forwardRef<HTMLButtonElement, TriVitaButtonProps>(function TriVitaButton(
  { variant = 'contained', color = 'primary', size = 'medium', sx, to, ...rest },
  ref
) {
  return (
    <Button
      ref={ref}
      variant={variant}
      color={color}
      size={size}
      {...(to != null ? { to } : {})}
      sx={{ minWidth: 96, boxSizing: 'border-box', ...sx }}
      {...rest}
    />
  );
});
