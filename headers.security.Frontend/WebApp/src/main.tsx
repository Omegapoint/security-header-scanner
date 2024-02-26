import '@fontsource/roboto';
import '@fontsource/roboto-mono';
import '@fontsource/roboto-slab';
import { CssBaseline, CssVarsProvider, extendTheme } from '@mui/joy';
import { RouterProvider } from '@tanstack/react-router';
import React from 'react';
import ReactDOM from 'react-dom/client';
import { omegapointTheme } from './data/branding.ts';
import router from './data/router.tsx';
import './main.css';

const theme = extendTheme(omegapointTheme);

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <CssVarsProvider defaultMode="system" theme={theme}>
      <CssBaseline />
      <RouterProvider router={router} />
    </CssVarsProvider>
  </React.StrictMode>
);
