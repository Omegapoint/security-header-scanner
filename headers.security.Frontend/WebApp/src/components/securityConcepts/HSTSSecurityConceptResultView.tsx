import { Stack, Typography } from '@mui/joy';
import { HstsSecurityConceptResult } from '../../contracts/securityConcepts/hstsSecurityConceptResult.ts';

interface HSTSSecurityConceptResultViewProps {
  hstsResult: HstsSecurityConceptResult;
}

export const HSTSSecurityConceptResultView = ({ hstsResult }: HSTSSecurityConceptResultViewProps) => {
  const hstsValue = hstsResult.processedValue;

  const hstsMissing = hstsValue.maxAge == null;

  if (hstsMissing) {
    // TODO: need to know if site is fetched over HTTPS or not here
    return <Typography>No {hstsResult.headerName} header configured for site fetched over HTTPS. A</Typography>;
  }

  return <Stack></Stack>;
};
