import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { LoginResponse } from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class AuthStateService {
  private readonly SESSION_KEY = 'tm_auth';

  private currentUserSubject: BehaviorSubject<LoginResponse | null>;
  public currentUser$: Observable<LoginResponse | null>;

  constructor() {
    const stored = sessionStorage.getItem(this.SESSION_KEY);
    const initial: LoginResponse | null = stored ? JSON.parse(stored) : null;
    this.currentUserSubject = new BehaviorSubject<LoginResponse | null>(initial);
    this.currentUser$ = this.currentUserSubject.asObservable();
  }

  get currentUser(): LoginResponse | null {
    return this.currentUserSubject.value;
  }

  get isLoggedIn(): boolean {
    return this.currentUserSubject.value !== null;
  }

  setUser(user: LoginResponse, username: string, password: string): void {
    const credentials = btoa(`${username}:${password}`);
    sessionStorage.setItem(this.SESSION_KEY, JSON.stringify(user));
    sessionStorage.setItem('tm_creds', credentials);
    this.currentUserSubject.next(user);
  }

  getCredentials(): string | null {
    return sessionStorage.getItem('tm_creds');
  }

  logout(): void {
    sessionStorage.removeItem(this.SESSION_KEY);
    sessionStorage.removeItem('tm_creds');
    this.currentUserSubject.next(null);
  }
}
