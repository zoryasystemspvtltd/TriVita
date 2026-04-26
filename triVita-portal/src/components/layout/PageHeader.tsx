import { Stack, Typography } from '@mui/material';
import type { ReactNode } from 'react';

export function PageHeader({
  title,
  subtitle,
  action,
}: {
  title: string;
  subtitle?: string;
  /** Optional toolbar (e.g. link to API registry). */
  action?: ReactNode;
}) {
  return (
    <Stack
      direction={{ xs: 'column', sm: 'row' }}
      spacing={1}
      alignItems={{ sm: 'flex-start' }}
      justifyContent="space-between"
      mb={3}
    >
      <Stack spacing={0.5} sx={{ flex: 1, minWidth: 0 }}>
        <Typography variant="h5" component="h1" sx={{ fontSize: '1.25rem', fontWeight: 600, lineHeight: 1.35 }}>
          {title}
        </Typography>
        {subtitle ? (
          <Typography variant="body1" color="text.secondary" sx={{ fontSize: '0.8125rem', lineHeight: 1.45 }}>
            {subtitle}
          </Typography>
        ) : null}
      </Stack>
      {action ? <Stack direction="row" spacing={1} flexShrink={0}>{action}</Stack> : null}
    </Stack>
  );
}
