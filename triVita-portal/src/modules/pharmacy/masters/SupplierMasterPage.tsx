import * as Yup from 'yup';
import { Stack } from '@mui/material';
import { DetailKv } from '@/components/masters/DetailKv';
import { MasterEntityShell } from '@/components/masters/MasterEntityShell';

type Row = Record<string, unknown> & { id?: number };

function supplierListSearch(row: Row, q: string): boolean {
  const name = String(row.supplierName ?? '').toLowerCase();
  const code = String(row.supplierCode ?? '').toLowerCase();
  const pan = String(row.pan ?? '').toLowerCase();
  const phone = String(row.phone ?? '').toLowerCase();
  const email = String(row.email ?? '').toLowerCase();
  const contact = String(row.contactPerson ?? '').toLowerCase();
  return name.includes(q) || code.includes(q) || pan.includes(q) || phone.includes(q) || email.includes(q) || contact.includes(q);
}

const schema = Yup.object({
  supplierName: Yup.string().trim().required().max(250),
  supplierCode: Yup.string().trim().required().max(80),
  pan: Yup.string()
    .trim()
    .required('PAN is required')
    .length(10, 'PAN must be 10 characters')
    .matches(/^[A-Za-z]{5}[0-9]{4}[A-Za-z]{1}$/, 'Invalid PAN format'),
  gstNo: Yup.string()
    .trim()
    .default('')
    .test('gst-len', 'GST No. must be 15 characters', (v) => !v || v.trim().length === 15),
  tan: Yup.string().trim().max(20).default(''),
  msme: Yup.string().trim().max(50).default(''),
  exportImportCode: Yup.string().trim().max(30).default(''),
  cin: Yup.string().trim().max(30).default(''),
  contactPerson: Yup.string().trim().max(120).default(''),
  phone: Yup.string().trim().max(40).default(''),
  email: Yup.string().trim().max(120).default(''),
  address: Yup.string().trim().max(300).default(''),
  description: Yup.string().trim().max(500).default(''),
});

const defaults = {
  supplierName: '',
  supplierCode: '',
  pan: '',
  gstNo: '',
  tan: '',
  msme: '',
  exportImportCode: '',
  cin: '',
  contactPerson: '',
  phone: '',
  email: '',
  address: '',
  description: '',
};

const fields = [
  { kind: 'section' as const, name: 'basicInfo', label: 'Basic info' },
  { kind: 'text' as const, name: 'supplierName', label: 'Supplier name', required: true, gridCols: 6 },
  { kind: 'text' as const, name: 'supplierCode', label: 'Supplier code', required: true, gridCols: 6 },
  { kind: 'text' as const, name: 'contactPerson', label: 'Contact person', gridCols: 6 },
  { kind: 'text' as const, name: 'phone', label: 'Phone', gridCols: 6 },
  { kind: 'text' as const, name: 'email', label: 'Email', gridCols: 6 },
  { kind: 'text' as const, name: 'address', label: 'Address', gridCols: 6 },
  { kind: 'textarea' as const, name: 'description', label: 'Description', gridCols: 12, minRows: 3 },
  { kind: 'section' as const, name: 'complianceInfo', label: 'Compliance info' },
  { kind: 'text' as const, name: 'pan', label: 'PAN', required: true, gridCols: 6 },
  { kind: 'text' as const, name: 'gstNo', label: 'GST No.', gridCols: 6 },
  { kind: 'text' as const, name: 'tan', label: 'TAN', gridCols: 6 },
  { kind: 'text' as const, name: 'msme', label: 'MSME', gridCols: 6 },
  { kind: 'text' as const, name: 'exportImportCode', label: 'Export/Import code (IEC)', gridCols: 6 },
  { kind: 'text' as const, name: 'cin', label: 'CIN', gridCols: 6 },
];

