import { List, ListItem, Stack, Table, Typography } from '@mui/joy';
import { zip } from 'lodash';
import { AppCard } from '../../components/AppCard.tsx';
import { AppTableRow } from '../../components/AppTableRow.tsx';
import { ImpactIcon } from '../../components/ImpactIcon.tsx';
import { LinkDecorator } from '../../components/LinkDecorator.tsx';
import { CSPSecurityConceptResultView } from '../../components/securityConcepts/CSPSecurityConceptResultView.tsx';
import { HSTSSecurityConceptResultView } from '../../components/securityConcepts/HSTSSecurityConceptResultView.tsx';
import { ApiResponse } from '../../contracts/apiResponse.ts';
import {
  ISecurityConceptResult,
  SecurityConceptHandlerName,
  SecurityConceptResultInfo,
  ServerResult,
} from '../../contracts/apiTypes.ts';
import { CspSecurityConceptResult } from '../../contracts/securityConcepts/cspSecurityConcept.ts';
import { HstsSecurityConceptResult } from '../../contracts/securityConcepts/hstsSecurityConceptResult.ts';

interface ScanHeadersProps {
  data: ServerResult;
  response: ApiResponse;
}

const GenericScanHeadersTableExpansion = ({ handlerResult }: { handlerResult: ISecurityConceptResult }) => {
  const value = handlerResult.processedValue;

  const stringValue = typeof value === 'string' ? value : JSON.stringify(value, null, 2);
  const getSummary = (handlerName: SecurityConceptHandlerName): string | undefined => {
    switch (handlerName) {
      case SecurityConceptHandlerName.AccessControlAllowOrigin:
        break;
      case SecurityConceptHandlerName.ContentSecurityPolicy:
        break;
      case SecurityConceptHandlerName.HTTPStrictTransportSecurity:
        break;
      case SecurityConceptHandlerName.PermissionsPolicy:
        break;
      case SecurityConceptHandlerName.ReferrerPolicy:
        break;
      case SecurityConceptHandlerName.Server:
        return 'The "Server" header is used to inform the client what server software is in use.';
      case SecurityConceptHandlerName.XContentTypeOptions:
        break;
      case SecurityConceptHandlerName.XFrameOptions:
        break;
    }
  };
  const summary = getSummary(handlerResult.handlerName);

  return (
    <Stack>
      {summary && <Typography>{summary}</Typography>}
      <Stack spacing={1}>
        <Typography minWidth="fit-content" paddingTop="0.2em">
          Effective value:
        </Typography>
        <pre>
          <Typography variant="outlined" fontFamily="code">
            {stringValue}
          </Typography>
        </pre>
      </Stack>
    </Stack>
  );
};

const ScanHeadersInfos = ({ handlerResult }: { handlerResult: ISecurityConceptResult }) => {
  const infos = handlerResult.infos;

  if (infos.length < 1) {
    return <Typography color="neutral">No action necessary.</Typography>;
  }

  const constructMessage = ({ message, formatTokens }: SecurityConceptResultInfo) => {
    const parts = zip(message.split(/{\d+}/g), formatTokens ?? []);
    return parts.map(([messagePart, tokens], idx) => (
      <span key={idx}>
        <Typography>{messagePart}</Typography>
        {tokens?.map((token, idx) => (
          <span key={idx}>
            <Typography variant="outlined" fontFamily="code" whiteSpace="nowrap">
              {token}
            </Typography>
            {idx != tokens?.length - 1 ? ', ' : ''}
          </span>
        ))}
      </span>
    ));
  };

  const getDecorator = ({ externalLink }: SecurityConceptResultInfo) => {
    if (externalLink != null) {
      return <LinkDecorator url={externalLink} />;
    }
  };

  return (
    <List sx={{ padding: 0, margin: '-4px 0' }}>
      {handlerResult.infos.map((i, idx) => (
        <ListItem sx={{ paddingLeft: 0, paddingRight: 0 }} key={idx}>
          <Typography
            fontSize="sm"
            sx={(theme) => ({
              [theme.breakpoints.only('xs')]: {
                '--Typography-fontSize': '9pt',
              },
            })}
          >
            {constructMessage(i)}
          </Typography>
          {getDecorator(i)}
        </ListItem>
      ))}
    </List>
  );
};

const getDocumentationLink = (handlerName: SecurityConceptHandlerName) => {
  switch (handlerName) {
    case SecurityConceptHandlerName.AccessControlAllowOrigin:
      return 'https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Access-Control-Allow-Origin';
    case SecurityConceptHandlerName.ContentSecurityPolicy:
      return 'https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy';
    case SecurityConceptHandlerName.HTTPStrictTransportSecurity:
      return 'https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Strict-Transport-Security';
    case SecurityConceptHandlerName.PermissionsPolicy:
      return 'https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Permissions-Policy';
    case SecurityConceptHandlerName.ReferrerPolicy:
      return 'https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Referrer-Policy';
    case SecurityConceptHandlerName.Server:
      break;
    case SecurityConceptHandlerName.XContentTypeOptions:
      return 'https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Content-Type-Options';
    case SecurityConceptHandlerName.XFrameOptions:
      break;
  }
};

const ScanHeadersTable = ({ handlerResults }: { handlerResults: ISecurityConceptResult[] }) => {
  const getExpansion = (handlerResult: ISecurityConceptResult) => {
    if (handlerResult?.processedValue == null) {
      return null;
    }

    switch (handlerResult?.handlerName) {
      case SecurityConceptHandlerName.ContentSecurityPolicy:
        return CSPSecurityConceptResultView(handlerResult as CspSecurityConceptResult);
      case SecurityConceptHandlerName.HTTPStrictTransportSecurity:
        return HSTSSecurityConceptResultView(handlerResult as HstsSecurityConceptResult);
    }

    return <GenericScanHeadersTableExpansion handlerResult={handlerResult} />;
  };

  const getDecorator = ({ handlerName }: ISecurityConceptResult) => {
    const link = getDocumentationLink(handlerName);

    if (link) {
      const url = new URL(link);
      return <LinkDecorator url={url} />;
    }
  };

  return (
    <Table
      sx={(theme) => ({
        '& th[scope="col"]': theme.variants.solid.primary,
        '& thead th:nth-of-type(1)': { width: '30%' },
        '& thead th:nth-of-type(2)': { width: '1.5em' },
        '& thead th:last-child': { width: '2.25em' },
        [theme.breakpoints.up('xs')]: { '--TableCell-paddingX': '1.2em' },
        [theme.breakpoints.only('xs')]: { '--TableCell-paddingX': '0.5em' },
      })}
    >
      <thead>
        <tr>
          <th scope="col">Header name</th>
          <th scope="col"></th>
          <th scope="col">Message(s)</th>
          <th scope="col"></th>
        </tr>
      </thead>
      <tbody>
        {handlerResults.map((h) => (
          <AppTableRow
            rowLabel={h.headerName}
            key={h.handlerName}
            expansion={getExpansion(h)}
            expansionColSpanPadding={2}
            fullWidthExpansion
            labelDecorator={getDecorator(h)}
            thinCols={[1]}
          >
            <ImpactIcon impact={h.impact} />
            <ScanHeadersInfos handlerResult={h} />
          </AppTableRow>
        ))}
      </tbody>
    </Table>
  );
};

export const ScanHeaders = ({ data }: ScanHeadersProps) => {
  const { handlerResults } = data.result;

  return (
    <AppCard
      title="Header Information"
      overflowComponent={<ScanHeadersTable handlerResults={handlerResults} />}
      overflowProps={{ sx: { padding: 0 } }}
    />
  );
};
