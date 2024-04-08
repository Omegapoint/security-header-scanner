import { ISecurityConceptResult, SecurityConceptHandlerName } from '../apiTypes.ts';

export interface CspPolicy {
  source: 'HttpEquiv' | 'Header' | 'Merged';
  enforcing: boolean;
  directives: Record<string, string[]>;
}

interface CspConfiguration {
  all: CspPolicy[];
  allNonEnforcing: CspPolicy[];
  effective: CspPolicy;
  hasPolicy: boolean;
}

export interface CspSecurityConceptResult extends ISecurityConceptResult {
  handlerName: SecurityConceptHandlerName.ContentSecurityPolicy;
  processedValue: CspConfiguration;
}
