import { NgForm } from '@angular/forms'

export class NgFormExtension {
    static markAllAsTouched(form: NgForm) {
        const controls = form.controls
        for (const element of Object.keys(controls)) {
            const control = controls[element]
            control.markAsTouched()
        }
    }
}
