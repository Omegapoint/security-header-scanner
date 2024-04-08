import { Table } from '@mui/joy';
import { AppTableRow } from '../../../components/AppTableRow.tsx';
import { FormattedDate } from '../../../components/FormattedDate.tsx';
import { ApiResponse } from '../../../contracts/apiResponse.ts';
import { ServerResult } from '../../../contracts/apiTypes.ts';
import { SummaryTableHeaders } from './SummaryTableHeaders.tsx';
import { SummaryTableIPsRow } from './SummaryTableIPsRow.tsx';
import { SummaryTableTarget } from './SummaryTableTarget.tsx';
import { SummaryTableUrls } from './SummaryTableUrls.tsx';

interface SummaryTableProps {
  data: ServerResult;
  response: ApiResponse;
}

export const SummaryTable = ({ data, response }: SummaryTableProps) => {
  return (
    <Table sx={{ '& tr th:nth-of-type(1)': { width: '7em' } }}>
      <tbody>
        <AppTableRow rowLabel="URL">
          <SummaryTableUrls data={data} />
        </AppTableRow>
        <AppTableRow rowLabel="Report time">
          <FormattedDate date={response.scanStart} />
        </AppTableRow>
        <SummaryTableIPsRow ips={data.ips} />
        <AppTableRow rowLabel="Scan type">
          <SummaryTableTarget result={data.result} />
        </AppTableRow>
        <AppTableRow rowLabel="Headers">
          <SummaryTableHeaders data={data} />
        </AppTableRow>
      </tbody>
    </Table>
  );
};
