import { Component, OnInit } from '@angular/core';
import { CurrentUser } from '../models/current-user';
import { Subscription } from 'rxjs';
import { AuthServiceService } from '../services/auth-service.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  currentUser: CurrentUser = new CurrentUser();
  $authSubscription: Subscription;

  constructor(private authService: AuthServiceService, private router: Router) {
    this.$authSubscription = this.authService.user$.subscribe(u => {
      this.currentUser = u;
    });
  }

  ngOnInit() {
  }

}
