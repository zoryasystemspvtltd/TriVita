import {
  Alert,
  Box,
  Button,
  Checkbox,
  FormControlLabel,
  Stack,
  Tab,
  Tabs,
  TextField,
  Typography,
} from '@mui/material';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useCallback, useMemo, useState } from 'react';
import { AppModal } from '@/components/common/AppModal';
import { DataTable } from '@/components/common/DataTable';
import { PageHeader } from '@/components/layout/PageHeader';
import type { BaseResponse } from '@/types/api';
import {
  createBusinessUnit,
  createCompany,
  createDepartment,
  createEnterprise,
  createFacility,
  deleteBusinessUnit,
  deleteCompany,
  deleteDepartment,
  deleteEnterprise,
  deleteFacility,
  listBusinessUnits,
  listCompanies,
  listDepartments,
  listEnterprises,
  listFacilities,
  updateBusinessUnit,
  updateCompany,
  updateDepartment,
  updateEnterprise,
  updateFacility,
} from '@/services/sharedService';

type Banner = { severity: 'success' | 'error'; text: string };

function parseOptLong(raw: string): number | undefined {
  const t = raw.trim();
  if (!t) return undefined;
  const n = Number(t);
  return Number.isFinite(n) ? n : undefined;
}

function parseOptDate(raw: string): string | undefined {
  const t = raw.trim();
  if (!t) return undefined;
  return new Date(t).toISOString();
}

function usePagedRows<T extends object>(rows: readonly T[], pageSize = 20) {
  const [page, setPage] = useState(0);
  const [size, setSize] = useState(pageSize);
  const slice = useMemo(() => rows.slice(page * size, page * size + size), [rows, page, size]);
  return { page, size, slice, setPage, setSize, total: rows.length };
}

function EnterprisesCrud({ setBanner }: { setBanner: (b: Banner | null) => void }) {
  const qc = useQueryClient();
  const q = useQuery({
    queryKey: ['shared', 'enterprises'],
    queryFn: () => listEnterprises(),
  });
  const rows =
    q.data?.success && q.data.data ? ([...q.data.data] as Record<string, unknown>[]) : [];
  const pg = usePagedRows(rows);
  const [open, setOpen] = useState(false);
  const [editId, setEditId] = useState<number | null>(null);
  const [code, setCode] = useState('');
  const [name, setName] = useState('');
  const [reg, setReg] = useState('');
  const [effFrom, setEffFrom] = useState('');
  const [effTo, setEffTo] = useState('');
  const [addr, setAddr] = useState('');
  const [contact, setContact] = useState('');
  const [active, setActive] = useState(true);

  const reset = () => {
    setEditId(null);
    setCode('');
    setName('');
    setReg('');
    setEffFrom('');
    setEffTo('');
    setAddr('');
    setContact('');
    setActive(true);
  };

  const openCreate = () => {
    reset();
    setOpen(true);
  };

  const openEdit = (r: Record<string, unknown>) => {
    setEditId(Number(r.id));
    setCode(String(r.enterpriseCode ?? ''));
    setName(String(r.enterpriseName ?? ''));
    setReg(String(r.registrationDetails ?? ''));
    setEffFrom(r.effectiveFrom ? String(r.effectiveFrom).slice(0, 10) : '');
    setEffTo(r.effectiveTo ? String(r.effectiveTo).slice(0, 10) : '');
    setAddr(r.primaryAddressId != null ? String(r.primaryAddressId) : '');
    setContact(r.primaryContactId != null ? String(r.primaryContactId) : '');
    setActive(Boolean(r.isActive));
    setOpen(true);
  };

  const bodyBase = () => ({
    enterpriseCode: code.trim(),
    enterpriseName: name.trim(),
    registrationDetails: reg.trim() || undefined,
    globalSettingsJson: undefined,
    primaryAddressId: parseOptLong(addr),
    primaryContactId: parseOptLong(contact),
    effectiveFrom: parseOptDate(effFrom),
    effectiveTo: parseOptDate(effTo),
  });

  const saveMut = useMutation({
    mutationFn: async () => {
      if (!code.trim() || !name.trim()) throw new Error('Code and name are required.');
      if (editId == null) {
        return createEnterprise(bodyBase());
      }
      return updateEnterprise(editId, { ...bodyBase(), isActive: active });
    },
    onSuccess: (res) => {
      if (!res.success) {
        setBanner({ severity: 'error', text: res.message ?? 'Save failed' });
        return;
      }
      setBanner({ severity: 'success', text: editId ? 'Enterprise updated.' : 'Enterprise created.' });
      setOpen(false);
      void qc.invalidateQueries({ queryKey: ['shared', 'enterprises'] });
    },
    onError: (e: Error) => setBanner({ severity: 'error', text: e.message }),
  });

  const delMut = useMutation({
    mutationFn: (id: number) => deleteEnterprise(id),
    onSuccess: (res) => {
      if (!res.success) {
        setBanner({ severity: 'error', text: res.message ?? 'Delete failed' });
        return;
      }
      setBanner({ severity: 'success', text: 'Enterprise deleted.' });
      void qc.invalidateQueries({ queryKey: ['shared', 'enterprises'] });
    },
    onError: (e: Error) => setBanner({ severity: 'error', text: e.message }),
  });

  const onDelete = (r: Record<string, unknown>) => {
    if (!window.confirm(`Soft-delete enterprise ${r.enterpriseCode}?`)) return;
    delMut.mutate(Number(r.id));
  };

  return (
    <Stack spacing={2}>
      <Stack direction="row" justifyContent="space-between" alignItems="center" flexWrap="wrap" gap={1}>
        <Typography variant="body2" color="text.secondary">
          GET/POST/PUT/DELETE /api/v1/enterprises
        </Typography>
        <Button variant="contained" onClick={openCreate}>
          Add enterprise
        </Button>
      </Stack>
      <DataTable
        columns={[
          { id: 'id', label: 'ID' },
          { id: 'enterpriseCode', label: 'Code' },
          { id: 'enterpriseName', label: 'Name' },
          { id: 'isActive', label: 'Active', format: (row) => (row.isActive ? 'Yes' : 'No') },
          {
            id: '_a',
            label: '',
            format: (row) => (
              <Stack direction="row" spacing={1}>
                <Button size="small" variant="outlined" onClick={() => openEdit(row)}>
                  Edit
                </Button>
                <Button size="small" color="error" variant="outlined" onClick={() => onDelete(row)}>
                  Delete
                </Button>
              </Stack>
            ),
          },
        ]}
        rows={pg.slice as Record<string, unknown>[]}
        rowKey={(r) => String(r.id)}
        totalCount={pg.total}
        page={pg.page}
        pageSize={pg.size}
        onPageChange={(p, ps) => {
          pg.setPage(p);
          pg.setSize(ps);
        }}
        loading={q.isLoading}
        tableAriaLabel="Enterprises"
      />
      <AppModal
        open={open}
        onClose={() => setOpen(false)}
        title={editId == null ? 'Create enterprise' : 'Edit enterprise'}
        maxWidth="md"
        actions={
          <>
            <Button onClick={() => setOpen(false)}>Cancel</Button>
            <Button variant="contained" disabled={saveMut.isPending} onClick={() => saveMut.mutate()}>
              Save
            </Button>
          </>
        }
      >
        <Stack spacing={2} sx={{ pt: 1 }}>
          <TextField label="Enterprise code" required value={code} onChange={(e) => setCode(e.target.value)} />
          <TextField label="Enterprise name" required value={name} onChange={(e) => setName(e.target.value)} />
          <TextField label="Registration details" multiline minRows={2} value={reg} onChange={(e) => setReg(e.target.value)} />
          <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
            <TextField
              label="Effective from"
              type="date"
              InputLabelProps={{ shrink: true }}
              value={effFrom}
              onChange={(e) => setEffFrom(e.target.value)}
              fullWidth
            />
            <TextField
              label="Effective to"
              type="date"
              InputLabelProps={{ shrink: true }}
              value={effTo}
              onChange={(e) => setEffTo(e.target.value)}
              fullWidth
            />
          </Stack>
          <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
            <TextField label="Primary address ID" value={addr} onChange={(e) => setAddr(e.target.value)} fullWidth />
            <TextField label="Primary contact ID" value={contact} onChange={(e) => setContact(e.target.value)} fullWidth />
          </Stack>
          {editId != null ? (
            <FormControlLabel
              control={<Checkbox checked={active} onChange={(e) => setActive(e.target.checked)} />}
              label="Active"
            />
          ) : null}
        </Stack>
      </AppModal>
    </Stack>
  );
}

