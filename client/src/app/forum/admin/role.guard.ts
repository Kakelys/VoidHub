import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, CanActivateChildFn, CanActivateFn, Router, RouterStateSnapshot } from "@angular/router";
import { TranslateService } from "@ngx-translate/core";
import { ToastrService } from "ngx-toastr";
import { map, take } from "rxjs";
import { AuthService } from "src/app/auth/auth.service";
import { User } from "src/shared/models/user.model";
import { Roles } from "src/shared/roles.enum";

export const canActivateAdmin: CanActivateFn = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const toastr = inject(ToastrService);
  const trans = inject(TranslateService);

  return authService.user$.pipe(take(1), map((user:User) => {
    if (!user || user.role == Roles.USER) {
      console.log('here');
      trans.get('labels.no-page-access-permission')
      .subscribe(str => {
        toastr.warning(str);
      })

      router.navigate(["../"]);
      return false;
    }

    return true;
  }));
};

export const canActivateAdminChild: CanActivateChildFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => canActivateAdmin(route, state);

export const canActivateModer: CanActivateFn = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const toastr = inject(ToastrService);

  return authService.user$.pipe(take(1), map((user:User) => {
    if (!user || user.role == Roles.USER) {
      toastr.warning("You don't have permission to access this page");
      router.navigate(["../"]);
      return false;
    }

    return true;
  }));
};

export const canActivateModerChild: CanActivateChildFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => canActivateModer(route, state);


