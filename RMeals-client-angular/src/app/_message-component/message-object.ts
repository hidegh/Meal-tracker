import {MessageTypeEnum} from "./message-type-enum";

export class MessageObject {
    id: number;
    type: MessageTypeEnum;
    message: string;
    isHtml: boolean;
    expireDate: Date;

    constructor(id: number, type: MessageTypeEnum, message: string, timeout: number, isHtml: boolean) {
        this.id = id;
        this.type = type;
        this.message = message;
        this.isHtml = isHtml;
        this.expireDate = new Date(Date.now() + timeout);
    }
}
