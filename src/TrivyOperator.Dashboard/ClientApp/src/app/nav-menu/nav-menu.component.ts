import { Component, OnDestroy, OnInit } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { WatcherStateInfoService } from '../../api/services/watcher-state-info.service';
import { WatcherStateInfoDto } from '../../api/models/watcher-state-info-dto'
import { Subscription } from 'rxjs';

import { AlertsService } from '../services/alerts.service';
import { AlertDto } from '../../api/models/alert-dto'
import { MainAppInitService } from '../services/main-app-init.service';

import { faHouse, faShieldHalved, faClipboardList, faUserShield, faKey, faGears, IconDefinition } from '@fortawesome/free-solid-svg-icons'

interface TrivyMenuItem extends MenuItem {
  faIcon: IconDefinition | null | undefined;
}

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})

export class NavMenuComponent implements OnInit, OnDestroy {
  items: TrivyMenuItem[] = [];
  alertsCount: number = 0;
  isDarkMode!: boolean;

  private alertSubscription!: Subscription;
  alerts: AlertDto[] = [];
  enabledTrivyReports: string[] = ["crar", "car", "esr", "vr"];

  sidebarVisible = false;
  
  faHouse = faHouse;
  faShieldHalved = faShieldHalved;
  faClipboardList = faClipboardList;
  faUserShield = faUserShield;
  faKey = faKey;
  faGears = faGears;
  
  constructor(private router: Router, private alertsService: AlertsService, private mainAppInitService: MainAppInitService) {
    
  }

  ngOnInit() {
    this.isDarkMode = this.getDarkMode();
    this.alertSubscription = this.alertsService.alerts$.subscribe((alerts: AlertDto[]) => {
      this.onNewAlerts(alerts);
    });
    this.mainAppInitService.backendSettingsDto$.subscribe(updatedBackendSettingsDto => {
      this.enabledTrivyReports = updatedBackendSettingsDto.trivyReportConfigDtos?.filter(x => x.enabled).map(x => x.id ?? "") ?? this.enabledTrivyReports;
      this.setMenu();
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

  private setMenu() {
    this.items = [
      {
        label: 'Home',
        faIcon: faHouse,
        command: () => { this.router.navigate(['/']); this.sidebarVisible = false; },
      },
      {
        label: 'Vulnerabilities',
        faIcon: this.faShieldHalved,
        disabled: !this.enabledTrivyReports.includes("vr"),
        expanded: true,
        items: [
          {
            label: 'Browse',
            disabled: !this.enabledTrivyReports.includes("vr"),
            command: () => { this.router.navigate(['/vulnerability-reports']); this.sidebarVisible = false; },
          },
          {
            label: 'Detailed',
            disabled: !this.enabledTrivyReports.includes("vr"),
            command: () => { this.router.navigate(['/vulnerability-reports-detailed']); this.sidebarVisible = false; },
          }
        ],
      },
      {
        label: 'Config Audits',
        faIcon: faClipboardList,
        disabled: !this.enabledTrivyReports.includes("car"),
        expanded: true,
        items: [
          {
            label: 'Browse',
            disabled: !this.enabledTrivyReports.includes("car"),
            command: () => { this.router.navigate(['/config-audit-reports']); this.sidebarVisible = false; },
          },
          {
            label: 'Detailed',
            disabled: !this.enabledTrivyReports.includes("car"),
            command: () => { this.router.navigate(['/config-audit-reports-detailed']); this.sidebarVisible = false; },
          }
        ],
      },
      {
        label: 'Cluster RBAC Assessments',
        faIcon: faUserShield,
        disabled: !this.enabledTrivyReports.includes("crar"),
        expanded: true,
        items: [
          {
            label: 'Browse',
            disabled: !this.enabledTrivyReports.includes("crar"),
            command: () => { this.router.navigate(['/cluster-rbac-assessment-reports']); this.sidebarVisible = false; },
          },
          {
            label: 'Detailed',
            disabled: !this.enabledTrivyReports.includes("crar"),
            command: () => { this.router.navigate(['/cluster-rbac-assessment-reports-detailed']); this.sidebarVisible = false; },
          }
        ],
      },
      {
        label: 'Exposed Secrets',
        faIcon: faKey,
        disabled: !this.enabledTrivyReports.includes("esr"),
        expanded: true,
        items: [
          {
            label: 'Browse',
            disabled: !this.enabledTrivyReports.includes("esr"),
            command: () => { this.router.navigate(['/exposed-secret-reports']); this.sidebarVisible = false; },
          },
          {
            label: 'Detailed',
            disabled: !this.enabledTrivyReports.includes("esr"),
            command: () => { this.router.navigate(['/exposed-secret-reports-detailed']); this.sidebarVisible = false; },
          }
        ],
      },
      {
        label: 'System',
        faIcon: faGears,
        expanded: true,
        items: [
          {
            label: 'Watcher States',
            command: () => { this.router.navigate(['/watcher-states']); this.sidebarVisible = false; },
          },
          {
            label: 'Settings',
            command: () => { this.router.navigate(['/settings']); this.sidebarVisible = false; },
          }
        ]
      }
    ];

  }

  openSidebar() {
    this.sidebarVisible = true;
  }
}
