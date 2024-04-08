import { useColorScheme } from '@mui/joy';
import opLogoBlack from '/op-logo-black.svg';
import opLogoWhite from '/op-logo-white.svg';

export const ModeAwareLogo = (props: { className?: string }) => {
  const { mode, systemMode } = useColorScheme();

  const isDarkMode = (mode == 'system' ? systemMode : mode) == 'dark';
  const logo = isDarkMode ? opLogoWhite : opLogoBlack;

  return <img className={props.className} src={logo} alt="Omegapoint logo" />;
};
