import {
  AdminPanelSettings,
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

export const mainNavigation: NavItem[] = [
  {
    label: 'HMS',
    path: '/hms',
    icon: LocalHospital,
    permission: TriVitaPermissions.HmsApi,
    children: [
      { label: 'Patient registration', path: '/hms/patients' },
      { label: 'Appointments', path: '/hms/appointments' },
      { label: 'OPD dashboard', path: '/hms/opd' },
      { label: 'Billing', path: '/hms/billing' },
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
      { label: 'Test booking', path: '/lms/bookings' },
      { label: 'Barcode management', path: '/lms/barcodes' },
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
      { label: 'Result viewer', path: '/lis/results' },
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
      { label: 'Inventory', path: '/pharmacy/inventory' },
      { label: 'Billing', path: '/pharmacy/billing' },
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
    children: [
      { label: 'Notifications & templates', path: '/communication/notifications' },
    ],
  },
];
