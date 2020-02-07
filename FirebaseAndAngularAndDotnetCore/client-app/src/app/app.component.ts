import { Component } from '@angular/core';
import { AuthServiceService } from './services/auth-service.service';
import { Subscription } from 'rxjs';
import { CurrentUser } from './models/current-user';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  currentUser: CurrentUser = new CurrentUser();
  $authSubscription: Subscription;
  constructor(private authService: AuthServiceService, private router: Router) {
    this.$authSubscription = this.authService.user$.subscribe(u => {
      this.currentUser = u;
    });
  }

  logTheUserOut() {
    this.authService.logout().then(() => {      
    });
  }
}