function CompaniesCrud({ setBanner }: { setBanner: (b: Banner | null) => void }) {
  const qc = useQueryClient();
  const [filterEnt, setFilterEnt] = useState('');
  const enterpriseId = parseOptLong(filterEnt);
  const q = useQuery({
    queryKey: ['shared', 'companies', enterpriseId ?? 'all'],
    queryFn: () => listCompanies(enterpriseId),
  });
  const rows =
    q.data?.success && q.data.data ? ([...q.data.data] as Record<string, unknown>[]) : [];
  const pg = usePagedRows(rows);
  const [open, setOpen] = useState(false);
  const [editId, setEditId] = useState<number | null>(null);
  const [entId, setEntId] = useState('');
  const [code, setCode] = useState('');
  const [name, setName] = useState('');
  const [pan, setPan] = useState('');
  const [gstin, setGstin] = useState('');
  const [effFrom, setEffFrom] = useState('');
  const [effTo, setEffTo] = useState('');
  const [addr, setAddr] = useState('');
  const [contact, setContact] = useState('');
  const [active, setActive] = useState(true);

  const reset = () => {
    setEditId(null);
    setEntId('');
    setCode('');
    setName('');
    setPan('');
    setGstin('');
    setEffFrom('');
    setEffTo('');
    setAddr('');
    setContact('');
    setActive(true);
  };

  const openCreate = () => {
    reset();
    if (filterEnt.trim()) setEntId(filterEnt.trim());
    setOpen(true);
  };

  const openEdit = (r: Record<string, unknown>) => {
    setEditId(Number(r.id));
    setEntId(String(r.enterpriseId ?? ''));
    setCode(String(r.companyCode ?? ''));
    setName(String(r.companyName ?? ''));
    setPan(String(r.pan ?? ''));
    setGstin(String(r.gstin ?? ''));
    setEffFrom(r.effectiveFrom ? String(r.effectiveFrom).slice(0, 10) : '');
    setEffTo(r.effectiveTo ? String(r.effectiveTo).slice(0, 10) : '');
    setAddr(r.primaryAddressId != null ? String(r.primaryAddressId) : '');
    setContact(r.primaryContactId != null ? String(r.primaryContactId) : '');
    setActive(Boolean(r.isActive));
    setOpen(true);
  };

  const bodyBase = () => ({
    enterpriseId: Number(entId.trim()),
    companyCode: code.trim(),
    companyName: name.trim(),
    pan: pan.trim() || undefined,
    gstin: gstin.trim() || undefined,
    legalIdentifier1: undefined,
    legalIdentifier2: undefined,
    primaryAddressId: parseOptLong(addr),
    primaryContactId: parseOptLong(contact),
    effectiveFrom: parseOptDate(effFrom),
    effectiveTo: parseOptDate(effTo),
  });

  const saveMut = useMutation({
    mutationFn: async () => {
      if (!entId.trim() || !code.trim() || !name.trim()) throw new Error('Enterprise ID, code, and name are required.');
      if (editId == null) return createCompany(bodyBase());
      return updateCompany(editId, { ...bodyBase(), isActive: active });
    },
    onSuccess: (res: BaseResponse<unknown>) => {
      if (!res.success) {
        setBanner({ severity: 'error', text: res.message ?? 'Save failed' });
        return;
      }
      setBanner({ severity: 'success', text: editId ? 'Company updated.' : 'Company created.' });
      setOpen(false);
      void qc.invalidateQueries({ queryKey: ['shared', 'companies'] });
    },
    onError: (e: Error) => setBanner({ severity: 'error', text: e.message }),
  });

  const delMut = useMutation({
    mutationFn: (id: number) => deleteCompany(id),
    onSuccess: (res) => {
      if (!res.success) {
        setBanner({ severity: 'error', text: res.message ?? 'Delete failed' });
        return;
      }
      setBanner({ severity: 'success', text: 'Company deleted.' });
      void qc.invalidateQueries({ queryKey: ['shared', 'companies'] });
    },
    onError: (e: Error) => setBanner({ severity: 'error', text: e.message }),
  });

  return (
    <Stack spacing={2}>
      <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} alignItems={{ sm: 'center' }}>
        <TextField
          label="Filter by enterprise ID"
          size="small"
          value={filterEnt}
          onChange={(e) => setFilterEnt(e.target.value)}
          sx={{ maxWidth: 240 }}
        />
        <Button variant="outlined" onClick={() => void q.refetch()}>
          Refresh
        </Button>
        <Box flex={1} />
        <Button variant="contained" onClick={openCreate}>
          Add company
        </Button>
      </Stack>
      <Typography variant="body2" color="text.secondary">
        GET/POST/PUT/DELETE /api/v1/companies
      </Typography>
      <DataTable
        columns={[
          { id: 'id', label: 'ID' },
          { id: 'enterpriseId', label: 'Ent.' },
          { id: 'companyCode', label: 'Code' },
          { id: 'companyName', label: 'Name' },
          {
            id: '_a',
            label: '',
            format: (row) => (
              <Stack direction="row" spacing={1}>
                <Button size="small" variant="outlined" onClick={() => openEdit(row)}>
                  Edit
                </Button>
                <Button size="small" color="error" variant="outlined" onClick={() => {
                  if (window.confirm(`Delete company ${row.companyCode}?`)) delMut.mutate(Number(row.id));
                }}>
                  Delete
                </Button>
              </Stack>
            ),
          },
        ]}
        rows={pg.slice as Record<string, unknown>[]}
        rowKey={(r) => String(r.id)}
        totalCount={pg.total}
        page={pg.page}
        pageSize={pg.size}
        onPageChange={(p, ps) => {
          pg.setPage(p);
          pg.setSize(ps);
        }}
        loading={q.isLoading}
        tableAriaLabel="Companies"
      />
      <AppModal
        open={open}
        onClose={() => setOpen(false)}
        title={editId == null ? 'Create company' : 'Edit company'}
        maxWidth="md"
        actions={
          <>
            <Button onClick={() => setOpen(false)}>Cancel</Button>
            <Button variant="contained" disabled={saveMut.isPending} onClick={() => saveMut.mutate()}>
              Save
            </Button>
          </>
        }
      >
        <Stack spacing={2} sx={{ pt: 1 }}>
          <TextField label="Enterprise ID" required value={entId} onChange={(e) => setEntId(e.target.value)} />
          <TextField label="Company code" required value={code} onChange={(e) => setCode(e.target.value)} />
          <TextField label="Company name" required value={name} onChange={(e) => setName(e.target.value)} />
          <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
            <TextField label="PAN" value={pan} onChange={(e) => setPan(e.target.value)} fullWidth />
            <TextField label="GSTIN" value={gstin} onChange={(e) => setGstin(e.target.value)} fullWidth />
          </Stack>
          <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
            <TextField label="Effective from" type="date" InputLabelProps={{ shrink: true }} value={effFrom} onChange={(e) => setEffFrom(e.target.value)} fullWidth />
            <TextField label="Effective to" type="date" InputLabelProps={{ shrink: true }} value={effTo} onChange={(e) => setEffTo(e.target.value)} fullWidth />
          </Stack>
          <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
            <TextField label="Primary address ID" value={addr} onChange={(e) => setAddr(e.target.value)} fullWidth />
            <TextField label="Primary contact ID" value={contact} onChange={(e) => setContact(e.target.value)} fullWidth />
          </Stack>
          {editId != null ? (
            <FormControlLabel control={<Checkbox checked={active} onChange={(e) => setActive(e.target.checked)} />} label="Active" />
          ) : null}
        </Stack>
      </AppModal>
    </Stack>
  );
}

