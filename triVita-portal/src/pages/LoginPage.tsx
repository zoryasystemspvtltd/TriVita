import { Alert, Box, Button, Card, CardContent, Container, Typography } from '@mui/material';
import { Form, Formik } from 'formik';
import * as Yup from 'yup';
import { useNavigate, useLocation } from 'react-router-dom';
import { FormikTextField } from '@/components/common/FormikTextField';
import { TriVitaLogo } from '@/components/common/TriVitaLogo';
import { useAppDispatch } from '@/store/hooks';
import { setSession } from '@/store/slices/authSlice';
import { getMe, postToken } from '@/services/authApi';
import { useState } from 'react';
import { getApiErrorMessage } from '@/utils/errorMap';

const schema = Yup.object({
  email: Yup.string().email('Valid email required').required('Required'),
  password: Yup.string().required('Required'),
  tenantId: Yup.number().min(1).required(),
});

export function LoginPage() {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const location = useLocation();
  const from = (location.state as { from?: { pathname: string } } | null)?.from?.pathname ?? '/dashboard';
  const [error, setError] = useState<string | null>(null);

  return (
    <Box
      minHeight="100vh"
      display="flex"
      alignItems="center"
      justifyContent="center"
      sx={{
        background: 'radial-gradient(ellipse at top, rgba(13, 92, 74, 0.1), transparent 55%)',
        bgcolor: 'background.default',
      }}
    >
      <Container maxWidth="sm">
        <Box textAlign="center" mb={4}>
          <Box display="flex" justifyContent="center" mb={2}>
            <TriVitaLogo size="lg" showText />
          </Box>
          <Typography variant="h5" color="text.secondary" fontWeight={500}>
            Healthcare enterprise portal
          </Typography>
        </Box>
        <Card variant="outlined" sx={{ borderRadius: 2, boxShadow: 3 }}>
          <CardContent sx={{ p: 4 }}>
            {error ? (
              <Alert severity="error" sx={{ mb: 2 }}>
                {error}
              </Alert>
            ) : null}
            <Formik
              initialValues={{ email: '', password: '', tenantId: 1 }}
              validationSchema={schema}
              onSubmit={async (values, { setSubmitting }) => {
                setError(null);
                try {
                  const tokenRes = await postToken(values.email, values.password, values.tenantId);
                  if (!tokenRes.success || !tokenRes.data?.accessToken) {
                    setError(tokenRes.message ?? 'Login failed');
                    return;
                  }
                  const me = await getMe(tokenRes.data.accessToken);
                  if (!me.success || !me.data) {
                    setError(me.message ?? 'Could not load profile');
                    return;
                  }
                  dispatch(
                    setSession({
                      user: me.data,
                      accessToken: tokenRes.data.accessToken,
                      refreshToken: tokenRes.data.refreshToken ?? '',
                      tenantId: me.data.tenantId,
                      facilityId: me.data.facilityId ?? null,
                    })
                  );
                  navigate(from, { replace: true });
                } catch (e) {
                  setError(getApiErrorMessage(e));
                } finally {
                  setSubmitting(false);
                }
              }}
            >
              {({ isSubmitting }) => (
                <Form>
                  <FormikTextField name="email" label="Email" type="email" autoComplete="username" />
                  <FormikTextField name="password" label="Password" type="password" autoComplete="current-password" />
                  <FormikTextField name="tenantId" label="Tenant ID" type="number" />
                  <Button
                    type="submit"
                    variant="contained"
                    size="large"
                    fullWidth
                    disabled={isSubmitting}
                    sx={{ mt: 2 }}
                  >
                    Sign in
                  </Button>
                </Form>
              )}
            </Formik>
            <Typography variant="caption" color="text.secondary" display="block" mt={2}>
              Demo (dev): admin@demo.local / ChangeMe!123 — see HealthcarePlatform README.
            </Typography>
          </CardContent>
        </Card>
      </Container>
    </Box>
  );
}
