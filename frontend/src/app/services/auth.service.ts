import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { LoginDto, LoginResponse, CreateUserDto } from '../models/user.model';
import { AuthStateService } from './auth-state.service';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private baseUrl = `${environment.apiUrl}/auth`;

  constructor(private http: HttpClient, private authState: AuthStateService) {}

  login(dto: LoginDto): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.baseUrl}/login`, dto).pipe(
      tap(user => this.authState.setUser(user, dto.username, dto.password))
    );
  }

  logout(): void {
    this.authState.logout();
  }
}
