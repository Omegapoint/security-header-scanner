import { ArrowForward } from '@mui/icons-material';
import { Stack } from '@mui/joy';
import { isEqual } from 'lodash';
import { UriDataComponent } from '../../../components/UriDataComponent.tsx';
import { ServerResult } from '../../../contracts/apiTypes.ts';

export const SummaryTableUrls = ({ data }: { data: ServerResult }) => {
  const hasRedirect = !isEqual(data.requestTarget, data.finalTarget);

  return (
    <Stack direction="row" spacing={1} alignItems="center">
      {hasRedirect && <UriDataComponent data={data.requestTarget} />}
      {hasRedirect && <ArrowForward fontSize="md" />}
      <UriDataComponent data={data.finalTarget} />
    </Stack>
  );
};
