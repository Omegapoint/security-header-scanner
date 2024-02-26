import { Stack } from '@mui/joy';
import { SiteSelector } from './SiteSelector.tsx';

export const Banner = () => (
  <Stack height="100%" width="100%" justifyContent="flex-end" alignItems="center">
    <SiteSelector />
  </Stack>
);
