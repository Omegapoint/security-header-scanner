import { DarkModeOutlined, LightMode } from '@mui/icons-material';
import { IconButton, useTheme } from '@mui/joy';
import { useColorScheme } from '@mui/joy/styles';
import { useEffect } from 'react';

export const DarkModeToggle = () => {
  const { mode, systemMode, setMode } = useColorScheme();
  const { palette } = useTheme();

  const currentMode = (mode == 'system' ? systemMode : mode) ?? 'light';
  const isDarkMode = currentMode == 'dark';
  const nextMode = isDarkMode ? 'light' : 'dark';

  const backgroundColor = palette.background.body;

  useEffect(
    () => document.querySelector('meta[name="theme-color"]')?.setAttribute('content', backgroundColor),
    [backgroundColor]
  );

  const NextModeIcon = isDarkMode ? LightMode : DarkModeOutlined;

  return (
    <IconButton variant="outlined" color="neutral" onClick={() => setMode(nextMode)}>
      <NextModeIcon />
    </IconButton>
  );
};
