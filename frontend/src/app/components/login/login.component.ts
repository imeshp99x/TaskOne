import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  form: FormGroup;
  loading = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(2)]],
      password: ['', [Validators.required, Validators.minLength(4)]]
    });
  }

  get f() { return this.form.controls; }

  onSubmit(): void {
    if (this.form.invalid) { return; }
    this.loading = true;
    this.errorMessage = '';

    this.authService.login({
      username: this.f.username.value.trim(),
      password: this.f.password.value
    }).subscribe(
      () => {
        this.loading = false;
        this.router.navigate(['/tasks']);
      },
      err => {
        this.loading = false;
        this.errorMessage = err.error?.message || 'Login failed. Please check your credentials.';
      }
    );
  }
}
