import { Component } from '@angular/core';

import { PanelModule } from 'primeng/panel';

@Component({
  selector: 'app-about',
  standalone: true,
  imports: [PanelModule],
  templateUrl: './about.component.html',
  styleUrl: './about.component.scss'
})
export class AboutComponent {

}
