import { Stack, Typography } from '@mui/material';
import { TriVitaLogo } from './TriVitaLogo';

export function EmptyState({ title, subtitle }: { title: string; subtitle?: string }) {
  return (
    <Stack alignItems="center" spacing={1.5} py={2}>
      <TriVitaLogo size="sm" />
      <Typography variant="subtitle1" color="text.secondary">
        {title}
      </Typography>
      {subtitle ? (
        <Typography variant="body2" color="text.disabled">
          {subtitle}
        </Typography>
      ) : null}
    </Stack>
  );
}
