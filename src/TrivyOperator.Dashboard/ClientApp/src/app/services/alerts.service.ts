import { Injectable } from '@angular/core';
import { HttpTransportType, HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { BehaviorSubject, Observable } from 'rxjs';

import { AlertDto } from '../../api/models/alert-dto'
import { RetryPolicyUtils } from '../utils/retry-policy.utils'

@Injectable({
  providedIn: 'root'
})
export class AlertsService {
  private hubConnection!: HubConnection;
  private alertsSubject = new BehaviorSubject<AlertDto[]>([]);
  public alerts$: Observable<AlertDto[]> = this.alertsSubject.asObservable();

  private retryPolicy = new RetryPolicyUtils();

  constructor() {
    this.startConnection();
    this.addEventListeners();
  }

  private startConnection() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl('http://localhost:5032/alerts-hub', {
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets,
        // https://stackoverflow.com/questions/52086158/angular-signalr-error-failed-to-complete-negotiation-with-the-server
      })
      .configureLogging(LogLevel.Information)
      .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: () => this.retryPolicy.nextDelayMs()
      })
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('Connection started');
        this.retryPolicy.resetCounter();
      })
      .catch(err => {
        console.error('Connection error', err);
        this.retryConnection();
      });

    this.hubConnection.onreconnecting(error => {
      console.warn(`Connection lost due to ${error}. Reconnecting...`);
      this.alertsSubject.next([]);
    });

    this.hubConnection.onreconnected(connectionId => {
      console.log(`Connection reestablished. Connected with connectionId ${connectionId}.`);
      this.retryPolicy.resetCounter();
    });

    this.hubConnection.onclose(() => console.error('Connection closed.'));
  }

  private retryConnection() {
    setTimeout(() => {
      this.hubConnection.start().catch(err => {
        console.error('Retry connection error', err);
        this.retryConnection();
      });
    }, this.retryPolicy.nextDelayMs());
  }

  private addEventListeners() {
    this.hubConnection.on('ReceiveAddedAlert', (alert: AlertDto) => {
      this.addAlert(alert);
    });
    
    this.hubConnection.on('ReceiveRemovedAlert', (alert: AlertDto) => {
      this.removeAlert(alert);
    });
  }

  private addAlert(alert: AlertDto) {
    const currentAlerts = this.alertsSubject.value;
    this.alertsSubject.next([...currentAlerts, alert]);
  }

  private removeAlert(alert: AlertDto) {
    const currentAlerts = this.alertsSubject.value.filter(a => a.emiter != alert.emiter && a.emitterKey !== alert.emitterKey);
    this.alertsSubject.next(currentAlerts);
  }

  getAlerts(): AlertDto[] {
    return this.alertsSubject.value;
  }
}
