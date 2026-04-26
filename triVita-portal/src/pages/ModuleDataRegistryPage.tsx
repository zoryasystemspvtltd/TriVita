import { Box, List, ListItemButton, ListItemText, Paper, Stack, TextField, Typography } from '@mui/material';
import { useMemo, useState } from 'react';
import { useSearchParams } from 'react-router-dom';
import type { ApiRegistryModule, ApiRegistryEntry } from '@/generated/apiRegistry';
import { generatedApiRegistry } from '@/generated/apiRegistry';
import { EnterpriseCrudResourcePage } from '@/components/resource/EnterpriseCrudResourcePage';
import { PageHeader } from '@/components/layout/PageHeader';

function isUsableInPortal(e: ApiRegistryEntry): boolean {
  if (e.path === 'identity-admin') return false;
  return (
    e.hasPagedList ||
    e.hasGetById ||
    e.hasCreate ||
    e.hasUpdate ||
    e.hasDelete ||
    e.auxiliaryPosts.length > 0
  );
}

export function ModuleDataRegistryPage({ module }: { module: ApiRegistryModule }) {
  const [params, setParams] = useSearchParams();
  const q = params.get('resource') ?? '';

  const entries = useMemo(
    () => generatedApiRegistry.filter((e) => e.module === module && isUsableInPortal(e)),
    [module]
  );

  const [filter, setFilter] = useState('');
  const filtered = useMemo(() => {
    const f = filter.trim().toLowerCase();
    if (!f) return entries;
    return entries.filter((e) => e.path.toLowerCase().includes(f) || e.title.toLowerCase().includes(f));
  }, [entries, filter]);

  const selected = entries.find((e) => e.path === q) ?? null;

  const selectPath = (path: string) => {
    const next = new URLSearchParams(params);
    next.set('resource', path);
    setParams(next, { replace: true });
  };

  return (
    <Stack spacing={3} sx={{ height: '100%' }}>
      <PageHeader
        title="API registry"
        subtitle={`Administrator fallback — full CRUD, ID lookup, and workflow actions mapped to live APIs (${module.toUpperCase()}).`}
      />
      <Stack direction={{ xs: 'column', lg: 'row' }} spacing={3} alignItems="stretch">
        <Paper
          variant="outlined"
          elevation={0}
          sx={{
            flex: { lg: '0 0 320px' },
            bgcolor: '#ffffff',
            border: '1px solid #e5e7eb',
            borderRadius: '10px',
            boxShadow: '0 2px 6px rgba(0,0,0,0.05)',
            p: 1,
            maxHeight: { lg: 'calc(100vh - 220px)' },
            overflow: 'hidden',
            display: 'flex',
            flexDirection: 'column',
          }}
        >
          <Box sx={{ p: 1 }}>
            <TextField
              size="small"
              fullWidth
              placeholder="Filter resources…"
              value={filter}
              onChange={(e) => setFilter(e.target.value)}
              aria-label="Filter API resources"
            />
          </Box>
          <List dense sx={{ overflow: 'auto', flex: 1, py: 0 }}>
            {filtered.length === 0 ? (
              <Typography variant="body2" color="text.secondary" sx={{ px: 2, py: 2 }}>
                No generated resources for this module (or all entries are filtered out).
              </Typography>
            ) : null}
            {filtered.map((e) => (
              <ListItemButton
                key={e.path}
                selected={e.path === q}
                onClick={() => selectPath(e.path)}
                alignItems="flex-start"
                sx={{ borderRadius: 1, mx: 0.5 }}
              >
                <ListItemText
                  primary={e.title}
                  secondary={e.path}
                  primaryTypographyProps={{ variant: 'body2', fontWeight: e.path === q ? 600 : 400 }}
                  secondaryTypographyProps={{ variant: 'caption', sx: { wordBreak: 'break-all' } }}
                />
              </ListItemButton>
            ))}
          </List>
          <Typography variant="caption" color="text.secondary" sx={{ px: 1.5, py: 1 }}>
            {filtered.length} resource{filtered.length === 1 ? '' : 's'}
          </Typography>
        </Paper>
        <Box sx={{ flex: 1, minWidth: 0 }}>
          {selected ? (
            <EnterpriseCrudResourcePage def={selected} />
          ) : (
            <Paper
              variant="outlined"
              elevation={0}
              sx={{
                p: 3,
                border: '1px solid #e5e7eb',
                borderRadius: '10px',
                boxShadow: '0 2px 6px rgba(0,0,0,0.05)',
                bgcolor: '#ffffff',
              }}
            >
              <Typography color="text.secondary">
                Select a resource from the list, or open a deep link with <code>?resource=medicine</code> using the
                registered path segment.
              </Typography>
            </Paper>
          )}
        </Box>
      </Stack>
    </Stack>
  );
}
