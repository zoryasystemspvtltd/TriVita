import { lazy } from 'react';

/** Route-level code splitting (NFR: performance). */
export const HmsPatientRegistrationView = lazy(() =>
  import('@/modules/hms/views').then((m) => ({ default: m.HmsPatientRegistrationView }))
);
export const HmsAppointmentsView = lazy(() =>
  import('@/modules/hms/views').then((m) => ({ default: m.HmsAppointmentsView }))
);
export const HmsOpdDashboardView = lazy(() =>
  import('@/modules/hms/views').then((m) => ({ default: m.HmsOpdDashboardView }))
);
export const HmsBillingView = lazy(() =>
  import('@/modules/hms/views').then((m) => ({ default: m.HmsBillingView }))
);
export const HmsIpdHubView = lazy(() =>
  import('@/modules/hms/extendedViews').then((m) => ({ default: m.HmsIpdHubView }))
);
export const HmsPrescriptionsView = lazy(() =>
  import('@/modules/hms/extendedViews').then((m) => ({ default: m.HmsPrescriptionsView }))
);
export const HmsVisitsView = lazy(() =>
  import('@/modules/hms/extendedViews').then((m) => ({ default: m.HmsVisitsView }))
);
export const HmsBillingHubView = lazy(() =>
  import('@/modules/hms/extendedViews').then((m) => ({ default: m.HmsBillingHubView }))
);
export const HmsMasterVisitTypeView = lazy(() =>
  import('@/modules/hms/masters/VisitTypePage').then((m) => ({ default: m.VisitTypePage }))
);
export const HmsMasterPaymentModeView = lazy(() =>
  import('@/modules/hms/masters/PaymentModePage').then((m) => ({ default: m.PaymentModePage }))
);

export const LmsTestMasterView = lazy(() =>
  import('@/modules/lms/views').then((m) => ({ default: m.LmsTestMasterView }))
);
export const LmsEquipmentMasterView = lazy(() =>
  import('@/modules/lms/views').then((m) => ({ default: m.LmsEquipmentMasterView }))
);
export const LmsTestBookingView = lazy(() =>
  import('@/modules/lms/views').then((m) => ({ default: m.LmsTestBookingView }))
);
export const LmsBarcodeView = lazy(() =>
  import('@/modules/lms/views').then((m) => ({ default: m.LmsBarcodeView }))
);
export const LmsWorkflowDashboardView = lazy(() =>
  import('@/modules/lms/views').then((m) => ({ default: m.LmsWorkflowDashboardView }))
);
export const LmsMasterProcessingStageView = lazy(() =>
  import('@/modules/lms/masters/ProcessingStagePage').then((m) => ({ default: m.ProcessingStagePage }))
);
export const LmsMasterEquipmentTypeView = lazy(() =>
  import('@/modules/lms/masters/LmsEquipmentTypePage').then((m) => ({ default: m.LmsEquipmentTypePage }))
);
export const LmsEquipmentMappingsView = lazy(() =>
  import('@/modules/lms/extendedViews').then((m) => ({ default: m.LmsEquipmentMappingsView }))
);
export const LmsWorkQueueView = lazy(() =>
  import('@/modules/lms/extendedViews').then((m) => ({ default: m.LmsWorkQueueView }))
);

export const LisAnalyzerView = lazy(() =>
  import('@/modules/lis/views').then((m) => ({ default: m.LisAnalyzerView }))
);
export const LisResultsView = lazy(() =>
  import('@/modules/lis/views').then((m) => ({ default: m.LisResultsView }))
);
export const LisVerificationView = lazy(() =>
  import('@/modules/lis/views').then((m) => ({ default: m.LisVerificationView }))
);
export const LisLabOrdersView = lazy(() =>
  import('@/modules/lis/extendedViews').then((m) => ({ default: m.LisLabOrdersView }))
);
export const LisSampleTrackingView = lazy(() =>
  import('@/modules/lis/extendedViews').then((m) => ({ default: m.LisSampleTrackingView }))
);
export const LisResultHistoryView = lazy(() =>
  import('@/modules/lis/extendedViews').then((m) => ({ default: m.LisResultHistoryView }))
);
export const LisMasterSampleTypeView = lazy(() =>
  import('@/modules/lis/masters/SampleTypePage').then((m) => ({ default: m.SampleTypePage }))
);
export const LisMasterTestCategoryView = lazy(() =>
  import('@/modules/lis/masters/TestCategoryPage').then((m) => ({ default: m.TestCategoryPage }))
);

