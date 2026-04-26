import {
  Alert,
  Box,
  Drawer,
  IconButton,
  Stack,
  Tab,
  Tabs,
  Tooltip,
  Typography,
} from '@mui/material';
import { Add, Delete, Edit, OpenInNew, Refresh, Search } from '@mui/icons-material';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useCallback, useMemo, useState } from 'react';
import type { ApiRegistryEntry } from '@/generated/apiRegistry';
import type { BaseResponse, PagedQueryParams, PagedResponse } from '@/types/api';
import { enrichApiRegistryEntry } from '@/config/apiRegistryEnrichment';
import { getAxiosForModule } from '@/services/moduleClients';
import { PageHeader } from '@/components/layout/PageHeader';
import { DataTable, type Column } from '@/components/common/DataTable';
import { QueryStateBar } from '@/components/common/QueryState';
import { TriVitaButton } from '@/components/ds/TriVitaButton';
import { TriVitaTextField } from '@/components/ds/TriVitaTextField';
import { TriVitaModal } from '@/components/ds/TriVitaModal';
import { useToast } from '@/components/toast/ToastProvider';
import { getApiErrorMessage } from '@/utils/errorMap';

function pickColumns(sample: Record<string, unknown> | undefined): Column<Record<string, unknown>>[] {
  if (!sample) return [{ id: 'id', label: 'ID' }];
  const keys = Object.keys(sample).filter((k) => !k.startsWith('_'));
  const preferred = ['id', 'name', 'code', 'title', 'status', 'createdAt', 'updatedAt'];
  const ordered = [
    ...preferred.filter((k) => keys.includes(k)),
    ...keys.filter((k) => !preferred.includes(k)),
  ].slice(0, 10);
  return ordered.map((k) => ({
    id: k,
    label: k.replace(/([A-Z])/g, ' $1').replace(/^./, (s) => s.toUpperCase()),
  }));
}

function stripIdForPayload(row: Record<string, unknown>) {
  const { id, ...rest } = row;
  void id;
  return rest;
}

type ListSegment =
  | { kind: 'primary'; label: string }
  | { kind: 'secondary'; relPath: string; label: string };

