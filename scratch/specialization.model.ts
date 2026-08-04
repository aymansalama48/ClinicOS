export enum DayOfWeek {
  Sunday = 0,
  Monday = 1,
  Tuesday = 2,
  Wednesday = 3,
  Thursday = 4,
  Friday = 5,
  Saturday = 6
}

export enum PeriodType {
  Morning = 1,
  Evening = 2
}

export interface SpecializationSchedule {
  id?: string;
  dayOfWeek: DayOfWeek;
  period: PeriodType;
  startTime: string; // 'HH:mm'
  endTime: string;   // 'HH:mm'
  isActive?: boolean;
}

export interface Specialization {
  id: string;
  name: string;
  description?: string;
  isActive: boolean;
  iconAttachmentId?: string;
  schedules?: SpecializationSchedule[];
}
