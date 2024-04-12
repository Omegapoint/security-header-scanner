import { SecurityGrade } from './securityGrade.ts';
import { SecurityImpact } from './securityImpact.ts';

export interface SecurityConceptResultInfo {
  message: string;
  externalLink?: URL;
  formatTokens?: string[][];
}

export enum SecurityConceptHandlerName {
  AccessControlAllowOrigin = 'Access-Control-Allow-Origin',
  ContentSecurityPolicy = 'Content Security Policy',
  HTTPStrictTransportSecurity = 'HTTP Strict Transport Security',
  PermissionsPolicy = 'Permissions-Policy',
  ReferrerPolicy = 'Referrer-Policy',
  Server = 'Server',
  XContentTypeOptions = 'X-Content-Type-Options',
  XFrameOptions = 'X-Frame-Options',
}

export enum TargetKind {
  'Detect' = 'Detect',
  'Frontend' = 'Frontend',
  'Api' = 'Api',
  'Both' = 'Both',
}

export const targetKindToString = (kind: TargetKind) => {
  return kind == 'Api' ? 'API' : kind;
};

export interface ISecurityConceptResult {
  handlerName: SecurityConceptHandlerName;
  headerName: string;
  infos: SecurityConceptResultInfo[];
  impact: SecurityImpact;
  processedValue: object | string;
}

export interface ScanResult {
  handlerResults: ISecurityConceptResult[];
  rawHeaders: Record<string, string[]>;
  targetKind: TargetKind;
}

export interface UriData {
  scheme: string;
  utfDomain: string;
  asciiDomain: string;
  port: number;
  path: string;
  isDefaultPort: boolean;
}

export const getUriDataLength = (data: UriData) => {
  const schemeLength = data.scheme.length + 3;
  const portLength = data.isDefaultPort ? 0 : 1 + data.port.toString().length;
  return schemeLength + data.utfDomain.length + portLength + data.path.length;
};

export interface ServerResult {
  requestTarget: UriData;
  finalTarget: UriData;
  ips: string[];
  result: ScanResult;
  grade: SecurityGrade;
  error: ApiError;
}

export enum ErrorOrigin {
  'Other' = 'Other',
  'Target' = 'Target',
  'Client' = 'Client',
}

export interface ApiError {
  message: string | null;
  origin: ErrorOrigin;
}
