import {
  AdminPanelSettings,
  AltRoute,
  Biotech,
  Business,
  ChatBubbleOutline,
  LocalHospital,
  LocalPharmacy,
  Science,
} from '@mui/icons-material';
import { TriVitaPermissions } from '@/utils/permissions';

export interface NavChild {
  label: string;
  path: string;
}

export interface NavGroup {
  /** Stable id for state keys, e.g. "masters" */
  id: string;
  label: string;
  children: NavChild[];
}

export interface NavItem {
  label: string;
  path: string;
  icon: typeof LocalHospital;
  permission: string;
  groups: NavGroup[];
}

/** Always visible for authenticated users (no module permission gate). */
export const utilityNavigation: { label: string; path: string; icon: typeof AltRoute }[] = [
  { label: 'Clinical journeys', path: '/journeys', icon: AltRoute },
];

export const mainNavigation: NavItem[] = [
  {
    label: 'HMS',
    path: '/hms',
    icon: LocalHospital,
    permission: TriVitaPermissions.HmsApi,
    groups: [
      {
        id: 'masters',
        label: 'Masters',
        children: [
          { label: 'Visit types', path: '/hms/masters/visit-types' },
          { label: 'Payment modes', path: '/hms/masters/payment-modes' },
        ],
      },
      {
        id: 'transactions',
        label: 'Transactions',
        children: [
          { label: 'Patient registration', path: '/hms/patients' },
          { label: 'Appointments', path: '/hms/appointments' },
          { label: 'Visits (OPD)', path: '/hms/visits' },
          { label: 'OPD dashboard', path: '/hms/opd' },
          { label: 'Prescriptions', path: '/hms/prescriptions' },
          { label: 'IPD (wards / beds / admissions)', path: '/hms/ipd' },
          { label: 'Billing (line items)', path: '/hms/billing' },
          { label: 'Billing & payments (hub)', path: '/hms/billing-hub' },
        ],
      },
      {
        id: 'reports',
        label: 'Reports',
        children: [{ label: 'API registry (admin)', path: '/hms/data-registry' }],
      },
    ],
  },
  {
    label: 'LMS',
    path: '/lms',
    icon: Science,
    permission: TriVitaPermissions.LmsApi,
    groups: [
      {
        id: 'masters',
        label: 'Masters',
        children: [
          { label: 'Test master', path: '/lms/test-master' },
          { label: 'Equipment master', path: '/lms/equipment' },
          { label: 'Equipment ↔ facility', path: '/lms/equipment-mappings' },
          { label: 'Sample workflow', path: '/lms/workflow' },
          { label: 'Processing stages', path: '/lms/masters/processing-stage' },
          { label: 'Equipment types', path: '/lms/masters/equipment-type' },
        ],
      },
      {
        id: 'transactions',
        label: 'Transactions',
        children: [
          { label: 'Test booking', path: '/lms/bookings' },
          { label: 'Barcode management', path: '/lms/barcodes' },
          { label: 'Work queue', path: '/lms/work-queue' },
        ],
      },
      {
        id: 'reports',
        label: 'Reports',
        children: [{ label: 'API registry (admin)', path: '/lms/data-registry' }],
      },
    ],
  },
  {
    label: 'LIS',
    path: '/lis',
    icon: Biotech,
    permission: TriVitaPermissions.LisApi,
    groups: [
      {
        id: 'masters',
        label: 'Masters',
        children: [
          { label: 'Sample types', path: '/lis/masters/sample-type' },
          { label: 'Test categories', path: '/lis/masters/test-category' },
        ],
      },
      {
        id: 'transactions',
        label: 'Transactions',
        children: [
          { label: 'Analyzer monitoring', path: '/lis/analyzer' },
          { label: 'Lab orders', path: '/lis/lab-orders' },
          { label: 'Sample tracking', path: '/lis/sample-tracking' },
          { label: 'Result viewer', path: '/lis/results' },
          { label: 'Result history', path: '/lis/result-history' },
          { label: 'Result verification', path: '/lis/verification' },
        ],
      },
      {
        id: 'reports',
        label: 'Reports',
        children: [{ label: 'API registry (admin)', path: '/lis/data-registry' }],
      },
    ],
  },
  {
    label: 'Pharmacy',
    path: '/pharmacy',
    icon: LocalPharmacy,
    permission: TriVitaPermissions.PharmacyApi,
    groups: [
      {
        id: 'masters',
        label: 'Masters',
        children: [
          { label: 'Medicine Master', path: '/pharmacy/masters/medicine' },
          { label: 'Category master', path: '/pharmacy/masters/medicine-category' },
          { label: 'Manufacturer master', path: '/pharmacy/masters/manufacturer' },
          { label: 'Composition master', path: '/pharmacy/masters/composition' },
          { label: 'Unit Master', path: '/pharmacy/masters/unit' },
          { label: 'Form Master', path: '/pharmacy/masters/form' },
          { label: 'Supplier Master', path: '/pharmacy/masters/supplier' },
              { label: 'Customer Master', path: '/pharmacy/masters/customer' },
          { label: 'Medicine Batches', path: '/pharmacy/masters/medicine-batches' },
        ],
      },
      {
        id: 'transactions',
        label: 'Transactions',
        children: [
          { label: 'Purchase order', path: '/pharmacy/transactions/purchase-order' },
          { label: 'Goods receipt (GRN)', path: '/pharmacy/transactions/goods-receipt' },
          { label: 'Sales / billing', path: '/pharmacy/transactions/sales' },
        ],
      },
      {
        id: 'reports',
        label: 'Reports',
        children: [
          { label: 'Inventory', path: '/pharmacy/reports/inventory' },
          { label: 'Stock ledger', path: '/pharmacy/reports/stock-ledger' },
          { label: 'API registry (admin)', path: '/pharmacy/data-registry' },
        ],
      },
    ],
  },
  {
    label: 'Shared',
    path: '/shared',
    icon: Business,
    permission: TriVitaPermissions.SharedApi,
    groups: [
      {
        id: 'masters',
        label: 'Masters',
        children: [
          { label: 'Enterprise hierarchy', path: '/shared/hierarchy' },
          { label: 'Facilities', path: '/shared/facilities' },
        ],
      },
      {
        id: 'transactions',
        label: 'Transactions',
        children: [{ label: 'Enterprise admin (CRUD)', path: '/shared/enterprise-admin' }],
      },
      {
        id: 'reports',
        label: 'Reports',
        children: [{ label: 'API registry (admin)', path: '/shared/data-registry' }],
      },
    ],
  },
  {
    label: 'Identity',
    path: '/identity',
    icon: AdminPanelSettings,
    permission: TriVitaPermissions.IdentityAdmin,
    groups: [
      {
        id: 'masters',
        label: 'Masters',
        children: [
          { label: 'User management', path: '/identity/users' },
          { label: 'Role management', path: '/identity/roles' },
        ],
      },
      {
        id: 'transactions',
        label: 'Transactions',
        children: [],
      },
      {
        id: 'reports',
        label: 'Reports',
        children: [{ label: 'API registry (admin)', path: '/identity/data-registry' }],
      },
    ],
  },
  {
    label: 'Communication',
    path: '/communication',
    icon: ChatBubbleOutline,
    permission: TriVitaPermissions.CommunicationApi,
    groups: [
      {
        id: 'masters',
        label: 'Masters',
        children: [{ label: 'Templates & delivery logs', path: '/communication/notifications' }],
      },
      {
        id: 'transactions',
        label: 'Transactions',
        children: [],
      },
      {
        id: 'reports',
        label: 'Reports',
        children: [{ label: 'API registry (admin)', path: '/communication/data-registry' }],
      },
    ],
  },
];

