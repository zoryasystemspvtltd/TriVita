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

export interface NavItem {
  label: string;
  path: string;
  icon: typeof LocalHospital;
  permission: string;
  children?: { label: string; path: string }[];
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
    ],
  },
  {
    label: 'Pharmacy',
    path: '/pharmacy',
    icon: LocalPharmacy,
    permission: TriVitaPermissions.PharmacyApi,
    children: [
      { label: 'Medicine master', path: '/pharmacy/medicines' },
      { label: 'Medicine batches', path: '/pharmacy/batches' },
      { label: 'Inventory', path: '/pharmacy/inventory' },
      { label: 'Stock ledger', path: '/pharmacy/stock-ledger' },
      { label: 'Billing (sales)', path: '/pharmacy/billing' },
      { label: 'Goods receipt', path: '/pharmacy/goods-receipt' },
      { label: 'Purchase orders', path: '/pharmacy/purchase-orders' },
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
    ],
  },
  {
    label: 'Communication',
    path: '/communication',
    icon: ChatBubbleOutline,
    permission: TriVitaPermissions.CommunicationApi,
    children: [{ label: 'Templates & delivery logs', path: '/communication/notifications' }],
  },
];
