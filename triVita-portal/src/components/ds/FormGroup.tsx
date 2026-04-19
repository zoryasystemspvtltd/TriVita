import { Grid, type GridProps } from '@mui/material';
import type { ReactNode } from 'react';

export interface FormGroupProps extends Omit<GridProps, 'container' | 'children'> {
  children: ReactNode;
}

/** Responsive two-column form grid (12-column system, spacing=2 = 16px). */
export function FormGroup({ children, ...rest }: FormGroupProps) {
  return (
    <Grid container spacing={2} {...rest}>
      {children}
    </Grid>
  );
}
