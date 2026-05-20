import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { TaskItem, CreateTaskDto, UpdateTaskDto, TaskStatus } from '../models/task.model';

export interface TaskFilter {
  status?: TaskStatus;
  assigneeId?: number;
  sortBy?: string;
  sortDesc?: boolean;
}

@Injectable({ providedIn: 'root' })
export class TaskService {
  private baseUrl = `${environment.apiUrl}/tasks`;

  constructor(private http: HttpClient) {}

  getAll(filter: TaskFilter = {}): Observable<TaskItem[]> {
    let params = new HttpParams();
    if (filter.status !== undefined && filter.status !== null) {
      params = params.set('status', filter.status.toString());
    }
    if (filter.assigneeId) {
      params = params.set('assigneeId', filter.assigneeId.toString());
    }
    if (filter.sortBy) {
      params = params.set('sortBy', filter.sortBy);
    }
    if (filter.sortDesc) {
      params = params.set('sortDesc', 'true');
    }
    return this.http.get<TaskItem[]>(this.baseUrl, { params });
  }

  getById(id: number): Observable<TaskItem> {
    return this.http.get<TaskItem>(`${this.baseUrl}/${id}`);
  }

  create(dto: CreateTaskDto): Observable<TaskItem> {
    return this.http.post<TaskItem>(this.baseUrl, dto);
  }

  update(id: number, dto: UpdateTaskDto): Observable<TaskItem> {
    return this.http.put<TaskItem>(`${this.baseUrl}/${id}`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
