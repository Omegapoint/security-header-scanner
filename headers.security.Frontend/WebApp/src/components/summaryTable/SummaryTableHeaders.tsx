import { List, ListDivider, ListItem, ListItemContent, ListItemDecorator, Typography } from '@mui/joy';
import { ISecurityConceptResult, ServerResult } from '../../contracts/apiTypes.ts';
import { ImpactIcon } from '../ImpactIcon.tsx';

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

export const SummaryTableHeaders = ({ data }: { data: ServerResult }) => {
  // TODO: maybe need to separate certain handlers in backend already...
  const headers = data.result.handlerResults.filter((h) => h.impact != 'Info');
  const last = headers.length - 1;
  return (
    <List orientation="horizontal" wrap={true} size="sm" sx={{ marginLeft: '-0.5em', marginTop: '-0.1em' }}>
      {headers.map((handlerResult, idx) => (
        <SummaryTableHeadersItem handlerResult={handlerResult} last={idx != last} key={idx} />
      ))}
    </List>
  );
};
