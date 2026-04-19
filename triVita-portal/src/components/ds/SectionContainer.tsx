import { Card, CardContent, CardHeader, Divider, type CardProps } from '@mui/material';
import type { ReactNode } from 'react';

export interface SectionContainerProps extends Omit<CardProps, 'title'> {
  title: string;
  subtitle?: string;
  action?: ReactNode;
  children: ReactNode;
}

/** Card-based section with consistent header spacing (8px grid). */
export function SectionContainer({ title, subtitle, action, children, sx, ...rest }: SectionContainerProps) {
  return (
    <Card variant="outlined" elevation={0} sx={{ borderRadius: 2, ...sx }} {...rest}>
      <CardHeader
        title={title}
        subheader={subtitle}
        action={action}
        titleTypographyProps={{ variant: 'h6', fontWeight: 600 }}
        subheaderTypographyProps={{ variant: 'body2', color: 'text.secondary' }}
        sx={{ pb: 0, '& .MuiCardHeader-action': { alignSelf: 'center' } }}
      />
      <Divider />
      <CardContent sx={{ pt: 2 }}>{children}</CardContent>
    </Card>
  );
}
