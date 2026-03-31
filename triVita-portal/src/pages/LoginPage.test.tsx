import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { Provider } from 'react-redux';
import { MemoryRouter, Route, Routes } from 'react-router-dom';
import { ThemeProvider } from '@mui/material';
import { describe, expect, it } from 'vitest';
import { store } from '@/store';
import { clearSession } from '@/store/slices/authSlice';
import { trivitaTheme } from '@/app/theme';
import { LoginPage } from './LoginPage';

describe('LoginPage', () => {
  it('submits credentials and reaches dashboard', async () => {
    store.dispatch(clearSession());
    const user = userEvent.setup();
    render(
      <Provider store={store}>
        <ThemeProvider theme={trivitaTheme}>
          <MemoryRouter initialEntries={['/login']}>
            <Routes>
              <Route path="/login" element={<LoginPage />} />
              <Route path="/dashboard" element={<div>Dashboard OK</div>} />
            </Routes>
          </MemoryRouter>
        </ThemeProvider>
      </Provider>
    );

    await user.type(screen.getByLabelText(/email/i), 'test@demo.local');
    await user.type(screen.getByLabelText(/^password$/i), 'secret');
    await user.click(screen.getByRole('button', { name: /sign in/i }));

    await waitFor(() => {
      expect(screen.getByText('Dashboard OK')).toBeInTheDocument();
    });
  });
});
