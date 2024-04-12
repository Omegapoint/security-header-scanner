import { Typography } from '@mui/joy';
import { UriData } from '../contracts/apiTypes.ts';

interface UriDataComponentProps {
  data: UriData;
}

export const UriDataComponent = ({ data }: UriDataComponentProps) => {
  const scheme = `${data.scheme}://`;
  const port = `:${data.port}`;

  let asciiUri = `${scheme}${data.asciiDomain}`;
  if (!data.isDefaultPort) asciiUri += port;
  if (data.path.length > 1) asciiUri += data.path;

  return (
    <Typography title={asciiUri} noWrap maxWidth="100%">
      <Typography color="neutral">{scheme}</Typography>
      <Typography>{data.utfDomain}</Typography>
      {!data.isDefaultPort && <Typography color="neutral">{port}</Typography>}
      {data.path.length > 1 && <Typography>{data.path}</Typography>}
    </Typography>
  );
};