function BusinessUnitsCrud({ setBanner }: { setBanner: (b: Banner | null) => void }) {
  const qc = useQueryClient();
  const [filterCo, setFilterCo] = useState('');
  const companyId = parseOptLong(filterCo);
  const q = useQuery({
    queryKey: ['shared', 'business-units', companyId ?? 'all'],
    queryFn: () => listBusinessUnits(companyId),
  });
  const rows =
    q.data?.success && q.data.data ? ([...q.data.data] as Record<string, unknown>[]) : [];
  const pg = usePagedRows(rows);
  const [open, setOpen] = useState(false);
  const [editId, setEditId] = useState<number | null>(null);
  const [coId, setCoId] = useState('');
  const [code, setCode] = useState('');
  const [name, setName] = useState('');
  const [buType, setBuType] = useState('Hospital');
  const [region, setRegion] = useState('');
  const [country, setCountry] = useState('');
  const [effFrom, setEffFrom] = useState('');
  const [effTo, setEffTo] = useState('');
  const [addr, setAddr] = useState('');
  const [contact, setContact] = useState('');
  const [active, setActive] = useState(true);

  const reset = () => {
    setEditId(null);
    setCoId('');
    setCode('');
    setName('');
    setBuType('Hospital');
    setRegion('');
    setCountry('');
    setEffFrom('');
    setEffTo('');
    setAddr('');
    setContact('');
    setActive(true);
  };

  const openCreate = () => {
    reset();
    if (filterCo.trim()) setCoId(filterCo.trim());
    setOpen(true);
  };

  const openEdit = (r: Record<string, unknown>) => {
    setEditId(Number(r.id));
    setCoId(String(r.companyId ?? ''));
    setCode(String(r.businessUnitCode ?? ''));
    setName(String(r.businessUnitName ?? ''));
    setBuType(String(r.businessUnitType ?? 'Hospital'));
    setRegion(String(r.regionCode ?? ''));
    setCountry(String(r.countryCode ?? ''));
    setEffFrom(r.effectiveFrom ? String(r.effectiveFrom).slice(0, 10) : '');
    setEffTo(r.effectiveTo ? String(r.effectiveTo).slice(0, 10) : '');
    setAddr(r.primaryAddressId != null ? String(r.primaryAddressId) : '');
    setContact(r.primaryContactId != null ? String(r.primaryContactId) : '');
    setActive(Boolean(r.isActive));
    setOpen(true);
  };

  const bodyBase = () => ({
    companyId: Number(coId.trim()),
    businessUnitCode: code.trim(),
    businessUnitName: name.trim(),
    businessUnitType: buType.trim() || 'Hospital',
    regionCode: region.trim() || undefined,
    countryCode: country.trim() || undefined,
    primaryAddressId: parseOptLong(addr),
    primaryContactId: parseOptLong(contact),
    effectiveFrom: parseOptDate(effFrom),
    effectiveTo: parseOptDate(effTo),
  });

  const saveMut = useMutation({
    mutationFn: async () => {
      if (!coId.trim() || !code.trim() || !name.trim()) throw new Error('Company ID, code, and name are required.');
      if (editId == null) return createBusinessUnit(bodyBase());
      return updateBusinessUnit(editId, { ...bodyBase(), isActive: active });
    },
    onSuccess: (res: BaseResponse<unknown>) => {
      if (!res.success) {
        setBanner({ severity: 'error', text: res.message ?? 'Save failed' });
        return;
      }
      setBanner({ severity: 'success', text: editId ? 'Business unit updated.' : 'Business unit created.' });
      setOpen(false);
      void qc.invalidateQueries({ queryKey: ['shared', 'business-units'] });
    },
    onError: (e: Error) => setBanner({ severity: 'error', text: e.message }),
  });

  const delMut = useMutation({
    mutationFn: (id: number) => deleteBusinessUnit(id),
    onSuccess: (res) => {
      if (!res.success) {
        setBanner({ severity: 'error', text: res.message ?? 'Delete failed' });
        return;
      }
      setBanner({ severity: 'success', text: 'Business unit deleted.' });
      void qc.invalidateQueries({ queryKey: ['shared', 'business-units'] });
    },
    onError: (e: Error) => setBanner({ severity: 'error', text: e.message }),
  });

  return (
    <Stack spacing={2}>
      <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} alignItems={{ sm: 'center' }}>
        <TextField
          label="Filter by company ID"
          size="small"
          value={filterCo}
          onChange={(e) => setFilterCo(e.target.value)}
          sx={{ maxWidth: 240 }}
        />
        <Button variant="outlined" onClick={() => void q.refetch()}>
          Refresh
        </Button>
        <Box flex={1} />
        <Button variant="contained" onClick={openCreate}>
          Add business unit
        </Button>
      </Stack>
      <Typography variant="body2" color="text.secondary">
        GET/POST/PUT/DELETE /api/v1/business-units
      </Typography>
      <DataTable
        columns={[
          { id: 'id', label: 'ID' },
          { id: 'companyId', label: 'Co.' },
          { id: 'businessUnitCode', label: 'Code' },
          { id: 'businessUnitName', label: 'Name' },
          { id: 'businessUnitType', label: 'Type' },
          {
            id: '_a',
            label: '',
            format: (row) => (
              <Stack direction="row" spacing={1}>
                <Button size="small" variant="outlined" onClick={() => openEdit(row)}>
                  Edit
                </Button>
                <Button size="small" color="error" variant="outlined" onClick={() => {
                  if (window.confirm(`Delete business unit ${row.businessUnitCode}?`)) delMut.mutate(Number(row.id));
                }}>
                  Delete
                </Button>
              </Stack>
            ),
          },
        ]}
        rows={pg.slice as Record<string, unknown>[]}
        rowKey={(r) => String(r.id)}
        totalCount={pg.total}
        page={pg.page}
        pageSize={pg.size}
        onPageChange={(p, ps) => {
          pg.setPage(p);
          pg.setSize(ps);
        }}
        loading={q.isLoading}
        tableAriaLabel="Business units"
      />
      <AppModal
        open={open}
        onClose={() => setOpen(false)}
        title={editId == null ? 'Create business unit' : 'Edit business unit'}
        maxWidth="md"
        actions={
          <>
            <Button onClick={() => setOpen(false)}>Cancel</Button>
            <Button variant="contained" disabled={saveMut.isPending} onClick={() => saveMut.mutate()}>
              Save
            </Button>
          </>
        }
      >
        <Stack spacing={2} sx={{ pt: 1 }}>
          <TextField label="Company ID" required value={coId} onChange={(e) => setCoId(e.target.value)} />
          <TextField label="Business unit code" required value={code} onChange={(e) => setCode(e.target.value)} />
          <TextField label="Business unit name" required value={name} onChange={(e) => setName(e.target.value)} />
          <TextField label="Business unit type" required value={buType} onChange={(e) => setBuType(e.target.value)} helperText="e.g. Hospital, LabNetwork" />
          <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
            <TextField label="Region code" value={region} onChange={(e) => setRegion(e.target.value)} fullWidth />
            <TextField label="Country code" value={country} onChange={(e) => setCountry(e.target.value)} fullWidth />
          </Stack>
          <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
            <TextField label="Effective from" type="date" InputLabelProps={{ shrink: true }} value={effFrom} onChange={(e) => setEffFrom(e.target.value)} fullWidth />
            <TextField label="Effective to" type="date" InputLabelProps={{ shrink: true }} value={effTo} onChange={(e) => setEffTo(e.target.value)} fullWidth />
          </Stack>
          <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
            <TextField label="Primary address ID" value={addr} onChange={(e) => setAddr(e.target.value)} fullWidth />
            <TextField label="Primary contact ID" value={contact} onChange={(e) => setContact(e.target.value)} fullWidth />
          </Stack>
          {editId != null ? (
            <FormControlLabel control={<Checkbox checked={active} onChange={(e) => setActive(e.target.checked)} />} label="Active" />
          ) : null}
        </Stack>
      </AppModal>
    </Stack>
  );
}

