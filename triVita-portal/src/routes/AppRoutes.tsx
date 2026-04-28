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
import * as Lazy from '@/routes/lazyScreens';

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
        <Route path="journeys" element={<Lazy.ClinicalJourneysPage />} />
        <Route path=":module/data-registry" element={<Lazy.DataRegistryRoute />} />
        <Route
          path="hms/patients"
          element={
            <RequirePermission permission={TriVitaPermissions.HmsApi}>
              <Lazy.HmsPatientRegistrationView />
            </RequirePermission>
          }
        />
        <Route
          path="hms/appointments"
          element={
            <RequirePermission permission={TriVitaPermissions.HmsApi}>
              <Lazy.HmsAppointmentsView />
            </RequirePermission>
          }
        />
        <Route
          path="hms/visits"
          element={
            <RequirePermission permission={TriVitaPermissions.HmsApi}>
              <Lazy.HmsVisitsView />
            </RequirePermission>
          }
        />
        <Route
          path="hms/opd"
          element={
            <RequirePermission permission={TriVitaPermissions.HmsApi}>
              <Lazy.HmsOpdDashboardView />
            </RequirePermission>
          }
        />
        <Route
          path="hms/prescriptions"
          element={
            <RequirePermission permission={TriVitaPermissions.HmsApi}>
              <Lazy.HmsPrescriptionsView />
            </RequirePermission>
          }
        />
        <Route
          path="hms/ipd"
          element={
            <RequirePermission permission={TriVitaPermissions.HmsApi}>
              <Lazy.HmsIpdHubView />
            </RequirePermission>
          }
        />
        <Route
          path="hms/billing"
          element={
            <RequirePermission permission={TriVitaPermissions.HmsApi}>
              <Lazy.HmsBillingView />
            </RequirePermission>
          }
        />
        <Route
          path="hms/billing-hub"
          element={
            <RequirePermission permission={TriVitaPermissions.HmsApi}>
              <Lazy.HmsBillingHubView />
            </RequirePermission>
          }
        />
        <Route
          path="hms/masters/visit-types"
          element={
            <RequirePermission permission={TriVitaPermissions.HmsApi}>
              <Lazy.HmsMasterVisitTypeView />
            </RequirePermission>
          }
        />
        <Route
          path="hms/masters/payment-modes"
          element={
            <RequirePermission permission={TriVitaPermissions.HmsApi}>
              <Lazy.HmsMasterPaymentModeView />
            </RequirePermission>
          }
        />
        <Route
          path="lms/test-master"
          element={
            <RequirePermission permission={TriVitaPermissions.LmsApi}>
              <Lazy.LmsTestMasterView />
            </RequirePermission>
          }
        />
        <Route
          path="lms/equipment"
          element={
            <RequirePermission permission={TriVitaPermissions.LmsApi}>
              <Lazy.LmsEquipmentMasterView />
            </RequirePermission>
          }
        />
        <Route
          path="lms/equipment-mappings"
          element={
            <RequirePermission permission={TriVitaPermissions.LmsApi}>
              <Lazy.LmsEquipmentMappingsView />
            </RequirePermission>
          }
        />
        <Route
          path="lms/bookings"
          element={
            <RequirePermission permission={TriVitaPermissions.LmsApi}>
              <Lazy.LmsTestBookingView />
            </RequirePermission>
          }
        />
        <Route
          path="lms/barcodes"
          element={
            <RequirePermission permission={TriVitaPermissions.LmsApi}>
              <Lazy.LmsBarcodeView />
            </RequirePermission>
          }
        />
        <Route
          path="lms/work-queue"
          element={
            <RequirePermission permission={TriVitaPermissions.LmsApi}>
              <Lazy.LmsWorkQueueView />
            </RequirePermission>
          }
        />
        <Route
          path="lms/workflow"
          element={
            <RequirePermission permission={TriVitaPermissions.LmsApi}>
              <Lazy.LmsWorkflowDashboardView />
            </RequirePermission>
          }
        />
        <Route
          path="lms/masters/processing-stage"
          element={
            <RequirePermission permission={TriVitaPermissions.LmsApi}>
              <Lazy.LmsMasterProcessingStageView />
            </RequirePermission>
          }
        />
        <Route
          path="lms/masters/equipment-type"
          element={
            <RequirePermission permission={TriVitaPermissions.LmsApi}>
              <Lazy.LmsMasterEquipmentTypeView />
            </RequirePermission>
          }
        />
        <Route
          path="lis/analyzer"
          element={
            <RequirePermission permission={TriVitaPermissions.LisApi}>
              <Lazy.LisAnalyzerView />
            </RequirePermission>
          }
        />
        <Route
          path="lis/lab-orders"
          element={
            <RequirePermission permission={TriVitaPermissions.LisApi}>
              <Lazy.LisLabOrdersView />
            </RequirePermission>
          }
        />
        <Route
          path="lis/sample-tracking"
          element={
            <RequirePermission permission={TriVitaPermissions.LisApi}>
              <Lazy.LisSampleTrackingView />
            </RequirePermission>
          }
        />
        <Route
          path="lis/results"
          element={
            <RequirePermission permission={TriVitaPermissions.LisApi}>
              <Lazy.LisResultsView />
            </RequirePermission>
          }
        />
        <Route
          path="lis/result-history"
          element={
            <RequirePermission permission={TriVitaPermissions.LisApi}>
              <Lazy.LisResultHistoryView />
            </RequirePermission>
          }
        />
        <Route
          path="lis/verification"
          element={
            <RequirePermission permission={TriVitaPermissions.LisApi}>
              <Lazy.LisVerificationView />
            </RequirePermission>
          }
        />
        <Route
          path="lis/masters/sample-type"
          element={
            <RequirePermission permission={TriVitaPermissions.LisApi}>
              <Lazy.LisMasterSampleTypeView />
            </RequirePermission>
          }
        />
        <Route
          path="lis/masters/test-category"
          element={
            <RequirePermission permission={TriVitaPermissions.LisApi}>
              <Lazy.LisMasterTestCategoryView />
            </RequirePermission>
          }
        />
        <Route path="pharmacy/medicines" element={<Navigate to="/pharmacy/masters/medicine" replace />} />
        <Route
          path="pharmacy/masters/medicine"
          element={
            <RequirePermission permission={TriVitaPermissions.PharmacyApi}>
              <Lazy.PharmacyMedicineView />
            </RequirePermission>
          }
        />
        <Route
          path="pharmacy/masters/medicine-category"
          element={
            <RequirePermission permission={TriVitaPermissions.PharmacyApi}>
              <Lazy.PharmacyMasterMedicineCategoryView />
            </RequirePermission>
          }
        />
        <Route
          path="pharmacy/masters/manufacturer"
          element={
            <RequirePermission permission={TriVitaPermissions.PharmacyApi}>
              <Lazy.PharmacyMasterManufacturerView />
            </RequirePermission>
          }
        />
        <Route
          path="pharmacy/masters/composition"
          element={
            <RequirePermission permission={TriVitaPermissions.PharmacyApi}>
              <Lazy.PharmacyMasterCompositionView />
            </RequirePermission>
          }
        />
        <Route
          path="pharmacy/masters/unit"
          element={
            <RequirePermission permission={TriVitaPermissions.PharmacyApi}>
              <Lazy.PharmacyMasterUnitView />
            </RequirePermission>
          }
        />
        <Route
          path="pharmacy/masters/form"
          element={
            <RequirePermission permission={TriVitaPermissions.PharmacyApi}>
              <Lazy.PharmacyMasterFormView />
            </RequirePermission>
          }
        />
        <Route
          path="pharmacy/masters/supplier"
          element={
            <RequirePermission permission={TriVitaPermissions.PharmacyApi}>
              <Lazy.PharmacyMasterSupplierView />
            </RequirePermission>
          }
        />
        <Route
          path="pharmacy/masters/medicine-batches"
          element={
            <RequirePermission permission={TriVitaPermissions.PharmacyApi}>
              <Lazy.PharmacyBatchesView />
            </RequirePermission>
          }
        />
        <Route
          path="pharmacy/transactions/purchase-order"
          element={
            <RequirePermission permission={TriVitaPermissions.PharmacyApi}>
              <Lazy.PharmacyPurchaseOrdersView />
            </RequirePermission>
          }
        />
        <Route
          path="pharmacy/transactions/goods-receipt"
          element={
            <RequirePermission permission={TriVitaPermissions.PharmacyApi}>
              <Lazy.PharmacyGoodsReceiptView />
            </RequirePermission>
          }
        />
        <Route
          path="pharmacy/transactions/sales"
          element={
            <RequirePermission permission={TriVitaPermissions.PharmacyApi}>
              <Lazy.PharmacyBillingView />
            </RequirePermission>
          }
        />
        <Route
          path="pharmacy/reports/inventory"
          element={
            <RequirePermission permission={TriVitaPermissions.PharmacyApi}>
              <Lazy.PharmacyInventoryView />
            </RequirePermission>
          }
        />
        <Route
          path="pharmacy/reports/stock-ledger"
          element={
            <RequirePermission permission={TriVitaPermissions.PharmacyApi}>
              <Lazy.PharmacyStockLedgerView />
            </RequirePermission>
          }
        />
        <Route path="pharmacy/batches" element={<Navigate to="/pharmacy/masters/medicine-batches" replace />} />
        <Route path="pharmacy/inventory" element={<Navigate to="/pharmacy/reports/inventory" replace />} />
        <Route path="pharmacy/stock-ledger" element={<Navigate to="/pharmacy/reports/stock-ledger" replace />} />
        <Route path="pharmacy/billing" element={<Navigate to="/pharmacy/transactions/sales" replace />} />
        <Route path="pharmacy/goods-receipt" element={<Navigate to="/pharmacy/transactions/goods-receipt" replace />} />
        <Route path="pharmacy/purchase-orders" element={<Navigate to="/pharmacy/transactions/purchase-order" replace />} />
        <Route
          path="shared/hierarchy"
          element={
            <RequirePermission permission={TriVitaPermissions.SharedApi}>
              <Lazy.SharedHierarchyView />
            </RequirePermission>
          }
        />
        <Route
          path="shared/facilities"
          element={
            <RequirePermission permission={TriVitaPermissions.SharedApi}>
              <Lazy.SharedFacilitiesView />
            </RequirePermission>
          }
        />
        <Route
          path="shared/enterprise-admin"
          element={
            <RequirePermission permission={TriVitaPermissions.SharedApi}>
              <Lazy.SharedEnterpriseAdminView />
            </RequirePermission>
          }
        />
        <Route
          path="identity/users"
          element={
            <RequirePermission permission={TriVitaPermissions.IdentityAdmin}>
              <Lazy.IdentityUsersView />
            </RequirePermission>
          }
        />
        <Route
          path="identity/roles"
          element={
            <RequirePermission permission={TriVitaPermissions.IdentityAdmin}>
              <Lazy.IdentityRolesView />
            </RequirePermission>
          }
        />
        <Route
          path="communication/notifications"
          element={
            <RequirePermission permission={TriVitaPermissions.CommunicationApi}>
              <Lazy.CommunicationNotificationsView />
            </RequirePermission>
          }
        />
      </Route>
      <Route path="*" element={<Navigate to="/dashboard" replace />} />
    </Routes>
  );
}
