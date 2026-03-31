import {
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  type DialogProps,
} from '@mui/material';

export interface AppModalProps extends Omit<DialogProps, 'open'> {
  open: boolean;
  title: string;
  onClose: () => void;
  actions?: React.ReactNode;
  children: React.ReactNode;
}

export function AppModal({ open, title, onClose, actions, children, ...rest }: AppModalProps) {
  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm" {...rest}>
      <DialogTitle>{title}</DialogTitle>
      <DialogContent dividers>{children}</DialogContent>
      {actions ? <DialogActions>{actions}</DialogActions> : null}
    </Dialog>
  );
}
