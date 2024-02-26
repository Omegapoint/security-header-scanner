import { Stack, Typography } from '@mui/joy';
import { useStore } from '@tanstack/react-store';
import { ApiError } from '../contracts/apiTypes.ts';
import { store } from '../data/store.tsx';

interface ErrorPageProps {
  reason?: ApiError;
}

export const fallbackMessage =
  "Unrecoverable error. Don't worry, the admins now know about it, and will develop a fix.";

export const ErrorPage = ({ reason }: ErrorPageProps) => {
  const { apiError } = useStore(store, (state) => state);

  const message = reason?.message ?? apiError?.message ?? fallbackMessage;

  return (
    <Stack alignItems="center" width="100%">
      <Typography color="danger" variant="outlined">
        {message}
      </Typography>
    </Stack>
  );
};
