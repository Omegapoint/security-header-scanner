export enum SecurityGrade {
  Unknown = 'Unknown',
  F = 'F',
  E = 'E',
  D = 'D',
  C = 'C',
  B = 'B',
  A = 'A',
  NonInfluencing = 'NonInfluencing',
}

export type OverallSecurityGrade = Exclude<SecurityGrade, SecurityGrade.NonInfluencing | SecurityGrade.Unknown>;
