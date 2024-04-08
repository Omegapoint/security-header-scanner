import { useColorScheme } from '@mui/joy';
import opSymbolBlack from '/op-symbol-black.svg';
import opSymbolWhite from '/op-symbol-white.svg';

export const ModeAwareSymbol = (props: { className?: string }) => {
  const { mode, systemMode } = useColorScheme();

  const isDarkMode = (mode == 'system' ? systemMode : mode) == 'dark';
  const logo = isDarkMode ? opSymbolWhite : opSymbolBlack;

  return <img className={props.className} src={logo} alt="Omegapoint symbol" />;
};
