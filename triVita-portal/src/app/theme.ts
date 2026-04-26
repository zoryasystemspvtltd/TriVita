import { createTheme } from '@mui/material/styles';

const pageBg = '#f7f9fb';
const cardBorder = '#e5e7eb';
const tableHeaderBg = '#f1f5f9';
const tableRowHover = '#f9fafb';
const textBorder = 'rgba(15, 23, 42, 0.08)';

const cardShadow = '0 2px 6px rgba(0, 0, 0, 0.05)';

export const trivitaTheme = createTheme({
  palette: {
    mode: 'light',
    primary: {
      main: '#0d5c4a',
      dark: '#084236',
      light: '#1a7a63',
      contrastText: '#ffffff',
    },
    secondary: {
      main: '#0f3d4d',
      dark: '#0a2a35',
      light: '#1f5a70',
    },
    background: {
      default: pageBg,
      paper: '#ffffff',
    },
    divider: textBorder,
    text: { primary: '#0f172a', secondary: '#64748b', disabled: 'rgba(15, 23, 42, 0.38)' },
    action: {
      active: 'rgba(15, 23, 42, 0.54)',
      hover: 'rgba(15, 23, 42, 0.05)',
      selected: 'rgba(13, 92, 74, 0.08)',
      disabled: 'rgba(15, 23, 42, 0.26)',
      disabledBackground: 'rgba(15, 23, 42, 0.08)',
    },
  },
  shape: { borderRadius: 10 },
  spacing: 8,
  typography: {
    fontFamily: '"DM Sans", "Segoe UI", Roboto, Helvetica, Arial, sans-serif',
    h5: {
      fontSize: '1.25rem',
      fontWeight: 600,
      lineHeight: 1.35,
    },
    h6: { fontSize: '1rem', fontWeight: 500, lineHeight: 1.4 },
    subtitle1: { fontSize: '1rem', fontWeight: 500, lineHeight: 1.45 },
    body1: { fontSize: '0.8125rem', lineHeight: 1.5 },
    body2: { fontSize: '0.75rem', lineHeight: 1.5 },
    caption: { fontSize: '0.75rem' },
    button: { fontSize: '0.8125rem', fontWeight: 500, lineHeight: 1.4, textTransform: 'none' as const },
    overline: { fontSize: '0.6875rem' },
  },
  components: {
    MuiCssBaseline: {
      styleOverrides: {
        body: { backgroundColor: pageBg },
      },
    },
    MuiPaper: {
      defaultProps: { elevation: 0 },
      styleOverrides: {
        root: {
          backgroundImage: 'none',
        },
        outlined: {
          border: `1px solid ${cardBorder}`,
          backgroundColor: '#ffffff',
          boxShadow: cardShadow,
          borderRadius: 10,
        },
        elevation0: { boxShadow: 'none' },
        elevation1: { boxShadow: cardShadow, border: `1px solid ${cardBorder}` },
      },
    },
    MuiCard: {
      defaultProps: { variant: 'outlined', elevation: 0 },
      styleOverrides: {
        root: {
          backgroundColor: '#ffffff',
          border: `1px solid ${cardBorder}`,
          boxShadow: cardShadow,
          borderRadius: 10,
          backgroundImage: 'none',
        },
      },
    },
    MuiButton: {
      defaultProps: { disableElevation: true },
      styleOverrides: {
        root: { borderRadius: 8, minHeight: 36, paddingLeft: 16, paddingRight: 16, fontWeight: 500 },
        sizeSmall: { minHeight: 32, fontSize: '0.75rem' },
        containedPrimary: {
          color: '#ffffff',
          backgroundColor: '#0d5c4a',
          '&:hover': { backgroundColor: '#084236' },
          '&:active': { backgroundColor: '#06332a' },
          '&.Mui-disabled': {
            color: 'rgba(255,255,255,0.85)',
            backgroundColor: 'rgba(13, 92, 74, 0.35)',
          },
        },
        containedSecondary: { '&:hover': { backgroundColor: '#0a2a35' } },
        outlined: {
          borderColor: 'rgba(15, 23, 42, 0.2)',
          '&:hover': { borderColor: 'rgba(15, 23, 42, 0.35)', backgroundColor: 'rgba(15, 23, 42, 0.04)' },
        },
      },
    },
    MuiTextField: {
      defaultProps: { size: 'small', margin: 'dense' },
    },
    MuiInputBase: {
      styleOverrides: {
        root: { fontSize: '0.8125rem' },
        sizeSmall: { minHeight: 36 },
      },
    },
    MuiFormLabel: {
      styleOverrides: { root: { fontSize: '0.75rem', fontWeight: 500, color: 'text.secondary' } },
    },
    MuiFormHelperText: {
      styleOverrides: { root: { fontSize: '0.7rem' } },
    },
    MuiInputLabel: {
      styleOverrides: {
        root: { fontSize: '0.75rem' },
        shrink: { fontWeight: 500 },
      },
    },
    MuiOutlinedInput: {
      styleOverrides: {
        notchedOutline: { borderColor: 'rgba(15, 23, 42, 0.2)' },
        root: {
          borderRadius: 8,
          '&.Mui-focused .MuiOutlinedInput-notchedOutline': {
            borderColor: '#0d5c4a',
            borderWidth: 1.5,
          },
        },
      },
    },
    MuiTable: {
      defaultProps: { size: 'medium' },
      styleOverrides: {
        root: { backgroundColor: '#ffffff' },
      },
    },
    MuiTableCell: {
      styleOverrides: {
        head: {
          backgroundColor: tableHeaderBg,
          fontSize: '0.875rem',
          fontWeight: 600,
          lineHeight: 1.35,
          color: '#0f172a',
          borderBottom: `1px solid ${cardBorder}`,
        },
        body: {
          fontSize: '0.8125rem',
          lineHeight: 1.5,
          padding: '12px 16px',
          borderBottom: `1px solid ${cardBorder}`,
        },
        stickyHeader: { backgroundColor: tableHeaderBg },
      },
    },
    MuiTableRow: {
      styleOverrides: {
        root: {
          'tbody &': { '&:hover': { backgroundColor: tableRowHover } },
        },
      },
    },
    MuiTableContainer: {
      styleOverrides: {
        root: { backgroundColor: '#ffffff' },
      },
    },
    MuiTablePagination: {
      styleOverrides: {
        root: { borderTop: `1px solid ${cardBorder}`, backgroundColor: '#fafbfc' },
        toolbar: { minHeight: 48, fontSize: '0.875rem' },
      },
    },
    MuiDrawer: {
      styleOverrides: { paper: { borderRight: `1px solid ${textBorder}` } },
    },
    MuiAppBar: {
      styleOverrides: { root: { backgroundImage: 'none' } },
    },
    MuiLink: { defaultProps: { underline: 'hover' } },
    MuiDialog: {
      styleOverrides: { paper: { borderRadius: 10, boxShadow: '0 12px 32px rgba(15, 23, 42, 0.12)' } },
    },
  },
});
