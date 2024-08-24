import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-nav-menu',
  //standalone: true,
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})

export class NavMenuComponent {
  items!: MenuItem[];
  alertsCount: number = 6;
  isDarkMode!: boolean;

  constructor(private router: Router) {
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
}
