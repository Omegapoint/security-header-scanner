import { ArrowDropDown, Public } from '@mui/icons-material';
import { Button, ButtonGroup, Checkbox, IconButton, Input, Menu, MenuItem, Stack } from '@mui/joy';
import { useNavigate } from '@tanstack/react-router';
import { useStore } from '@tanstack/react-store';
import React, { ChangeEvent, FormEvent, useEffect, useMemo, useRef, useState } from 'react';
import { ErrorOrigin, TargetKind, targetKindToString } from '../contracts/apiTypes.ts';
import { ensureLoaded, store } from '../data/store.tsx';
import { getUrl } from '../helpers/getUrl.ts';
import { isUrl } from '../helpers/isUrl.ts';

export const SiteSelector = () => {
  const [isValid, setIsValid] = useState(true);
  const [targetInputFocused, setTargetInputFocused] = useState(false);

  const [open, setOpen] = useState(false);
  const actionRef = useRef<() => void | null>(null);
  const anchorRef = useRef(null);

  const navigate = useNavigate();
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

  const handleSelectKind = (kind: TargetKind) => {
    store.setState((state) => ({ ...state, kind }));
    setOpen(false);
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
      if (!alreadyLoaded) {
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
      <ButtonGroup variant="solid" color="primary">
        <Button type="submit" loading={loading} sx={{ borderRadius: 0 }}>
          {getButtonTitle()}
        </Button>
        <IconButton
          size="sm"
          sx={{ left: '0.8rem' }}
          aria-controls={open ? 'split-button-menu' : undefined}
          aria-expanded={open ? 'true' : undefined}
          aria-haspopup="menu"
          title="Select scan type"
          onMouseDown={() => {
            // @ts-expect-error - ref
            actionRef.current = () => setOpen(!open);
          }}
          onClick={() => actionRef.current?.()}
        >
          <ArrowDropDown ref={anchorRef} />
        </IconButton>
      </ButtonGroup>
      <Menu open={open} onClose={() => setOpen(false)} anchorEl={anchorRef.current}>
        {options.map((option) => (
          <MenuItem key={option} selected={option == kind} onClick={() => handleSelectKind(option)}>
            {targetKindToString(option)}
          </MenuItem>
        ))}
      </Menu>
    </React.Fragment>
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
