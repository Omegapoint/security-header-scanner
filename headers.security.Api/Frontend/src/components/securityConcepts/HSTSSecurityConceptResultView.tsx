import { Typography } from '@mui/joy';
import { HstsSecurityConceptResult } from '../../contracts/securityConcepts/hstsSecurityConceptResult.ts';

const Day = 86_400;

export const HSTSSecurityConceptResultView = (hstsResult: HstsSecurityConceptResult) => {
  const { maxAge, includeSubdomains } = hstsResult.processedValue;
  const noPolicy = maxAge == null;

  const MaxAge = () => {
    if (noPolicy) {
      return <Typography>No Strict-Transport-Security header configured.</Typography>;
    }

    const getExpiryTime = (maxAge: number) => {
      const count = Math.floor(maxAge / Day);
      const suffix = count > 1 ? 'days' : 'day';
      return `${count} ${suffix}`;
    };

    return (
      <Typography>
        The configured Strict-Transport-Security header has an expiry time of{' '}
        <Typography variant="outlined">{getExpiryTime(maxAge)}</Typography>.
      </Typography>
    );
  };

  const IncludeSubdomains = () => {
    const suffix = includeSubdomains
      ? 'is effective for the scanned domain and any subdomains'
      : 'only affects the current domain';
    return (
      <Typography>
        The{' '}
        <Typography fontFamily="code" variant="outlined">
          includeSubdomains
        </Typography>{' '}
        directive is set to{' '}
        <Typography fontFamily="code" variant="outlined">
          {String(includeSubdomains)}
        </Typography>
        , which means the policy {suffix}.
      </Typography>
    );
  };

  return (
    <Typography>
      <MaxAge /> {!noPolicy && <IncludeSubdomains />}
    </Typography>
  );
};
