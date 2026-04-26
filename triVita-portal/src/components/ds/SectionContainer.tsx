import { Card, CardContent, CardHeader, Divider, type CardProps } from '@mui/material';
import type { ReactNode } from 'react';

const SHELL = {
  border: '1px solid #e5e7eb',
  boxShadow: '0 2px 6px rgba(0, 0, 0, 0.05)',
} as const;

export interface SectionContainerProps extends Omit<CardProps, 'title'> {
  title: string;
  subtitle?: string;
  action?: ReactNode;
  children: ReactNode;
}

/** Section card: 16px inner padding, light border, depth vs page background. */
export function SectionContainer({ title, subtitle, action, children, sx, ...rest }: SectionContainerProps) {
  return (
    <Card
      variant="outlined"
      elevation={0}
      sx={{
        borderRadius: '10px',
        bgcolor: '#ffffff',
        border: SHELL.border,
        boxShadow: SHELL.boxShadow,
        ...sx,
      }}
      {...rest}
    >
      <CardHeader
        title={title}
        subheader={subtitle}
        action={action}
        titleTypographyProps={{ variant: 'subtitle1', fontWeight: 500, fontSize: '1rem' }}
        subheaderTypographyProps={{ variant: 'body1', color: 'text.secondary', sx: { fontSize: '0.8125rem' } }}
        sx={{ py: 2, px: 2, pb: 1, '& .MuiCardHeader-action': { alignSelf: 'center' } }}
      />
      <Divider />
      <CardContent
        sx={{
          p: 2,
          pt: 2,
          '&:last-child': { pb: 2 },
        }}
      >
        {children}
      </CardContent>
    </Card>
  );
}
