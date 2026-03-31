import type { ReactElement } from 'react';
import { render, screen } from '@testing-library/react';
import { ThemeProvider } from '@mui/material';
import { describe, expect, it } from 'vitest';
import { trivitaTheme } from '@/app/theme';
import { TriVitaLogo } from './TriVitaLogo';

function wrap(ui: ReactElement) {
  return render(<ThemeProvider theme={trivitaTheme}>{ui}</ThemeProvider>);
}

describe('TriVitaLogo', () => {
  it('renders accessible brand image', () => {
    wrap(<TriVitaLogo size="md" />);
    expect(screen.getByRole('img', { name: /trivita/i })).toBeInTheDocument();
  });

  it('shows wordmark when showText is true', () => {
    wrap(<TriVitaLogo size="lg" showText />);
    expect(screen.getByText('TriVita')).toBeInTheDocument();
  });
});
