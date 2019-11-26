import { Component } from '@angular/core';

import { AuthService } from '@core/services/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'Cityton.Web';
  isConnected: boolean;

  constructor(private authService: AuthService) {
    this.isConnected = authService.currentUserValue() != null;
  }
}
