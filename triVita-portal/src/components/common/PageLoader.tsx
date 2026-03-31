import { Box, CircularProgress, Stack, Typography } from '@mui/material';
import { TriVitaLogo } from './TriVitaLogo';

export function PageLoader({ message = 'Loading TriVita…' }: { message?: string }) {
  return (
    <Box
      minHeight="60vh"
      display="flex"
      alignItems="center"
      justifyContent="center"
      sx={{ background: 'linear-gradient(180deg, #f4f8f9 0%, #fff 100%)' }}
    >
      <Stack spacing={3} alignItems="center">
        <TriVitaLogo size="lg" />
        <CircularProgress color="primary" size={36} />
        <Typography color="text.secondary">{message}</Typography>
      </Stack>
    </Box>
  );
}
