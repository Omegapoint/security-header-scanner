import { Stack, Typography } from '@mui/joy';
import { TypographyProps } from '@mui/joy/Typography/TypographyProps';
import { CspPolicy, CspSecurityConceptResult } from '../../contracts/securityConcepts/cspSecurityConcept.ts';

export const CSPSecurityConceptResultView = (cspResult: CspSecurityConceptResult) => {
  const { hasPolicy, all, effective } = cspResult.processedValue;

  if (!hasPolicy) {
    return null;
  }

  const hasMultiple = all.length > 1;

  const effectiveExplanation = hasMultiple
    ? 'The following is a simplified representation of how a spec compliant browser will see these multiple policies:'
    : 'Effective policy:';

  return (
    <Stack spacing={1}>
      {hasMultiple && (
        <Stack direction="column" spacing={1} alignItems="start">
          <Typography minWidth="fit-content" paddingTop="0.2em">
            The site has multiple Content Security Policies configured. When rendering the site, a spec compliant
            browser will apply each policy in series, as such, only directives and tokens present in all policies will
            be allowed.
          </Typography>
          <Stack direction="row" spacing={2} flexWrap="wrap" useFlexGap>
            {all.map((policy, idx) => (
              <CSPPolicyView policy={policy} effective={effective} key={idx} />
            ))}
          </Stack>
        </Stack>
      )}
      <Stack direction="column" spacing={1} alignItems="start">
        <Typography minWidth="fit-content" paddingTop="0.2em">
          {effectiveExplanation}
        </Typography>
        <CSPPolicyView effective={effective} policy={effective} hideOrigin />
      </Stack>
    </Stack>
  );
};

const DroppedProps: TypographyProps = {
  color: 'neutral',
  sx: { textDecoration: 'line-through' },
};

const CSPPolicyView = ({
  effective,
  policy,
  hideOrigin,
}: {
  effective: CspPolicy;
  policy: CspPolicy;
  hideOrigin?: boolean;
}) => {
  const effectiveDirectives = Object.keys(effective.directives);
  const directives = Object.keys(policy.directives);
  const droppedDirectives = directives.filter((key) => !effectiveDirectives.includes(key));

  const isLastToken = (directive: string, idx: number) => policy.directives[directive].length - 1 === idx;
  const isLast = (idx: number) => directives.length - 1 === idx;
  const directiveDropped = (directive: string) => droppedDirectives.includes(directive);
  const tokenDropped = (directive: string, token: string) =>
    !directiveDropped(directive) && !effective.directives[directive].includes(token);

  const directiveProps = (d: string): TypographyProps => (directiveDropped(d) ? DroppedProps : {});
  const tokenProps = (d: string, t: string): TypographyProps => (tokenDropped(d, t) ? DroppedProps : {});

  return (
    <Stack>
      {!hideOrigin && <Typography fontSize="small">Policy origin: {policy.source}</Typography>}
      <Typography fontSize="x-small" variant="outlined" fontFamily="code">
        {directives.map((directive, idx) => (
          <span key={idx}>
            <Typography {...directiveProps(directive)}>
              {directive}{' '}
              {policy.directives[directive].map((token, tokenIdx) => (
                <span key={tokenIdx}>
                  <Typography {...tokenProps(directive, token)}>{token}</Typography>
                  <Typography>{isLastToken(directive, tokenIdx) ? ';' : ' '}</Typography>
                </span>
              ))}
            </Typography>
            {!isLast(idx) && <br />}
          </span>
        ))}
      </Typography>
    </Stack>
  );
};
