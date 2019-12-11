import {NgModule} from '@angular/core';
import {CommonModule} from "@angular/common";
import {MessageComponent} from "./message.component";

@NgModule({
    imports: [
        CommonModule
    ],
    declarations: [
        MessageComponent
    ],
    exports: [
        MessageComponent
    ]
})
export class MessageModule {
}
