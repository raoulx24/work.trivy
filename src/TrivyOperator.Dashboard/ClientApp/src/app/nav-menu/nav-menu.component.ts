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
  isExpanded = false;
  items!: MenuItem[];
  alertsCount: number = 6;

  constructor(private router: Router) {
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
      {
        label: 'Alerts',
        icon: 'pi pi-home',
        route: '/alerts',
        command: () => { this.router.navigate(['/alerts']); },
        badge: this.alertsCount.toString(),
        badgeStyleClass: 'p-badge-danger',
      }
    ];
  }


  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
}
