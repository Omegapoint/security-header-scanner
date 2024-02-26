import { Outlet } from '@tanstack/react-router';
import { Footer } from './Footer.tsx';
import { Header } from './Header.tsx';
import { Banner } from './components/Banner.tsx';
import Layout from './components/Layout';

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
  </>
);

export default App;
