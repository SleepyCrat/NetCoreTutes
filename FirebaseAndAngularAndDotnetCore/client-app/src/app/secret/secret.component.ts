import { Component, OnInit } from '@angular/core';
import { AuthServiceService } from '../services/auth-service.service';

@Component({
  selector: 'app-secret',
  templateUrl: './secret.component.html',
  styleUrls: ['./secret.component.scss']
})
export class SecretComponent implements OnInit {

  secrets: string[] = [];
  constructor(private authService: AuthServiceService) { }

  ngOnInit() {
    this.authService.getUserSecrets().subscribe(secretData => { this.secrets = secretData });
  }

}
