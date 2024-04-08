import { OpenInNew } from '@mui/icons-material';
import IconButton from '@mui/joy/IconButton';
import { SyntheticEvent } from 'react';

interface LinkDecoratorProps {
  url: URL;
}

export const LinkDecorator = ({ url }: LinkDecoratorProps) => {
  const clickHandler = (e: SyntheticEvent) => {
    e.stopPropagation();
    window.open(url, '_blank');
  };

  return (
    <IconButton
      component="a"
      href={url.href}
      target="_blank"
      color="neutral"
      sx={{ '--IconButton-size': '20px' }}
      onClick={clickHandler}
    >
      <OpenInNew />
    </IconButton>
  );
};
