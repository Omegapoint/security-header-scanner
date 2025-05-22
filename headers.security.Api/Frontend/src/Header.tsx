import { GitHub, OpenInNew } from '@mui/icons-material';
import { Button, Stack, Typography, useTheme } from '@mui/joy';
import { useMediaQuery } from '@mui/material';
import { ReactNode } from 'react';
import classes from './Header.module.css';
import { DarkModeToggle } from './components/DarkModeToggle.tsx';
import { ModeAwareLogo } from './components/ModeAwareLogo.tsx';
import { RouterLink } from './components/RouterLink.tsx';

interface HeaderButtonProps {
  href?: string,
  icon?: ReactNode,
  title?: string
}

const HeaderButton = ({ href, icon, title }: HeaderButtonProps) => {
  const { breakpoints } = useTheme();
  const isMobile = useMediaQuery(breakpoints.only("xs"));

  const level = isMobile ? "title-sm" : "title-md";
  const padding = isMobile ? "0 0.5rem" : "0.5rem 1rem";
  const buttonGap = isMobile ? "0.3rem" : "0.5rem";
  const iconSize = `var(${isMobile ? "--joy-fontSize-md" : "--joy-fontSize-lg"})`;

  return <Button
    component="a"
    href={href}
    target="_blank"
    variant="plain"
    color="neutral"
    endDecorator={icon}
    sx={{
      padding,
      '--Button-gap': buttonGap,
      '--Icon-fontSize': iconSize
    }}
  >
    <Typography textAlign="center" lineHeight="120%" level={level}>{title}</Typography>
  </Button>;
};

export const Header = () => {
  const { breakpoints } = useTheme();
  const isMobile = useMediaQuery(breakpoints.down("md"));

  const spacing = isMobile ? 0 : 1;

  return (
    <>
      <Stack direction="row" alignItems="center" spacing={3}>
        <RouterLink to="/" title="Home" color="neutral" underline="none">
          <Stack direction="row" alignItems="center" spacing={2}>
            <ModeAwareLogo className={classes.logo} />
            {!isMobile && <>
              <Typography color="primary" level="h4" noWrap>
                Security Header Scanner
              </Typography>
            </>}
          </Stack>
        </RouterLink>
      </Stack>
      <Stack direction="row" alignItems="center" spacing={spacing}>
        <HeaderButton title="GitHub" href="https://github.com/Omegapoint/security-header-scanner" icon={<GitHub />} />
        <HeaderButton title="Security Blog" href="https://securityblog.omegapoint.se" icon={<OpenInNew />} />
        <DarkModeToggle />
      </Stack>
    </>
  );
};