function DepartmentsCrud({ setBanner }: { setBanner: (b: Banner | null) => void }) {
  const qc = useQueryClient();
  const [filterFac, setFilterFac] = useState('');
  const facilityId = parseOptLong(filterFac);
  const q = useQuery({
    queryKey: ['shared', 'departments', facilityId ?? 'all'],
    queryFn: () => listDepartments(facilityId),
  });
  const rows =
    q.data?.success && q.data.data ? ([...q.data.data] as Record<string, unknown>[]) : [];
  const pg = usePagedRows(rows);
  const [open, setOpen] = useState(false);
  const [editId, setEditId] = useState<number | null>(null);
  const [facParent, setFacParent] = useState('');
  const [code, setCode] = useState('');
  const [name, setName] = useState('');
  const [deptType, setDeptType] = useState('Clinical');
  const [parentDept, setParentDept] = useState('');
  const [effFrom, setEffFrom] = useState('');
  const [effTo, setEffTo] = useState('');
  const [addr, setAddr] = useState('');
  const [contact, setContact] = useState('');
  const [active, setActive] = useState(true);

  const reset = () => {
    setEditId(null);
    setFacParent('');
    setCode('');
    setName('');
    setDeptType('Clinical');
    setParentDept('');
    setEffFrom('');
    setEffTo('');
    setAddr('');
    setContact('');
    setActive(true);
  };

  const openCreate = () => {
    reset();
    if (filterFac.trim()) setFacParent(filterFac.trim());
    setOpen(true);
  };

  const openEdit = (r: Record<string, unknown>) => {
    setEditId(Number(r.id));
    setFacParent(String(r.facilityParentId ?? r.facilityId ?? ''));
    setCode(String(r.departmentCode ?? ''));
    setName(String(r.departmentName ?? ''));
    setDeptType(String(r.departmentType ?? 'Clinical'));
    setParentDept(r.parentDepartmentId != null ? String(r.parentDepartmentId) : '');
    setEffFrom(r.effectiveFrom ? String(r.effectiveFrom).slice(0, 10) : '');
    setEffTo(r.effectiveTo ? String(r.effectiveTo).slice(0, 10) : '');
    setAddr(r.primaryAddressId != null ? String(r.primaryAddressId) : '');
    setContact(r.primaryContactId != null ? String(r.primaryContactId) : '');
    setActive(Boolean(r.isActive));
    setOpen(true);
  };

  const bodyBase = () => ({
    facilityParentId: Number(facParent.trim()),
    departmentCode: code.trim(),
    departmentName: name.trim(),
    departmentType: deptType.trim() || 'Clinical',
    parentDepartmentId: parseOptLong(parentDept),
    primaryAddressId: parseOptLong(addr),
    primaryContactId: parseOptLong(contact),
    effectiveFrom: parseOptDate(effFrom),
    effectiveTo: parseOptDate(effTo),
  });

  const saveMut = useMutation({
    mutationFn: async () => {
      if (!facParent.trim() || !code.trim() || !name.trim()) {
        throw new Error('Facility parent ID, code, and name are required.');
      }
      if (editId == null) return createDepartment(bodyBase());
      return updateDepartment(editId, { ...bodyBase(), isActive: active });
    },
    onSuccess: (res: BaseResponse<unknown>) => {
      if (!res.success) {
        setBanner({ severity: 'error', text: res.message ?? 'Save failed' });
        return;
      }
      setBanner({ severity: 'success', text: editId ? 'Department updated.' : 'Department created.' });
      setOpen(false);
      void qc.invalidateQueries({ queryKey: ['shared', 'departments'] });
    },
    onError: (e: Error) => setBanner({ severity: 'error', text: e.message }),
  });

  const delMut = useMutation({
    mutationFn: (id: number) => deleteDepartment(id),
    onSuccess: (res) => {
      if (!res.success) {
        setBanner({ severity: 'error', text: res.message ?? 'Delete failed' });
        return;
      }
      setBanner({ severity: 'success', text: 'Department deleted.' });
      void qc.invalidateQueries({ queryKey: ['shared', 'departments'] });
    },
    onError: (e: Error) => setBanner({ severity: 'error', text: e.message }),
  });

  return (
    <Stack spacing={2}>
      <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} alignItems={{ sm: 'center' }}>
        <TextField
          label="Filter by facility ID"
          size="small"
          value={filterFac}
          onChange={(e) => setFilterFac(e.target.value)}
          sx={{ maxWidth: 240 }}
        />
        <Button variant="outlined" onClick={() => void q.refetch()}>
          Refresh
        </Button>
        <Box flex={1} />
        <Button variant="contained" onClick={openCreate}>
          Add department
        </Button>
      </Stack>
      <Typography variant="body2" color="text.secondary">
        GET/POST/PUT/DELETE /api/v1/departments (list filter: facilityId)
      </Typography>
      <DataTable
        columns={[
          { id: 'id', label: 'ID' },
          { id: 'facilityId', label: 'Facility' },
          { id: 'departmentCode', label: 'Code' },
          { id: 'departmentName', label: 'Name' },
          {
            id: '_a',
            label: '',
            format: (row) => (
              <Stack direction="row" spacing={1}>
                <Button size="small" variant="outlined" onClick={() => openEdit(row)}>
                  Edit
                </Button>
                <Button size="small" color="error" variant="outlined" onClick={() => {
                  if (window.confirm(`Delete department ${row.departmentCode}?`)) delMut.mutate(Number(row.id));
                }}>
                  Delete
                </Button>
              </Stack>
            ),
          },
        ]}
        rows={pg.slice as Record<string, unknown>[]}
        rowKey={(r) => String(r.id)}
        totalCount={pg.total}
        page={pg.page}
        pageSize={pg.size}
        onPageChange={(p, ps) => {
          pg.setPage(p);
          pg.setSize(ps);
        }}
        loading={q.isLoading}
        tableAriaLabel="Departments"
      />
      <AppModal
        open={open}
        onClose={() => setOpen(false)}
        title={editId == null ? 'Create department' : 'Edit department'}
        maxWidth="md"
        actions={
          <>
            <Button onClick={() => setOpen(false)}>Cancel</Button>
            <Button variant="contained" disabled={saveMut.isPending} onClick={() => saveMut.mutate()}>
              Save
            </Button>
          </>
        }
      >
        <Stack spacing={2} sx={{ pt: 1 }}>
          <TextField
            label="Facility parent ID"
            required
            value={facParent}
            onChange={(e) => setFacParent(e.target.value)}
            helperText="Maps to facilityParentId on create/update DTO"
          />
          <TextField label="Department code" required value={code} onChange={(e) => setCode(e.target.value)} />
          <TextField label="Department name" required value={name} onChange={(e) => setName(e.target.value)} />
          <TextField label="Department type" required value={deptType} onChange={(e) => setDeptType(e.target.value)} />
          <TextField label="Parent department ID" value={parentDept} onChange={(e) => setParentDept(e.target.value)} />
          <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
            <TextField label="Effective from" type="date" InputLabelProps={{ shrink: true }} value={effFrom} onChange={(e) => setEffFrom(e.target.value)} fullWidth />
            <TextField label="Effective to" type="date" InputLabelProps={{ shrink: true }} value={effTo} onChange={(e) => setEffTo(e.target.value)} fullWidth />
          </Stack>
          <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
            <TextField label="Primary address ID" value={addr} onChange={(e) => setAddr(e.target.value)} fullWidth />
            <TextField label="Primary contact ID" value={contact} onChange={(e) => setContact(e.target.value)} fullWidth />
          </Stack>
          {editId != null ? (
            <FormControlLabel control={<Checkbox checked={active} onChange={(e) => setActive(e.target.checked)} />} label="Active" />
          ) : null}
        </Stack>
      </AppModal>
    </Stack>
  );
}

