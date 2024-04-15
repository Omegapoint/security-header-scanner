import Box, { BoxProps } from '@mui/joy/Box';

const Root = (props: BoxProps) => (
  <Box
    {...props}
    sx={[
      {
        bgcolor: 'background.appBody',
        display: 'grid',
        gridTemplateColumns: {
          xs: '0 1fr 0',
          md: '5em 1fr 5em',
        },
        gridTemplateRows: '64px 200px 1fr 48px',
        minHeight: '100vh',
        width: '100%',
      },
      ...(Array.isArray(props.sx) ? props.sx : [props.sx]),
    ]}
  />
);

const Header = (props: BoxProps) => (
  <Box
    component="header"
    className="Header"
    {...props}
    sx={[
      {
        p: 2,
        gap: 2,
        bgcolor: 'background.body',
        display: 'flex',
        flexDirection: 'row',
        justifyContent: 'space-between',
        alignItems: 'center',
        gridColumn: '2',
        position: 'sticky',
        top: 0,
        zIndex: 1100,
      },
      ...(Array.isArray(props.sx) ? props.sx : [props.sx]),
    ]}
  />
);

const Banner = (props: BoxProps) => (
  <Box
    component="header"
    className="Banner"
    {...props}
    sx={[
      {
        p: 2,
        gap: 2,
        background:
          'linear-gradient(180deg, var(--joy-palette-background-surface) 0%, var(--joy-palette-background-surface) 69%, rgba(0,0,0,0) 69%)',
        display: 'flex',
        flexDirection: 'row',
        justifyContent: 'space-between',
        alignItems: 'center',
        gridColumn: '2',
      },
      ...(Array.isArray(props.sx) ? props.sx : [props.sx]),
    ]}
  />
);

const Footer = (props: BoxProps) => (
  <Box
    component="footer"
    className="Footer"
    {...props}
    sx={[
      {
        p: 2,
        gap: 2,
        bgcolor: 'background.appBody',
        display: 'flex',
        flexDirection: 'row',
        justifyContent: 'space-between',
        alignItems: 'center',
        gridColumn: '1 / -1',
        borderTop: '1px solid',
        borderColor: 'divider',
        bottom: 0,
        zIndex: 1100,
      },
      ...(Array.isArray(props.sx) ? props.sx : [props.sx]),
    ]}
  />
);

const Main = (props: BoxProps) => (
  <Box
    display="flex"
    component="main"
    className="Main"
    {...props}
    sx={[{ p: 2, gridColumn: 2 }, ...(Array.isArray(props.sx) ? props.sx : [props.sx])]}
  />
);

export default {
  Root,
  Header,
  Banner,
  Footer,
  Main,
};
