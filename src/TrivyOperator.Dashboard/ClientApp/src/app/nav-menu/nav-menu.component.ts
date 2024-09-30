import { Component } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { WatcherStateInfoService } from '../../api/services/watcher-state-info.service';
import { WatcherStateInfoDto } from '../../api/models/watcher-state-info-dto'
import { filter } from 'rxjs';

@Component({
  selector: 'app-nav-menu',
  //standalone: true,
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})

export class NavMenuComponent {
  items!: MenuItem[];
  alertsCount: number = 9;
  isDarkMode!: boolean;

  constructor(private router: Router, private watcherStateInfoService: WatcherStateInfoService) {
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
        icon: 'pi pi-palette',
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
    ];

    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      this.getWatcherStateErrors();
    });
    //this.getWatcherStateErrors();

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
    this.router.navigate(['/alerts']);
  }

  public onAboutClick() {
    this.router.navigate(['/about']);
  }

  private getDarkMode(): boolean {
    return window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
  }

  private getWatcherStateErrors() {
    this.watcherStateInfoService.getWatcherStateInfos()
      .subscribe({
        next: (res) => {
          this.alertsCount = res.filter(x => x.status === "Red").length;
        },
        error: (err) => console.error(err)
      });
  }

}
