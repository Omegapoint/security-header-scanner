import { List, ListItem, Table, Typography } from '@mui/joy';
import { ListProps } from '@mui/joy/List/ListProps';
import { AppCard } from './AppCard.tsx';
import { AppTableRow } from './AppTableRow.tsx';

interface ScanRawHeadersProps {
  rawHeaders: Record<string, string[]>;
}

const ScanRawHeadersTableRow = ({ headerName, values }: { headerName: string; values: string[] }) => {
  const listProps: ListProps = {
    // @ts-expect-error missing type information
    size: 'xs',
    sx: { paddingTop: '0.4em', margin: '-4px 0' },
  };

  return (
    <AppTableRow rowLabel={headerName}>
      <List {...listProps}>
        {values.map((headerValue, idx) => (
          <ListItem sx={{ padding: '0 0 0.4em 0' }} key={idx}>
            <Typography variant="outlined" fontFamily="code" fontSize="sm" style={{ wordBreak: 'break-word' }}>
              {headerValue}
            </Typography>
          </ListItem>
        ))}
      </List>
    </AppTableRow>
  );
};

const ScanRawHeadersTable = ({ rawHeaders }: ScanRawHeadersProps) => {
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
          <th scope="col" style={{ width: '100%' }}>
            Message
          </th>
          <th scope="col"></th>
        </tr>
      </thead>
      <tbody>
        {Object.entries(rawHeaders).map(([headerName, values], idx) => (
          <ScanRawHeadersTableRow headerName={headerName} values={values} key={idx} />
        ))}
      </tbody>
    </Table>
  );
};

export const ScanRawHeaders = ({ rawHeaders }: ScanRawHeadersProps) => (
  <AppCard
    title="Raw Headers"
    overflowComponent={<ScanRawHeadersTable rawHeaders={rawHeaders} />}
    overflowProps={{ sx: { padding: 0 } }}
    hideable
  />
);
