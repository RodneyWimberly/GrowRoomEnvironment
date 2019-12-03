import { Utilities } from '../helpers/utilities';


export class NotificationModel {


    public id: number;
    public header: string;
    public body: string;
    public isRead: boolean;
    public isPinned: boolean;
    public date: Date;

    public static Create(data: {}) {
        const n = new NotificationModel();
        Object.assign(n, data);

        if (n.date) {
            n.date = Utilities.parseDate(n.date);
        }

        return n;
    }
}
