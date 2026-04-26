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
  /** When set, a sidebar subheading is rendered above this item. */
  section?: string;
}

export interface NavItem {
  label: string;
  path: string;
  icon: typeof LocalHospital;
  permission: string;
  children?: NavChild[];
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
    children: [
      { label: 'Patient registration', path: '/hms/patients' },
      { label: 'Appointments', path: '/hms/appointments' },
      { label: 'Visits (OPD)', path: '/hms/visits' },
      { label: 'OPD dashboard', path: '/hms/opd' },
      { label: 'Prescriptions', path: '/hms/prescriptions' },
      { label: 'IPD (wards / beds / admissions)', path: '/hms/ipd' },
      { label: 'Billing (line items)', path: '/hms/billing' },
      { label: 'Billing & payments (hub)', path: '/hms/billing-hub' },
      { label: 'Visit types', path: '/hms/masters/visit-types' },
      { label: 'Payment modes', path: '/hms/masters/payment-modes' },
      { label: 'API registry (admin)', path: '/hms/data-registry' },
    ],
  },
  {
    label: 'LMS',
    path: '/lms',
    icon: Science,
    permission: TriVitaPermissions.LmsApi,
    children: [
      { label: 'Test master', path: '/lms/test-master' },
      { label: 'Equipment master', path: '/lms/equipment' },
      { label: 'Equipment ↔ facility', path: '/lms/equipment-mappings' },
      { label: 'Test booking', path: '/lms/bookings' },
      { label: 'Barcode management', path: '/lms/barcodes' },
      { label: 'Work queue', path: '/lms/work-queue' },
      { label: 'Sample workflow', path: '/lms/workflow' },
      { label: 'Processing stages', path: '/lms/masters/processing-stage' },
      { label: 'Equipment types', path: '/lms/masters/equipment-type' },
      { label: 'API registry (admin)', path: '/lms/data-registry' },
    ],
  },
  {
    label: 'LIS',
    path: '/lis',
    icon: Biotech,
    permission: TriVitaPermissions.LisApi,
    children: [
      { label: 'Analyzer monitoring', path: '/lis/analyzer' },
      { label: 'Lab orders', path: '/lis/lab-orders' },
      { label: 'Sample tracking', path: '/lis/sample-tracking' },
      { label: 'Result viewer', path: '/lis/results' },
      { label: 'Result history', path: '/lis/result-history' },
      { label: 'Result verification', path: '/lis/verification' },
      { label: 'Sample types', path: '/lis/masters/sample-type' },
      { label: 'Test categories', path: '/lis/masters/test-category' },
      { label: 'API registry (admin)', path: '/lis/data-registry' },
    ],
  },
  {
    label: 'Pharmacy',
    path: '/pharmacy',
    icon: LocalPharmacy,
    permission: TriVitaPermissions.PharmacyApi,
    children: [
      { label: 'Medicine master', path: '/pharmacy/medicines', section: 'Masters' },
      { label: 'Category master', path: '/pharmacy/masters/medicine-category' },
      { label: 'Manufacturer master', path: '/pharmacy/masters/manufacturer' },
      { label: 'Composition master', path: '/pharmacy/masters/composition' },
      { label: 'Unit master', path: '/pharmacy/masters/unit' },
      { label: 'Form master', path: '/pharmacy/masters/form' },
      { label: 'Medicine batches', path: '/pharmacy/masters/medicine-batches' },
      { label: 'Purchase order', path: '/pharmacy/transactions/purchase-order', section: 'Transactions' },
      { label: 'Goods receipt (GRN)', path: '/pharmacy/transactions/goods-receipt' },
      { label: 'Sales / billing', path: '/pharmacy/transactions/sales' },
      { label: 'Inventory', path: '/pharmacy/reports/inventory', section: 'Reports' },
      { label: 'Stock ledger', path: '/pharmacy/reports/stock-ledger' },
      { label: 'API registry (admin)', path: '/pharmacy/data-registry', section: 'Admin' },
    ],
  },
  {
    label: 'Shared',
    path: '/shared',
    icon: Business,
    permission: TriVitaPermissions.SharedApi,
    children: [
      { label: 'Enterprise hierarchy', path: '/shared/hierarchy' },
      { label: 'Facilities', path: '/shared/facilities' },
      { label: 'Enterprise admin (CRUD)', path: '/shared/enterprise-admin' },
      { label: 'API registry (admin)', path: '/shared/data-registry' },
    ],
  },
  {
    label: 'Identity',
    path: '/identity',
    icon: AdminPanelSettings,
    permission: TriVitaPermissions.IdentityAdmin,
    children: [
      { label: 'User management', path: '/identity/users' },
      { label: 'Role management', path: '/identity/roles' },
      { label: 'API registry (admin)', path: '/identity/data-registry' },
    ],
  },
  {
    label: 'Communication',
    path: '/communication',
    icon: ChatBubbleOutline,
    permission: TriVitaPermissions.CommunicationApi,
    children: [
      { label: 'Templates & delivery logs', path: '/communication/notifications' },
      { label: 'API registry (admin)', path: '/communication/data-registry' },
    ],
  },
];
