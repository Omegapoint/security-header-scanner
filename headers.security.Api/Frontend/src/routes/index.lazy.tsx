import { ArrowForward } from '@mui/icons-material';
import { Stack, Typography } from '@mui/joy';
import { createLazyFileRoute } from '@tanstack/react-router';
import { ModeAwareSymbol } from '../components/ModeAwareSymbol.tsx';
import classes from './index.module.css';

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
        <Typography maxWidth="26em">We store no URLs or results.</Typography>
      </Stack>
      <Stack spacing={2}>
        <ModeAwareSymbol className={classes.symbol} />
      </Stack>
    </Stack>
  );
};

export const Route = createLazyFileRoute('/')({
  component: Entrypoint,
});
