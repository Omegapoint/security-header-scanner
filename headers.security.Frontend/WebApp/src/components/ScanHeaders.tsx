import { List, ListItem, Stack, Table, Typography } from '@mui/joy';
import { ApiResponse } from '../contracts/apiResponse.ts';
import { ISecurityConceptResult, SecurityConceptHandlerName, ServerResult } from '../contracts/apiTypes.ts';
import { CspSecurityConceptResult } from '../contracts/securityConcepts/cspSecurityConcept.ts';
import { HstsSecurityConceptResult } from '../contracts/securityConcepts/hstsSecurityConceptResult.ts';
import { AppCard } from './AppCard.tsx';
import { AppTableRow } from './AppTableRow.tsx';
import { GradingIcon } from './GradingIcon.tsx';
import { CSPSecurityConceptResultView } from './securityConcepts/CSPSecurityConceptResultView.tsx';
import { HSTSSecurityConceptResultView } from './securityConcepts/HSTSSecurityConceptResultView.tsx';

interface ScanHeadersProps {
  data: ServerResult;
  response: ApiResponse;
}

const GenericScanHeadersTableExpansion = ({ handlerResult }: { handlerResult: ISecurityConceptResult }) => {
  const value = handlerResult.processedValue;

  const stringValue = typeof value === 'string' ? value : JSON.stringify(value, null, 2);
  const getSummary = (handlerName: SecurityConceptHandlerName): string | undefined => {
    switch (handlerName) {
      case 'HTTP Strict Transport Security':
        break;
      case 'X-Content-Type-Options':
        break;
      case 'X-Frame-Options':
        break;
      case 'Permissions-Policy':
        break;
      case 'Referrer-Policy':
        break;
      case 'Server':
        return 'The "Server" header is used to inform the client what server software is in use.';
      case 'Access-Control-Allow-Origin':
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

  return (
    <List sx={{ padding: 0, margin: '-4px 0' }}>
      {handlerResult.infos.map((i, idx) => (
        <ListItem sx={{ paddingLeft: 0 }} key={idx}>
          <Typography fontSize="sm">"{i.message}"</Typography>
        </ListItem>
      ))}
    </List>
  );
};

const ScanHeadersTable = ({ handlerResults }: { handlerResults: ISecurityConceptResult[] }) => {
  const getExpansion = (handlerResult: ISecurityConceptResult) => {
    if (handlerResult?.processedValue == null) {
      return null;
    }

    switch (handlerResult?.handlerName) {
      case 'Content Security Policy':
        return <CSPSecurityConceptResultView cspResult={handlerResult as CspSecurityConceptResult} />;
      case 'HTTP Strict Transport Security':
        return <HSTSSecurityConceptResultView hstsResult={handlerResult as HstsSecurityConceptResult} />;
    }

    return <GenericScanHeadersTableExpansion handlerResult={handlerResult} />;
  };

  return (
    <Table
      sx={(theme) => ({
        '& th[scope="col"]': theme.variants.solid.primary,
        tableLayout: 'auto',
        '--TableCell-paddingX': '1.2em',
      })}
    >
      <thead>
        <tr>
          <th scope="col">Header name</th>
          <th scope="col">Grade</th>
          <th scope="col" style={{ width: '100%' }}>
            Message
          </th>
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
          >
            <Stack height="100%" alignItems="center">
              <Stack height="2em" direction="row" alignItems="center">
                <GradingIcon grade={h.grade} />
              </Stack>
            </Stack>
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
