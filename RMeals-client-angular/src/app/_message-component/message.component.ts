import {Component, Input, TemplateRef} from "@angular/core";
import {MessageTypeEnum} from "./message-type-enum";
import {MessageObject} from "./message-object";

@Component({
    selector: 'ks-message-component',
    template: `
        <ng-template #defaultTemplate>
            <div *ngFor="let m of messages"
                 class="alert alert-dismissible show"
                 role="alert"
                 [ngClass]="{ 'alert-info': hasFlag(m.type, _messageTypeEnum.Info), 'alert-success': hasFlag(m.type, _messageTypeEnum.Success), 'alert-warning': hasFlag(m.type, _messageTypeEnum.Warning), 'alert-danger': hasFlag(m.type, _messageTypeEnum.Error) }">
                <button type="button" class="close" aria-label="Close" data-dismiss (click)="remove(m.id)">
                    <span aria-hidden="true">&times;</span>
                </button>
                <div *ngIf="m.isHtml" [innerHtml]="m.message"></div>
                <div *ngIf="!m.isHtml">{{m.message}}</div>
            </div>
        </ng-template>

        <ng-container *ngTemplateOutlet="customTemplate ? customTemplate : defaultTemplate"></ng-container>
    `,
})
export class MessageComponent {

    @Input() public defaultTimeout: number = 0;
    @Input() public defaultIsHtml: boolean = false;
    @Input() public singleMessageMode: boolean = false;
    @Input('template') public customTemplate: TemplateRef<any>;

    protected lastUsedId: number = 0;

    // NOTE: public due HTML ref.
    public messages: Array<MessageObject> = new Array();

    // NOTE: referencing enum in templates
    public _messageTypeEnum = MessageTypeEnum;

    private addMessage(type: MessageTypeEnum, message: string, timeout: number, isHtml: boolean): void {

        let messageObject = new MessageObject(++this.lastUsedId, type, message, timeout, isHtml);

        if (this.singleMessageMode)
            this.messages = new Array<MessageObject>(messageObject);
        else
            this.messages.push(messageObject);

        if (timeout > 0)
            setTimeout(_ => this.removeObsolete(), timeout);
    }

    // NOTE: public due HTML ref.
    public hasFlag(value: MessageTypeEnum, flag: MessageTypeEnum): boolean {
        return (value & flag) ? true : false;
    }

    public remove(id: number) {
        this.messages = this.messages.filter(_ => _.id != id /* expression to keep an item in the list */)
    }

    protected removeObsolete() {
        let currentDate = new Date(Date.now());
        this.messages = this.messages.filter(_ => _.expireDate > currentDate /* expression to keep an item in the list */)
    }

    public clear(): void {
        this.messages = new Array<MessageObject>();
    }

    public info(message: string, timeout: number = this.defaultTimeout, isHtml: boolean = this.defaultIsHtml): void {
        this.addMessage(MessageTypeEnum.Info, message, timeout, isHtml);
    }

    public success(message: string, timeout: number = this.defaultTimeout, isHtml: boolean = this.defaultIsHtml): void {
        this.addMessage(MessageTypeEnum.Success, message, timeout, isHtml);
    }

    public warn(message: string, timeout: number = this.defaultTimeout, isHtml: boolean = this.defaultIsHtml): void {
        this.addMessage(MessageTypeEnum.Warning, message, timeout, isHtml);
    }

    public error(message: string, timeout: number = this.defaultTimeout, isHtml: boolean = this.defaultIsHtml): void {
        this.addMessage(MessageTypeEnum.Error, message, timeout, isHtml);
    }

}
