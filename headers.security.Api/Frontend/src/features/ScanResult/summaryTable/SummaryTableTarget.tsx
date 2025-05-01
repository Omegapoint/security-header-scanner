import { Link, Typography } from '@mui/joy';
import { useNavigate } from '@tanstack/react-router';
import { useStore } from '@tanstack/react-store';
import { ScanResult, TargetKind, targetKindToString } from '../../../contracts/apiTypes.ts';
import { ensureLoaded, store } from '../../../data/store.tsx';
import { Route } from '../../../routes/scan.tsx';

export const SummaryTableTarget = (props: { result: ScanResult }) => {
  const navigate = useNavigate();
  const response = useStore(store, (state) => state.apiResponse);
  const { kind } = Route.useSearch();

  const resultKind = props.result.targetKind;
  const detectKind = (kind ?? TargetKind.Detect) === TargetKind.Detect;
  const oppositeKind = [TargetKind.Api, TargetKind.Both].includes(resultKind) ? TargetKind.Frontend : TargetKind.Api;

  if (response?.request == null) {
    return;
  }

  const cleanTarget = (target: string) => {
    if (target?.startsWith('https://')) {
      target = target?.substring(8);
    }
    if (target?.endsWith('/')) {
      target = target?.substring(0, target?.length - 1);
    }
    return target;
  };

  const rescan = async () => {
    const search = {
      ...response.request,
      kind: oppositeKind,
      target: cleanTarget(response.request.target),
    };

    const alreadyLoaded = await ensureLoaded(search);
    if (!alreadyLoaded) {
      await navigate({ to: '/scan', search });
    }
  };

  const detected = (
    <>
      <Typography color="neutral" noWrap>
        {' '}
        (detected,{' '}
        <Link color="neutral" underline="always" onClick={rescan}>
          scan as {targetKindToString(oppositeKind)}
        </Link>
        )
      </Typography>
    </>
  );

  return (
    <Typography>
      <Typography>{targetKindToString(resultKind)}</Typography>
      {detectKind && detected}
    </Typography>
  );
};
