import { Navigate, useLocation } from 'react-router-dom';
import { PageLoader } from '@/components/common/PageLoader';
import { useAppSelector } from '@/store/hooks';
import { STORAGE_KEYS } from '@/utils/storageKeys';

export function RequireAuth({ children }: { children: React.ReactNode }) {
  const { user, hydrated } = useAppSelector((s) => s.auth);
  const location = useLocation();
  const token = typeof sessionStorage !== 'undefined' ? sessionStorage.getItem(STORAGE_KEYS.accessToken) : null;

  if (!hydrated) return <PageLoader message="Restoring session…" />;
  if (!token || !user) {
    return <Navigate to="/login" replace state={{ from: location }} />;
  }
  return <>{children}</>;
}
