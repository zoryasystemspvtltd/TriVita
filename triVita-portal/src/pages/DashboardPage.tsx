import { Box, Card, CardActionArea, CardContent, Stack, Typography } from '@mui/material';
import { useMemo } from 'react';
import { Link } from 'react-router-dom';
import { useAppSelector } from '@/store/hooks';
import { hasPermission } from '@/utils/permissions';
import { mainNavigation } from '@/layouts/navigation';
import { EmptyState } from '@/components/common/EmptyState';

export function DashboardPage() {
  const user = useAppSelector((s) => s.auth.user);
  const cards = useMemo(() => {
    return mainNavigation
      .filter((m) => hasPermission(user?.permissions, m.permission))
      .map((m) => ({
        title: m.label,
        description: `${m.children?.length ?? 0} workspaces`,
        to: m.children?.[0]?.path ?? m.path,
      }));
  }, [user?.permissions]);

  if (!cards.length) {
    return <EmptyState title="No modules assigned" subtitle="Your account has no module permissions yet." />;
  }

  return (
    <Box>
      <Typography variant="h5" gutterBottom>
        Welcome{user?.email ? `, ${user.email.split('@')[0]}` : ''}
      </Typography>
      <Typography color="text.secondary" paragraph>
        Choose a module to continue. Use the sidebar for full navigation.
      </Typography>
      <Stack direction="row" flexWrap="wrap" gap={2} sx={{ mt: 1 }}>
        {cards.map((c) => (
          <Box key={c.title} sx={{ width: { xs: '100%', sm: 'calc(50% - 8px)', md: 'calc(33.33% - 11px)' } }}>
            <Card elevation={1} sx={{ borderRadius: 2, height: '100%' }}>
              <CardActionArea component={Link} to={c.to} sx={{ height: '100%' }}>
                <CardContent>
                  <Typography variant="h6">{c.title}</Typography>
                  <Typography variant="body2" color="text.secondary">
                    {c.description}
                  </Typography>
                </CardContent>
              </CardActionArea>
            </Card>
          </Box>
        ))}
      </Stack>
    </Box>
  );
}
