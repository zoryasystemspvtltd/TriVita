import { Box, Typography } from '@mui/material';

export function DetailKv({ label, value }: { label: string; value: string }) {
  return (
    <Box sx={{ py: 0.5 }}>
      <Typography variant="caption" color="text.secondary" display="block" sx={{ mb: 0.5 }}>
        {label}
      </Typography>
      <Typography variant="body2">{value || '—'}</Typography>
    </Box>
  );
}
