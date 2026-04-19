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
    <Drawer anchor="right" onClose={onClose} PaperProps={{ sx: { width: { xs: '100%', sm: 440, md: 520 } } }} {...rest}>
      <Stack sx={{ height: '100%' }}>
        <Stack direction="row" alignItems="flex-start" justifyContent="space-between" sx={{ p: 2, gap: 1 }}>
          <Box>
            <Typography variant="h6" component="h2">
              {title}
            </Typography>
            {subtitle ? (
              <Typography variant="body2" color="text.secondary">
                {subtitle}
              </Typography>
            ) : null}
          </Box>
          <IconButton aria-label="Close detail" onClick={onClose} size="small">
            <Close />
          </IconButton>
        </Stack>
        <Divider />
        <Box sx={{ flex: 1, overflow: 'auto', p: 2 }}>{children}</Box>
      </Stack>
    </Drawer>
  );
}
