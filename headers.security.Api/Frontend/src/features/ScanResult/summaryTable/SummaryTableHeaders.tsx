import { List, ListDivider, ListItem, ListItemContent, ListItemDecorator, Typography, useTheme } from '@mui/joy';
import { useMediaQuery } from '@mui/material';
import { ImpactIcon } from '../../../components/ImpactIcon.tsx';
import { ISecurityConceptResult, SecurityConceptHandlerName, ServerResult } from '../../../contracts/apiTypes.ts';

interface SummaryTableHeadersItemProps {
  handlerResult: ISecurityConceptResult;
  showSeparator: boolean;
}

const SummaryTableHeadersItem = ({ handlerResult, showSeparator }: SummaryTableHeadersItemProps) => {
  const { breakpoints } = useTheme();
  const isMobile = useMediaQuery(breakpoints.only("xs"));

  const { headerName, impact } = handlerResult;

  const label = <ListItemContent>
    <Typography level="title-sm">{headerName}</Typography>
  </ListItemContent>
  const decorator = <ListItemDecorator>
    <ImpactIcon impact={impact} />
  </ListItemDecorator>

  return (
    <>
      <ListItem>
        {isMobile && decorator}
        {label}
        {!isMobile && decorator}
      </ListItem>
      {showSeparator && <ListDivider inset="gutter" />}
    </>
  );
};

const hiddenHandlers = [SecurityConceptHandlerName.Server];

export const SummaryTableHeaders = ({ data }: { data: ServerResult }) => {
  const { breakpoints } = useTheme();
  const isMobile = useMediaQuery(breakpoints.only("xs"));

  const headers = data.result.handlerResults.filter((h) => {
    // Remove explicitly hidden handlers
    if (hiddenHandlers.includes(h.handlerName)) {
      return false;
    }

    // Remove handlers showing an info impact, ensure all with a non-zero impact are always shown
    if (h.impact == 'Info') return false;
    if (h.impact != 'None') return true;

    // Remove handlers with no impact and no processed value
    return h.processedValue != null;
  });

  const last = headers.length - 1;

  const orientation = isMobile ? 'vertical' : 'horizontal';

  return (
    <List orientation={orientation} wrap size="sm" sx={{ marginLeft: '-0.5em', marginTop: '-0.1em' }}>
      {headers.map((handlerResult, idx) => (
        <SummaryTableHeadersItem handlerResult={handlerResult} showSeparator={idx != last && !isMobile} key={idx} />
      ))}
    </List>
  );
};
