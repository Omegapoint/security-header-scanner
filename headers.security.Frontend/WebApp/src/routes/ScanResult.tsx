import { Stack, Typography } from '@mui/joy';
import { useStore } from '@tanstack/react-store';
import { AppCard } from '../components/AppCard.tsx';
import { ScanHeaders } from '../components/ScanHeaders.tsx';
import { ScanRawHeaders } from '../components/ScanRawHeaders.tsx';
import { ScanSummary } from '../components/ScanSummary.tsx';
import { store } from '../data/store.tsx';
import { ErrorPage } from './ErrorPage.tsx';

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
      <AppCard title="Raw Result" hideable>
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