export function EnterpriseCrudResourcePage({
  def,
  variant = 'registry',
}: {
  def: ApiRegistryEntry;
  /** Registry/admin surface uses JSON payloads; business screens use structured forms instead. */
  variant?: 'registry';
}) {
  const spec = enrichApiRegistryEntry(def);
  const client = getAxiosForModule(spec.module);
  const basePath = `/api/v1/${spec.path}`;
  const qc = useQueryClient();
  const { showToast } = useToast();

  const segments = useMemo((): ListSegment[] => {
    const s: ListSegment[] = [];
    if (spec.hasPagedList) s.push({ kind: 'primary', label: 'Primary list' });
    for (const x of spec.secondaryPagedGets ?? []) {
      s.push({ kind: 'secondary', relPath: x.relPath, label: x.label });
    }
    return s;
  }, [spec.hasPagedList, spec.secondaryPagedGets]);

  const [segmentIdx, setSegmentIdx] = useState(0);
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const [search, setSearch] = useState('');
  const [searchApplied, setSearchApplied] = useState('');
  const [idLookup, setIdLookup] = useState('');
  const [detailOpen, setDetailOpen] = useState(false);
  const [detailRecord, setDetailRecord] = useState<Record<string, unknown> | null>(null);
  const [jsonModal, setJsonModal] = useState<
    null | { mode: 'create' | 'edit' | 'auxPost'; title: string; initialJson: string; postPath?: string }
  >(null);
  const [jsonDraft, setJsonDraft] = useState('');
  const [deleteId, setDeleteId] = useState<number | null>(null);

  const activeSeg = segments[segmentIdx] ?? segments[0];
  const listUrl =
    activeSeg?.kind === 'secondary' ? `${basePath}/${activeSeg.relPath}` : activeSeg ? basePath : basePath;
  const listEnabled = Boolean(activeSeg);

  const listQuery = useQuery({
    queryKey: ['registry-list', spec.module, spec.path, listUrl, page, pageSize, searchApplied],
    queryFn: async () => {
      const params: PagedQueryParams & Record<string, string | number | boolean | undefined> = {
        page: page + 1,
        pageSize,
      };
      if (searchApplied.trim()) params.search = searchApplied.trim();
      const { data } = await client.get<BaseResponse<PagedResponse<Record<string, unknown>>>>(listUrl, {
        params,
      });
      return data;
    },
    enabled: listEnabled,
  });

  const rows = listQuery.data?.success && listQuery.data.data ? [...listQuery.data.data.items] : [];
  const totalCount = listQuery.data?.success && listQuery.data.data ? listQuery.data.data.totalCount : 0;
  const columns = useMemo(() => pickColumns(rows[0]), [rows]);

  const runIdLookup = async () => {
    if (!spec.hasGetById || !idLookup.trim()) {
      showToast('Enter a numeric id to load.', 'warning');
      return;
    }
    try {
      const { data } = await client.get<BaseResponse<Record<string, unknown>>>(
        `${basePath}/${Number(idLookup)}`
      );
      if (!data.success) {
        showToast(data.message ?? 'Lookup failed', 'error');
        return;
      }
      setDetailRecord(data.data ?? {});
      setDetailOpen(true);
      showToast('Record loaded', 'success');
    } catch (e) {
      showToast(getApiErrorMessage(e), 'error');
    }
  };

  const invalidate = () =>
    void qc.invalidateQueries({ queryKey: ['registry-list', spec.module, spec.path] });

  const createMut = useMutation({
    mutationFn: async (body: unknown) => {
      const { data } = await client.post<BaseResponse<unknown>>(basePath, body);
      return data;
    },
    onSuccess: (d) => {
      if (!d.success) showToast(d.message ?? 'Create failed', 'error');
      else {
        showToast('Created successfully', 'success');
        setJsonModal(null);
        invalidate();
      }
    },
    onError: (e) => showToast(getApiErrorMessage(e), 'error'),
  });

  const updateMut = useMutation({
    mutationFn: async ({ id, body }: { id: number; body: unknown }) => {
      const { data } = await client.put<BaseResponse<unknown>>(`${basePath}/${id}`, body);
      return data;
    },
    onSuccess: (d) => {
      if (!d.success) showToast(d.message ?? 'Update failed', 'error');
      else {
        showToast('Saved', 'success');
        setJsonModal(null);
        invalidate();
      }
    },
    onError: (e) => showToast(getApiErrorMessage(e), 'error'),
  });

  const deleteMut = useMutation({
    mutationFn: async (id: number) => {
      const { data } = await client.delete<BaseResponse<unknown>>(`${basePath}/${id}`);
      return data;
    },
    onSuccess: (d) => {
      if (!d.success) showToast(d.message ?? 'Delete failed', 'error');
      else {
        showToast('Deleted', 'success');
        setDeleteId(null);
        invalidate();
      }
    },
    onError: (e) => showToast(getApiErrorMessage(e), 'error'),
  });

  const auxPostMut = useMutation({
    mutationFn: async ({ path, body }: { path: string; body: unknown }) => {
      const { data } = await client.post<BaseResponse<unknown>>(path, body);
      return data;
    },
    onSuccess: (d) => {
      if (!d.success) showToast(d.message ?? 'Action failed', 'error');
      else {
        showToast('Action completed', 'success');
        setJsonModal(null);
        invalidate();
      }
    },
    onError: (e) => showToast(getApiErrorMessage(e), 'error'),
  });

  const openCreate = () => {
    const initial = '{\n  \n}';
    setJsonDraft(initial);
    setJsonModal({ mode: 'create', title: `Create — ${spec.title}`, initialJson: initial });
  };

  const openEdit = (row: Record<string, unknown>) => {
    setDetailRecord(row);
    const initial = JSON.stringify(stripIdForPayload(row), null, 2);
    setJsonDraft(initial);
    setJsonModal({
      mode: 'edit',
      title: `Edit #${row.id} — ${spec.title}`,
      initialJson: initial,
    });
  };

  const openAuxPost = (relPath: string, usesId: boolean, label: string) => {
    let path = `${basePath}/${relPath}`;
    if (usesId) {
      const rid = Number(
        idLookup || (detailRecord?.id != null ? String(detailRecord.id) : '') || NaN
      );
      if (!Number.isFinite(rid)) {
        showToast('Select a row, open detail, or enter an id in Lookup before this action.', 'warning');
        return;
      }
      path = `${basePath}/${rid}/${relPath.replace(/^\{id\}\//, '')}`;
    }
    const initial = '{\n  \n}';
    setJsonDraft(initial);
    setJsonModal({
      mode: 'auxPost',
      title: `${label} — ${spec.title}`,
      initialJson: initial,
      postPath: path,
    });
  };

  const submitJsonModal = () => {
    let body: unknown;
    try {
      body = JSON.parse(jsonDraft || '{}');
    } catch {
      showToast('Invalid JSON body', 'error');
      return;
    }
    if (jsonModal?.mode === 'create') void createMut.mutateAsync(body);
    else if (jsonModal?.mode === 'edit') {
      const id = Number(detailRecord?.id ?? idLookup);
      if (!Number.isFinite(id)) {
        showToast('Missing record id for update', 'error');
        return;
      }
      void updateMut.mutateAsync({ id, body });
    } else if (jsonModal?.mode === 'auxPost' && jsonModal.postPath) {
      void auxPostMut.mutateAsync({ path: jsonModal.postPath, body });
    }
  };

  const onRowClick = useCallback((row: Record<string, unknown>) => {
    setDetailRecord(row);
    setDetailOpen(true);
    if (row.id != null) setIdLookup(String(row.id));
  }, []);

  const showListSection = segments.length > 0;
  const notificationsLayout = spec.module === 'communication' && spec.path === 'notifications';

  return (
    <Stack spacing={3}>
      {variant === 'registry' ? (
        <Alert severity="info" sx={{ borderRadius: 2 }}>
          Administrator / integration surface — JSON payloads are intentional here so every contract stays reachable
          without codegen. Use module business menus for day-to-day operations.
        </Alert>
      ) : null}
      <PageHeader title={spec.title} subtitle={`${spec.module.toUpperCase()} · data registry`} />

      <Stack direction={{ xs: 'column', md: 'row' }} spacing={2} alignItems={{ md: 'flex-end' }} flexWrap="wrap">
        {showListSection && spec.hasPagedList ? (
          <Box sx={{ flex: '1 1 240px', minWidth: 200 }}>
            <TriVitaTextField
              label="Search"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              onKeyDown={(e) => {
                if (e.key === 'Enter') setSearchApplied(search);
              }}
              helperText="Server-side search on the active list. Press Enter to apply."
            />
          </Box>
        ) : null}
        {spec.hasGetById ? (
          <Box sx={{ flex: '1 1 200px', minWidth: 180 }}>
            <TriVitaTextField
              label="Lookup by ID"
              value={idLookup}
              onChange={(e) => setIdLookup(e.target.value)}
              helperText="Numeric primary key"
            />
          </Box>
        ) : null}
        <Stack direction="row" spacing={1} flexWrap="wrap" useFlexGap sx={{ pb: 0.5 }}>
          {spec.hasGetById ? (
            <TriVitaButton startIcon={<Search />} variant="contained" onClick={() => void runIdLookup()}>
              Load
            </TriVitaButton>
          ) : null}
          <TriVitaButton startIcon={<Refresh />} variant="outlined" onClick={() => void listQuery.refetch()}>
            Refresh
          </TriVitaButton>
          {spec.hasCreate ? (
            <TriVitaButton startIcon={<Add />} color="secondary" onClick={openCreate}>
              Create
            </TriVitaButton>
          ) : null}
        </Stack>
      </Stack>

      {notificationsLayout ? (
        <Alert severity="info">
          This module uses multiple paged collections. Pick a tab, then search or page results.
        </Alert>
      ) : null}

      {segments.length > 1 ? (
        <Tabs
          value={segmentIdx}
          onChange={(_, v) => {
            setSegmentIdx(v);
            setPage(0);
          }}
          sx={{ borderBottom: 1, borderColor: 'divider' }}
        >
          {segments.map((s, i) => (
            <Tab key={`${s.kind}-${i}`} label={s.label} value={i} />
          ))}
        </Tabs>
      ) : null}

      <QueryStateBar isLoading={listQuery.isFetching} isError={listQuery.isError} error={listQuery.error} />
      {listQuery.data && listQuery.data.success === false ? (
        <Alert severity="warning">{listQuery.data.message ?? 'Request failed'}</Alert>
      ) : null}

      {showListSection ? (
        <DataTable
          tableAriaLabel={spec.title}
          columns={[
            ...columns,
            {
              id: '__actions',
              label: 'Actions',
              minWidth: 160,
              format: (row) => (
                <Stack direction="row" spacing={0.5}>
                  <Tooltip title="View">
                    <IconButton size="small" onClick={() => onRowClick(row)} aria-label="view">
                      <OpenInNew fontSize="small" />
                    </IconButton>
                  </Tooltip>
                  {spec.hasUpdate ? (
                    <Tooltip title="Edit">
                      <IconButton size="small" onClick={() => openEdit(row)} aria-label="edit">
                        <Edit fontSize="small" />
                      </IconButton>
                    </Tooltip>
                  ) : null}
                  {spec.hasDelete ? (
                    <Tooltip title="Delete">
                      <IconButton
                        size="small"
                        color="error"
                        onClick={() => setDeleteId(Number(row.id))}
                        aria-label="delete"
                      >
                        <Delete fontSize="small" />
                      </IconButton>
                    </Tooltip>
                  ) : null}
                </Stack>
              ),
            },
          ]}
          rows={rows}
          rowKey={(r) => (r.id != null && r.id !== '' ? String(r.id) : `tmp-${JSON.stringify(r).slice(0, 48)}`)}
          totalCount={totalCount}
          page={page}
          pageSize={pageSize}
          onPageChange={(p, ps) => {
            setPage(p);
            setPageSize(ps);
          }}
          loading={listQuery.isLoading}
        />
      ) : (
        <Alert severity="info">No paged list is configured for this route. Use ID lookup or workflow actions.</Alert>
      )}

      {spec.auxiliaryPosts.length > 0 ? (
        <Box>
          <Typography variant="subtitle2" gutterBottom>
            Workflow actions
          </Typography>
          <Stack direction="row" spacing={1} flexWrap="wrap" useFlexGap>
            {spec.auxiliaryPosts.map((a) => (
              <TriVitaButton
                key={a.relPath + a.label}
                variant="outlined"
                size="small"
                onClick={() => openAuxPost(a.relPath, a.usesId, a.label)}
              >
                {a.label}
              </TriVitaButton>
            ))}
          </Stack>
        </Box>
      ) : null}

      <Drawer
        anchor="right"
        open={detailOpen}
        onClose={() => setDetailOpen(false)}
        PaperProps={{ sx: { width: { xs: '100%', sm: 440 } } }}
      >
        <Stack spacing={2} sx={{ p: 2 }}>
          <Typography variant="h6">Detail</Typography>
          <Typography
            component="pre"
            sx={{ m: 0, fontSize: 12, overflow: 'auto', bgcolor: 'grey.50', p: 2, borderRadius: 1 }}
          >
            {JSON.stringify(detailRecord, null, 2)}
          </Typography>
          {detailRecord?.id != null && spec.hasUpdate ? (
            <TriVitaButton onClick={() => detailRecord && openEdit(detailRecord)} startIcon={<Edit />}>
              Edit in modal
            </TriVitaButton>
          ) : null}
        </Stack>
      </Drawer>

      <TriVitaModal
        open={Boolean(jsonModal)}
        onClose={() => setJsonModal(null)}
        title={jsonModal?.title ?? ''}
        actions={
          <>
            <TriVitaButton onClick={() => setJsonModal(null)}>Cancel</TriVitaButton>
            <TriVitaButton variant="contained" onClick={() => submitJsonModal()}>
              Submit
            </TriVitaButton>
          </>
        }
      >
        <TriVitaTextField
          label="JSON body"
          multiline
          minRows={12}
          value={jsonDraft}
          onChange={(e) => setJsonDraft(e.target.value)}
          InputProps={{ sx: { fontFamily: 'ui-monospace, monospace', fontSize: 13 } }}
        />
      </TriVitaModal>

      <TriVitaModal
        open={deleteId != null}
        onClose={() => setDeleteId(null)}
        title="Confirm delete"
        actions={
          <>
            <TriVitaButton onClick={() => setDeleteId(null)}>Cancel</TriVitaButton>
            <TriVitaButton
              color="error"
              variant="contained"
              onClick={() => deleteId != null && void deleteMut.mutateAsync(deleteId)}
            >
              Delete
            </TriVitaButton>
          </>
        }
      >
        <Typography>Delete record #{deleteId}? This cannot be undone.</Typography>
      </TriVitaModal>
    </Stack>
  );
}
