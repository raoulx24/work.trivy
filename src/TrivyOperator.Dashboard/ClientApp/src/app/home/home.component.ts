import { Component } from '@angular/core';

import { PanelModule } from 'primeng/panel';

import { HomeVulnerabilityReportsComponent } from '../home-vulnerability-reports/home-vulnerability-reports.component'


@Component({
  selector: 'app-home',
  standalone: true,
  imports: [HomeVulnerabilityReportsComponent, PanelModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})

export class HomeComponent {
}
