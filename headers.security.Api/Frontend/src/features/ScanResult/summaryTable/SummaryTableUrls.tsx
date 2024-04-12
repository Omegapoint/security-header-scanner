import { ArrowForward } from '@mui/icons-material';
import { Stack, Theme } from '@mui/joy';
import Box from '@mui/joy/Box';
import { isEqual } from 'lodash';
import { CSSProperties } from 'react';
import { UriDataComponent } from '../../../components/UriDataComponent.tsx';
import { ServerResult, getUriDataLength } from '../../../contracts/apiTypes.ts';

const maxLength = {
  xs: 25,
  all: 80,
};

export const SummaryTableUrls = ({ data }: { data: ServerResult }) => {
  const hasRedirect = !isEqual(data.requestTarget, data.finalTarget);

  const totalLength = getUriDataLength(data.requestTarget) + getUriDataLength(data.finalTarget);

  const columnStyle: CSSProperties = { flexDirection: 'column', alignItems: 'flex-start' };
  const rowStyle: CSSProperties = { flexDirection: 'row', alignItems: 'center' };
  const rotateIconStyle: CSSProperties = { transform: 'rotate(90deg)' };

  const selectProps =
    (columnProps: CSSProperties = {}, rowProps: CSSProperties = {}) =>
    (theme: Theme) => ({
      [theme.breakpoints.only('xs')]: totalLength >= maxLength.xs ? columnProps : rowProps,
      [theme.breakpoints.up('xs')]: totalLength >= maxLength.all ? columnProps : rowProps,
    });

  return (
    <Box columnGap={1} display="flex" sx={selectProps(columnStyle, rowStyle)}>
      {hasRedirect && <UriDataComponent data={data.requestTarget} />}
      {hasRedirect && (
        <Stack direction="row" justifyContent="center" sx={selectProps({ paddingLeft: '1em' })}>
          {/* Icons are from @mui/icons-material, so we need to cast to a Joy theme here */}
          <ArrowForward fontSize="md" sx={(t) => selectProps(rotateIconStyle)(t as unknown as Theme)} />
        </Stack>
      )}
      <UriDataComponent data={data.finalTarget} />
    </Box>
  );
};
