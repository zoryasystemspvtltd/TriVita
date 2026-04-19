import {
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  type DialogProps,
  Stack,
} from '@mui/material';
import type { ReactNode } from 'react';

export interface TriVitaModalProps extends Omit<DialogProps, 'title' | 'children'> {
  title: string;
  children: ReactNode;
  actions?: ReactNode;
}

/** Standard modal shell — consistent padding (8px grid). */
export function TriVitaModal({ title, children, actions, ...rest }: TriVitaModalProps) {
  return (
    <Dialog fullWidth maxWidth="md" {...rest}>
      <DialogTitle>{title}</DialogTitle>
      <DialogContent dividers>
        <Stack spacing={2}>{children}</Stack>
      </DialogContent>
      {actions ? <DialogActions sx={{ px: 3, py: 2 }}>{actions}</DialogActions> : null}
    </Dialog>
  );
}
