import { AfterViewInit, Component, OnInit } from '@angular/core';
declare var mermaid: any;

@Component({
  selector: 'app-mermaid-tests',
  standalone: true,
  imports: [],
  templateUrl: './mermaid-tests.component.html',
  styleUrl: './mermaid-tests.component.scss'
})
export class MermaidTestsComponent implements OnInit, AfterViewInit {
  constructor() {
  }

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    const mermaidConfig = {
      startOnLoad: true
    };
    mermaid.initialize(mermaidConfig);
    mermaid.contentLoaded();
  }
}