function FacilitiesCrud({ setBanner }: { setBanner: (b: Banner | null) => void }) {
  const qc = useQueryClient();
  const [filterBu, setFilterBu] = useState('');
  const businessUnitId = parseOptLong(filterBu);
  const q = useQuery({
    queryKey: ['shared', 'facilities-crud', businessUnitId ?? 'all'],
    queryFn: () => listFacilities(businessUnitId),
  });
  const rows =
    q.data?.success && q.data.data ? ([...q.data.data] as Record<string, unknown>[]) : [];
  const pg = usePagedRows(rows);
  const [open, setOpen] = useState(false);
  const [editId, setEditId] = useState<number | null>(null);
  const [buId, setBuId] = useState('');
  const [code, setCode] = useState('');
  const [name, setName] = useState('');
  const [facType, setFacType] = useState('Hospital');
  const [license, setLicense] = useState('');
  const [tz, setTz] = useState('');
  const [geo, setGeo] = useState('');
  const [effFrom, setEffFrom] = useState('');
  const [effTo, setEffTo] = useState('');
  const [addr, setAddr] = useState('');
  const [contact, setContact] = useState('');
  const [active, setActive] = useState(true);

  const reset = () => {
    setEditId(null);
    setBuId('');
    setCode('');
    setName('');
    setFacType('Hospital');
    setLicense('');
    setTz('');
    setGeo('');
    setEffFrom('');
    setEffTo('');
    setAddr('');
    setContact('');
    setActive(true);
  };

  const openCreate = () => {
    reset();
    if (filterBu.trim()) setBuId(filterBu.trim());
    setOpen(true);
  };

  const openEdit = (r: Record<string, unknown>) => {
    setEditId(Number(r.id));
    setBuId(String(r.businessUnitId ?? ''));
    setCode(String(r.facilityCode ?? ''));
    setName(String(r.facilityName ?? ''));
    setFacType(String(r.facilityType ?? 'Hospital'));
    setLicense(String(r.licenseDetails ?? ''));
    setTz(String(r.timeZoneId ?? ''));
    setGeo(String(r.geoCode ?? ''));
    setEffFrom(r.effectiveFrom ? String(r.effectiveFrom).slice(0, 10) : '');
    setEffTo(r.effectiveTo ? String(r.effectiveTo).slice(0, 10) : '');
    setAddr(r.primaryAddressId != null ? String(r.primaryAddressId) : '');
    setContact(r.primaryContactId != null ? String(r.primaryContactId) : '');
    setActive(Boolean(r.isActive));
    setOpen(true);
  };

  const bodyBase = () => ({
    businessUnitId: Number(buId.trim()),
    facilityCode: code.trim(),
    facilityName: name.trim(),
    facilityType: facType.trim() || 'Hospital',
    licenseDetails: license.trim() || undefined,
    timeZoneId: tz.trim() || undefined,
    geoCode: geo.trim() || undefined,
    primaryAddressId: parseOptLong(addr),
    primaryContactId: parseOptLong(contact),
    effectiveFrom: parseOptDate(effFrom),
    effectiveTo: parseOptDate(effTo),
  });

  const saveMut = useMutation({
    mutationFn: async () => {
      if (!buId.trim() || !code.trim() || !name.trim()) {
        throw new Error('Business unit ID, code, and name are required.');
      }
      if (editId == null) return createFacility(bodyBase());
      return updateFacility(editId, { ...bodyBase(), isActive: active });
    },
    onSuccess: (res: BaseResponse<unknown>) => {
      if (!res.success) {
        setBanner({ severity: 'error', text: res.message ?? 'Save failed' });
        return;
      }
      setBanner({ severity: 'success', text: editId ? 'Facility updated.' : 'Facility created.' });
      setOpen(false);
      void qc.invalidateQueries({ queryKey: ['shared', 'facilities-crud'] });
      void qc.invalidateQueries({ queryKey: ['shared', 'facilities'] });
    },
    onError: (e: Error) => setBanner({ severity: 'error', text: e.message }),
  });

  const delMut = useMutation({
    mutationFn: (id: number) => deleteFacility(id),
    onSuccess: (res) => {
      if (!res.success) {
        setBanner({ severity: 'error', text: res.message ?? 'Delete failed' });
        return;
      }
      setBanner({ severity: 'success', text: 'Facility deleted.' });
      void qc.invalidateQueries({ queryKey: ['shared', 'facilities-crud'] });
      void qc.invalidateQueries({ queryKey: ['shared', 'facilities'] });
    },
    onError: (e: Error) => setBanner({ severity: 'error', text: e.message }),
  });

  return (
    <Stack spacing={2}>
      <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} alignItems={{ sm: 'center' }}>
        <TextField
          label="Filter by business unit ID"
          size="small"
          value={filterBu}
          onChange={(e) => setFilterBu(e.target.value)}
          sx={{ maxWidth: 260 }}
        />
        <Button variant="outlined" onClick={() => void q.refetch()}>
          Refresh
        </Button>
        <Box flex={1} />
        <Button variant="contained" onClick={openCreate}>
          Add facility
        </Button>
      </Stack>
      <Typography variant="body2" color="text.secondary">
        GET/POST/PUT/DELETE /api/v1/facilities
      </Typography>
      <DataTable
        columns={[
          { id: 'id', label: 'ID' },
          { id: 'businessUnitId', label: 'BU' },
          { id: 'facilityCode', label: 'Code' },
          { id: 'facilityName', label: 'Name' },
          { id: 'facilityType', label: 'Type' },
          {
            id: '_a',
            label: '',
            format: (row) => (
              <Stack direction="row" spacing={1}>
                <Button size="small" variant="outlined" onClick={() => openEdit(row)}>
                  Edit
                </Button>
                <Button size="small" color="error" variant="outlined" onClick={() => {
                  if (window.confirm(`Delete facility ${row.facilityCode}?`)) delMut.mutate(Number(row.id));
                }}>
                  Delete
                </Button>
              </Stack>
            ),
          },
        ]}
        rows={pg.slice as Record<string, unknown>[]}
        rowKey={(r) => String(r.id)}
        totalCount={pg.total}
        page={pg.page}
        pageSize={pg.size}
        onPageChange={(p, ps) => {
          pg.setPage(p);
          pg.setSize(ps);
        }}
        loading={q.isLoading}
        tableAriaLabel="Facilities CRUD"
      />
      <AppModal
        open={open}
        onClose={() => setOpen(false)}
        title={editId == null ? 'Create facility' : 'Edit facility'}
        maxWidth="md"
        actions={
          <>
            <Button onClick={() => setOpen(false)}>Cancel</Button>
            <Button variant="contained" disabled={saveMut.isPending} onClick={() => saveMut.mutate()}>
              Save
            </Button>
          </>
        }
      >
        <Stack spacing={2} sx={{ pt: 1 }}>
          <TextField label="Business unit ID" required value={buId} onChange={(e) => setBuId(e.target.value)} />
          <TextField label="Facility code" required value={code} onChange={(e) => setCode(e.target.value)} />
          <TextField label="Facility name" required value={name} onChange={(e) => setName(e.target.value)} />
          <TextField label="Facility type" required value={facType} onChange={(e) => setFacType(e.target.value)} />
          <TextField label="License details" multiline minRows={2} value={license} onChange={(e) => setLicense(e.target.value)} />
          <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
            <TextField label="Time zone ID" value={tz} onChange={(e) => setTz(e.target.value)} fullWidth />
            <TextField label="Geo code" value={geo} onChange={(e) => setGeo(e.target.value)} fullWidth />
          </Stack>
          <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
            <TextField label="Effective from" type="date" InputLabelProps={{ shrink: true }} value={effFrom} onChange={(e) => setEffFrom(e.target.value)} fullWidth />
            <TextField label="Effective to" type="date" InputLabelProps={{ shrink: true }} value={effTo} onChange={(e) => setEffTo(e.target.value)} fullWidth />
          </Stack>
          <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
            <TextField label="Primary address ID" value={addr} onChange={(e) => setAddr(e.target.value)} fullWidth />
            <TextField label="Primary contact ID" value={contact} onChange={(e) => setContact(e.target.value)} fullWidth />
          </Stack>
          {editId != null ? (
            <FormControlLabel control={<Checkbox checked={active} onChange={(e) => setActive(e.target.checked)} />} label="Active" />
          ) : null}
        </Stack>
      </AppModal>
    </Stack>
  );
}

