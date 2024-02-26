import { CancelRounded, CheckCircleRounded, HelpRounded, RemoveCircleRounded } from '@mui/icons-material';
import { Avatar } from '@mui/joy';
import { SecurityGrade } from '../contracts/securityGrade.ts';

const getIcon = (grade: SecurityGrade) => {
  switch (grade) {
    case SecurityGrade.Unknown:
      return HelpRounded;
    case SecurityGrade.D:
    case SecurityGrade.C:
      return RemoveCircleRounded;
    case SecurityGrade.B:
    case SecurityGrade.A:
    case SecurityGrade.NonInfluencing:
      return CheckCircleRounded;
  }
  return CancelRounded;
};

const getColour = (grade: SecurityGrade): string => {
  switch (grade) {
    case SecurityGrade.Unknown:
      return '#4444CC';
    case SecurityGrade.D:
    case SecurityGrade.C:
      return '#DD9933';
    case SecurityGrade.B:
    case SecurityGrade.A:
    case SecurityGrade.NonInfluencing:
      return '#339933';
  }
  return '#EE3333';
};

interface GradingIconProps {
  grade: SecurityGrade;
}

export const GradingIcon = ({ grade }: GradingIconProps) => {
  const Icon = getIcon(grade);
  const iconColour = getColour(grade);
  const background = 'white';

  return (
    <Avatar sx={{ '--Avatar-size': '1.69em', background }}>
      <Icon fontSize="xl" htmlColor={iconColour} />
    </Avatar>
  );
};
