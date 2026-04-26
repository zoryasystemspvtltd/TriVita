import { ExpandMore } from '@mui/icons-material';
import {
  Box,
  Collapse,
  Divider,
  IconButton,
  List,
  ListItemButton,
  ListItemText,
  Menu,
  MenuItem,
  Stack,
  Tooltip,
  Typography,
} from '@mui/material';
import { useCallback, useEffect, useState } from 'react';
import { Link, useLocation } from 'react-router-dom';
import {
  makeGroupKey,
  isNavPathActive,
  findActiveNavKeys,
  loadNavSidebarState,
  saveNavSidebarState,
  type NavItem,
} from './navigation';

const TRANSITION = 220;

type Props = {
  items: readonly NavItem[];
  desktopCollapsed: boolean;
  onSelectNav: () => void;
};

function stripGroupsForModule(prev: Record<string, boolean>, modulePath: string) {
  const n = { ...prev };
  Object.keys(n).forEach((k) => {
    if (k.startsWith(`${modulePath}::`)) delete n[k];
  });
  return n;
}

export function AppSideNav({ items, desktopCollapsed, onSelectNav }: Props) {
  const location = useLocation();
  const p = location.pathname;
  const [openModules, setOpenModules] = useState<Record<string, boolean>>({});
  const [openGroups, setOpenGroups] = useState<Record<string, boolean>>({});
  const [modMenu, setModMenu] = useState<null | { mod: NavItem; anchor: HTMLElement }>(null);

  useEffect(() => {
    const s = loadNavSidebarState();
    setOpenModules(() => {
      const o: Record<string, boolean> = {};
      s.modules.forEach((m) => {
        o[m] = true;
      });
      return o;
    });
    setOpenGroups(() => {
      const o: Record<string, boolean> = {};
      s.groups.forEach((g) => {
        o[g] = true;
      });
      return o;
    });
  }, []);

  useEffect(() => {
    for (const mod of items) {
      const a = findActiveNavKeys(p, mod);
      const gid = a.groupId;
      if (gid) {
        setOpenModules((prev) => ({ ...prev, [mod.path]: true }));
        setOpenGroups((prev) => ({ ...prev, [makeGroupKey(mod.path, gid)]: true }));
        return;
      }
    }
  }, [p, items]);

  useEffect(() => {
    const modules = Object.keys(openModules).filter((k) => openModules[k]);
    const groups = Object.keys(openGroups).filter((k) => openGroups[k]);
    saveNavSidebarState(modules, groups);
  }, [openModules, openGroups]);

  const toggleMod = useCallback(
    (modulePath: string) => {
      setOpenModules((prev) => {
        const n = { ...prev, [modulePath]: !prev[modulePath] };
        if (!n[modulePath]) {
          setOpenGroups((g) => stripGroupsForModule(g, modulePath));
        }
        return n;
      });
    },
    []
  );

  const toggleGrp = useCallback((gKey: string) => {
    setOpenGroups((prev) => ({ ...prev, [gKey]: !prev[gKey] }));
  }, []);

  const isChildActive = useCallback((itemPath: string) => isNavPathActive(p, itemPath), [p]);

  const anyActiveInMod = (mod: NavItem) => mod.groups.some((g) => g.children.some((c) => isChildActive(c.path)));

  if (desktopCollapsed) {
    return (
      <Stack role="navigation" alignItems="center" spacing={0.5} py={1} width={1} aria-label="Module menu">
        {items.map((mod) => {
          const ModIcon = mod.icon;
          return (
            <Box key={mod.path}>
              <Tooltip title={mod.label} placement="right">
                <span>
                  <IconButton
                    size="small"
                    color={anyActiveInMod(mod) ? 'primary' : 'default'}
                    onClick={(e) => setModMenu({ mod, anchor: e.currentTarget })}
                    aria-label={mod.label}
                    aria-current={anyActiveInMod(mod) ? 'true' : undefined}
                  >
                    <ModIcon fontSize="small" />
                  </IconButton>
                </span>
              </Tooltip>
            </Box>
          );
        })}
        <ModuleOverflowMenu
          modMenu={modMenu}
          isChildActive={isChildActive}
          onClose={() => setModMenu(null)}
          onNavigate={onSelectNav}
        />
      </Stack>
    );
  }

  return (
    <Box component="nav" aria-label="Application" sx={{ px: 0, py: 0 }}>
      {items.map((mod, modIndex) => {
        const ModIcon = mod.icon;
        const modKey = mod.path;
        const modOpen = openModules[modKey] === true;
        return (
          <Box key={modKey}>
            <ListItemButton
              onClick={() => toggleMod(modKey)}
              selected={!modOpen && anyActiveInMod(mod)}
              sx={{ borderRadius: 1, mx: 0.5, pl: 1, py: 0.75 }}
              aria-expanded={modOpen}
            >
              <Box sx={{ mr: 0.5, color: 'primary.main', display: 'flex' }}>
                <ModIcon fontSize="small" />
              </Box>
              <ListItemText
                primary={mod.label}
                primaryTypographyProps={{ variant: 'body2', fontWeight: 600, fontSize: '0.8125rem' }}
                sx={{ flex: 1, minWidth: 0, mr: 0.5 }}
              />
              <ExpandMore
                fontSize="small"
                sx={{
                  color: 'text.secondary',
                  transition: `transform ${TRANSITION}ms ease`,
                  transform: modOpen ? 'rotate(0deg)' : 'rotate(-90deg)',
                }}
              />
            </ListItemButton>
            <Collapse in={modOpen} timeout="auto" unmountOnExit={false}>
              <Box sx={{ pl: 0, borderLeft: '1px solid', borderColor: 'divider', ml: 1, mr: 0.5, mb: 0.5 }}>
                {mod.groups.map((g) => {
                  const gKey = makeGroupKey(modKey, g.id);
                  const gOpen = openGroups[gKey] === true;
                  const isEmpty = g.children.length === 0;
                  return (
                    <Box key={gKey} sx={{ pl: 0.5 }}>
                      <ListItemButton
                        dense
                        onClick={() => (isEmpty ? null : toggleGrp(gKey))}
                        disabled={isEmpty}
                        sx={{ borderRadius: 1, py: 0.4, pl: 0.75, minHeight: 34 }}
                        aria-expanded={!isEmpty ? gOpen : undefined}
                      >
                        <ListItemText
                          primary={g.label}
                          primaryTypographyProps={{ variant: 'caption', fontWeight: 600, textTransform: 'none' }}
                          sx={{ flex: 1 }}
                        />
                        {isEmpty ? null : (
                          <ExpandMore
                            fontSize="small"
                            sx={{
                              color: 'text.disabled',
                              transition: `transform ${TRANSITION}ms ease`,
                              transform: gOpen ? 'rotate(0deg)' : 'rotate(-90deg)',
                            }}
                          />
                        )}
                      </ListItemButton>
                      {isEmpty ? null : (
                        <Collapse in={gOpen} timeout="auto" unmountOnExit={false}>
                          <List disablePadding>
                            {g.children.map((c) => {
                              const active = isChildActive(c.path);
                              return (
                                <ListItemButton
                                  key={c.path}
                                  dense
                                  selected={active}
                                  component={Link}
                                  to={c.path}
                                  onClick={onSelectNav}
                                  sx={{
                                    borderRadius: 1,
                                    pl: 1.25,
                                    ml: 0.5,
                                    minHeight: 32,
                                    '&.Mui-selected': { bgcolor: 'action.selected' },
                                    '&.Mui-selected:hover': { bgcolor: 'action.selected' },
                                    '&:hover': { bgcolor: 'action.hover' },
                                  }}
                                >
                                  <ListItemText
                                    primary={c.label}
                                    primaryTypographyProps={{
                                      variant: 'body2',
                                      noWrap: true,
                                      color: 'text.primary',
                                      fontSize: '0.8125rem',
                                      fontWeight: active ? 600 : 400,
                                    }}
                                  />
                                </ListItemButton>
                              );
                            })}
                          </List>
                        </Collapse>
                      )}
                      {isEmpty ? (
                        <Typography variant="caption" color="text.disabled" sx={{ pl: 1, display: 'block', pb: 0.5 }}>
                          No items
                        </Typography>
                      ) : null}
                    </Box>
                  );
                })}
              </Box>
            </Collapse>
            {modIndex < items.length - 1 ? (
              <Box sx={{ borderTop: 1, borderColor: 'divider', opacity: 0.4, my: 0.5, mx: 0.5 }} />
            ) : null}
          </Box>
        );
      })}
    </Box>
  );
}

