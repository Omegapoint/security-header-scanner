export const getUrl = (target?: string) => {
  let theTarget = target ?? '';
  if (!theTarget.includes('://')) {
    theTarget = `https://${theTarget}`;
  }

  try {
    return new URL(theTarget);
  } catch {
    /* empty */
  }

  return null;
};