/** Full CRUD for enterprise → facility chain (SharedService hierarchy APIs). */
export function SharedEnterpriseAdminView() {
  const [tab, setTab] = useState(0);
  const [banner, setBanner] = useState<Banner | null>(null);
  const clearBanner = useCallback(() => setBanner(null), []);

  return (
    <Stack spacing={2}>
      <PageHeader
        title="Enterprise administration"
        subtitle="CRUD for enterprises, companies, business units, departments, and facilities via IIS SharedService."
      />
      {banner ? (
        <Alert severity={banner.severity} onClose={clearBanner}>
          {banner.text}
        </Alert>
      ) : null}
      <Tabs value={tab} onChange={(_, v) => setTab(v)} variant="scrollable" allowScrollButtonsMobile>
        <Tab label="Enterprises" />
        <Tab label="Companies" />
        <Tab label="Business units" />
        <Tab label="Departments" />
        <Tab label="Facilities" />
      </Tabs>
      <Box role="tabpanel" sx={{ py: 1 }}>
        {tab === 0 ? <EnterprisesCrud setBanner={setBanner} /> : null}
        {tab === 1 ? <CompaniesCrud setBanner={setBanner} /> : null}
        {tab === 2 ? <BusinessUnitsCrud setBanner={setBanner} /> : null}
        {tab === 3 ? <DepartmentsCrud setBanner={setBanner} /> : null}
        {tab === 4 ? <FacilitiesCrud setBanner={setBanner} /> : null}
      </Box>
    </Stack>
  );
}
