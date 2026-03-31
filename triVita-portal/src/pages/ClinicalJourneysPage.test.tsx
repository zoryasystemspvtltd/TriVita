import { render, screen } from '@testing-library/react';
import { Provider } from 'react-redux';
import { MemoryRouter } from 'react-router-dom';
import { ThemeProvider } from '@mui/material';
import { describe, expect, it } from 'vitest';
import { store } from '@/store';
import { setSession } from '@/store/slices/authSlice';
import { trivitaTheme } from '@/app/theme';
import ClinicalJourneysPage from './ClinicalJourneysPage';

describe('ClinicalJourneysPage', () => {
  it('renders journey maps for authenticated user', () => {
    store.dispatch(
      setSession({
        user: {
          userId: 1,
          email: 'a@b.c',
          tenantId: 1,
          role: 'Admin',
          roles: ['Admin'],
          permissions: ['*'],
        },
        accessToken: 'x',
        refreshToken: 'y',
        tenantId: 1,
        facilityId: 1,
      })
    );
    render(
      <Provider store={store}>
        <ThemeProvider theme={trivitaTheme}>
          <MemoryRouter>
            <ClinicalJourneysPage />
          </MemoryRouter>
        </ThemeProvider>
      </Provider>
    );
    expect(screen.getByText(/Clinical journeys/i)).toBeInTheDocument();
    expect(screen.getByText(/Patient → appointment → lab → pharmacy/i)).toBeInTheDocument();
  });
});
