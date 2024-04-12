import { Typography } from '@mui/joy';

export const FormattedDate = ({ date }: { date: string }) => {
  // TODO: should implement some i18n here when we do that for the whole site
  const dateFormatter = new Intl.DateTimeFormat('en-GB', {
    dateStyle: 'full',
    timeStyle: 'long',
  });

  return <Typography noWrap>{dateFormatter.format(new Date(date))}</Typography>;
};
