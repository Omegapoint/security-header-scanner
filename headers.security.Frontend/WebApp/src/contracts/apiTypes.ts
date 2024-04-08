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
  utfDomain: string;
  asciiDomain: string;
  path: string;
  port: number;
  scheme: string;
  isDefaultPort: boolean;
}

export const getUriData = (url: URL | null): UriData | null => {
  if (url === null) {
    return null;
  }

  const path = url.href.substring(url.origin.length);
  const port = Number(url.port);
  const scheme = url.protocol.substring(0, url.protocol.length - 1);
  const isDefaultPort = (scheme === 'http' && port == 80) || (scheme === 'https' && port == 443);
  return {
    utfDomain: url.host,
    asciiDomain: url.host,
    path,
    port,
    scheme,
    isDefaultPort,
  };
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
