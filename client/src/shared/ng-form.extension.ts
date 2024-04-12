import { NgForm } from "@angular/forms";

export class NgFormExtension{

  static markAllAsTouched(form: NgForm) {
    let controls = form.controls;
    for(const element of Object.keys(controls)) {
      let control = controls[element];
      control.markAsTouched();
    }
  }

}
