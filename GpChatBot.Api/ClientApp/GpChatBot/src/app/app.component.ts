import {Component, OnDestroy, OnInit} from '@angular/core';
import {ChatService} from "./services/chat.service";
import {ChatApiModel, MessageApiModel, UpdateBotMessageStatusRequest} from "./models/api-models";
import {ChatSignalRService} from "./services/signalR.service";
import {MatSnackBar} from "@angular/material/snack-bar";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, OnDestroy {
  chats: ChatApiModel[] = [];
  messages: MessageApiModel[] = [];
  newMessage: string = '';
  isConnected: boolean = false;
  isLoading: boolean = false;

  constructor(private chatService: ChatService,
              private chatSignalRService: ChatSignalRService,
              private snackBar: MatSnackBar) {
  }

  ngOnInit(): void {
    this.fetchChats();
  }

  ngOnDestroy(): void {
    this.stopConnection();
  }

  fetchChats(): void {
    this.chatService.getChats().subscribe((data) => {
      this.chats = data;
    });
  }

  public selectChat(chat: ChatApiModel): void {
    this.messages = [];
    this.messages = chat.messages;
  }

  public startChat(): void {
    this.messages = [];
    this.chatSignalRService.startConnection().then(() => {
      this.isConnected = true;
      this.chatSignalRService.onMessageReceived((message) => {
        const existingMessage = this.messages
          .find(m => m.id === message.id);

        if (existingMessage) {
          existingMessage.message = message.message;
        } else {
          this.messages.push(message);
        }
      });

      this.chatSignalRService.onConnectionStateChange(() => {
      });
    });
  }

  sendMessage(): void {
    if (this.newMessage.trim() === '') {
      return;
    }
    this.isLoading = true;
    this.chatSignalRService.sendMessage(this.newMessage).then(() => {
      this.newMessage = '';
      this.isLoading = false;
    }).catch(() => {
      this.isLoading = false;
    });
  }

  public stopConnection(): void {
    this.chatSignalRService.removeMessageListener();
    this.chatSignalRService.stopConnection().then(() => {
      this.newMessage = '';
      this.isLoading = false;
      this.isConnected = false;
      this.fetchChats();
    });
  }

  public updateReaction(message: MessageApiModel, like: boolean): void {
    let request: UpdateBotMessageStatusRequest = {
      chatId: message.chatId,
      messageId: message.id,
      like: like
    }

    this.chatService.updateBotMessageStatus(request).subscribe({
      next: (updatedMessage: MessageApiModel) => {
        this.snackBar.open('Thank you for your feedback', 'Close', {
          duration: 2000,
          verticalPosition: 'top',
          horizontalPosition: 'right'
        });
        message.isLiked = updatedMessage.isLiked;
      },
      error: () => {
      }
    });
  }

  public truncateMessage(message: string | undefined, limit: number = 20): string {
    if (!message) {
      return '';
    }
    return message.length > limit ? message.substring(0, limit) + '...' : message;
  }
}
