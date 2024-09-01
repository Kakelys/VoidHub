import { map, take } from 'rxjs'

import { inject } from '@angular/core'
import {
    ActivatedRouteSnapshot,
    CanActivateChildFn,
    CanActivateFn,
    Router,
    RouterStateSnapshot,
} from '@angular/router'

import { AuthService, User } from '../../auth'

export const canActivateSelf: CanActivateFn = () => {
    const authService = inject(AuthService)
    const router = inject(Router)

    return authService.user$.pipe(
        take(1),
        map((user: User) => {
            if (!user) {
                router.navigate(['../'])
                return false
            }

            return true
        })
    )
}

export const canActivateSelfChild: CanActivateChildFn = (
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
) => canActivateSelf(route, state)
