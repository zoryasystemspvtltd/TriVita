import { Stack } from '@mui/material';
import { useQuery } from '@tanstack/react-query';
import { useState } from 'react';
import { DataTable } from '@/components/common/DataTable';
import { PageHeader } from '@/components/layout/PageHeader';
import {
  getInventoryLocationsPaged,
  getMedicinePaged,
  getPharmacyInfo,
  getPharmacySalesPaged,
  getPurchaseOrdersPaged,
} from '@/services/pharmacyService';

export function PharmacyMedicineView() {
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const q = useQuery({
    queryKey: ['pharmacy', 'medicine', page, pageSize],
    queryFn: () => getMedicinePaged({ page: page + 1, pageSize }),
  });
  const rows =
    q.data?.success && q.data.data
      ? (q.data.data.items as Record<string, unknown>[])
      : [];
  const total = q.data?.success && q.data.data ? q.data.data.totalCount : 0;

  return (
    <Stack spacing={2}>
      <PageHeader title="Medicine master" subtitle="/api/v1/medicine" />
      <DataTable
        columns={[
          { id: 'id', label: 'ID' },
          { id: 'medicineName', label: 'Name' },
          { id: 'medicineCode', label: 'Code' },
        ]}
        rows={rows}
        rowKey={(r) => String(r.id ?? Math.random())}
        totalCount={total}
        page={page}
        pageSize={pageSize}
        onPageChange={(p, ps) => {
          setPage(p);
          setPageSize(ps);
        }}
        loading={q.isLoading}
      />
    </Stack>
  );
}

export function PharmacyInventoryView() {
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const q = useQuery({
    queryKey: ['pharmacy', 'inventory-loc', page, pageSize],
    queryFn: () => getInventoryLocationsPaged({ page: page + 1, pageSize }),
  });
  const rows =
    q.data?.success && q.data.data
      ? (q.data.data.items as Record<string, unknown>[])
      : [];
  const total = q.data?.success && q.data.data ? q.data.data.totalCount : 0;

  return (
    <Stack spacing={2}>
      <PageHeader title="Inventory dashboard" subtitle="/api/v1/inventory-locations" />
      <DataTable
        columns={[
          { id: 'id', label: 'ID' },
          { id: 'locationCode', label: 'Code' },
          { id: 'locationName', label: 'Name' },
        ]}
        rows={rows}
        rowKey={(r) => String(r.id ?? Math.random())}
        totalCount={total}
        page={page}
        pageSize={pageSize}
        onPageChange={(p, ps) => {
          setPage(p);
          setPageSize(ps);
        }}
        loading={q.isLoading}
      />
    </Stack>
  );
}

export function PharmacyBillingView() {
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const q = useQuery({
    queryKey: ['pharmacy', 'sales', page, pageSize],
    queryFn: () => getPharmacySalesPaged({ page: page + 1, pageSize }),
  });
  const rows =
    q.data?.success && q.data.data
      ? (q.data.data.items as Record<string, unknown>[])
      : [];
  const total = q.data?.success && q.data.data ? q.data.data.totalCount : 0;

  return (
    <Stack spacing={2}>
      <PageHeader title="Pharmacy billing" subtitle="Sales headers from /api/v1/pharmacy-sale." />
      <DataTable
        columns={[
          { id: 'id', label: 'ID' },
          { id: 'saleNumber', label: 'Sale #' },
          { id: 'patientId', label: 'Patient' },
        ]}
        rows={rows}
        rowKey={(r) => String(r.id ?? Math.random())}
        totalCount={total}
        page={page}
        pageSize={pageSize}
        onPageChange={(p, ps) => {
          setPage(p);
          setPageSize(ps);
        }}
        loading={q.isLoading}
      />
    </Stack>
  );
}

export function PharmacyPurchaseOrdersView() {
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const q = useQuery({
    queryKey: ['pharmacy', 'po', page, pageSize],
    queryFn: () => getPurchaseOrdersPaged({ page: page + 1, pageSize }),
  });
  const info = useQuery({
    queryKey: ['pharmacy', 'info'],
    queryFn: () => getPharmacyInfo(),
  });
  const rows =
    q.data?.success && q.data.data
      ? (q.data.data.items as Record<string, unknown>[])
      : [];
  const total = q.data?.success && q.data.data ? q.data.data.totalCount : 0;

  return (
    <Stack spacing={2}>
      <PageHeader
        title="Purchase orders"
        subtitle={info.data?.success ? JSON.stringify(info.data.data) : '/api/v1/purchase-order'}
      />
      <DataTable
        columns={[
          { id: 'id', label: 'ID' },
          { id: 'poNumber', label: 'PO #' },
          { id: 'supplierId', label: 'Supplier' },
        ]}
        rows={rows}
        rowKey={(r) => String(r.id ?? Math.random())}
        totalCount={total}
        page={page}
        pageSize={pageSize}
        onPageChange={(p, ps) => {
          setPage(p);
          setPageSize(ps);
        }}
        loading={q.isLoading}
      />
    </Stack>
  );
}
