import { Stack, Typography } from '@mui/joy';
import { HstsSecurityConceptResult } from '../../contracts/securityConcepts/hstsSecurityConceptResult.ts';

const OneYear = 31536000;
const HalfYear = 15768000;
const Month = 2628000;

interface HSTSSecurityConceptResultViewProps {
  hstsResult: HstsSecurityConceptResult;
}

export const HSTSSecurityConceptResultView = ({ hstsResult }: HSTSSecurityConceptResultViewProps) => {
  const { maxAge, preload, includeSubdomains } = hstsResult.processedValue;

  if (maxAge == null) {
    // TODO: need to know if site is fetched over HTTPS or not here
    return <Typography>No {hstsResult.headerName} header configured for site fetched over HTTPS.</Typography>;
  }

  const getExpiryTime = (maxAge: number) => {
    if (maxAge >= OneYear) {
      const count = Math.floor(maxAge / OneYear);
      const suffix = count > 1 ? 'years' : 'year';
      return `${count} ${suffix}`;
    }
    if (maxAge >= HalfYear) {
      return 'half a year';
    }

    if (maxAge >= Month) {
      const count = Math.floor(maxAge / Month);
      const suffix = count > 1 ? 'months' : 'month';
      return `${count} ${suffix}`;
    }

    return 'less than a month';
  };

  return (
    <Stack>
      <Typography>
        The configured {hstsResult.headerName} header has an expiry time of {getExpiryTime(maxAge)}.
      </Typography>
    </Stack>
  );
};
