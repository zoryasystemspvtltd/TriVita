/** Mirrors Healthcare.Common.Security.TriVitaPermissions */
export const TriVitaPermissions = {
  Wildcard: '*',
  HmsApi: 'hms.api',
  LisApi: 'lis.api',
  LmsApi: 'lms.api',
  PharmacyApi: 'pharmacy.api',
  SharedApi: 'shared.api',
  CommunicationApi: 'communication.api',
  IdentityAdmin: 'identity.admin',
} as const;

export type PermissionCode = (typeof TriVitaPermissions)[keyof typeof TriVitaPermissions];

export function hasPermission(
  permissions: readonly string[] | undefined,
  required: PermissionCode | string
): boolean {
  if (!permissions?.length) return false;
  if (permissions.includes(TriVitaPermissions.Wildcard)) return true;
  return permissions.includes(required);
}
