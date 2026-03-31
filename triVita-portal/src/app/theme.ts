import { createTheme } from '@mui/material/styles';

/** Medical-grade teal / blue palette aligned with TriVita branding. */
export const trivitaTheme = createTheme({
  palette: {
    mode: 'light',
    primary: { main: '#0d7377', dark: '#095a5d', light: '#3d9a9e' },
    secondary: { main: '#1a5f7a', dark: '#0f3d52', light: '#4a85a0' },
    background: { default: '#f4f8f9', paper: '#ffffff' },
  },
  shape: { borderRadius: 10 },
  typography: {
    fontFamily: '"DM Sans", "Segoe UI", Roboto, Helvetica, Arial, sans-serif',
    h5: { fontWeight: 600 },
    h6: { fontWeight: 600 },
  },
  components: {
    MuiButton: { styleOverrides: { root: { textTransform: 'none' } } },
    MuiDrawer: {
      styleOverrides: { paper: { borderRight: '1px solid rgba(13, 115, 119, 0.12)' } },
    },
  },
});
