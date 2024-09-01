import { CommonModule } from '@angular/common'
import { Component, EventEmitter, Input, Output } from '@angular/core'

@Component({
    selector: 'app-paginator',
    templateUrl: './paginator.component.html',
    styleUrls: ['./paginator.component.css'],
    standalone: true,
    imports: [CommonModule],
})
export class PaginatorComponent {
    private _currentPage: number
    get currentPage(): number {
        return this._currentPage
    }

    @Input()
    set currentPage(value: number) {
        this._currentPage = +value

        if (this._currentPage < this.min) this._currentPage = this.min

        this.updatePages()
    }

    @Input()
    range = 3
    min = 1

    private _max = 0
    get max(): number {
        return this._max
    }

    @Input()
    set max(value: number) {
        value = +value

        if (value < this.min) return

        this._max = value % 1 > 0 && value > 1 ? Math.floor(value + 1) : value
    }

    pages: number[] = []

    @Output()
    changePage = new EventEmitter<number>()

    changePageClick(page: number) {
        this.changePage.emit(page)
    }

    updatePages() {
        this.pages = []
        const currentPage = +this.currentPage

        let min = currentPage - this.range >= this.min ? currentPage - this.range : this.min
        let max = currentPage + this.range <= this.max ? currentPage + this.range : this.max

        if (min > max || min == max) {
            if (min == 1) return

            min = this.max - this.range > 0 ? this.max - this.range : 1
            max = this.max
        }

        for (let i = min; i <= max; i++) {
            this.pages.push(i)
        }
    }
}
