import { ISecurityConceptResult, SecurityConceptHandlerName } from '../apiTypes.ts';

interface HSTSValue {
  maxAge?: number;
  includeSubdomains: boolean;
  preload: boolean;
}

export interface HstsSecurityConceptResult extends ISecurityConceptResult {
  handlerName: SecurityConceptHandlerName.HTTPStrictTransportSecurity;
  processedValue: HSTSValue;
}
