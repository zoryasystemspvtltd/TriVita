import { http, HttpResponse } from 'msw';

const origin = 'http://127.0.0.1:5999';

/** MSW handlers for unit tests (critical auth flow). Matches .env.test API bases. */
export const handlers = [
  http.post(`${origin}/api/v1/auth/token`, async () =>
    HttpResponse.json({
      success: true,
      data: {
        accessToken: 'test-access',
        tokenType: 'Bearer',
        expiresInSeconds: 3600,
        refreshToken: 'test-refresh',
        refreshExpiresInSeconds: 86400,
      },
    })
  ),
  http.get(`${origin}/api/v1/auth/me`, () =>
    HttpResponse.json({
      success: true,
      data: {
        userId: 1,
        email: 'test@demo.local',
        tenantId: 1,
        facilityId: 1,
        role: 'Admin',
        roles: ['Admin'],
        permissions: ['*'],
      },
    })
  ),
];
