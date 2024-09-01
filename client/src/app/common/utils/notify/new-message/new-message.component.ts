import { Toast } from 'ngx-toastr'

import { Component } from '@angular/core'

@Component({
    selector: 'app-new-message',
    templateUrl: './new-message.component.html',
    styleUrls: ['./new-message.component.css'],
})
export class NewMessageComponent extends Toast {}
