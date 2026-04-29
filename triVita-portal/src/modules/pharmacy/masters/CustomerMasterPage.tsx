import * as Yup from 'yup';
import { Stack } from '@mui/material';
import { DetailKv } from '@/components/masters/DetailKv';
import { MasterEntityShell } from '@/components/masters/MasterEntityShell';
import { fromDatetimeLocal, toDatetimeLocal } from '@/utils/dateFields';

type Row = Record<string, unknown> & { id?: number };

function customerListSearch(row: Row, q: string): boolean {
  const name = String(row.customerName ?? '').toLowerCase();
  const mobile = String(row.mobileNumber ?? '').toLowerCase();
  const aadhaar = String(row.aadhaarNumber ?? '').toLowerCase();
  return name.includes(q) || mobile.includes(q) || aadhaar.includes(q);
}

const schema = Yup.object({
  customerName: Yup.string().trim().required().max(250),
  mobileNumber: Yup.string().trim().required().matches(/^[0-9]{10}$/, 'Mobile number must be exactly 10 digits'),
  alternatePhone: Yup.string().trim().max(15).default(''),
  email: Yup.string().trim().max(120).email('Invalid email').default(''),
  address: Yup.string().trim().max(300).default(''),
  dob: Yup.string()
    .default('')
    .test('dob-not-future', 'DOB cannot be in the future', (v) => {
      const t = (v ?? '').trim();
      if (!t) return true;
      const d = new Date(t);
      if (Number.isNaN(d.getTime())) return false;
      const today = new Date();
      today.setHours(0, 0, 0, 0);
      return d.getTime() <= today.getTime();
    }),
  aadhaarNumber: Yup.string()
    .trim()
    .default('')
    .test('aadhaar-format', 'Aadhaar number must be exactly 12 digits', (v) => {
      const t = (v ?? '').trim();
      if (!t) return true;
      return /^[0-9]{12}$/.test(t);
    }),
  gender: Yup.string().trim().default(''),
});

const defaults = {
  customerName: '',
  mobileNumber: '',
  alternatePhone: '',
  email: '',
  address: '',
  dob: '',
  aadhaarNumber: '',
  gender: '',
};

const fields = [
  { kind: 'text' as const, name: 'customerName', label: 'Customer name', required: true, gridCols: 6 },
  { kind: 'text' as const, name: 'mobileNumber', label: 'Mobile number', required: true, gridCols: 6 },
  { kind: 'text' as const, name: 'alternatePhone', label: 'Alternate phone', gridCols: 6 },
  { kind: 'text' as const, name: 'email', label: 'Email', gridCols: 6 },
  { kind: 'date' as const, name: 'dob', label: 'DOB', gridCols: 6 },
  { kind: 'text' as const, name: 'aadhaarNumber', label: 'Aadhaar number', gridCols: 6 },
  {
    kind: 'select' as const,
    name: 'gender',
    label: 'Gender',
    gridCols: 6,
    options: [
      { value: '', label: '—' },
      { value: 'Male', label: 'Male' },
      { value: 'Female', label: 'Female' },
      { value: 'Other', label: 'Other' },
    ],
  },
  { kind: 'text' as const, name: 'address', label: 'Address', gridCols: 12 },
];

export function CustomerMasterPage() {
  return (
    <MasterEntityShell<Row>
      module="pharmacy"
      resourcePath="customer"
      title="Customer Master"
      schema={schema}
      defaultCreateValues={defaults}
      fields={fields}
      getDrawerTitle={(r) => String(r.customerName ?? 'Customer')}
      getDrawerSubtitle={(r) => String(r.mobileNumber ?? '')}
      columns={[
        { id: 'customerName', label: 'Name', minWidth: 220 },
        { id: 'mobileNumber', label: 'Mobile', minWidth: 140 },
        { id: 'aadhaarNumber', label: 'Aadhaar', minWidth: 160, format: (r) => String(r.aadhaarNumber ?? '—') },
      ]}
      rowToFormValues={(r) => ({
        customerName: String(r.customerName ?? ''),
        mobileNumber: String(r.mobileNumber ?? ''),
        alternatePhone: String(r.alternatePhone ?? ''),
        email: String(r.email ?? ''),
        address: String(r.address ?? ''),
        dob: toDatetimeLocal(r.dob),
        aadhaarNumber: String(r.aadhaarNumber ?? ''),
        gender: String(r.gender ?? ''),
      })}
      toCreatePayload={(v) => ({
        customerName: v.customerName.trim(),
        mobileNumber: v.mobileNumber.trim(),
        alternatePhone: v.alternatePhone.trim() || undefined,
        email: v.email.trim() || undefined,
        address: v.address.trim() || undefined,
        dob: fromDatetimeLocal(v.dob),
        aadhaarNumber: v.aadhaarNumber.trim() || undefined,
        gender: v.gender.trim() || undefined,
        isActive: true,
      })}
      toUpdatePayload={(v) => ({
        customerName: v.customerName.trim(),
        mobileNumber: v.mobileNumber.trim(),
        alternatePhone: v.alternatePhone.trim() || undefined,
        email: v.email.trim() || undefined,
        address: v.address.trim() || undefined,
        dob: fromDatetimeLocal(v.dob),
        aadhaarNumber: v.aadhaarNumber.trim() || undefined,
        gender: v.gender.trim() || undefined,
        isActive: true,
      })}
      renderDetail={(r) => (
        <Stack spacing={1}>
          <DetailKv label="Name" value={String(r.customerName ?? '')} />
          <DetailKv label="Mobile" value={String(r.mobileNumber ?? '')} />
          <DetailKv label="Alternate phone" value={String(r.alternatePhone ?? '—')} />
          <DetailKv label="Email" value={String(r.email ?? '—')} />
          <DetailKv label="DOB" value={r.dob != null ? String(r.dob) : '—'} />
          <DetailKv label="Aadhaar" value={String(r.aadhaarNumber ?? '—')} />
          <DetailKv label="Gender" value={String(r.gender ?? '—')} />
          <DetailKv label="Address" value={String(r.address ?? '—')} />
        </Stack>
      )}
      clientListSearch={customerListSearch}
      searchFieldLabel="Search (mobile / name / aadhaar)"
    />
  );
}

