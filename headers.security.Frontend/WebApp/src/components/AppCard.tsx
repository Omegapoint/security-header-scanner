import { Card, CardContent, CardOverflow, Stack, Typography } from '@mui/joy';
import { CardOverflowProps } from '@mui/joy/CardOverflow/CardOverflowProps';
import { TypographyProps } from '@mui/joy/Typography/TypographyProps';
import { ReactNode, useState } from 'react';

interface AppCardProps {
  title: ReactNode;
  overflowComponent?: ReactNode;
  overflowProps?: CardOverflowProps;
  children?: ReactNode;
  hideable?: boolean;
  revealedByDefault?: boolean;
}

export const AppCard = ({
  children,
  overflowComponent,
  overflowProps,
  title,
  hideable,
  revealedByDefault = false,
}: AppCardProps) => {
  const [open, setOpen] = useState(revealedByDefault || !hideable);
  const reveal = () => setOpen(true);

  const titleProps: TypographyProps = {
    level: 'h3',
  };
  if (!open) {
    titleProps.sx = { cursor: 'pointer' };
    titleProps.color = 'neutral';
    titleProps.onClick = reveal;
  }

  const fullTitle = open ? title : `Show ${title}`;

  return (
    <Stack spacing={1} sx={{ width: '80%' }}>
      <Typography {...titleProps}>{fullTitle}</Typography>
      {open && (
        <Card sx={{ boxShadow: 'md', paddingBottom: '0.3em' }}>
          {overflowComponent && <CardOverflow {...overflowProps}>{overflowComponent}</CardOverflow>}
          {children && <CardContent>{children}</CardContent>}
        </Card>
      )}
    </Stack>
  );
};
