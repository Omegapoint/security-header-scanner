import { createRootRoute, createRoute, createRouter, redirect } from '@tanstack/react-router';
import { z } from 'zod';
import App from '../App.tsx';
import { isUrl } from '../helpers/isUrl.ts';
import Entrypoint from '../routes/Entrypoint.tsx';
import ScanResult from '../routes/ScanResult.tsx';
import { ensureLoaded } from './store.tsx';

const rootRoute = createRootRoute({
  component: App,
});

const indexRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: '/',
  component: Entrypoint,
});

const scanSearchSchema = z.object({
  target: z.string().optional().catch(undefined),
  followRedirects: z.boolean().optional().catch(false),
});

export type ScanQuerySchema = z.infer<typeof scanSearchSchema>;

export const scanRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: 'scan',
  validateSearch: scanSearchSchema,
  component: ScanResult,
  beforeLoad: ({ search: { target } }) => {
    if (!target || !isUrl(target)) {
      throw redirect({
        to: '/',
      });
    }
  },
  loaderDeps: ({ search }) => ({ search }),
  loader: async ({ deps: { search } }) => await ensureLoaded(search),
});

const routeTree = rootRoute.addChildren([indexRoute, scanRoute]);

const router = createRouter({ routeTree });

declare module '@tanstack/react-router' {
  interface Register {
    router: typeof router;
  }
}

export default router;
