import Box, { BoxProps } from '@mui/joy/Box';

const Root = (props: BoxProps) => (
  <Box
    {...props}
    sx={[
      {
        bgcolor: 'background.appBody',
        display: 'grid',
        gridTemplateColumns: {
          xs: '5vw 1fr 5vw', // sm: 'minmax(64px, 200px) minmax(450px, 1fr)',
          // md: 'minmax(160px, 300px) minmax(300px, 500px) minmax(500px, 1fr)',
        },
        gridTemplateRows: '64px 200px 1fr 48px',
        minHeight: '100vh',
        maxWidth: '100vw',
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
        // borderBottom: '1px solid',
        // borderColor: 'divider',
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
        // bgcolor: 'background.surface',
        background:
          'linear-gradient(180deg, var(--joy-palette-background-surface) 0%, var(--joy-palette-background-surface) 69%, rgba(0,0,0,0) 69%)',
        display: 'flex',
        flexDirection: 'row',
        justifyContent: 'space-between',
        alignItems: 'center',
        gridColumn: '2',
        // borderBottom: '1px solid',
        // borderColor: 'divider',
        // position: 'sticky',
        // top: '64px',
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
        // position: 'sticky',
        bottom: 0,
        zIndex: 1100,
        // marginLeft: '-5vw',
        // width: '95vw',
      },
      ...(Array.isArray(props.sx) ? props.sx : [props.sx]),
    ]}
  />
);

// const SideNav = (props: BoxProps) => (
//   <Box
//     component="nav"
//     className="Navigation"
//     {...props}
//     sx={[
//       {
//         p: 2,
//         bgcolor: 'background.surface',
//         borderRight: '1px solid',
//         borderColor: 'divider',
//         display: {
//           xs: 'none',
//           sm: 'initial',
//         },
//       },
//       ...(Array.isArray(props.sx) ? props.sx : [props.sx]),
//     ]}
//   />
// );

// const SidePane = (props: BoxProps) => (
//   <Box
//     className="Inbox"
//     {...props}
//     sx={[
//       {
//         bgcolor: 'background.surface',
//         borderRight: '1px solid',
//         borderColor: 'divider',
//         display: {
//           xs: 'none',
//           md: 'initial',
//         },
//       },
//       ...(Array.isArray(props.sx) ? props.sx : [props.sx]),
//     ]}
//   />
// );

const Main = (props: BoxProps) => (
  <Box
    display="flex"
    component="main"
    className="Main"
    {...props}
    sx={[{ p: 2, maxWidth: '90vw', gridColumn: 2 }, ...(Array.isArray(props.sx) ? props.sx : [props.sx])]}
  />
);

// const SideDrawer = (props: BoxProps & { onClose: React.MouseEventHandler<HTMLDivElement> }) => {
//   const { onClose, ...other } = props;
//   return (
//     <Box
//       {...other}
//       sx={[
//         { position: 'fixed', zIndex: 1200, width: '100%', height: '100%' },
//         ...(Array.isArray(other.sx) ? other.sx : [other.sx]),
//       ]}
//     >
//       <Box
//         role="button"
//         onClick={onClose}
//         sx={{
//           position: 'absolute',
//           inset: 0,
//           bgcolor: (theme) => `rgba(${theme.vars.palette.neutral.darkChannel} / 0.8)`,
//         }}
//       />
//       <Sheet
//         sx={{
//           minWidth: 256,
//           width: 'max-content',
//           height: '100%',
//           p: 2,
//           boxShadow: 'lg',
//           bgcolor: 'background.surface',
//         }}
//       >
//         {other.children}
//       </Sheet>
//     </Box>
//   );
// };

export default {
  Root,
  Header,
  Banner,
  Footer,
  // SideNav,
  // SidePane,
  // SideDrawer,
  Main,
};
