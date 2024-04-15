import { extendTheme } from '@mui/joy';
import type { TypographySystem } from '@mui/joy/styles';
import { CssVarsThemeOptions } from '@mui/joy/styles/extendTheme';
import { DefaultColorPalette, DefaultFontSize } from '@mui/joy/styles/types';

export const OPColour = {
  WarmGray: '#D7D2CB',
  OmegaSun: '#F9BC2E',
  Tango: '#E87722',
  Petroleum: '#286166',
  Marine: '#003349',
  MineShaft: '#383838',
};

const omegaSunGradient = {
  '50': '#fefce8',
  '100': '#f9f1c4',
  '200': '#f7e6a0',
  '300': '#f6d97d',
  '400': '#f7cb58',
  '500': OPColour.OmegaSun,
  '600': '#d59f23',
  '700': '#b28316',
  '800': '#91690a',
  '900': '#704f00',
};
const tangoGradient = {
  '50': '#fff7ed',
  '100': '#fbdfbf',
  '200': '#f7c695',
  '300': '#f2ad6e',
  '400': '#ed9348',
  '500': OPColour.Tango,
  '600': '#d16819',
  '700': '#bb5910',
  '800': '#a54a07',
  '900': '#8f3c00',
};
const petroleumGradient = {
  '50': '#dff1f1',
  '100': '#bad3d3',
  '200': '#95b5b6',
  '300': '#71989a',
  '400': '#4e7c80',
  '500': OPColour.Petroleum,
  '600': '#1e5458',
  '700': '#134649',
  '800': '#08393b',
  '900': '#002c2e',
};

const defaultTheme = extendTheme();
const defaultNeutral = defaultTheme.palette.neutral;

export const omegapointTheme: CssVarsThemeOptions = {
  shadow: {},
  fontFamily: {
    display: 'Roboto Slab',
    body: 'Roboto',
    code: 'Roboto Mono',
  },
  colorSchemes: {
    dark: {
      palette: {
        background: {
          surface: defaultNeutral['600'],
          body: defaultNeutral['700'],
        },
        warning: omegaSunGradient,
        danger: tangoGradient,
        primary: petroleumGradient,
      },
    },
    light: {
      palette: {
        background: {
          surface: '#F5F4F1',
          body: '#FFF',
        },
        warning: omegaSunGradient,
        danger: tangoGradient,
        primary: petroleumGradient,
      },
    },
  },
  components: {
    JoyIconButton: {
      styleOverrides: {
        root: ({ ownerState, theme }) => ({
          ...(ownerState.size === 'xs' && {
            '--Icon-fontSize': '1rem',
            '--Button-gap': '0.25rem',
            height: '1.25rem',
            width: '1.25rem',
            fontSize: theme.vars.fontSize.xs,
            paddingBlock: '2px',
            paddingInline: '0.5rem',
          }),
        }),
      },
    },
  },
};

type FontSizeOverrides = { [k in keyof DefaultFontSize]: true } & { [k in keyof TypographySystem]: true };
type ColorPaletteOverrides = { [k in DefaultColorPalette]: true };

declare module '@mui/material/SvgIcon' {
  interface SvgIconPropsSizeOverrides extends FontSizeOverrides {}
  interface SvgIconPropsColorOverrides extends ColorPaletteOverrides {}
}

declare module '@mui/joy/IconButton' {
  interface IconButtonPropsSizeOverrides {
    xs: true;
  }
}
