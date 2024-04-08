import '@fontsource/roboto';
import '@fontsource/roboto-mono';
import '@fontsource/roboto-slab';
import { CssBaseline, CssVarsProvider, extendTheme } from '@mui/joy';
import { RouterProvider, createRouter } from '@tanstack/react-router';
import React from 'react';
import ReactDOM from 'react-dom/client';
import { omegapointTheme } from './data/branding.ts';
import './main.css';
import { routeTree } from './routeTree.gen';

const theme = extendTheme(omegapointTheme);

const router = createRouter({ routeTree });

declare module '@tanstack/react-router' {
  interface Register {
    router: typeof router;
  }
}

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <CssVarsProvider defaultMode="system" theme={theme}>
      <CssBaseline />
      <RouterProvider router={router} />
    </CssVarsProvider>
  </React.StrictMode>
);