export function SupplierMasterPage() {
  return (
    <MasterEntityShell<Row>
      module="pharmacy"
      resourcePath="supplier"
      title="Supplier Master"
      schema={schema}
      defaultCreateValues={defaults}
      fields={fields}
      getDrawerTitle={(r) => String(r.supplierName ?? 'Supplier')}
      getDrawerSubtitle={(r) => String(r.supplierCode ?? '')}
      columns={[
        { id: 'supplierName', label: 'Name', minWidth: 220 },
        { id: 'supplierCode', label: 'Code', minWidth: 140 },
        { id: 'pan', label: 'PAN', minWidth: 140, format: (r) => String(r.pan ?? '—') },
        { id: 'gstNo', label: 'GST', minWidth: 160, format: (r) => String(r.gstNo ?? '—') },
        { id: 'contactPerson', label: 'Contact', minWidth: 160, format: (r) => String(r.contactPerson ?? '—') },
        { id: 'phone', label: 'Phone', minWidth: 140, format: (r) => String(r.phone ?? '—') },
      ]}
      rowToFormValues={(r) => ({
        supplierName: String(r.supplierName ?? ''),
        supplierCode: String(r.supplierCode ?? ''),
        pan: String(r.pan ?? ''),
        gstNo: String(r.gstNo ?? ''),
        tan: String(r.tan ?? ''),
        msme: String(r.msme ?? ''),
        exportImportCode: String(r.exportImportCode ?? ''),
        cin: String(r.cin ?? ''),
        contactPerson: String(r.contactPerson ?? ''),
        phone: String(r.phone ?? ''),
        email: String(r.email ?? ''),
        address: String(r.address ?? ''),
        description: String(r.description ?? ''),
      })}
      toCreatePayload={(v) => ({
        supplierName: v.supplierName.trim(),
        supplierCode: v.supplierCode.trim(),
        pan: v.pan.trim().toUpperCase(),
        gstNo: v.gstNo.trim() || undefined,
        tan: v.tan.trim() || undefined,
        msme: v.msme.trim() || undefined,
        exportImportCode: v.exportImportCode.trim() || undefined,
        cin: v.cin.trim() || undefined,
        contactPerson: v.contactPerson.trim() || undefined,
        phone: v.phone.trim() || undefined,
        email: v.email.trim() || undefined,
        address: v.address.trim() || undefined,
        description: v.description.trim() || undefined,
        isActive: true,
      })}
      toUpdatePayload={(v) => ({
        supplierName: v.supplierName.trim(),
        supplierCode: v.supplierCode.trim(),
        pan: v.pan.trim().toUpperCase(),
        gstNo: v.gstNo.trim() || undefined,
        tan: v.tan.trim() || undefined,
        msme: v.msme.trim() || undefined,
        exportImportCode: v.exportImportCode.trim() || undefined,
        cin: v.cin.trim() || undefined,
        contactPerson: v.contactPerson.trim() || undefined,
        phone: v.phone.trim() || undefined,
        email: v.email.trim() || undefined,
        address: v.address.trim() || undefined,
        description: v.description.trim() || undefined,
        isActive: true,
      })}
      renderDetail={(r) => (
        <Stack spacing={1}>
          <DetailKv label="Name" value={String(r.supplierName ?? '')} />
          <DetailKv label="Code" value={String(r.supplierCode ?? '')} />
          <DetailKv label="PAN" value={String(r.pan ?? '—')} />
          <DetailKv label="GST No." value={String(r.gstNo ?? '—')} />
          <DetailKv label="TAN" value={String(r.tan ?? '—')} />
          <DetailKv label="MSME" value={String(r.msme ?? '—')} />
          <DetailKv label="Export/Import code (IEC)" value={String(r.exportImportCode ?? '—')} />
          <DetailKv label="CIN" value={String(r.cin ?? '—')} />
          <DetailKv label="Contact person" value={String(r.contactPerson ?? '—')} />
          <DetailKv label="Phone" value={String(r.phone ?? '—')} />
          <DetailKv label="Email" value={String(r.email ?? '—')} />
          <DetailKv label="Address" value={String(r.address ?? '—')} />
          <DetailKv label="Description" value={String(r.description ?? '—')} />
        </Stack>
      )}
      clientListSearch={supplierListSearch}
      searchFieldLabel="Search (name / code / PAN / phone / email)"
    />
  );
}

