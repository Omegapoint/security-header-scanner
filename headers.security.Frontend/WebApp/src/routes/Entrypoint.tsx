import { ArrowForward } from '@mui/icons-material';
import { Stack, Typography } from '@mui/joy';
import { ModeAwareSymbol } from '../components/ModeAwareSymbol.tsx';
import classes from './Entrypoint.module.css';

const Entrypoint = () => {
  return (
    <Stack flexGrow={1} direction="row" justifyContent="center" alignItems="center" spacing={6}>
      <Stack spacing={0}>
        <Typography level="h1">Secure your Webapp</Typography>
        <Stack direction="row" spacing={1} alignItems="center">
          <ArrowForward fontSize="xl4" color="danger" />
          <Typography level="h1">Scan your URL</Typography>
        </Stack>
        <br />
        <Typography maxWidth="26em">
          We use a <Typography title="1 minute">short-lived cache</Typography> for performance reasons, but store no
          data, metrics, recent scans, or results otherwise.
        </Typography>
      </Stack>
      <Stack spacing={2}>
        <ModeAwareSymbol className={classes.symbol} />
      </Stack>
    </Stack>
  );
};

export default Entrypoint;
