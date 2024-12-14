import {Injectable} from '@angular/core';
import * as signalR from '@microsoft/signalr';
import {environment} from "../../environments/environment";
import {MessageApiModel} from "../models/api-models";

@Injectable({
  providedIn: 'root',
})
export class ChatSignalRService {
  private hubConnection: signalR.HubConnection;
  private readonly hubUrl = `${environment.apiUrl}/chatHub`;

  constructor() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.hubUrl)
      .withAutomaticReconnect()
      .build();
  }

  async startConnection(): Promise<void> {
    try {
      await this.hubConnection.start();
    } catch (error) {
      console.error('Error establishing SignalR connection:', error);
    }
  }

  async stopConnection(): Promise<void> {
    try {
      await this.hubConnection.stop();
    } catch (error) {
      console.error('Error stopping SignalR connection:', error);
    }
  }

  async sendMessage(message: string): Promise<void> {
    try {
      await this.hubConnection.invoke('SendMessage', message);
    } catch (error) {
    }
  }

  onMessageReceived(callback: (message: MessageApiModel) => void): void {
    this.hubConnection.on('ReceiveMessage', callback);
  }

  removeMessageListener(): void {
    this.hubConnection.off('ReceiveMessage');
  }

  onConnectionStateChange(callback: (state: signalR.HubConnectionState) => void): void {
    this.hubConnection.onreconnected(() => callback(signalR.HubConnectionState.Connected));
    this.hubConnection.onreconnecting(() => callback(signalR.HubConnectionState.Reconnecting));
    this.hubConnection.onclose(() => callback(signalR.HubConnectionState.Disconnected));
  }
}
