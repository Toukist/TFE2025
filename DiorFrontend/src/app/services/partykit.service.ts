import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class PartykitService {
  private socket: WebSocket;
  private messagesSubject = new Subject<string>();
  public messages$: Observable<string> = this.messagesSubject.asObservable();

  constructor() {
    // Remplace l'URL par celle de ta room PartyKit si besoin
    this.socket = new WebSocket('wss://localhost:1999/rooms/demo');

    this.socket.addEventListener('message', (event: MessageEvent) => {
      this.messagesSubject.next(event.data);
    });
  }

  sendMessage(message: string) {
    this.socket.send(message);
  }
}
