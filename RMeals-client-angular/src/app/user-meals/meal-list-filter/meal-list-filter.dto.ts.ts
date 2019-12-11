import * as moment from 'moment';

export class MealListFilterDto {
  public dateStringFrom: string;
  public dateStringTo: string;
  public timeStringFrom: string;
  public timeStringTo: string;

  private stringToDate(dateString: string): Date {
    if (dateString) {
      const m = moment(dateString);
      return m.isValid() ? m.toDate() : undefined;
    }

    return undefined;
  }

  private stringToTimeSpanString(timeString: string): string {
    if (timeString) {
      const m = moment.utc(timeString, "HH:mm a");
      return m.isValid() ? m.format("HH:mm") : undefined;
    }

    return undefined;
  }

  public get dateFrom(): Date { return this.stringToDate(this.dateStringFrom); }
  public get dateTo(): Date { return this.stringToDate(this.dateStringTo); }

  public get timeFrom(): string { return this.stringToTimeSpanString(this.timeStringFrom); }
  public get timeTo(): string { return this.stringToTimeSpanString(this.timeStringTo); }

}
