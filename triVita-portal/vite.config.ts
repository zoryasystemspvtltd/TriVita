import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import path from 'path';

/** Dev proxy avoids browser CORS to local .NET APIs (no backend changes). */
export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, 'src'),
    },
  },
  server: {
    proxy: {
      '/dev/identity': {
        target: 'http://localhost:5160',
        changeOrigin: true,
        rewrite: (p) => p.replace(/^\/dev\/identity/, ''),
      },
      '/dev/hms': {
        target: 'http://localhost:5146',
        changeOrigin: true,
        rewrite: (p) => p.replace(/^\/dev\/hms/, ''),
      },
      '/dev/lis': {
        target: 'http://localhost:5150',
        changeOrigin: true,
        rewrite: (p) => p.replace(/^\/dev\/lis/, ''),
      },
      '/dev/lms': {
        target: 'http://localhost:5151',
        changeOrigin: true,
        rewrite: (p) => p.replace(/^\/dev\/lms/, ''),
      },
      '/dev/pharmacy': {
        target: 'http://localhost:5152',
        changeOrigin: true,
        rewrite: (p) => p.replace(/^\/dev\/pharmacy/, ''),
      },
      '/dev/shared': {
        target: 'http://localhost:5153',
        changeOrigin: true,
        rewrite: (p) => p.replace(/^\/dev\/shared/, ''),
      },
      '/dev/communication': {
        target: 'http://localhost:5800',
        changeOrigin: true,
        rewrite: (p) => p.replace(/^\/dev\/communication/, ''),
      },
    },
  },
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: ['./src/test/setup.ts'],
  },
});
