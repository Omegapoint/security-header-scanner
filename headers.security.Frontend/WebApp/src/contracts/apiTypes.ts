import { CspHandlerName } from './securityConcepts/cspSecurityConcept.ts';
import { HSTSHandlerName } from './securityConcepts/hstsSecurityConceptResult.ts';
import { OverallSecurityGrade } from './securityGrade.ts';
import { SecurityImpact } from './securityImpact.ts';

export interface SecurityConceptResultInfo {
  message: string;
}

type XContentTypeOptionsHandlerName = 'X-Content-Type-Options';
type XFrameOptionsHandlerName = 'X-Frame-Options';
type PermissionsPolicyHandlerName = 'Permissions-Policy';
type ReferrerPolicyHandlerName = 'Referrer-Policy';
export type GradeInfluencingHandlerName =
  | CspHandlerName
  | HSTSHandlerName
  | XContentTypeOptionsHandlerName
  | XFrameOptionsHandlerName
  | PermissionsPolicyHandlerName
  | ReferrerPolicyHandlerName;

type CorsHandlerName = 'Access-Control-Allow-Origin';
type ServerHandlerName = 'Server';
export type NonInfluencingHandlerName = ServerHandlerName | CorsHandlerName;

export type SecurityConceptHandlerName = GradeInfluencingHandlerName | NonInfluencingHandlerName;

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
  grade: OverallSecurityGrade;
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
