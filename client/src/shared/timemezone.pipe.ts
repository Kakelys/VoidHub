import { Pipe, PipeTransform } from '@angular/core'

@Pipe({
    standalone: true,
    name: 'withtimezone',
})
export class TimezonePipe implements PipeTransform {
    transform(utcDate: Date): Date {
        return utcDate
        // TODO: fix timezone
        utcDate = new Date(utcDate)
        const timezoneOffsetHours = new Date().getTimezoneOffset() / 60
        const hoursToAddOrSubtract = -timezoneOffsetHours

        utcDate.setHours(utcDate.getHours() + hoursToAddOrSubtract)
        return utcDate
    }
}