export const PharmacyMedicineView = lazy(() =>
  import('@/modules/pharmacy/views').then((m) => ({ default: m.PharmacyMedicineView }))
);
export const PharmacyMasterMedicineCategoryView = lazy(() =>
  import('@/modules/pharmacy/masters/MedicineCategoryPage').then((m) => ({ default: m.MedicineCategoryPage }))
);
export const PharmacyMasterManufacturerView = lazy(() =>
  import('@/modules/pharmacy/masters/ManufacturerPage').then((m) => ({ default: m.ManufacturerPage }))
);
export const PharmacyMasterCompositionView = lazy(() =>
  import('@/modules/pharmacy/masters/CompositionPage').then((m) => ({ default: m.CompositionPage }))
);
export const PharmacyMasterUnitView = lazy(() =>
  import('@/modules/pharmacy/masters/PharmacyUnitMasterPage').then((m) => ({ default: m.PharmacyUnitMasterPage }))
);
export const PharmacyMasterFormView = lazy(() =>
  import('@/modules/pharmacy/masters/PharmacyFormMasterPage').then((m) => ({ default: m.PharmacyFormMasterPage }))
);
export const PharmacyInventoryView = lazy(() =>
  import('@/modules/pharmacy/views').then((m) => ({ default: m.PharmacyInventoryView }))
);
export const PharmacyBillingView = lazy(() =>
  import('@/modules/pharmacy/views').then((m) => ({ default: m.PharmacyBillingView }))
);
export const PharmacyPurchaseOrdersView = lazy(() =>
  import('@/modules/pharmacy/views').then((m) => ({ default: m.PharmacyPurchaseOrdersView }))
);
export const PharmacyBatchesView = lazy(() =>
  import('@/modules/pharmacy/extendedViews').then((m) => ({ default: m.PharmacyBatchesView }))
);
export const PharmacyGoodsReceiptView = lazy(() =>
  import('@/modules/pharmacy/extendedViews').then((m) => ({ default: m.PharmacyGoodsReceiptView }))
);
export const PharmacyStockLedgerView = lazy(() =>
  import('@/modules/pharmacy/extendedViews').then((m) => ({ default: m.PharmacyStockLedgerView }))
);

export const SharedHierarchyView = lazy(() =>
  import('@/modules/shared/views').then((m) => ({ default: m.SharedHierarchyView }))
);
export const SharedFacilitiesView = lazy(() =>
  import('@/modules/shared/views').then((m) => ({ default: m.SharedFacilitiesView }))
);
export const SharedEnterpriseAdminView = lazy(() =>
  import('@/modules/shared/extendedViews').then((m) => ({ default: m.SharedEnterpriseAdminView }))
);
export const IdentityUsersView = lazy(() =>
  import('@/modules/identity/views').then((m) => ({ default: m.IdentityUsersView }))
);
export const IdentityRolesView = lazy(() =>
  import('@/modules/identity/views').then((m) => ({ default: m.IdentityRolesView }))
);
export const CommunicationNotificationsView = lazy(() =>
  import('@/modules/communication/views').then((m) => ({ default: m.CommunicationNotificationsView }))
);

export const ClinicalJourneysPage = lazy(() => import('@/pages/ClinicalJourneysPage'));
export const DataRegistryRoute = lazy(() =>
  import('@/routes/DataRegistryRoute').then((m) => ({ default: m.DataRegistryRoute }))
);
