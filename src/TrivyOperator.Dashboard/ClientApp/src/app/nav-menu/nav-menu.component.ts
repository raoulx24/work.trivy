import { Component, OnDestroy, OnInit } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { WatcherStateInfoService } from '../../api/services/watcher-state-info.service';
import { WatcherStateInfoDto } from '../../api/models/watcher-state-info-dto'
import { filter, Subscription } from 'rxjs';

import { } from '../services/alerts.service'
import { AlertsService } from '../services/alerts.service';
import { AlertDto } from '../../api/models/alert-dto'

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})

export class NavMenuComponent implements OnInit, OnDestroy {
  items!: MenuItem[];
  alertsCount: number = 0;
  isDarkMode!: boolean;

  //private alertSubscription!: Subscription;
  alerts: AlertDto[] = [];

  constructor(private router: Router, private alertsService: AlertsService) {
    this.isDarkMode = this.getDarkMode();
    this.items = [
      {
        label: 'Home',
        icon: 'pi pi-home',
        route: '/',
        command: () => { this.router.navigate(['/']); },
      },
      {
        label: 'Vulnerability Reports',
        icon: 'pi pi-flag',
        items: [
          {
            label: 'Browse',
            route: '/vulnerability-reports',
            command: () => { this.router.navigate(['/vulnerability-reports']); },
          },
          {
            label: 'Detailed',
            route: '/vulnerability-reports-detailed',
            command: () => { this.router.navigate(['/vulnerability-reports-detailed']); },
          }
        ],
      },
      {
        label: 'System',
        icon: 'pi pi-cog',
        items: [
          {
            label: 'Watcher States',
            route: '/watcher-states',
            command: () => { this.router.navigate(['/watcher-states']); },
          },
          {
            label: 'Settings',
            route: '/settings',
            command: () => { this.router.navigate(['/settings']); },
          }
        ]
      }
    ];

    //this.router.events.pipe(
    //  filter(event => event instanceof NavigationEnd)
    //).subscribe(() => {
    //  this.getWatcherStateErrors();
    //});
    //this.getWatcherStateErrors();
  }

  ngOnInit() {
    //this.alertSubscription = this.alertsService.alerts$.subscribe((alerts: AlertDto[]) => {
    //  this.onNewAlerts(alerts);
    //});
  }

  ngOnDestroy() {
    //this.alertSubscription.unsubscribe();
  }

  public switchLightDarkMode() {
    this.isDarkMode = !this.isDarkMode;
    const primengThemeLink = document.getElementById("primeng-theme") as HTMLLinkElement | null;
    if (primengThemeLink == null) {
      return;
    }
    primengThemeLink.href = this.isDarkMode ? "primeng-dark.css" : "primeng-light.css";
  }

  public onAlertsClick() {
    this.router.navigate(['/watcher-states']);
  }

  public onAboutClick() {
    this.router.navigate(['/about']);
  }

  private getDarkMode(): boolean {
    return window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
  }

  //private getWatcherStateErrors() {
  //  this.watcherStateInfoService.getWatcherStateInfos()
  //    .subscribe({
  //      next: (res: { filter: (arg0: (x: any) => boolean) => { (): any; new(): any; length: number; }; }) => {
  //        this.alertsCount = res.filter((x: { status: string; }) => x.status === "Red").length;
  //      },
  //      error: (err: any) => console.error(err)
  //    });
  //}

  private onNewAlerts(alerts: AlertDto[]) {
    this.alerts = alerts;

    this.alertsCount = alerts ? alerts.length : 0;
  }
}
