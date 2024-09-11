import { TranslateService } from '@ngx-translate/core'
import { ToastrService } from 'ngx-toastr'
import { map, take } from 'rxjs'

import { inject } from '@angular/core'
import {
    ActivatedRouteSnapshot,
    CanActivateChildFn,
    CanActivateFn,
    Router,
    RouterStateSnapshot,
} from '@angular/router'

import { Roles } from 'src/shared/roles.enum'

import { AuthService, User } from '../../auth'

export const canActivateAdmin: CanActivateFn = () => {
    const authService = inject(AuthService)
    const router = inject(Router)
    const toastr = inject(ToastrService)
    const trans = inject(TranslateService)

    return authService.user$.pipe(
        take(1),
        map((user: User) => {
            if (!user || user.role == Roles.USER) {
                trans.get('labels.no-page-access-permission').subscribe((str) => {
                    toastr.warning(str)
                })

                router.navigate(['../'])
                return false
            }

            return true
        })
    )
}

export const canActivateAdminChild: CanActivateChildFn = (
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
) => canActivateAdmin(route, state)

export const canActivateModer: CanActivateFn = (
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
) => {
    const authService = inject(AuthService)
    const router = inject(Router)
    const toastr = inject(ToastrService)

    return authService.user$.pipe(
        take(1),
        map((user: User) => {
            if (!user || user.role == Roles.USER) {
                toastr.warning("You don't have permission to access this page")
                router.navigate(['../'])
                return false
            }

            return true
        })
    )
}

export const canActivateModerChild: CanActivateChildFn = (
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
) => canActivateModer(route, state)
