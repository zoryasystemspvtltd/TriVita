import { Navigate, useParams } from 'react-router-dom';
import { RequirePermission } from '@/components/auth/RequirePermission';
import { ModuleDataRegistryPage } from '@/pages/ModuleDataRegistryPage';
import type { ApiRegistryModule } from '@/generated/apiRegistry';
import { TriVitaPermissions, type PermissionCode } from '@/utils/permissions';

const MODULES = new Set<string>(['hms', 'lis', 'lms', 'pharmacy', 'shared', 'communication', 'identity']);

const permissionByModule: Record<string, PermissionCode> = {
  hms: TriVitaPermissions.HmsApi,
  lis: TriVitaPermissions.LisApi,
  lms: TriVitaPermissions.LmsApi,
  pharmacy: TriVitaPermissions.PharmacyApi,
  shared: TriVitaPermissions.SharedApi,
  communication: TriVitaPermissions.CommunicationApi,
  identity: TriVitaPermissions.IdentityAdmin,
};

export function DataRegistryRoute() {
  const { module } = useParams();
  if (!module || !MODULES.has(module)) return <Navigate to="/dashboard" replace />;
  const permission = permissionByModule[module];
  if (!permission) return <Navigate to="/dashboard" replace />;

  return (
    <RequirePermission permission={permission}>
      <ModuleDataRegistryPage module={module as ApiRegistryModule} />
    </RequirePermission>
  );
}
