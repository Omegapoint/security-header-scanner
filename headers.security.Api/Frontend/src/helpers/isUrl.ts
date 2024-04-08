import { getUrl } from './getUrl.ts';

export const isUrl = (target?: string) => {
  return target != null && getUrl(target) != null;
};
