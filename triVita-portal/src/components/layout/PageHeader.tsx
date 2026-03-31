import { Stack, Typography } from '@mui/material';

export function PageHeader({ title, subtitle }: { title: string; subtitle?: string }) {
  return (
    <Stack spacing={0.5} mb={2}>
      <Typography variant="h5">{title}</Typography>
      {subtitle ? (
        <Typography variant="body2" color="text.secondary">
          {subtitle}
        </Typography>
      ) : null}
    </Stack>
  );
}
