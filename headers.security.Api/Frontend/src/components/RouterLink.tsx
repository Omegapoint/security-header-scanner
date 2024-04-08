import { Link as JoyLink, LinkProps } from '@mui/joy';
import { AnyRoute, LinkOptions, RegisteredRouter, Link as RouterLinkComponent } from '@tanstack/react-router';

type LinkRouterProps<
  TRouteTree extends AnyRoute = RegisteredRouter['routeTree'],
  TTo extends string = '',
> = LinkOptions<TRouteTree, '/', TTo> & LinkProps;

export const RouterLink = <TRouteTree extends AnyRoute = RegisteredRouter['routeTree'], TTo extends string = ''>(
  props: LinkRouterProps<TRouteTree, TTo>
) => {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  return <JoyLink {...props} component={RouterLinkComponent as any} />;
};
