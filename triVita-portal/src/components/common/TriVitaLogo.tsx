import { Box, Typography, useMediaQuery, useTheme } from '@mui/material';
import logoSm from '@/assets/images/trivita-logo-sm.png';
import logoMd from '@/assets/images/trivita-logo-md.png';
import logoLg from '@/assets/images/trivita-logo-lg.png';

export type LogoSize = 'sm' | 'md' | 'lg';

const HEIGHT: Record<LogoSize, number> = { sm: 28, md: 36, lg: 48 };

const SRC: Record<LogoSize, string> = { sm: logoSm, md: logoMd, lg: logoLg };

export interface TriVitaLogoProps {
  size?: LogoSize;
  /** When true, shows wordmark beside the mark (desktop-friendly). */
  showText?: boolean;
  className?: string;
}

/**
 * Responsive TriVita mark; collapses to icon-only sizing on small screens when combined with layout.
 */
export function TriVitaLogo({ size = 'md', showText = false, className }: TriVitaLogoProps) {
  const theme = useTheme();
  const compact = useMediaQuery(theme.breakpoints.down('sm'));
  const effective: LogoSize = compact ? 'sm' : size;
  const h = HEIGHT[effective];
  const logoSrc = SRC[effective];

  return (
    <Box
      className={className}
      sx={{
        display: 'inline-flex',
        alignItems: 'center',
        gap: 1.25,
        transition: 'opacity 0.2s ease, transform 0.2s ease',
        '&:hover': { opacity: 0.92, transform: 'scale(1.02)' },
      }}
    >
      <Box
        component="img"
        src={logoSrc}
        alt="TriVita"
        sx={{
          height: h,
          width: 'auto',
          maxWidth: showText ? Math.min(h * 3.2, 140) : h * 2.4,
          objectFit: 'contain',
          display: 'block',
        }}
      />
      {showText && !compact && (
        <Typography
          variant="h6"
          sx={{
            fontWeight: 700,
            letterSpacing: '-0.02em',
            color: 'primary.dark',
            lineHeight: 1.1,
          }}
        >
          TriVita
        </Typography>
      )}
    </Box>
  );
}
