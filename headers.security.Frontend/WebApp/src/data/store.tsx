import { Store } from '@tanstack/react-store';
import axios, { AxiosError } from 'axios';
import { ApiRequest } from '../contracts/apiRequest.ts';
import { ApiResponse } from '../contracts/apiResponse.ts';
import { ApiError, ErrorOrigin } from '../contracts/apiTypes.ts';
import { getUrl } from '../helpers/getUrl.ts';
import { fallbackMessage } from '../routes/ErrorPage.tsx';
import { ScanQuerySchema } from './router.tsx';

export interface RootState {
  target?: string;
  apiResponse?: ApiResponse;
  apiError?: ApiError;
  loading: boolean;
  followRedirects: boolean;
}

const initialState: RootState = { loading: false, followRedirects: true };
export const store = new Store(initialState);

const queryDiffers = (current: ApiRequest, query: ScanQuerySchema) => {
  if (current.followRedirects != query.followRedirects) {
    return false;
  }

  const currentUrl = getUrl(current.target);
  const queryUrl = getUrl(query.target);

  return currentUrl?.href != queryUrl?.href;
};

export const ensureLoaded = async (scanQuery: ScanQuerySchema) => {
  const state = store.state;
  // TODO: check if response is older than 1 minute as well, api will only respond with new scans once a minute anyway
  if (!state.apiResponse || queryDiffers(state.apiResponse.request, scanQuery)) {
    store.setState((state) => ({ ...state, ...scanQuery }));
    await scan();

    return false;
  }
  return true;
};

export const scan = async () => {
  const { target, followRedirects } = store.state;

  const payload: ApiRequest = {
    target: getUrl(target)?.href,
    followRedirects,
  };

  if (!payload.target) {
    return;
  }

  store.setState((state) => ({ ...state, apiResponse: undefined, apiError: undefined, loading: true }));

  try {
    const response = await axios.post<ApiResponse>('/api/Scan', payload, {
      timeout: 10000,
    });

    if (response.status == 200) {
      const apiResponse = response.data;
      store.setState((state) => ({ ...state, apiResponse }));
    } else {
      store.setState((state) => ({ ...state, apiError: response.data as unknown as ApiError }));
    }
  } catch (err: unknown) {
    const axiosError = err as AxiosError<ApiError>;

    const isTimeout = axiosError.code == 'ECONNABORTED';
    const apiError: ApiError = axiosError.response?.data ?? {
      message: isTimeout ? 'API did not respond in time.' : fallbackMessage,
      origin: isTimeout ? ErrorOrigin.Client : ErrorOrigin.Other,
    };

    console.log('store', apiError);

    store.setState((state) => ({ ...state, apiError }));
  } finally {
    store.setState((state) => ({ ...state, loading: false }));
  }
};
