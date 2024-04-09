import { TargetKind } from './apiTypes.ts';

export interface ApiRequest {
  target: string;
  followRedirects: boolean;
  kind?: TargetKind;
}
