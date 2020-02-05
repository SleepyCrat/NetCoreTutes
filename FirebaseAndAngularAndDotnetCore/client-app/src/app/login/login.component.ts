import { Component, OnInit } from '@angular/core';
import { AuthServiceService } from '../services/auth-service.service';
import { Router } from '@angular/router';
import { CurrentUser } from '../models/current-user';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  currentUser: CurrentUser = new CurrentUser();
  $authSubscription: Subscription;
  constructor(private authService: AuthServiceService, private router: Router) {
    this.$authSubscription = this.authService.user$.subscribe(u => {
      this.currentUser = u;
    });
  }

  ngOnInit() {
  }

  loginWithGoogle() {
    this.authService.doGoogleSignIn().then(() => {
      this.router.navigate(['']);
    });
  }
}
