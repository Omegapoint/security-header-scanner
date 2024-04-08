import { List, ListDivider, ListItem, ListItemContent, ListItemDecorator, Typography } from '@mui/joy';
import { ImpactIcon } from '../../../components/ImpactIcon.tsx';
import { ISecurityConceptResult, SecurityConceptHandlerName, ServerResult } from '../../../contracts/apiTypes.ts';

interface SummaryTableHeadersItemProps {
  handlerResult: ISecurityConceptResult;
  last: boolean;
}

const SummaryTableHeadersItem = ({ handlerResult, last }: SummaryTableHeadersItemProps) => {
  const { headerName, impact } = handlerResult;

  return (
    <>
      <ListItem>
        <ListItemContent>
          <Typography level="title-sm">{headerName}</Typography>
        </ListItemContent>
        <ListItemDecorator>
          <ImpactIcon impact={impact} />
        </ListItemDecorator>
      </ListItem>
      {last && <ListDivider inset="gutter" />}
    </>
  );
};

const hiddenHandlers = [SecurityConceptHandlerName.Server];

export const SummaryTableHeaders = ({ data }: { data: ServerResult }) => {
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

  return (
    <List orientation="horizontal" wrap={true} size="sm" sx={{ marginLeft: '-0.5em', marginTop: '-0.1em' }}>
      {headers.map((handlerResult, idx) => (
        <SummaryTableHeadersItem handlerResult={handlerResult} last={idx != last} key={idx} />
      ))}
    </List>
  );
};
