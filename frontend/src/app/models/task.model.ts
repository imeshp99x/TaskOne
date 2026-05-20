export enum TaskStatus {
  New = 0,
  InDevelopment = 1,
  InTesting = 2,
  InDeployment = 3,
  Completed = 4
}

export const TASK_STATUS_LABELS: { value: TaskStatus; label: string }[] = [
  { value: TaskStatus.New, label: 'New' },
  { value: TaskStatus.InDevelopment, label: 'In Development' },
  { value: TaskStatus.InTesting, label: 'In Testing' },
  { value: TaskStatus.InDeployment, label: 'In Deployment' },
  { value: TaskStatus.Completed, label: 'Completed' }
];

export interface TaskItem {
  id: number;
  title: string;
  description?: string;
  status: TaskStatus;
  statusLabel: string;
  createdAt: string;
  updatedAt?: string;
  dueDate?: string;
  assigneeId?: number;
  assigneeName?: string;
}

export interface CreateTaskDto {
  title: string;
  description?: string;
  status: TaskStatus;
  dueDate?: string;
  assigneeId?: number;
}

export interface UpdateTaskDto {
  title: string;
  description?: string;
  status: TaskStatus;
  dueDate?: string;
  assigneeId?: number;
}