function ModuleOverflowMenu({
  modMenu,
  isChildActive,
  onClose,
  onNavigate,
}: {
  modMenu: { mod: NavItem; anchor: HTMLElement } | null;
  isChildActive: (path: string) => boolean;
  onClose: () => void;
  onNavigate: () => void;
}) {
  if (!modMenu) return null;
  return (
    <Menu
      anchorEl={modMenu.anchor}
      open={Boolean(modMenu)}
      onClose={onClose}
      anchorOrigin={{ vertical: 'bottom', horizontal: 'left' }}
      transformOrigin={{ vertical: 'top', horizontal: 'left' }}
      slotProps={{ paper: { sx: { minWidth: 220, maxWidth: 320, py: 0.5, borderRadius: 1 } } }}
    >
      <Box sx={{ px: 1, py: 0.5 }}>
        <Typography variant="overline" color="text.secondary" display="block">
          {modMenu.mod.label}
        </Typography>
      </Box>
        {modMenu.mod.groups.map((g, i, arr) => {
        if (g.children.length === 0) {
          return (
            <Box key={g.id} sx={{ py: 0.5, px: 1 }}>
              <Typography variant="caption" fontWeight={600} color="text.secondary" display="block" gutterBottom>
                {g.label}
              </Typography>
              <Typography variant="caption" color="text.disabled" sx={{ pl: 0.5 }}>
                No items
              </Typography>
            </Box>
          );
        }
        return (
          <Box key={g.id} sx={{ py: 0.5 }}>
            <Typography variant="caption" fontWeight={600} color="text.secondary" sx={{ px: 1.5, display: 'block', mb: 0.25 }}>
              {g.label}
            </Typography>
            {g.children.map((c) => (
              <MenuItem
                key={c.path}
                selected={isChildActive(c.path)}
                component={Link}
                to={c.path}
                onClick={() => {
                  onNavigate();
                  onClose();
                }}
                dense
                sx={{ borderRadius: 0.5, fontSize: '0.8125rem', py: 0.5 }}
              >
                {c.label}
              </MenuItem>
            ))}
            {i < arr.length - 1 ? <Divider sx={{ my: 0.5 }} /> : null}
          </Box>
        );
      })}
    </Menu>
  );
}
