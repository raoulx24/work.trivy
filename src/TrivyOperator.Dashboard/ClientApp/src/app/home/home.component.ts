import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

import { HomeVulnerabilityReportsComponent } from '../home-vulnerability-reports/home-vulnerability-reports.component'

import { PanelModule } from 'primeng/panel';
import { CarouselModule } from 'primeng/carousel';


@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, HomeVulnerabilityReportsComponent, CarouselModule, PanelModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})

export class HomeComponent {
  public slides: string[] = ["vr", "lorem"];
  constructor() {
  }
}
