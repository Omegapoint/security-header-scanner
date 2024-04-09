import { AxiosError } from 'axios';
import { ApiError, ErrorOrigin } from '../contracts/apiTypes.ts';
import { fallbackMessage } from '../features/ScanResult/ErrorPage.tsx';

export const parseError = (axiosError: AxiosError<ApiError>) => {
  const response = axiosError.response;

  if (response?.data) {
    return response.data;
  }

  const isTimeout = axiosError.code == 'ECONNABORTED';
  if (isTimeout) {
    return {
      message: 'API did not respond in time.',
      origin: ErrorOrigin.Other,
    };
  }

  return {
    message: fallbackMessage,
    origin: ErrorOrigin.Client,
  };
};
