import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { AuthStateService } from '../services/auth-state.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(private authState: AuthStateService, private router: Router) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const credentials = this.authState.getCredentials();
    let authReq = req;
    if (credentials) {
      authReq = req.clone({
        setHeaders: { Authorization: `Basic ${credentials}` }
      });
    }

    return next.handle(authReq).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          this.authState.logout();
          this.router.navigate(['/login']);
        }
        return throwError(error);
      })
    );
  }
}
