import { Navigate, Route, Routes } from 'react-router-dom';
import { RequireAuth } from '@/components/auth/RequireAuth';
import { RequirePermission } from '@/components/auth/RequirePermission';
import { MainLayout } from '@/layouts/MainLayout';
import { LoginPage } from '@/pages/LoginPage';
import { PageLoader } from '@/components/common/PageLoader';
import { useAppSelector } from '@/store/hooks';
import { STORAGE_KEYS } from '@/utils/storageKeys';
import { DashboardPage } from '@/pages/DashboardPage';
import { TriVitaPermissions } from '@/utils/permissions';
import {
  HmsAppointmentsView,
  HmsBillingView,
  HmsOpdDashboardView,
  HmsPatientRegistrationView,
} from '@/modules/hms/views';
import {
  LmsBarcodeView,
  LmsEquipmentMasterView,
  LmsTestBookingView,
  LmsTestMasterView,
  LmsWorkflowDashboardView,
} from '@/modules/lms/views';
import { LisAnalyzerView, LisResultsView, LisVerificationView } from '@/modules/lis/views';
import {
  PharmacyBillingView,
  PharmacyInventoryView,
  PharmacyMedicineView,
  PharmacyPurchaseOrdersView,
} from '@/modules/pharmacy/views';
import { SharedFacilitiesView, SharedHierarchyView } from '@/modules/shared/views';
import { IdentityRolesView, IdentityUsersView } from '@/modules/identity/views';
import { CommunicationNotificationsView } from '@/modules/communication/views';

function LoginRoute() {
  const { user, hydrated } = useAppSelector((s) => s.auth);
  const token = sessionStorage.getItem(STORAGE_KEYS.accessToken);
  if (!hydrated) return <PageLoader message="Restoring session…" />;
  if (token && user) return <Navigate to="/dashboard" replace />;
  return <LoginPage />;
}

export function AppRoutes() {
  return (
    <Routes>
      <Route path="/login" element={<LoginRoute />} />
      <Route
        path="/"
        element={
          <RequireAuth>
            <MainLayout />
          </RequireAuth>
        }
      >
        <Route index element={<Navigate to="/dashboard" replace />} />
        <Route path="dashboard" element={<DashboardPage />} />
        <Route
          path="hms/patients"
          element={
            <RequirePermission permission={TriVitaPermissions.HmsApi}>
              <HmsPatientRegistrationView />
            </RequirePermission>
          }
        />
        <Route
          path="hms/appointments"
          element={
            <RequirePermission permission={TriVitaPermissions.HmsApi}>
              <HmsAppointmentsView />
            </RequirePermission>
          }
        />
        <Route
          path="hms/opd"
          element={
            <RequirePermission permission={TriVitaPermissions.HmsApi}>
              <HmsOpdDashboardView />
            </RequirePermission>
          }
        />
        <Route
          path="hms/billing"
          element={
            <RequirePermission permission={TriVitaPermissions.HmsApi}>
              <HmsBillingView />
            </RequirePermission>
          }
        />
        <Route
          path="lms/test-master"
          element={
            <RequirePermission permission={TriVitaPermissions.LmsApi}>
              <LmsTestMasterView />
            </RequirePermission>
          }
        />
        <Route
          path="lms/equipment"
          element={
            <RequirePermission permission={TriVitaPermissions.LmsApi}>
              <LmsEquipmentMasterView />
            </RequirePermission>
          }
        />
        <Route
          path="lms/bookings"
          element={
            <RequirePermission permission={TriVitaPermissions.LmsApi}>
              <LmsTestBookingView />
            </RequirePermission>
          }
        />
        <Route
          path="lms/barcodes"
          element={
            <RequirePermission permission={TriVitaPermissions.LmsApi}>
              <LmsBarcodeView />
            </RequirePermission>
          }
        />
        <Route
          path="lms/workflow"
          element={
            <RequirePermission permission={TriVitaPermissions.LmsApi}>
              <LmsWorkflowDashboardView />
            </RequirePermission>
          }
        />
        <Route
          path="lis/analyzer"
          element={
            <RequirePermission permission={TriVitaPermissions.LisApi}>
              <LisAnalyzerView />
            </RequirePermission>
          }
        />
        <Route
          path="lis/results"
          element={
            <RequirePermission permission={TriVitaPermissions.LisApi}>
              <LisResultsView />
            </RequirePermission>
          }
        />
        <Route
          path="lis/verification"
          element={
            <RequirePermission permission={TriVitaPermissions.LisApi}>
              <LisVerificationView />
            </RequirePermission>
          }
        />
        <Route
          path="pharmacy/medicines"
          element={
            <RequirePermission permission={TriVitaPermissions.PharmacyApi}>
              <PharmacyMedicineView />
            </RequirePermission>
          }
        />
        <Route
          path="pharmacy/inventory"
          element={
            <RequirePermission permission={TriVitaPermissions.PharmacyApi}>
              <PharmacyInventoryView />
            </RequirePermission>
          }
        />
        <Route
          path="pharmacy/billing"
          element={
            <RequirePermission permission={TriVitaPermissions.PharmacyApi}>
              <PharmacyBillingView />
            </RequirePermission>
          }
        />
        <Route
          path="pharmacy/purchase-orders"
          element={
            <RequirePermission permission={TriVitaPermissions.PharmacyApi}>
              <PharmacyPurchaseOrdersView />
            </RequirePermission>
          }
        />
        <Route
          path="shared/hierarchy"
          element={
            <RequirePermission permission={TriVitaPermissions.SharedApi}>
              <SharedHierarchyView />
            </RequirePermission>
          }
        />
        <Route
          path="shared/facilities"
          element={
            <RequirePermission permission={TriVitaPermissions.SharedApi}>
              <SharedFacilitiesView />
            </RequirePermission>
          }
        />
        <Route
          path="identity/users"
          element={
            <RequirePermission permission={TriVitaPermissions.IdentityAdmin}>
              <IdentityUsersView />
            </RequirePermission>
          }
        />
        <Route
          path="identity/roles"
          element={
            <RequirePermission permission={TriVitaPermissions.IdentityAdmin}>
              <IdentityRolesView />
            </RequirePermission>
          }
        />
        <Route
          path="communication/notifications"
          element={
            <RequirePermission permission={TriVitaPermissions.CommunicationApi}>
              <CommunicationNotificationsView />
            </RequirePermission>
          }
        />
      </Route>
      <Route path="*" element={<Navigate to="/dashboard" replace />} />
    </Routes>
  );
}
