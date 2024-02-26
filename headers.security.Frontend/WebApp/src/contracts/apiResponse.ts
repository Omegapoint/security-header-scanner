import { ApiRequest } from './apiRequest.ts';
import { ServerResult } from './apiTypes.ts';

export interface ApiResponse {
  request: ApiRequest;
  results: ServerResult[];
  scanStart: string;
  scanFinish: string;
}
