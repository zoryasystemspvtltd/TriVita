import { Grid, type GridProps } from '@mui/material';
import type { ReactNode } from 'react';

export interface FormGroupProps extends Omit<GridProps, 'container' | 'children'> {
  children: ReactNode;
}

/** Responsive two-column form grid: 16px gaps (MUI spacing 2). */
export function FormGroup({ children, ...rest }: FormGroupProps) {
  return (
    <Grid container spacing={2} {...rest}>
      {children}
    </Grid>
  );
}
