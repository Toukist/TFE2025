import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PartykitService } from '../../services/partykit.service';

@Component({
    selector: 'app-chat-room',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './chat-room.component.html',
    styleUrls: ['./chat-room.component.css']
})
export class ChatRoomComponent {
    message = '';
    messages: string[] = [];

    constructor(private partykit: PartykitService) {
        this.partykit.messages$.subscribe(msg => {
            this.messages.push(msg);
        });
    }

    send() {
        if (this.message.trim()) {
            this.partykit.sendMessage(this.message);
            this.message = '';
        }
    }
}
