import { ListItem, Stack, Typography } from '@mui/joy';
import { AppTableRow } from '../../../components/AppTableRow.tsx';
import { CloudIcon } from '../../../components/CloudIcon.tsx';
import { IPWithCloud } from '../../../contracts/apiTypes.ts';

const SummaryTableIP = ({ ip: { ip, cloud } }: { ip: IPWithCloud }) => (
  <ListItem variant="outlined" sx={{ maxWidth: '100%' }}>
    <CloudIcon cloud={cloud} />
    <Typography fontFamily="code" marginRight="0.3em" lineHeight="1.8" title={ip} noWrap>
      {ip}
    </Typography>
  </ListItem>
);

export const SummaryTableIPsRow = ({ ips }: { ips: IPWithCloud[] }) => {
  const single = ips.length === 1;

  const contents = <Stack direction="row">{single ? <SummaryTableIP ip={ips[0]} /> : 'Multiple IP addresses'}</Stack>;
  const title = single ? 'IP address' : 'IP addresses';

  const expansion = single ? null : (
    <Stack direction="row" flexWrap="wrap" spacing={1} useFlexGap={true}>
      {ips.map((ip) => (
        <SummaryTableIP ip={ip} key={ip.ip} />
      ))}
    </Stack>
  );

  return <AppTableRow rowLabel={title} children={contents} expansion={expansion} />;
};
