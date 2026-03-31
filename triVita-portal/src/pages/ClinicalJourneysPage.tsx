import { Box, Button, Card, CardContent, Stack, Step, StepLabel, Stepper, Typography } from '@mui/material';
import { Link as RouterLink } from 'react-router-dom';
import { PageHeader } from '@/components/layout/PageHeader';
import { useAppSelector } from '@/store/hooks';
import { hasPermission, TriVitaPermissions } from '@/utils/permissions';

const clinicalJourney = [
  { label: 'Register patient', to: '/hms/patients', permission: TriVitaPermissions.HmsApi },
  { label: 'Book appointment / visit', to: '/hms/appointments', permission: TriVitaPermissions.HmsApi },
  { label: 'OPD queue', to: '/hms/opd', permission: TriVitaPermissions.HmsApi },
  { label: 'Lab test booking (LMS)', to: '/lms/bookings', permission: TriVitaPermissions.LmsApi },
  { label: 'Register barcode', to: '/lms/barcodes', permission: TriVitaPermissions.LmsApi },
  { label: 'Lab results (LIS)', to: '/lis/results', permission: TriVitaPermissions.LisApi },
  { label: 'Verify results', to: '/lis/verification', permission: TriVitaPermissions.LisApi },
  { label: 'Pharmacy dispensing', to: '/pharmacy/billing', permission: TriVitaPermissions.PharmacyApi },
];

const labJourney = [
  { label: 'Test master', to: '/lms/test-master', permission: TriVitaPermissions.LmsApi },
  { label: 'Test booking', to: '/lms/bookings', permission: TriVitaPermissions.LmsApi },
  { label: 'Barcode', to: '/lms/barcodes', permission: TriVitaPermissions.LmsApi },
  { label: 'Analyzer', to: '/lis/analyzer', permission: TriVitaPermissions.LisApi },
  { label: 'Results', to: '/lis/results', permission: TriVitaPermissions.LisApi },
  { label: 'Verification', to: '/lis/verification', permission: TriVitaPermissions.LisApi },
];

export default function ClinicalJourneysPage() {
  const user = useAppSelector((s) => s.auth.user);

  return (
    <Stack spacing={4}>
      <PageHeader
        title="Clinical journeys"
        subtitle="Step-based maps linking HMS → LMS → LIS → Pharmacy. Open each step in order for end-to-end workflows."
      />
      <Card variant="outlined" sx={{ borderRadius: 2 }}>
        <CardContent>
          <Typography variant="h6" gutterBottom>
            Patient → appointment → lab → pharmacy
          </Typography>
          <Typography variant="body2" color="text.secondary" paragraph>
            Typical cross-module flow. Steps you are not permitted to use are still listed for context.
          </Typography>
          <Stepper activeStep={-1} orientation="vertical" nonLinear sx={{ alignItems: 'stretch' }}>
            {clinicalJourney.map((s, i) => {
              const ok = hasPermission(user?.permissions, s.permission);
              return (
                <Step key={s.to} expanded>
                  <StepLabel optional={!ok ? <Typography variant="caption">No access</Typography> : undefined}>
                    <Stack direction="row" alignItems="center" gap={1} flexWrap="wrap">
                      <Typography component="span" variant="body2">
                        {i + 1}. {s.label}
                      </Typography>
                      {ok ? (
                        <Button component={RouterLink} to={s.to} size="small" variant="contained">
                          Open
                        </Button>
                      ) : null}
                    </Stack>
                  </StepLabel>
                </Step>
              );
            })}
          </Stepper>
        </CardContent>
      </Card>
      <Card variant="outlined" sx={{ borderRadius: 2 }}>
        <CardContent>
          <Typography variant="h6" gutterBottom>
            Test booking → barcode → result
          </Typography>
          <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1, mt: 2 }}>
            {labJourney.map((s) => {
              const ok = hasPermission(user?.permissions, s.permission);
              return (
                <Button
                  key={s.to}
                  component={RouterLink}
                  to={s.to}
                  variant={ok ? 'outlined' : 'text'}
                  disabled={!ok}
                  size="small"
                >
                  {s.label}
                </Button>
              );
            })}
          </Box>
        </CardContent>
      </Card>
    </Stack>
  );
}
