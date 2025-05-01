import { Outlet } from '@tanstack/react-router';
import React, { Suspense } from 'react';
import { Footer } from './Footer.tsx';
import { Header } from './Header.tsx';
import { Banner } from './components/Banner.tsx';
import Layout from './components/Layout';

const TanStackRouterDevtools =
  process.env.NODE_ENV === 'development'
    ? React.lazy(() =>
        // Lazy load in development
        import('@tanstack/react-router-devtools').then((res) => ({
          default: res.TanStackRouterDevtools,
        }))
      )
    : () => null;

const App = () => (
  <>
    <Layout.Root>
      <Layout.Header>
        <Header />
      </Layout.Header>
      <Layout.Banner>
        <Banner />
      </Layout.Banner>
      <Layout.Main>
        <Outlet />
      </Layout.Main>
      <Layout.Footer>
        <Footer />
      </Layout.Footer>
    </Layout.Root>
    <Suspense>
      <TanStackRouterDevtools />
    </Suspense>
  </>
);

export default App;
