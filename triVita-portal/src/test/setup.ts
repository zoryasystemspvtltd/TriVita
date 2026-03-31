import '@testing-library/jest-dom/vitest';
import { afterAll, afterEach, beforeAll } from 'vitest';
import { store } from '@/store';
import { clearSession } from '@/store/slices/authSlice';
import { server } from './server';

beforeAll(() => server.listen({ onUnhandledRequest: 'error' }));
afterEach(() => {
  server.resetHandlers();
  sessionStorage.clear();
  store.dispatch(clearSession());
});
afterAll(() => server.close());
