import { NgForm } from '@angular/forms'

export const markAllAsTouched = (form: NgForm) => {
    const controls = form.controls
    for (const element of Object.keys(controls)) {
        const control = controls[element]
        control.markAsTouched()
    }
}
