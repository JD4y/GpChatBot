import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {environment} from 'src/environments/environment';
import {ChatApiModel, MessageApiModel, UpdateBotMessageStatusRequest} from "../models/api-models";

@Injectable({
  providedIn: 'root',
})
export class ChatService {
  private apiUrl = `${environment.apiUrl}/api/chat`;

  constructor(private http: HttpClient) {
  }

  getChats(): Observable<ChatApiModel[]> {
    return this.http.get<ChatApiModel[]>(`${this.apiUrl}`);
  }

  updateBotMessageStatus(
    request: UpdateBotMessageStatusRequest
  ): Observable<MessageApiModel> {
    return this.http.patch<MessageApiModel>(`${this.apiUrl}`, request);
  }
}
