import { Alert, Box, LinearProgress } from '@mui/material';
import { getApiErrorMessage } from '@/utils/errorMap';

/** Top-of-page loading + error feedback for React Query screens. */
export function QueryStateBar({
  isLoading,
  isError,
  error,
}: {
  isLoading: boolean;
  isError: boolean;
  error: unknown;
}) {
  return (
    <Box sx={{ mb: 2 }} role="status" aria-live="polite">
      {isLoading ? <LinearProgress sx={{ mb: 1 }} aria-label="Loading data" /> : null}
      {isError && error ? (
        <Alert severity="error">{getApiErrorMessage(error)}</Alert>
      ) : null}
    </Box>
  );
}
