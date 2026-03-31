import {
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TablePagination,
  TableRow,
  Typography,
} from '@mui/material';
import { useMemo, useState } from 'react';
import { EmptyState } from './EmptyState';

export interface Column<T> {
  id: string;
  label: string;
  minWidth?: number;
  align?: 'right' | 'left' | 'center';
  format?: (row: T) => React.ReactNode;
}

export interface DataTableProps<T> {
  columns: Column<T>[];
  rows: readonly T[];
  rowKey: (row: T) => string | number;
  totalCount?: number;
  page?: number;
  pageSize?: number;
  onPageChange?: (page: number, pageSize: number) => void;
  loading?: boolean;
  emptyTitle?: string;
}

/** Enterprise-style data grid using MUI Table + server-friendly pagination. */
export function DataTable<T extends object>({
  columns,
  rows,
  rowKey,
  totalCount,
  page: controlledPage,
  pageSize: controlledPageSize = 20,
  onPageChange,
  loading,
  emptyTitle = 'No records',
}: DataTableProps<T>) {
  const [localPage, setLocalPage] = useState(0);
  const [localSize, setLocalSize] = useState(controlledPageSize);
  const serverMode = totalCount != null && onPageChange != null;
  const page = serverMode ? (controlledPage ?? 0) : localPage;
  const pageSize = serverMode ? controlledPageSize : localSize;
  const count = totalCount ?? rows.length;

  const slice = useMemo(() => {
    if (serverMode) return rows;
    const start = page * pageSize;
    return rows.slice(start, start + pageSize);
  }, [rows, page, pageSize, serverMode]);

  return (
    <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, overflow: 'hidden' }}>
      <TableContainer sx={{ maxHeight: 560 }}>
        <Table stickyHeader size="small">
          <TableHead>
            <TableRow>
              {columns.map((c) => (
                <TableCell key={c.id} align={c.align} sx={{ minWidth: c.minWidth, fontWeight: 600 }}>
                  {c.label}
                </TableCell>
              ))}
            </TableRow>
          </TableHead>
          <TableBody>
            {loading ? (
              <TableRow>
                <TableCell colSpan={columns.length}>
                  <Typography color="text.secondary">Loading…</Typography>
                </TableCell>
              </TableRow>
            ) : slice.length === 0 ? (
              <TableRow>
                <TableCell colSpan={columns.length} sx={{ border: 0, py: 6 }}>
                  <EmptyState title={emptyTitle} />
                </TableCell>
              </TableRow>
            ) : (
              slice.map((row, idx) => (
                <TableRow hover key={`${rowKey(row)}-${idx}`}>
                  {columns.map((c) => (
                    <TableCell key={c.id} align={c.align}>
                      {c.format ? c.format(row) : String((row as Record<string, unknown>)[c.id] ?? '—')}
                    </TableCell>
                  ))}
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </TableContainer>
      <TablePagination
        component="div"
        count={count}
        page={page}
        onPageChange={(_, p) => {
          if (serverMode) onPageChange?.(p, pageSize);
          else setLocalPage(p);
        }}
        rowsPerPage={pageSize}
        onRowsPerPageChange={(e) => {
          const next = parseInt(e.target.value, 10);
          if (serverMode) onPageChange?.(0, next);
          else {
            setLocalSize(next);
            setLocalPage(0);
          }
        }}
        rowsPerPageOptions={[10, 20, 50, 100]}
      />
    </Paper>
  );
}
