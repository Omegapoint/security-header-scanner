import { Typography } from '@mui/joy';
import { UriData } from '../contracts/apiTypes.ts';

interface UriDataComponentProps {
  data: UriData;
}

export const UriDataComponent = ({ data }: UriDataComponentProps) => {
  let asciiUri = data.scheme + '://' + data.asciiDomain;
  if (!data.isDefaultPort) asciiUri += ':' + data.port;
  if (data.path.length > 1) asciiUri += data.path;
  return (
    <Typography title={asciiUri} sx={{ overflow: 'hidden' }}>
      <Typography color="neutral">{data.scheme}://</Typography>
      <Typography>{data.utfDomain}</Typography>
      {!data.isDefaultPort && <Typography color="neutral">:{data.port}</Typography>}
      {data.path.length > 1 && <Typography sx={{ textOverflow: 'ellipsis' }}>{data.path}</Typography>}
    </Typography>
  );
};