const LS_KEY = 'trivita.nav.sidebar.v1';

export function flattenNavItemChildren(mod: NavItem): NavChild[] {
  return mod.groups.flatMap((g) => g.children);
}

export function navFirstPath(mod: NavItem): string {
  return flattenNavItemChildren(mod)[0]?.path ?? mod.path;
}

export function countNavWorkspaces(mod: NavItem): number {
  return flattenNavItemChildren(mod).length;
}

export function isNavPathActive(pathname: string, itemPath: string): boolean {
  if (itemPath.length <= 1) return false;
  if (pathname === itemPath) return true;
  if (pathname.startsWith(`${itemPath}/`)) return true;
  return false;
}

export function findActiveNavKeys(
  pathname: string,
  mod: NavItem
): { moduleOpen: boolean; groupId: string | null } {
  for (const g of mod.groups) {
    for (const c of g.children) {
      if (isNavPathActive(pathname, c.path)) {
        return { moduleOpen: true, groupId: g.id };
      }
    }
  }
  return { moduleOpen: false, groupId: null };
}

export function loadNavSidebarState(): { modules: string[]; groups: string[] } {
  if (typeof window === 'undefined') return { modules: [], groups: [] };
  try {
    const raw = localStorage.getItem(LS_KEY);
    if (!raw) return { modules: [], groups: [] };
    const p = JSON.parse(raw) as { modules?: string[]; groups?: string[] };
    return { modules: p.modules ?? [], groups: p.groups ?? [] };
  } catch {
    return { modules: [], groups: [] };
  }
}

export function saveNavSidebarState(modules: string[], groups: string[]) {
  if (typeof window === 'undefined') return;
  try {
    localStorage.setItem(LS_KEY, JSON.stringify({ modules, groups }));
  } catch {
    /* ignore */
  }
}

export function makeGroupKey(modulePath: string, groupId: string) {
  return `${modulePath}::${groupId}`;
}
