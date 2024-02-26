import { DarkModeOutlined, LightMode } from '@mui/icons-material';
import IconButton from '@mui/joy/IconButton';
import { useColorScheme } from '@mui/joy/styles';

export const DarkModeToggle = () => {
  const { mode, systemMode, setMode } = useColorScheme();

  const isDarkMode = (mode == 'system' ? systemMode : mode) == 'dark';
  const nextMode = isDarkMode ? 'light' : 'dark';

  return (
    <IconButton variant="outlined" color="neutral" onClick={() => setMode(nextMode)}>
      {isDarkMode ? <LightMode /> : <DarkModeOutlined />}
    </IconButton>
  );
};
