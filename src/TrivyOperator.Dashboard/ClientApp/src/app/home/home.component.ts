import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

import { HomeVulnerabilityReportsComponent } from '../home-vulnerability-reports/home-vulnerability-reports.component'
import { HomeConfigAuditReportsComponent } from '../home-config-audit-reports/home-config-audit-reports.component'

import { PanelModule } from 'primeng/panel';
import { CarouselModule } from 'primeng/carousel';
import { TabViewModule } from 'primeng/tabview';


@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, HomeVulnerabilityReportsComponent, HomeConfigAuditReportsComponent, CarouselModule, PanelModule, TabViewModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})

export class HomeComponent {
  
  constructor() {
  }
}
