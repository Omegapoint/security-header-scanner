import { Store } from '@tanstack/react-store';
import axios, { AxiosError } from 'axios';
import { ApiRequest } from '../contracts/apiRequest.ts';
import { ApiResponse } from '../contracts/apiResponse.ts';
import { ApiError, TargetKind } from '../contracts/apiTypes.ts';
import { getUrl } from '../helpers/getUrl.ts';
import { parseError } from '../helpers/parseError.tsx';
import { ScanQuerySchema } from '../routes/scan.tsx';

export interface RootState {
  target?: string;
  apiResponse?: ApiResponse;
  apiError?: ApiError;
  loading: boolean;
  followRedirects: boolean;
  kind?: TargetKind;
}

const initialState: RootState = { loading: false, followRedirects: true };
export const store = new Store(initialState);

const queryDiffers = (current: ApiRequest, query: ScanQuerySchema) => {
  if (current.followRedirects != query.followRedirects) {
    return true;
  }

  const queryKind = query.kind ?? TargetKind.Detect;
  const currentKind = current.kind ?? TargetKind.Detect;
  if (currentKind != queryKind) {
    return true;
  }

  const currentUrl = getUrl(current.target);
  const queryUrl = getUrl(query.target);

  return currentUrl?.href != queryUrl?.href;
};

const simplifyTarget = (scanQuery: ScanQuerySchema) => {
  if (scanQuery.target?.startsWith('https://') == true) {
    scanQuery.target = scanQuery.target.substring(8);
  }
};

export const ensureLoaded = async (scanQuery: ScanQuerySchema) => {
  const { apiResponse } = store.state;
  if (!apiResponse || queryDiffers(apiResponse.request, scanQuery)) {
    if (!scanQuery.kind) {
      scanQuery.kind = undefined;
    }
    simplifyTarget(scanQuery);
    store.setState((state) => ({ ...state, ...scanQuery }));
    await scan();

    return false;
  }
  return true;
};

export const scan = async () => {
  const { target, kind, followRedirects } = store.state;

  const href = getUrl(target)?.href;
  if (!href) {
    return;
  }

  const payload: ApiRequest = {
    target: href,
    kind: kind,
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
    const apiError = parseError(err as AxiosError<ApiError>);

    store.setState((state) => ({ ...state, apiError }));
  } finally {
    store.setState((state) => ({ ...state, loading: false }));
  }
};
