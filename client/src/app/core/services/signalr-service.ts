import { Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { Order } from '../../shared/models/order';

@Injectable({
  providedIn: 'root',
})
export class SignalrService {
  hubUrl = environment.hubUrl;
  hubConnection?: HubConnection;
  orderSignal = signal<Order | null>(null);

  createHubConnection() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl, {
        withCredentials: true, // This will make sure to send the request with a cookie for authentication
      })
      .withAutomaticReconnect() // HubConnection to automatically attempt to reconnect if the connection is lost. By default, the client will wait 0, 2, 10 and 30 seconds respectively before trying up to 4 reconnect attempts
      .build();

    this.hubConnection.start().catch((error) => console.log(error));

    // Using the on method in order to listen to events
    this.hubConnection.on('OrderCompleteNotification', (order: Order) => {
      this.orderSignal.set(order);
    });
  }

  stopHubConnection() {
    if (this.hubConnection?.state === HubConnectionState.Connected) {
      this.hubConnection.stop().catch((error) => console.log(error));
    }
  }
}
