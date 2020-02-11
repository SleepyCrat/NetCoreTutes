import { Injectable } from '@angular/core';
import { SocketMessage } from './models/socket-message';
import { BehaviorSubject } from 'rxjs';
import { Square } from './models/square';
import { SquareChangeRequest } from './models/square-change-request';

@Injectable({
  providedIn: 'root'
})
export class WebSocketService {
  private socket: WebSocket;
  squares$: BehaviorSubject<Square[]> = new BehaviorSubject<Square[]>([]);
  announcement$: BehaviorSubject<string> = new BehaviorSubject<string>('');
  name$: BehaviorSubject<string> = new BehaviorSubject<string>('');
  private name: string;
  constructor() { }

  startSocket() {
    this.socket = new WebSocket('wss://localhost:5001/ws');
    this.socket.addEventListener("open", (ev => {
      console.log('opened')
    }));
    this.socket.addEventListener("message", (ev => {
      var messageBox: SocketMessage = JSON.parse(ev.data);
      console.log('message object', messageBox);
      switch (messageBox.MessageType) {
        case "name":
          this.name = messageBox.Payload;
          this.name$.next(this.name);
          break;
        case "announce":
          this.announcement$.next(messageBox.Payload);
          break;
        case "squares":
          this.squares$.next(messageBox.Payload);
          break;
        default:
          break;
      }
    }));
  }

  sendSquareChangeRequest(req: SquareChangeRequest) {
    req.Name = this.name;
    var requestAsJson = JSON.stringify(req);
    this.socket.send(requestAsJson);
  }
}
