import { Alert } from '@mui/material';
import { useAppSelector } from '@/store/hooks';
import { hasPermission, type PermissionCode } from '@/utils/permissions';
import { EmptyState } from '@/components/common/EmptyState';

export function RequirePermission({
  permission,
  children,
}: {
  permission: PermissionCode | string;
  children: React.ReactNode;
}) {
  const user = useAppSelector((s) => s.auth.user);
  const ok = hasPermission(user?.permissions, permission);

  if (!ok) {
    return (
      <>
        <Alert severity="warning" sx={{ mb: 2 }}>
          You need permission <strong>{permission}</strong> to use this area.
        </Alert>
        <EmptyState title="Access restricted" subtitle="Contact your administrator if you need access." />
      </>
    );
  }
  return <>{children}</>;
}
