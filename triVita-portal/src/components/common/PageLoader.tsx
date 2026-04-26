import { Box, CircularProgress, Stack, Typography } from '@mui/material';
import { TriVitaLogo } from './TriVitaLogo';

export function PageLoader({ message = 'Loading TriVita…' }: { message?: string }) {
  return (
    <Box
      minHeight="60vh"
      display="flex"
      alignItems="center"
      justifyContent="center"
      bgcolor="background.default"
    >
      <Stack spacing={2.5} alignItems="center" px={2}>
        <TriVitaLogo size="lg" />
        <CircularProgress color="primary" size={32} />
        <Typography color="text.secondary" variant="body1" sx={{ fontSize: '0.875rem' }}>
          {message}
        </Typography>
      </Stack>
    </Box>
  );
}
