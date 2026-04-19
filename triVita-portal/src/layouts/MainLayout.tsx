import {
  AppBar,
  Avatar,
  Box,
  Breadcrumbs,
  Divider,
  Drawer,
  IconButton,
  Link as MuiLink,
  List,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Menu,
  MenuItem,
  Toolbar,
  Tooltip,
  Typography,
  useTheme,
} from '@mui/material';
import {
  ChevronLeft,
  ChevronRight,
  Home,
  Logout,
  Menu as MenuIcon,
  NotificationsNone,
  ViewSidebar,
} from '@mui/icons-material';
import { Suspense, useMemo, useState } from 'react';
import { Link, Outlet, useLocation, useNavigate } from 'react-router-dom';
import { PageLoader } from '@/components/common/PageLoader';
import { TriVitaLogo } from '@/components/common/TriVitaLogo';
import { useAppDispatch, useAppSelector } from '@/store/hooks';
import { clearSession, setFacilityId } from '@/store/slices/authSlice';
import { hasPermission } from '@/utils/permissions';
import { mainNavigation, utilityNavigation } from './navigation';
import { useQuery } from '@tanstack/react-query';
import { listFacilities } from '@/services/sharedService';
import { postLogout } from '@/services/authApi';
import { STORAGE_KEYS } from '@/utils/storageKeys';

const drawerWidthExpanded = 268;
const drawerWidthCollapsed = 88;

function pathToBreadcrumbs(pathname: string) {
  const segments = pathname.split('/').filter(Boolean);
  const acc: { label: string; to: string }[] = [];
  let p = '';
  segments.forEach((seg) => {
    p += `/${seg}`;
    acc.push({ label: seg.replace(/-/g, ' '), to: p });
  });
  return acc;
}

