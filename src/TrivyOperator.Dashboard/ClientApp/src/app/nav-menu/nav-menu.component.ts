import { Component, OnDestroy, OnInit } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { WatcherStateInfoService } from '../../api/services/watcher-state-info.service';
import { WatcherStateInfoDto } from '../../api/models/watcher-state-info-dto'
import { Subscription } from 'rxjs';

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

  private alertSubscription!: Subscription;
  alerts: AlertDto[] = [];

  constructor(private router: Router, private alertsService: AlertsService) {
    this.isDarkMode = this.getDarkMode();
    this.items = [
      {
        label: 'Home',
        icon: 'pi pi-home',
        command: () => { this.router.navigate(['/']); },
      },
      {
        label: 'Vulnerabilities',
        icon: 'pi pi-flag',
        items: [
          {
            label: 'Browse',
            command: () => { this.router.navigate(['/vulnerability-reports']); },
          },
          {
            label: 'Detailed',
            command: () => { this.router.navigate(['/vulnerability-reports-detailed']); },
          }
        ],
      },
      {
        label: 'Config Audits',
        icon: 'pi pi-flag',
        items: [
          {
            label: 'Browse',
            command: () => { this.router.navigate(['/config-audit-reports']); },
          },
          {
            label: 'Detailed',
            command: () => { this.router.navigate(['/config-audit-reports-detailed']); },
          }
        ],
      },
      {
        label: 'Cluster RBAC Assessments',
        icon: 'pi pi-flag',
        items: [
          {
            label: 'Browse',
            command: () => { this.router.navigate(['/cluster-rbac-assessment-reports']); },
          },
          {
            label: 'Detailed',
            command: () => { this.router.navigate(['/cluster-rbac-assessment-reports-detailed']); },
          }
        ],
      },
      {
        label: 'Exposed Secrets',
        icon: 'pi pi-flag',
        items: [
          {
            label: 'Browse',
            command: () => { this.router.navigate(['/exposed-secret-reports']); },
          },
          {
            label: 'Detailed',
            command: () => { this.router.navigate(['/exposed-secret-reports-detailed']); },
          }
        ],
      },
      {
        label: 'System',
        icon: 'pi pi-cog',
        items: [
          {
            label: 'Watcher States',
            command: () => { this.router.navigate(['/watcher-states']); },
          },
          {
            label: 'Settings',
            command: () => { this.router.navigate(['/settings']); },
          }
        ]
      }
    ];
  }

  ngOnInit() {
    this.alertSubscription = this.alertsService.alerts$.subscribe((alerts: AlertDto[]) => {
      this.onNewAlerts(alerts);
    });
  }

  ngOnDestroy() {
    this.alertSubscription.unsubscribe();
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

  private onNewAlerts(alerts: AlertDto[]) {
    this.alerts = alerts;

    this.alertsCount = alerts ? alerts.length : 0;
  }
}
