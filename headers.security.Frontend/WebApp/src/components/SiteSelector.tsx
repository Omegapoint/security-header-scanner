import { Public } from '@mui/icons-material';
import { Button, Checkbox, Input, Stack } from '@mui/joy';
import { useNavigate } from '@tanstack/react-router';
import { useStore } from '@tanstack/react-store';
import { ChangeEvent, FormEvent, useEffect, useMemo, useState } from 'react';
import { ErrorOrigin } from '../contracts/apiTypes.ts';
import { ensureLoaded, store } from '../data/store.tsx';
import { getUrl } from '../helpers/getUrl.ts';
import { isUrl } from '../helpers/isUrl.ts';

export const SiteSelector = () => {
  const [isValid, setIsValid] = useState(true);
  const [targetInputFocused, setTargetInputFocused] = useState(false);
  const navigate = useNavigate();
  const { target, followRedirects, loading, apiError, apiResponse } = useStore(store, (state) => state);
  const targetWithProto = useMemo(() => {
    const uri = getUrl(target);
    return uri?.href ?? '';
  }, [target]);

  useEffect(() => {
    if (
      apiError?.origin === ErrorOrigin.Target ||
      apiResponse?.results.every((r) => r.error?.origin === ErrorOrigin.Target)
    ) {
      setIsValid(false);
    }
  }, [apiError?.origin, apiResponse]);

  const targetFieldValue = targetInputFocused ? target : targetWithProto;

  const updateTarget = (e: ChangeEvent<HTMLInputElement>) => {
    store.setState((state) => ({ ...state, target: e.target.value }));
    setIsValid(true);
  };

  const updateFollowRedirects = (e: ChangeEvent<HTMLInputElement>) => {
    store.setState((state) => ({ ...state, followRedirects: e.target.checked }));
  };

  const scanSite = async (event: FormEvent) => {
    event.preventDefault();

    const newIsValid = isUrl(target);
    setIsValid(newIsValid);

    if (newIsValid) {
      const search = { target, followRedirects };
      const alreadyLoaded = await ensureLoaded(search);
      if (!alreadyLoaded) {
        await navigate({ to: '/scan', search });
      }
    }
  };

  const endDecorator = (
    <Button type="submit" variant="solid" loading={loading} sx={{ borderTopLeftRadius: 0, borderBottomLeftRadius: 0 }}>
      Scan now
    </Button>
  );
  return (
    <form onSubmit={scanSite}>
      <Stack direction="column" alignItems="flex-end" alignContent="center" spacing={1} width="50vw">
        <Input
          sx={{
            boxShadow: 'md',
            '--Input-decoratorChildHeight': '45px',
          }}
          onFocus={() => setTargetInputFocused(true)}
          onBlur={() => setTargetInputFocused(false)}
          variant="soft"
          autoFocus={true}
          startDecorator={<Public />}
          endDecorator={endDecorator}
          placeholder="Enter URL to scan"
          value={targetFieldValue || ''}
          onChange={updateTarget}
          required
          error={!isValid}
          fullWidth={true}
        />
        <Checkbox size="sm" label="Follow redirects" checked={followRedirects} onChange={updateFollowRedirects} />
      </Stack>
    </form>
  );
};
