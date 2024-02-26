import { ISecurityConceptResult } from '../apiTypes.ts';

export type HSTSHandlerName = 'HTTP Strict Transport Security';

interface HSTSValue {
  maxAge?: number;
  includeSubdomains: boolean;
  preload: boolean;
}

export interface HstsSecurityConceptResult extends ISecurityConceptResult {
  handlerName: HSTSHandlerName;
  processedValue: HSTSValue;
}
