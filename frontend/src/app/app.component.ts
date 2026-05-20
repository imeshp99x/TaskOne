import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthStateService } from './services/auth-state.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  constructor(public authState: AuthStateService, public router: Router) {}
}
