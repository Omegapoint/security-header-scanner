import { Typography } from '@mui/joy';
import { ApiResponse } from '../contracts/apiResponse.ts';
import { ServerResult } from '../contracts/apiTypes.ts';
import { SecurityGrade } from '../contracts/securityGrade.ts';
import { AppCard } from './AppCard.tsx';
import { SummaryTable } from './summaryTable/SummaryTable.tsx';

interface ColourConfig {
  background: string;
  foreground: string;
}

const getBackgroundColour = (grade: SecurityGrade): string => {
  switch (grade) {
    case SecurityGrade.F:
      return '#DD4433';
    case SecurityGrade.E:
      return '#DD6633';
    case SecurityGrade.D:
      return '#DD9933';
    case SecurityGrade.C:
      return '#999933';
    case SecurityGrade.B:
      return '#669933';
    case SecurityGrade.A:
      return '#33AA33';
    case SecurityGrade.APlus:
      return '#22BB33';
  }
  return '#4444FF';
};

const getColours = (grade: SecurityGrade): ColourConfig => {
  const background = getBackgroundColour(grade);

  return {
    background,
    foreground: 'white',
  };
};

interface ScanSummaryProps {
  data: ServerResult;
  response: ApiResponse;
}

export const ScanSummary = ({ data, response }: ScanSummaryProps) => {
  const grade = data.grade;

  const colours = getColours(grade);
  const overflowComponent = (
    <Typography level="h1" sx={{ color: colours.foreground, fontSize: '4em' }}>
      {grade == SecurityGrade.APlus ? 'A+' : grade}
    </Typography>
  );

  return (
    <AppCard
      title="Report Summary"
      overflowComponent={overflowComponent}
      overflowProps={{ variant: 'solid', sx: { background: colours.background } }}
    >
      <SummaryTable data={data} response={response} />
    </AppCard>
  );
};
