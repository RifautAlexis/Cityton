import { EventEmitter, Injectable, RootRenderer } from '@angular/core';

import { environment } from 'src/environments/environment';

import { AuthService } from '@core/services/auth.service';

import { HubConnection, HubConnectionBuilder } from '@Microsoft/signalr';
import { HttpClient, HttpParams } from '@angular/common/http';
import { IMessage as Message } from '@shared/models/message';
import { Subject, Observable, BehaviorSubject } from 'rxjs';
import { IThread as Thread } from '@shared/models/Thread';

import * as signalR from "@microsoft/signalr";

@Injectable({
  providedIn: 'root'
})
export class ChatService {

  messageReceived = new EventEmitter<Message>();
  connectionEstablished = new EventEmitter<Boolean>();

  private hubConnection: signalR.HubConnection;
  private connectionIsEstablished: boolean = false;

  constructor(
    private http: HttpClient,
    private authService: AuthService) {
    this.buildConnection();
    this.startConnection();
  }

  getMessages(discussionId: number): Observable<Message[]> {
    return this.http.get<Message[]>(environment.apiUrl + 'chat/getMessages/' + discussionId);
  }

  getThreads(): Observable<Thread[]> {
    return this.http.get<Thread[]>(environment.apiUrl + 'chat/getThreads');
  }

  getHubConnection() {
    return this.hubConnection;
  }

  buildConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .configureLogging(signalR.LogLevel.Debug)
      .configureLogging(signalR.LogLevel.Information)
      .withUrl('http://localhost:5000/hub/chatHub', { accessTokenFactory: () => this.authService.currentTokenValue() })
      .build();
  }

  startConnection() {
    this.hubConnection
      .start()
      .then(() => {
        this.connectionIsEstablished = true;
        console.log('Connection started !');
        this.connectionEstablished.emit(this.connectionIsEstablished);
        this.messageReceivedEvent();
      })
      .catch(err => console.log(err));
  }

  closeConnection() {
    this.hubConnection
      .stop()
      .then(() => {
        console.log('Connection closed !');
        this.connectionIsEstablished = false;
      })
      .catch(err => console.log(err));
  }

  messageReceivedEvent() {
    this.hubConnection.on("messageReceived", (newMessage: Message) => {
      console.log(newMessage);
      this.messageReceived.emit(newMessage);
    });
  }

  sendMessage(newMessage: string, discussionId: number) {
    console.log(newMessage, discussionId);
    this.hubConnection
      .send("newMessage", newMessage, discussionId);
  }

}