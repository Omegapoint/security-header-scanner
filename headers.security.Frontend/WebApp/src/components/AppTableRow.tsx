import { KeyboardArrowDown, KeyboardArrowUp } from '@mui/icons-material';
import { Stack, Typography } from '@mui/joy';
import IconButton from '@mui/joy/IconButton';
import { CSSProperties, ReactNode, useState } from 'react';

interface AppTableRowProps {
  rowLabel: string;
  children: ReactNode[] | ReactNode;
  initialOpen?: boolean;
  expansion?: ReactNode;
  fullWidthExpansion?: boolean;
  expansionColSpanPadding?: number;
}

export const AppTableRow = ({
  rowLabel,
  children,
  initialOpen = false,
  expansion,
  fullWidthExpansion = false,
  expansionColSpanPadding = 0,
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

  const getColSpan = (idx: number) => (!hasExpansion ? (childrenLength - 1 == idx ? 2 : 1) : 1);

  const expansionColSpan = fullWidthExpansion
    ? {
        padding: 0,
        main: totalCols,
      }
    : {
        padding: expansionColSpanPadding || 1,
        main: totalCols - (expansionColSpanPadding || 1),
      };
  return (
    <>
      <tr onClick={toggleOpen} style={rowStyle}>
        <th scope="row" style={{ background: 'unset', maxWidth: '17vw', ...tdStyle }}>
          <Stack height="100%">
            <Stack height="2em" direction="row" alignItems="center">
              <Typography noWrap title={rowLabel}>
                {rowLabel}
              </Typography>
            </Stack>
          </Stack>
        </th>
        {childrenArr.map((child, idx) => (
          <td style={tdStyle} key={idx} colSpan={getColSpan(idx)}>
            {child}
          </td>
        ))}
        {hasExpansion && (
          <td style={{ verticalAlign: 'top', textAlign: 'right', padding: '0.3em 0.3em 0 0', ...tdStyle }}>
            <IconButton onClick={toggleOpen} size="sm" variant="outlined">
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
