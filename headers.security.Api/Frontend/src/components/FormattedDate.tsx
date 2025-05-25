import { Typography } from '@mui/joy';

export const FormattedDate = ({ date }: { date: string }) => {
  const dateFormatter = new Intl.DateTimeFormat('en-GB', {
    dateStyle: 'full',
    timeStyle: 'long',
  });

  return <Typography noWrap>{dateFormatter.format(new Date(date))}</Typography>;
};
