import { Component } from '@angular/core';
import { WebSocketService } from './web-socket.service';
import { Square } from './models/square';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  announcementSub;
  messages: string[] = [];
  squares: Square[] = [];
  constructor(private socketService: WebSocketService) {
    this.socketService.announcement$.subscribe(announcement => {
      if (announcement) {
        this.messages.unshift(announcement);
      }
    });
    this.socketService.squares$.subscribe(sq => {
      this.squares = sq;
    });

  }

  ngOnInit() {
    this.socketService.startSocket();

  }
}
