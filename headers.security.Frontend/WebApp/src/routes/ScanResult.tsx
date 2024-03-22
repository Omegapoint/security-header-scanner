import { Stack, Typography } from '@mui/joy';
import { useStore } from '@tanstack/react-store';
import { AppCard } from '../components/AppCard.tsx';
import { scanRoute } from '../data/router.tsx';
import { store } from '../data/store.tsx';
import { ScanHeaders } from '../features/ScanResult/ScanHeaders.tsx';
import { ScanRawHeaders } from '../features/ScanResult/ScanRawHeaders.tsx';
import { ScanSummary } from '../features/ScanResult/ScanSummary.tsx';
import { ErrorPage } from './ErrorPage.tsx';

const ScanResult = () => {
  const { apiResponse, loading } = useStore(store, (state) => state);
  const search = scanRoute.useSearch();
  console.log(JSON.parse(JSON.stringify(search ?? {})));

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
      <AppCard title="Raw Result" expandable>
        <pre>
          <Typography variant="outlined" fontFamily="code">
            {JSON.stringify(apiResponse, null, 2)}
          </Typography>
        </pre>
      </AppCard>
    </Stack>
  );
};

export default ScanResult;
