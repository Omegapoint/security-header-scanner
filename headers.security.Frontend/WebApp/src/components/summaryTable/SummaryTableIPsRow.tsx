import { Stack, Typography } from '@mui/joy';
import { AppTableRow } from '../AppTableRow.tsx';

const SummaryTableIP = ({ ip }: { ip: string }) => (
  <Typography fontFamily="code" variant="outlined" marginRight="0.2em">
    {ip}
  </Typography>
);

export const SummaryTableIPsRow = ({ ips }: { ips: string[] }) => {
  const single = ips.length === 1;
  const text = <Typography>{single ? <SummaryTableIP ip={ips[0]} /> : 'Multiple IP addresses'}</Typography>;
  const title = single ? 'IP address' : 'IP addresses';

  const expansion = single ? null : (
    <Stack direction="row" flexWrap="wrap" spacing={1} useFlexGap={true}>
      {ips.map((ip) => (
        <SummaryTableIP ip={ip} key={ip} />
      ))}
    </Stack>
  );

  return <AppTableRow rowLabel={title} children={text} expansion={expansion} />;
};
