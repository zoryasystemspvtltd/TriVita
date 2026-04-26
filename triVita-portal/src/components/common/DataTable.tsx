import { Box, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TablePagination, TableRow, Typography } from '@mui/material';
import { useMemo, useState } from 'react';
import { EmptyState } from './EmptyState';

const CARD_BORDER = '1px solid #e5e7eb';
const CARD_SHADOW = '0 2px 6px rgba(0, 0, 0, 0.05)';

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
  /** Accessible name for the table (screen readers). */
  tableAriaLabel?: string;
  /** When set, clicking a row (outside interactive cell content) invokes this handler. */
  onRowClick?: (row: T) => void;
}

function cellAlign<T>(c: Column<T>): 'right' | 'left' | 'center' {
  if (c.align) return c.align;
  if (c.id === '_actions' || c.id === '_a' || c.id === '__actions') return 'right';
  return 'left';
}

function isActionColumnId(id: string) {
  return id === '_actions' || id === '_a' || id === '__actions';
}

/** Enterprise data grid: white card shell, clear row separation, right-aligned actions. */
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
  tableAriaLabel = 'Data table',
  onRowClick,
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
    <Paper
      variant="outlined"
      elevation={0}
      sx={{
        borderRadius: '10px',
        border: CARD_BORDER,
        bgcolor: '#ffffff',
        boxShadow: CARD_SHADOW,
        overflow: 'hidden',
      }}
    >
      <TableContainer sx={{ maxHeight: 560, bgcolor: '#ffffff' }}>
        <Table stickyHeader size="medium" aria-label={tableAriaLabel} sx={{ borderCollapse: 'separate' }}>
          <TableHead>
            <TableRow>
              {columns.map((c) => {
                const align = cellAlign(c);
                return (
                  <TableCell key={c.id} align={align} sx={{ minWidth: c.minWidth }}>
                    {c.label}
                  </TableCell>
                );
              })}
            </TableRow>
          </TableHead>
          <TableBody>
            {loading ? (
              <TableRow>
                <TableCell colSpan={columns.length} sx={{ py: 3 }}>
                  <Typography variant="body1" color="text.secondary" sx={{ fontSize: '0.875rem' }}>
                    Loading…
                  </Typography>
                </TableCell>
              </TableRow>
            ) : slice.length === 0 ? (
              <TableRow>
                <TableCell colSpan={columns.length} sx={{ border: 0, py: 0, verticalAlign: 'middle' }}>
                  <EmptyState title={emptyTitle} />
                </TableCell>
              </TableRow>
            ) : (
              slice.map((row, idx) => (
                <TableRow
                  hover
                  key={`${rowKey(row)}-${idx}`}
                  onClick={onRowClick ? () => onRowClick(row) : undefined}
                  sx={onRowClick ? { cursor: 'pointer' } : undefined}
                >
                  {columns.map((c) => {
                    const align = cellAlign(c);
                    const actionCol = isActionColumnId(c.id);
                    return (
                      <TableCell
                        key={c.id}
                        align={align}
                        onClick={(e) => {
                          if (!onRowClick) return;
                          const t = e.target as HTMLElement;
                          if (t.closest('button, a, [role="button"]')) e.stopPropagation();
                        }}
                        sx={actionCol ? { verticalAlign: 'middle' } : undefined}
                      >
                        {c.format ? (
                          actionCol ? (
                            <Box
                              sx={{
                                display: 'flex',
                                justifyContent: 'flex-end',
                                alignItems: 'center',
                                flexWrap: 'wrap',
                                columnGap: 1.5,
                                rowGap: 0.5,
                                width: 1,
                                ml: 'auto',
                                '& .MuiLink-root, a': {
                                  fontWeight: 500,
                                  color: 'primary.main',
                                  textDecoration: 'none',
                                  '&:hover': { color: 'primary.dark' },
                                },
                                '& a[color="error"], & .MuiLink-root[color="error"]': {
                                  color: 'error.main',
                                  '&:hover': { color: 'error.dark' },
                                },
                              }}
                            >
                              {c.format(row)}
                            </Box>
                          ) : (
                            c.format(row)
                          )
                        ) : (
                          <Typography variant="body1" color="text.primary" sx={{ fontSize: '0.8125rem' }}>
                            {String((row as Record<string, unknown>)[c.id] ?? '—')}
                          </Typography>
                        )}
                      </TableCell>
                    );
                  })}
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
        slotProps={{
          select: { 'aria-label': 'Rows per page' } as object,
        }}
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