export function MainLayout() {
  const theme = useTheme();
  const [mobileOpen, setMobileOpen] = useState(false);
  const [desktopCollapsed, setDesktopCollapsed] = useState(false);
  const [anchor, setAnchor] = useState<null | HTMLElement>(null);
  const drawerWidth = desktopCollapsed ? drawerWidthCollapsed : drawerWidthExpanded;
  const location = useLocation();
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const user = useAppSelector((s) => s.auth.user);

  const canShared = hasPermission(user?.permissions, 'shared.api');

  const { data: facilitiesRes } = useQuery({
    queryKey: ['facilities', user?.tenantId],
    queryFn: () => listFacilities(),
    enabled: Boolean(user && canShared),
  });

  const facilities = facilitiesRes?.success ? facilitiesRes.data ?? [] : [];

  const visibleNav = useMemo(
    () => mainNavigation.filter((n) => hasPermission(user?.permissions, n.permission)),
    [user?.permissions]
  );

  const crumbs = pathToBreadcrumbs(location.pathname);

  const drawer = (
    <Box sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
      <Toolbar sx={{ gap: 1, py: 1.5, justifyContent: 'space-between' }}>
        <TriVitaLogo size="sm" showText={!desktopCollapsed} />
        <Tooltip title={desktopCollapsed ? 'Expand sidebar' : 'Collapse sidebar'}>
          <IconButton
            size="small"
            onClick={() => setDesktopCollapsed((c) => !c)}
            sx={{ display: { xs: 'none', md: 'inline-flex' } }}
            aria-label="toggle sidebar width"
          >
            {desktopCollapsed ? <ViewSidebar fontSize="small" /> : <ChevronLeft fontSize="small" />}
          </IconButton>
        </Tooltip>
      </Toolbar>
      <Divider />
      <List sx={{ flex: 1, px: 1, py: 2 }}>
        <MuiLink component={Link} to="/dashboard" underline="none" color="inherit">
          <Tooltip title={desktopCollapsed ? 'Home' : ''} placement="right">
            <ListItemButton selected={location.pathname === '/dashboard'} sx={{ borderRadius: 1, mx: 0.5 }}>
              <ListItemIcon sx={{ minWidth: 36, display: desktopCollapsed ? 'block' : 'none' }}>
                <Home fontSize="small" color="primary" />
              </ListItemIcon>
              <ListItemText
                primary="Home"
                primaryTypographyProps={{ fontWeight: 600 }}
                sx={{ display: desktopCollapsed ? 'none' : 'block' }}
              />
            </ListItemButton>
          </Tooltip>
        </MuiLink>
        {utilityNavigation.map((u) => {
          const UIcon = u.icon;
          return (
            <MuiLink key={u.path} component={Link} to={u.path} underline="none" color="inherit">
              <Tooltip title={desktopCollapsed ? u.label : ''} placement="right">
                <ListItemButton selected={location.pathname === u.path} sx={{ borderRadius: 1, mx: 0.5 }}>
                  <ListItemIcon sx={{ minWidth: 36 }}>
                    <UIcon fontSize="small" color="secondary" />
                  </ListItemIcon>
                  <ListItemText primary={u.label} sx={{ display: desktopCollapsed ? 'none' : 'block' }} />
                </ListItemButton>
              </Tooltip>
            </MuiLink>
          );
        })}
        {visibleNav.map((mod) => (
          <Box key={mod.path}>
            <Typography
              variant="caption"
              color="text.secondary"
              sx={{ px: 2, pt: 2, pb: 0.5, display: desktopCollapsed ? 'none' : 'block' }}
            >
              {mod.label}
            </Typography>
            {(mod.children ?? []).map((c) => {
              const ModIcon = mod.icon;
              return (
                <MuiLink key={c.path} component={Link} to={c.path} underline="none" color="inherit">
                  <Tooltip title={desktopCollapsed ? c.label : ''} placement="right">
                    <ListItemButton selected={location.pathname === c.path} sx={{ borderRadius: 1, mx: 0.5 }}>
                      <ListItemIcon sx={{ minWidth: 36 }}>
                        <ModIcon fontSize="small" color="primary" />
                      </ListItemIcon>
                      <ListItemText primary={c.label} sx={{ display: desktopCollapsed ? 'none' : 'block' }} />
                    </ListItemButton>
                  </Tooltip>
                </MuiLink>
              );
            })}
          </Box>
        ))}
      </List>
    </Box>
  );

  const logout = async () => {
    const rt = sessionStorage.getItem(STORAGE_KEYS.refreshToken);
    if (rt) {
      try {
        await postLogout(rt);
      } catch {
        /* still clear local session */
      }
    }
    dispatch(clearSession());
    navigate('/login', { replace: true });
  };

  return (
    <Box sx={{ display: 'flex', minHeight: '100vh' }}>
      <AppBar
        position="fixed"
        elevation={0}
        sx={{
          width: { md: `calc(100% - ${drawerWidth}px)` },
          ml: { md: `${drawerWidth}px` },
          bgcolor: 'background.paper',
          color: 'text.primary',
          borderBottom: '1px solid',
          borderColor: 'divider',
        }}
      >
        <Toolbar>
          <IconButton
            color="inherit"
            edge="start"
            onClick={() => setMobileOpen(true)}
            sx={{ mr: 2, display: { md: 'none' } }}
            aria-label="open menu"
          >
            <MenuIcon />
          </IconButton>
          <Box sx={{ flex: 1, display: 'flex', alignItems: 'center', gap: 2 }}>
            <TriVitaLogo size="md" />
            <Breadcrumbs separator={<ChevronRight fontSize="small" />} sx={{ display: { xs: 'none', sm: 'flex' } }}>
              <MuiLink component={Link} to="/dashboard" underline="hover" color="inherit">
                Portal
              </MuiLink>
              {crumbs.map((c, i) =>
                i === crumbs.length - 1 ? (
                  <Typography key={c.to} color="text.primary" sx={{ textTransform: 'capitalize' }}>
                    {c.label}
                  </Typography>
                ) : (
                  <MuiLink
                    key={c.to}
                    component={Link}
                    to={c.to}
                    underline="hover"
                    color="inherit"
                    sx={{ textTransform: 'capitalize' }}
                  >
                    {c.label}
                  </MuiLink>
                )
              )}
            </Breadcrumbs>
          </Box>
          {canShared && facilities.length > 0 ? (
            <Box component="label" sx={{ mr: 2, display: { xs: 'none', md: 'flex' }, alignItems: 'center', gap: 1 }}>
              <Typography variant="caption" color="text.secondary">
                Facility
              </Typography>
              <select
                value={user?.facilityId ?? ''}
                onChange={(e) => {
                  const v = e.target.value ? Number(e.target.value) : null;
                  dispatch(setFacilityId(v));
                }}
                style={{ padding: 8, borderRadius: 8, border: `1px solid ${theme.palette.divider}` }}
              >
                <option value="">Tenant default</option>
                {facilities.map((f) => {
                  const row = f as { id?: number; facilityName?: string };
                  return (
                    <option key={String(row.id)} value={String(row.id)}>
                      {row.facilityName ?? `#${row.id}`}
                    </option>
                  );
                })}
              </select>
            </Box>
          ) : null}
          <Typography variant="body2" color="text.secondary" sx={{ mr: 1, display: { xs: 'none', md: 'block' } }}>
            Tenant {user?.tenantId}
          </Typography>
          <Tooltip title="Notifications">
            <IconButton color="inherit" size="small">
              <NotificationsNone />
            </IconButton>
          </Tooltip>
          <IconButton onClick={(e) => setAnchor(e.currentTarget)} size="small" sx={{ ml: 1 }}>
            <Avatar sx={{ width: 32, height: 32, bgcolor: 'primary.main', fontSize: 14 }}>
              {user?.email?.[0]?.toUpperCase() ?? '?'}
            </Avatar>
          </IconButton>
          <Menu anchorEl={anchor} open={Boolean(anchor)} onClose={() => setAnchor(null)}>
            <MenuItem disabled>
              <Typography variant="body2">{user?.email}</Typography>
            </MenuItem>
            <MenuItem disabled>
              <Typography variant="caption" color="text.secondary">
                {user?.roles?.join(', ') || user?.role}
              </Typography>
            </MenuItem>
            <Divider />
            <MenuItem
              onClick={() => {
                setAnchor(null);
                void logout();
              }}
            >
              <Logout fontSize="small" sx={{ mr: 1 }} /> Log out
            </MenuItem>
          </Menu>
        </Toolbar>
      </AppBar>
      <Box component="nav" sx={{ width: { md: drawerWidth }, flexShrink: { md: 0 } }}>
        <Drawer
          variant="temporary"
          open={mobileOpen}
          onClose={() => setMobileOpen(false)}
          ModalProps={{ keepMounted: true }}
          sx={{
            display: { xs: 'block', md: 'none' },
            '& .MuiDrawer-paper': { boxSizing: 'border-box', width: drawerWidth },
          }}
        >
          {drawer}
        </Drawer>
        <Drawer
          variant="permanent"
          sx={{
            display: { xs: 'none', md: 'block' },
            '& .MuiDrawer-paper': { boxSizing: 'border-box', width: drawerWidth },
          }}
          open
        >
          {drawer}
        </Drawer>
      </Box>
      <Box
        component="main"
        sx={{
          flexGrow: 1,
          p: { xs: 2, sm: 3 },
          width: { md: `calc(100% - ${drawerWidth}px)` },
          mt: 8,
          backgroundColor: 'background.default',
          minHeight: '100vh',
        }}
      >
        <Suspense fallback={<PageLoader message="Loading module…" />}>
          <Outlet />
        </Suspense>
      </Box>
    </Box>
  );
}
