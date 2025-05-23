import { ArrowDropDown, Public } from '@mui/icons-material';
import { Button, ButtonGroup, Checkbox, Dropdown, Input, Menu, MenuButton, MenuItem, Stack } from '@mui/joy';
import { useNavigate, useRouterState } from '@tanstack/react-router';
import { useStore } from '@tanstack/react-store';
import React, { ChangeEvent, FormEvent, useEffect, useMemo, useState } from 'react';
import { ErrorOrigin, TargetKind, targetKindToString } from '../contracts/apiTypes.ts';
import { ensureLoaded, store } from '../data/store.tsx';
import { getUrl } from '../helpers/getUrl.ts';
import { isUrl } from '../helpers/isUrl.ts';

export const SiteSelector = () => {
  const [isValid, setIsValid] = useState(true);
  const [targetInputFocused, setTargetInputFocused] = useState(false);

  const navigate = useNavigate();
  const location = useRouterState({ select: (s) => s.location });

  const { target, kind, followRedirects, loading, apiError, apiResponse } = useStore(store, (state) => state);
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

  const options: TargetKind[] = [TargetKind.Api, TargetKind.Frontend, TargetKind.Both];

  const handleSelectKind = (selectedKind?: TargetKind) => {
    if (kind == selectedKind) {
      selectedKind = undefined;
    }
    store.setState((state) => ({ ...state, kind: selectedKind }));
  };

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
      const search = { target, kind, followRedirects };
      const alreadyLoaded = await ensureLoaded(search);
      if (!alreadyLoaded || location.pathname != '/scan') {
        await navigate({ to: '/scan', search });
      }
    }
  };

  const getButtonTitle = () => {
    if (!kind || kind == 'Both' || kind == 'Detect') {
      return 'Scan site';
    }

    if (kind == 'Api') {
      return 'Scan as API';
    }

    return `Scan as ${kind?.toLowerCase()}`;
  };

  const endDecorator = (
    <React.Fragment>
      <Dropdown>
      <ButtonGroup variant="solid" color="primary">
        <Button type="submit" loading={loading} sx={{ borderRadius: 0, zIndex: 'unset !important' }}>
          {getButtonTitle()}
        </Button>
        <MenuButton
          color="primary"
          size="sm"
          sx={{ left: '0.75rem', paddingInline: '0.1rem' }}
          title="Select scan type"
          >
            <ArrowDropDown />
          </MenuButton>
        </ButtonGroup>
        <Menu variant="soft" color="primary">
          {options.map((option) => (
            <MenuItem key={option} selected={option == kind} onClick={() => handleSelectKind(option)}>
              {targetKindToString(option)}
            </MenuItem>
          ))}
        </Menu>
      </Dropdown>
    </React.Fragment>
  );
  return (
    <form onSubmit={scanSite}>
      <Stack
        direction="column"
        alignItems="flex-end"
        alignContent="center"
        spacing={1}
        minWidth="50vw"
        sx={(theme) => ({
          [theme.breakpoints.down('md')]: { minWidth: '85vw' },
        })}
      >
        <Input
          sx={{
            boxShadow: 'md',
            '--Input-decoratorChildHeight': '45px',
          }}
          onFocus={() => setTargetInputFocused(true)}
          onBlur={() => setTargetInputFocused(false)}
          variant="soft"
          autoFocus={true}
          autoComplete="off"
          startDecorator={
            <Public
              sx={(theme) => ({
                [theme.breakpoints.only('xs')]: { display: 'none' },
              })}
            />
          }
          endDecorator={endDecorator}
          placeholder="Enter URL to scan"
          value={targetFieldValue || ''}
          onChange={updateTarget}
          required
          error={!isValid}
          fullWidth
        />
        <Checkbox size="sm" label="Follow redirects" checked={followRedirects} onChange={updateFollowRedirects} />
      </Stack>
    </form>
  );
};
