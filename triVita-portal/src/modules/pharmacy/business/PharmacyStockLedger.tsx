import {
  Box,
  Button,
  FormControl,
  InputLabel,
  MenuItem,
  Select,
  Stack,
  Tab,
  Tabs,
  TextField,
} from '@mui/material';
import { useQuery } from '@tanstack/react-query';
import { useCallback, useMemo, useState } from 'react';
import { DataTable } from '@/components/common/DataTable';
import { PageHeader } from '@/components/layout/PageHeader';
import { FormSection } from '@/components/forms/FormSection';
import {
  downloadStockLedgerDetailedExcel,
  downloadStockLedgerSummaryExcel,
  getStockLedgerDetailedReport,
  getStockLedgerSummaryReport,
  type StockLedgerReportParams,
} from '@/services/pharmacyService';

type Row = Record<string, unknown>;

function startOfMonthIso() {
  const d = new Date();
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-01`;
}

function todayIso() {
  const d = new Date();
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
}

function pickStr(r: Row, ...keys: string[]) {
  for (const k of keys) {
    const v = r[k];
    if (v != null && String(v).trim() !== '') return String(v);
  }
  return '';
}

function numOrUndef(s: string): number | undefined {
  const t = s.trim();
  if (t === '') return undefined;
  const n = Number(t);
  return Number.isFinite(n) ? n : undefined;
}

function triggerBlobDownload(blob: Blob, filename: string) {
  const url = URL.createObjectURL(blob);
  const a = document.createElement('a');
  a.href = url;
  a.download = filename;
  a.rel = 'noopener';
  document.body.appendChild(a);
  a.click();
  a.remove();
  URL.revokeObjectURL(url);
}

const detailedSortOptions = [
  { value: 'transactiondate', label: 'Transaction date' },
  { value: 'transactiontype', label: 'Transaction type' },
  { value: 'referenceno', label: 'Reference' },
  { value: 'medicinename', label: 'Medicine' },
  { value: 'batchnumber', label: 'Batch' },
  { value: 'expirydate', label: 'Expiry' },
  { value: 'quantityin', label: 'Qty in' },
  { value: 'quantityout', label: 'Qty out' },
  { value: 'balance', label: 'Balance' },
] as const;

const summarySortOptions = [
  { value: 'medicinename', label: 'Medicine' },
  { value: 'batchnumber', label: 'Batch' },
  { value: 'expirydate', label: 'Expiry' },
  { value: 'openingqty', label: 'Opening qty' },
  { value: 'totalin', label: 'Total in' },
  { value: 'totalout', label: 'Total out' },
  { value: 'closingqty', label: 'Closing qty' },
] as const;

export function PharmacyStockLedger() {
  const [tab, setTab] = useState(0);
  const [fromDate, setFromDate] = useState(startOfMonthIso);
  const [toDate, setToDate] = useState(todayIso);
  const [transactionType, setTransactionType] = useState<string>('');
  const [medicineId, setMedicineId] = useState('');
  const [batchId, setBatchId] = useState('');
  const [supplierId, setSupplierId] = useState('');
  const [salePartyId, setSalePartyId] = useState('');
  const [facilityId, setFacilityId] = useState('');
  const [medicineSearch, setMedicineSearch] = useState('');
  const [sortByDetailed, setSortByDetailed] = useState('transactiondate');
  const [sortBySummary, setSortBySummary] = useState('medicinename');
  const [sortDesc, setSortDesc] = useState(false);
  const [pageD, setPageD] = useState(0);
  const [pageSizeD, setPageSizeD] = useState(25);
  const [pageS, setPageS] = useState(0);
  const [pageSizeS, setPageSizeS] = useState(25);
  const [appliedKey, setAppliedKey] = useState(0);

  const baseFilters = useMemo((): StockLedgerReportParams => {
    const p: StockLedgerReportParams = {
      fromTransactionDate: fromDate,
      toTransactionDate: toDate,
      search: medicineSearch.trim() || undefined,
    };
    const tt = numOrUndef(transactionType);
    if (tt !== undefined) p.transactionType = tt;
    const mid = numOrUndef(medicineId);
    if (mid !== undefined) p.medicineId = mid;
    const bid = numOrUndef(batchId);
    if (bid !== undefined) p.medicineBatchId = bid;
    const sid = numOrUndef(supplierId);
    if (sid !== undefined) p.supplierId = sid;
    const pid = numOrUndef(salePartyId);
    if (pid !== undefined) p.salePartyId = pid;
    const fid = numOrUndef(facilityId);
    if (fid !== undefined) p.facilityId = fid;
    return p;
  }, [
    fromDate,
    toDate,
    transactionType,
    medicineId,
    batchId,
    supplierId,
    salePartyId,
    facilityId,
    medicineSearch,
  ]);

  const applyFilters = useCallback(() => {
    setAppliedKey((k) => k + 1);
    setPageD(0);
    setPageS(0);
  }, []);

  const detailedParams = useMemo(
    () => ({
      ...baseFilters,
      page: pageD + 1,
      pageSize: pageSizeD,
      sortBy: sortByDetailed,
      sortDescending: sortDesc,
    }),
    [baseFilters, pageD, pageSizeD, sortByDetailed, sortDesc]
  );

  const summaryParams = useMemo(
    () => ({
      ...baseFilters,
      page: pageS + 1,
      pageSize: pageSizeS,
      sortBy: sortBySummary,
      sortDescending: sortDesc,
    }),
    [baseFilters, pageS, pageSizeS, sortBySummary, sortDesc]
  );

  const qDetailed = useQuery({
    queryKey: ['pharmacy', 'stock-ledger-detailed', appliedKey, detailedParams],
    queryFn: () => getStockLedgerDetailedReport(detailedParams),
    enabled: tab === 0,
  });

  const qSummary = useQuery({
    queryKey: ['pharmacy', 'stock-ledger-summary', appliedKey, summaryParams],
    queryFn: () => getStockLedgerSummaryReport(summaryParams),
    enabled: tab === 1,
  });

  const detailedRows = useMemo(
    () => (qDetailed.data?.success && qDetailed.data.data ? [...qDetailed.data.data.items] : []) as Row[],
    [qDetailed.data]
  );
  const detailedTotal = qDetailed.data?.success && qDetailed.data.data ? qDetailed.data.data.totalCount : 0;

  const summaryRows = useMemo(
    () => (qSummary.data?.success && qSummary.data.data ? [...qSummary.data.data.items] : []) as Row[],
    [qSummary.data]
  );
  const summaryTotal = qSummary.data?.success && qSummary.data.data ? qSummary.data.data.totalCount : 0;

  const exportDetailed = async () => {
    const blob = await downloadStockLedgerDetailedExcel({
      ...baseFilters,
      sortBy: sortByDetailed,
      sortDescending: sortDesc,
    });
    triggerBlobDownload(blob, `StockLedger_Detailed_${fromDate}_${toDate}.xlsx`);
  };

  const exportSummary = async () => {
    const blob = await downloadStockLedgerSummaryExcel({
      ...baseFilters,
      sortBy: sortBySummary,
      sortDescending: sortDesc,
    });
    triggerBlobDownload(blob, `StockLedger_Summary_${fromDate}_${toDate}.xlsx`);
  };

  return (
    <Stack spacing={2}>
      <PageHeader
        title="Stock ledger"
        subtitle="Detailed and summary reports from stock ledger only (server paging, Excel export)."
      />

      <Tabs value={tab} onChange={(_, v) => setTab(v)} aria-label="Stock ledger report tabs">
        <Tab label="Detailed" />
        <Tab label="Summary" />
      </Tabs>

      <FormSection title="Filters">
        <Stack spacing={2} useFlexGap flexWrap="wrap" direction="row" alignItems="flex-end">
          <TextField
            size="small"
            type="date"
            label="From date"
            InputLabelProps={{ shrink: true }}
            value={fromDate}
            onChange={(e) => setFromDate(e.target.value)}
          />
          <TextField
            size="small"
            type="date"
            label="To date"
            InputLabelProps={{ shrink: true }}
            value={toDate}
            onChange={(e) => setToDate(e.target.value)}
          />
          <FormControl size="small" sx={{ minWidth: 160 }}>
            <InputLabel id="sl-tx-type">Transaction type</InputLabel>
            <Select
              labelId="sl-tx-type"
              label="Transaction type"
              value={transactionType}
              onChange={(e) => setTransactionType(String(e.target.value))}
            >
              <MenuItem value="">All</MenuItem>
              <MenuItem value="1">GRN</MenuItem>
              <MenuItem value="2">SALE</MenuItem>
              <MenuItem value="3">ADJUSTMENT</MenuItem>
            </Select>
          </FormControl>
          <TextField
            size="small"
            label="Medicine id"
            value={medicineId}
            onChange={(e) => setMedicineId(e.target.value)}
            sx={{ width: 120 }}
          />
          <TextField
            size="small"
            label="Batch id"
            value={batchId}
            onChange={(e) => setBatchId(e.target.value)}
            sx={{ width: 120 }}
          />
          <TextField
            size="small"
            label="Supplier id (GRN)"
            value={supplierId}
            onChange={(e) => setSupplierId(e.target.value)}
            sx={{ width: 140 }}
          />
          <TextField
            size="small"
            label="Customer / patient id"
            value={salePartyId}
            onChange={(e) => setSalePartyId(e.target.value)}
            sx={{ width: 160 }}
          />
          <TextField
            size="small"
            label="Facility id (optional)"
            value={facilityId}
            onChange={(e) => setFacilityId(e.target.value)}
            sx={{ width: 160 }}
          />
          <TextField
            size="small"
            label="Medicine name search"
            value={medicineSearch}
            onChange={(e) => setMedicineSearch(e.target.value)}
            sx={{ minWidth: 200 }}
          />
          <FormControl size="small" sx={{ minWidth: 170 }}>
            <InputLabel id="sl-sort">Sort by</InputLabel>
            <Select
              labelId="sl-sort"
              label="Sort by"
              value={tab === 0 ? sortByDetailed : sortBySummary}
              onChange={(e) => {
                const v = String(e.target.value);
                if (tab === 0) setSortByDetailed(v);
                else setSortBySummary(v);
              }}
            >
              {(tab === 0 ? detailedSortOptions : summarySortOptions).map((o) => (
                <MenuItem key={o.value} value={o.value}>
                  {o.label}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
          <FormControl size="small" sx={{ minWidth: 120 }}>
            <InputLabel id="sl-sort-dir">Order</InputLabel>
            <Select
              labelId="sl-sort-dir"
              label="Order"
              value={sortDesc ? 'desc' : 'asc'}
              onChange={(e) => setSortDesc(e.target.value === 'desc')}
            >
              <MenuItem value="asc">Ascending</MenuItem>
              <MenuItem value="desc">Descending</MenuItem>
            </Select>
          </FormControl>
          <Button variant="contained" onClick={applyFilters}>
            Apply filters
          </Button>
          {tab === 0 ? (
            <Button variant="outlined" onClick={() => void exportDetailed()}>
              Export Excel
            </Button>
          ) : (
            <Button variant="outlined" onClick={() => void exportSummary()}>
              Export Excel
            </Button>
          )}
        </Stack>
      </FormSection>

      {tab === 0 ? (
        <FormSection title="Detailed movements">
          <Box
            sx={{
              '& .MuiTableRow-root:nth-of-type(even)': { bgcolor: 'action.hover' },
            }}
          >
            <DataTable<Row>
              tableAriaLabel="Stock ledger detailed"
              columns={[
                {
                  id: 'transactionDate',
                  label: 'Date',
                  minWidth: 140,
                  format: (r) => pickStr(r, 'transactionDate', 'TransactionDate').slice(0, 16).replace('T', ' '),
                },
                { id: 'transactionType', label: 'Type', format: (r) => pickStr(r, 'transactionType', 'TransactionType') },
                { id: 'referenceNo', label: 'Reference', format: (r) => pickStr(r, 'referenceNo', 'ReferenceNo') },
                { id: 'medicineName', label: 'Medicine', format: (r) => pickStr(r, 'medicineName', 'MedicineName') },
                { id: 'batchNumber', label: 'Batch', format: (r) => pickStr(r, 'batchNumber', 'BatchNumber') },
                {
                  id: 'expiryDate',
                  label: 'Expiry',
                  format: (r) => {
                    const x = pickStr(r, 'expiryDate', 'ExpiryDate');
                    return x ? x.slice(0, 10) : '—';
                  },
                },
                {
                  id: 'qin',
                  label: 'Qty in',
                  align: 'right',
                  format: (r) => pickStr(r, 'quantityIn', 'QuantityIn'),
                },
                {
                  id: 'qout',
                  label: 'Qty out',
                  align: 'right',
                  format: (r) => pickStr(r, 'quantityOut', 'QuantityOut'),
                },
                {
                  id: 'bal',
                  label: 'Balance',
                  align: 'right',
                  format: (r) => pickStr(r, 'balance', 'Balance'),
                },
              ]}
              rows={detailedRows}
              rowKey={(r) =>
                [
                  pickStr(r, 'transactionDate', 'TransactionDate'),
                  pickStr(r, 'referenceNo', 'ReferenceNo'),
                  pickStr(r, 'batchNumber', 'BatchNumber'),
                  pickStr(r, 'quantityIn', 'QuantityIn'),
                  pickStr(r, 'quantityOut', 'QuantityOut'),
                ].join('|')
              }
              totalCount={detailedTotal}
              page={pageD}
              pageSize={pageSizeD}
              onPageChange={(p, ps) => {
                setPageD(p);
                setPageSizeD(ps);
              }}
              loading={qDetailed.isLoading}
              emptyTitle="No rows for this range"
            />
          </Box>
        </FormSection>
      ) : (
        <FormSection title="Summary by medicine and batch">
          <Box
            sx={{
              '& .MuiTableRow-root:nth-of-type(even)': { bgcolor: 'action.hover' },
            }}
          >
            <DataTable<Row>
              tableAriaLabel="Stock ledger summary"
              columns={[
                { id: 'm', label: 'Medicine', format: (r) => pickStr(r, 'medicineName', 'MedicineName') },
                { id: 'b', label: 'Batch', format: (r) => pickStr(r, 'batchNumber', 'BatchNumber') },
                {
                  id: 'ex',
                  label: 'Expiry',
                  format: (r) => {
                    const x = pickStr(r, 'expiryDate', 'ExpiryDate');
                    return x ? x.slice(0, 10) : '—';
                  },
                },
                {
                  id: 'op',
                  label: 'Opening',
                  align: 'right',
                  format: (r) => pickStr(r, 'openingQty', 'OpeningQty'),
                },
                {
                  id: 'ti',
                  label: 'Total in',
                  align: 'right',
                  format: (r) => pickStr(r, 'totalIn', 'TotalIn'),
                },
                {
                  id: 'to',
                  label: 'Total out',
                  align: 'right',
                  format: (r) => pickStr(r, 'totalOut', 'TotalOut'),
                },
                {
                  id: 'cl',
                  label: 'Closing',
                  align: 'right',
                  format: (r) => pickStr(r, 'closingQty', 'ClosingQty'),
                },
              ]}
              rows={summaryRows}
              rowKey={(r) =>
                [
                  pickStr(r, 'medicineName', 'MedicineName'),
                  pickStr(r, 'batchNumber', 'BatchNumber'),
                  pickStr(r, 'openingQty', 'OpeningQty'),
                  pickStr(r, 'closingQty', 'ClosingQty'),
                ].join('|')
              }
              totalCount={summaryTotal}
              page={pageS}
              pageSize={pageSizeS}
              onPageChange={(p, ps) => {
                setPageS(p);
                setPageSizeS(ps);
              }}
              loading={qSummary.isLoading}
              emptyTitle="No summary rows for this range"
            />
          </Box>
        </FormSection>
      )}
    </Stack>
  );
}
