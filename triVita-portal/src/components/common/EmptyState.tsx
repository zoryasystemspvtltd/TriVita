import { InboxOutlined } from '@mui/icons-material';
import { Box, Stack, Typography } from '@mui/material';

export function EmptyState({ title, subtitle }: { title: string; subtitle?: string }) {
  return (
    <Box role="status" py={5} px={2} bgcolor="#fafbfc" sx={{ borderRadius: 1, mx: 0.5 }}>
      <Stack alignItems="center" spacing={1.5}>
        <InboxOutlined sx={{ fontSize: 40, color: 'text.disabled', opacity: 0.85 }} aria-hidden />
        <Typography
          variant="body1"
          textAlign="center"
          color="text.secondary"
          sx={{ fontSize: '0.875rem', fontWeight: 500, maxWidth: 360 }}
        >
          {title}
        </Typography>
        {subtitle ? (
          <Typography variant="body2" color="text.disabled" textAlign="center" sx={{ maxWidth: 400 }}>
            {subtitle}
          </Typography>
        ) : null}
      </Stack>
    </Box>
  );
}
