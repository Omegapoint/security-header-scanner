import { OpenInNew } from '@mui/icons-material';
import { Button, Stack, Typography } from '@mui/joy';
import classes from './Header.module.css';
import { DarkModeToggle } from './components/DarkModeToggle.tsx';
import { ModeAwareLogo } from './components/ModeAwareLogo.tsx';
import { RouterLink } from './components/RouterLink.tsx';

export const Header = () => (
  <>
    <Stack direction="row" alignItems="center" spacing={3}>
      <RouterLink to="/" title="Home" color="neutral" underline="none">
        <Stack direction="row" alignItems="center" spacing={2}>
          <ModeAwareLogo className={classes.logo} />
          <Typography color="primary" level="h4">
            Security Header Scanner
          </Typography>
        </Stack>
      </RouterLink>
    </Stack>
    <Stack direction="row" alignItems="center" spacing={3}>
      <Button
        component="a"
        href="https://securityblog.omegapoint.se"
        target="_blank"
        variant="outlined"
        color="neutral"
        endDecorator={<OpenInNew />}
      >
        Security Blog
      </Button>
      <DarkModeToggle />
    </Stack>
  </>
);
