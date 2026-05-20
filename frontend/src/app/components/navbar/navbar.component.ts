import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthStateService } from '../../services/auth-state.service';
import { AuthService } from '../../services/auth.service';
import { LoginResponse } from '../../models/user.model';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit {
  currentUser: LoginResponse | null = null;

  constructor(
    private authState: AuthStateService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.authState.currentUser$.subscribe(user => this.currentUser = user);
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
