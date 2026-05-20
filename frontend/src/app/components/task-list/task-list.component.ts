import { Component, OnInit } from '@angular/core';
import { TaskItem, TaskStatus, TASK_STATUS_LABELS } from '../../models/task.model';
import { User } from '../../models/user.model';
import { TaskService, TaskFilter } from '../../services/task.service';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-task-list',
  templateUrl: './task-list.component.html',
  styleUrls: ['./task-list.component.scss']
})
export class TaskListComponent implements OnInit {
  tasks: TaskItem[] = [];
  users: User[] = [];
  loading = false;
  deleteConfirmId: number | null = null;
  errorMessage = '';
  infoMessage = '';

  selectedTask: TaskItem | null = null;

  // Filters & sort
  filterStatus: string = '';
  filterAssigneeId: string = '';
  sortBy = 'createdAt';
  sortDesc = true;

  statusOptions = TASK_STATUS_LABELS;
  TaskStatus = TaskStatus;

  constructor(
    private taskService: TaskService,
    private userService: UserService
  ) {}

  ngOnInit(): void {
    this.loadUsers();
    this.loadTasks();
  }

  loadUsers(): void {
    this.userService.getAll().subscribe(
      users => this.users = users,
      () => {}
    );
  }

  loadTasks(): void {
    this.loading = true;
    this.errorMessage = '';
    const filter: TaskFilter = {
      sortBy: this.sortBy,
      sortDesc: this.sortDesc
    };
    if (this.filterStatus !== '') {
      filter.status = +this.filterStatus as TaskStatus;
    }
    if (this.filterAssigneeId !== '') {
      filter.assigneeId = +this.filterAssigneeId;
    }
    this.taskService.getAll(filter).subscribe(
      tasks => {
        this.tasks = tasks;
        this.loading = false;
      },
      err => {
        this.errorMessage = 'Failed to load tasks.';
        this.loading = false;
      }
    );
  }

  onFilterChange(): void {
    this.loadTasks();
  }

  onSortChange(column: string): void {
    if (this.sortBy === column) {
      this.sortDesc = !this.sortDesc;
    } else {
      this.sortBy = column;
      this.sortDesc = false;
    }
    this.loadTasks();
  }

  getSortIcon(column: string): string {
    if (this.sortBy !== column) { return '↕'; }
    return this.sortDesc ? '↓' : '↑';
  }

  onEditTask(task: TaskItem): void {
    this.selectedTask = { ...task };
    this.infoMessage = '';
  }

  onTaskSaved(task: TaskItem): void {
    this.infoMessage = '';
    if (this.selectedTask) {
      // update in list
      const idx = this.tasks.findIndex(t => t.id === task.id);
      if (idx >= 0) { this.tasks[idx] = task; }
      this.selectedTask = null;
    } else {
      this.tasks.unshift(task);
    }
  }

  onFormCancelled(): void {
    this.selectedTask = null;
  }

  confirmDelete(id: number): void {
    this.deleteConfirmId = id;
  }

  cancelDelete(): void {
    this.deleteConfirmId = null;
  }

  onDeleteTask(id: number): void {
    this.taskService.delete(id).subscribe(
      () => {
        this.tasks = this.tasks.filter(t => t.id !== id);
        this.deleteConfirmId = null;
        if (this.selectedTask?.id === id) { this.selectedTask = null; }
        this.infoMessage = 'Task deleted.';
        setTimeout(() => this.infoMessage = '', 3000);
      },
      err => {
        this.errorMessage = err.error?.message || 'Failed to delete task.';
        this.deleteConfirmId = null;
      }
    );
  }

  getStatusClass(status: TaskStatus): string {
    const map: { [key: number]: string } = {
      [TaskStatus.New]: 'status-new',
      [TaskStatus.InDevelopment]: 'status-dev',
      [TaskStatus.InTesting]: 'status-test',
      [TaskStatus.InDeployment]: 'status-deploy',
      [TaskStatus.Completed]: 'status-done'
    };
    return map[status] || '';
  }

  clearFilters(): void {
    this.filterStatus = '';
    this.filterAssigneeId = '';
    this.sortBy = 'createdAt';
    this.sortDesc = true;
    this.loadTasks();
  }

  today(): string {
    return new Date().toISOString().substring(0, 10);
  }
}
