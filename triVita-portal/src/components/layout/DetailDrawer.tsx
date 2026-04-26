import { Box, Divider, Drawer, IconButton, Stack, Typography, type DrawerProps } from '@mui/material';
import { Close } from '@mui/icons-material';
import type { ReactNode } from 'react';

export interface DetailDrawerProps extends Omit<DrawerProps, 'children'> {
  title: string;
  subtitle?: string;
  children: ReactNode;
  onClose: () => void;
}

/** Right-rail detail panel for master-detail workflows. */
export function DetailDrawer({ title, subtitle, children, onClose, ...rest }: DetailDrawerProps) {
  return (
    <Drawer
      anchor="right"
      onClose={onClose}
      PaperProps={{
        sx: { width: { xs: '100%', sm: 440, md: 520 }, borderLeft: 1, borderColor: 'divider' },
      }}
      {...rest}
    >
      <Stack sx={{ height: '100%' }}>
        <Stack
          direction="row"
          alignItems="flex-start"
          justifyContent="space-between"
          sx={{ p: 2, gap: 2, bgcolor: 'background.paper' }}
        >
          <Box>
            <Typography
              component="h2"
              variant="h5"
              sx={{ fontSize: '1.25rem', fontWeight: 600, lineHeight: 1.35, mb: subtitle ? 0.5 : 0 }}
            >
              {title}
            </Typography>
            {subtitle ? (
              <Typography variant="body1" color="text.secondary" sx={{ fontSize: '0.8125rem' }}>
                {subtitle}
              </Typography>
            ) : null}
          </Box>
          <IconButton aria-label="Close detail" onClick={onClose} size="small" sx={{ mt: -0.5 }}>
            <Close fontSize="small" />
          </IconButton>
        </Stack>
        <Divider />
        <Box sx={{ flex: 1, overflow: 'auto', p: 2 }}>{children}</Box>
      </Stack>
    </Drawer>
  );
}
