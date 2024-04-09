import { Stack } from '@mui/joy';
import { createFileRoute, redirect } from '@tanstack/react-router';
import { useStore } from '@tanstack/react-store';
import { z } from 'zod';
import { TargetKind } from '../contracts/apiTypes.ts';
import { ensureLoaded, store } from '../data/store.tsx';
import { ErrorPage } from '../features/ScanResult/ErrorPage.tsx';
import { ScanHeaders } from '../features/ScanResult/ScanHeaders.tsx';
import { ScanRawHeaders } from '../features/ScanResult/ScanRawHeaders.tsx';
import { ScanSummary } from '../features/ScanResult/ScanSummary.tsx';
import { isUrl } from '../helpers/isUrl.ts';

const scanSearchSchema = z.object({
  target: z.string().optional().catch(undefined),
  followRedirects: z.boolean().optional().catch(false),
  kind: z.nativeEnum(TargetKind).optional().catch(undefined),
});

export type ScanQuerySchema = z.infer<typeof scanSearchSchema>;

const ScanResult = () => {
  const { apiResponse, loading } = useStore(store, (state) => state);

  if (loading) {
    return <></>;
  }

  if (apiResponse == undefined || apiResponse.results.length < 1) {
    return <ErrorPage />;
  }

  const firstServerResult = apiResponse!.results[0];

  if (firstServerResult.error != null) {
    return <ErrorPage reason={firstServerResult.error} />;
  }

  return (
    <Stack alignItems="center" width="100%" spacing={5}>
      <ScanSummary data={firstServerResult} response={apiResponse} />
      <ScanHeaders data={firstServerResult} response={apiResponse} />
      <ScanRawHeaders rawHeaders={firstServerResult.result.rawHeaders ?? {}} />
    </Stack>
  );
};

export const Route = createFileRoute('/scan')({
  validateSearch: scanSearchSchema.parse,
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
