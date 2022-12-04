export interface Message {
    id: number;
    senderId: string;
    senderUserName: string;
    senderPhotoUrl?: any;
    recipientId: string;
    recipientUserName: string;
    recipientPhotoUrl?: any;
    content: string;
    dateRead: Date;
    messageSent: Date;
}
