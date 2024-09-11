import { BehaviorSubject } from 'rxjs'

import { Injectable } from '@angular/core'

@Injectable()
export class LimiterService {
    private _defaultLimit = 3
    public get defaultLimit(): number {
        return this._defaultLimit
    }

    private _defaultName = 'all'
    public get defaultName(): string {
        return this._defaultName
    }

    private activeReq = new BehaviorSubject<number>(0)
    public activeReq$ = this.activeReq.asObservable()

    public nameMap: Map<string, BehaviorSubject<number>> = new Map()

    constructor() {
        this.nameMap.set(this._defaultName, new BehaviorSubject<number>(0))
    }

    plus(reqName: string) {
        if (this.nameMap.has(reqName)) {
            const subj = this.nameMap.get(reqName)
            subj.next(subj.value + 1)
        } else {
            this.nameMap.set(reqName, new BehaviorSubject<number>(1))
        }

        if (reqName != this.defaultName) {
            const subj = this.nameMap.get(this.defaultName)
            subj.next(subj.value + 1)
        }
    }

    minus(reqName: string) {
        const subj = this.nameMap.get(reqName)
        subj.next(subj.value - 1)

        if (reqName != this.defaultName) {
            const subj = this.nameMap.get(this.defaultName)
            subj.next(subj.value - 1)
        }
    }

    isOutOfLimit(reqName: string, limit: number) {
        if (!this.nameMap.has(reqName)) return false

        return this.nameMap.get(reqName).value >= limit
    }

    addEmptyIfNotExist(reqName: string) {
        if (!this.nameMap.has(reqName)) {
            this.nameMap.set(reqName, new BehaviorSubject<number>(0))
        }
    }
}
