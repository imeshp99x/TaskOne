import { Component, OnInit, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TaskItem, CreateTaskDto, UpdateTaskDto, TaskStatus, TASK_STATUS_LABELS } from '../../models/task.model';
import { User } from '../../models/user.model';
import { TaskService } from '../../services/task.service';

@Component({
  selector: 'app-task-form',
  templateUrl: './task-form.component.html',
  styleUrls: ['./task-form.component.scss']
})
export class TaskFormComponent implements OnInit, OnChanges {
  @Input() editTask: TaskItem | null = null;
  @Input() users: User[] = [];
  @Output() taskSaved = new EventEmitter<TaskItem>();
  @Output() formCancelled = new EventEmitter<void>();

  form: FormGroup;
  loading = false;
  successMessage = '';
  errorMessage = '';
  statusOptions = TASK_STATUS_LABELS;
  TaskStatus = TaskStatus;

  constructor(private fb: FormBuilder, private taskService: TaskService) {}

  ngOnInit(): void {
    this.buildForm();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.editTask && this.form) {
      this.populateForm();
    }
  }

  get isEditMode(): boolean {
    return this.editTask !== null;
  }

  get f() { return this.form.controls; }

  private buildForm(): void {
    this.form = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(200)]],
      description: ['', [Validators.maxLength(1000)]],
      status: [TaskStatus.New, Validators.required],
      dueDate: [''],
      assigneeId: [null]
    });
    this.populateForm();
  }

  private populateForm(): void {
    if (!this.form) { return; }
    if (this.editTask) {
      this.form.patchValue({
        title: this.editTask.title,
        description: this.editTask.description || '',
        status: this.editTask.status,
        dueDate: this.editTask.dueDate ? this.editTask.dueDate.substring(0, 10) : '',
        assigneeId: this.editTask.assigneeId || null
      });
    } else {
      this.form.reset({ status: TaskStatus.New, assigneeId: null, dueDate: '' });
    }
    this.successMessage = '';
    this.errorMessage = '';
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.loading = true;
    this.successMessage = '';
    this.errorMessage = '';

    const dto: CreateTaskDto = {
      title: this.f.title.value.trim(),
      description: this.f.description.value.trim() || undefined,
      status: +this.f.status.value,
      dueDate: this.f.dueDate.value || undefined,
      assigneeId: this.f.assigneeId.value ? +this.f.assigneeId.value : undefined
    };

    const obs = this.isEditMode
      ? this.taskService.update(this.editTask!.id, dto as UpdateTaskDto)
      : this.taskService.create(dto);

    obs.subscribe(
      task => {
        this.loading = false;
        this.successMessage = this.isEditMode
          ? 'Task updated successfully!'
          : 'Task created successfully!';
        if (!this.isEditMode) {
          this.form.reset({ status: TaskStatus.New, assigneeId: null, dueDate: '' });
        }
        this.taskSaved.emit(task);
      },
      err => {
        this.loading = false;
        this.errorMessage = err.error?.message || 'An error occurred. Please try again.';
      }
    );
  }

  onCancel(): void {
    this.formCancelled.emit();
  }
}
