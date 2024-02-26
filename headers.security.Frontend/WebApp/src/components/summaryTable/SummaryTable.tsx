import { Table } from '@mui/joy';
import { ApiResponse } from '../../contracts/apiResponse.ts';
import { ServerResult } from '../../contracts/apiTypes.ts';
import { AppTableRow } from '../AppTableRow.tsx';
import { FormattedDate } from '../FormattedDate.tsx';
import { SummaryTableHeaders } from './SummaryTableHeaders.tsx';
import { SummaryTableIPsRow } from './SummaryTableIPsRow.tsx';
import { SummaryTableUrls } from './SummaryTableUrls.tsx';

interface SummaryTableProps {
  data: ServerResult;
  response: ApiResponse;
}

export const SummaryTable = ({ data, response }: SummaryTableProps) => {
  return (
    <Table sx={{ tableLayout: 'auto' }}>
      <tbody>
        <AppTableRow rowLabel="URL">
          <SummaryTableUrls data={data} />
        </AppTableRow>
        <AppTableRow rowLabel="Report time">
          <FormattedDate date={response.scanStart} />
        </AppTableRow>
        <SummaryTableIPsRow ips={data.ips} />
        <AppTableRow rowLabel="Headers">
          <SummaryTableHeaders data={data} />
        </AppTableRow>
      </tbody>
    </Table>
  );
};
