import { CancelRounded, CheckCircleRounded, InfoRounded, RemoveCircleRounded } from '@mui/icons-material';
import { Avatar } from '@mui/joy';
import { SecurityImpact } from '../contracts/securityImpact.ts';

const getIcon = (impact: SecurityImpact) => {
  switch (impact) {
    case SecurityImpact.Medium:
    case SecurityImpact.Low:
      return RemoveCircleRounded;
    case SecurityImpact.Info:
      return InfoRounded;
    case SecurityImpact.None:
      return CheckCircleRounded;
  }
  return CancelRounded;
};

const getColour = (impact: SecurityImpact): string => {
  switch (impact) {
    case SecurityImpact.Medium:
    case SecurityImpact.Low:
      return '#DD9933';
    case SecurityImpact.Info:
      return '#3399DD';
    case SecurityImpact.None:
      return '#339933';
  }
  return '#EE3333';
};

interface ImpactIconProps {
  impact: SecurityImpact;
}

export const ImpactIcon = ({ impact }: ImpactIconProps) => {
  const Icon = getIcon(impact);
  const iconColour = getColour(impact);
  const background = 'white';

  return (
    <Avatar sx={{ '--Avatar-size': '1.6em', background, overflow: 'visible' }}>
      <Icon fontSize="xl" htmlColor={iconColour} />
    </Avatar>
  );
};
