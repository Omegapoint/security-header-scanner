import { KeyboardArrowDown, KeyboardArrowUp } from '@mui/icons-material';
import { Stack, Typography } from '@mui/joy';
import IconButton from '@mui/joy/IconButton';
import { CSSProperties, ReactNode, useState } from 'react';

interface AppTableRowProps {
  rowLabel: string;
  children: ReactNode[] | ReactNode;
  labelDecorator?: ReactNode;
  initialOpen?: boolean;
  expansion?: ReactNode;
  fullWidthExpansion?: boolean;
  expansionColSpanPadding?: number;
  thinCols?: Array<number>;
}

export const AppTableRow = ({
  rowLabel,
  children,
  labelDecorator,
  initialOpen = false,
  expansion,
  fullWidthExpansion = false,
  expansionColSpanPadding = 0,
  thinCols = [],
}: AppTableRowProps) => {
  const [open, setOpen] = useState(initialOpen);

  const hasExpansion = expansion != null;
  const Icon = open ? KeyboardArrowUp : KeyboardArrowDown;
  const toggleOpen = () => hasExpansion && setOpen(!open);

  const tdStyle: CSSProperties = {};
  if (open) {
    tdStyle.borderBottom = 0;
  }

  const childrenArr = Array.isArray(children) ? children : [children];
  const childrenLength = childrenArr.length;

  const rowStyle = hasExpansion ? { cursor: 'pointer' } : {};

  const totalCols = 1 + childrenLength + (hasExpansion ? 1 : 0);

  const getColSpan = (idx: number) => (idx == childrenLength - 1 && !hasExpansion ? 2 : 1);

  const expansionColSpan = fullWidthExpansion
    ? {
        padding: 0,
        main: totalCols,
      }
    : {
        padding: expansionColSpanPadding || 1,
        main: totalCols - (expansionColSpanPadding || 1),
      };
  const getTdStyle = (idx: number) => {
    const style: CSSProperties = { ...tdStyle };

    if (idx in thinCols) {
      style.paddingLeft = 0;
      style.paddingRight = 0;
    }

    return style;
  };

  return (
    <>
      <tr onClick={toggleOpen} style={rowStyle}>
        <th scope="row" style={{ background: 'unset', ...tdStyle }}>
          <Stack minHeight="2em" direction="row" alignItems="center">
            <Typography
              title={rowLabel}
              sx={(theme) => ({
                [theme.breakpoints.only('xs')]: {
                  '--Typography-fontSize': '9pt',
                },
                overflowWrap: 'break-word',
                textWrap: 'wrap',
                '& *': { marginInlineStart: 0 },
              })}
            >
              {rowLabel}
            </Typography>
            {labelDecorator}
          </Stack>
        </th>
        {childrenArr.map((child, idx) => (
          <td style={getTdStyle(idx)} key={idx} colSpan={getColSpan(idx)}>
            {child}
          </td>
        ))}
        {hasExpansion && (
          <td style={{ verticalAlign: 'bottom', textAlign: 'right', padding: '0 0.3em 0.45em 0', ...tdStyle }}>
            <IconButton onClick={toggleOpen} size="xs" variant="outlined">
              <Icon />
            </IconButton>
          </td>
        )}
      </tr>
      {open && (
        <tr>
          {expansionColSpan.padding > 0 && <td style={{ paddingTop: 0 }} colSpan={expansionColSpan.padding}></td>}
          <td style={{ paddingTop: 0 }} colSpan={expansionColSpan.main}>
            {expansion}
          </td>
        </tr>
      )}
    </>
  );
};
