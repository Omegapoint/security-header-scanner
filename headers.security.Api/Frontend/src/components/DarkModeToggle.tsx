import { DarkModeOutlined, LightMode } from '@mui/icons-material';
import { IconButton, useTheme } from '@mui/joy';
import { useColorScheme } from '@mui/joy/styles';
import { useEffect } from 'react';

export const DarkModeToggle = () => {
  const { mode, systemMode, setMode } = useColorScheme();
  const { palette } = useTheme();

  const isDarkMode = (mode == 'system' ? systemMode : mode) == 'dark';
  const nextMode = isDarkMode ? 'light' : 'dark';

  const bgColour = palette.background.body;

  useEffect(() => {
    document.querySelector('meta[name="theme-color"]')?.setAttribute('content', bgColour);
  }, [bgColour]);

  return (
    <IconButton variant="outlined" color="neutral" onClick={() => setMode(nextMode)}>
      {isDarkMode ? <LightMode /> : <DarkModeOutlined />}
    </IconButton>
  );
};
