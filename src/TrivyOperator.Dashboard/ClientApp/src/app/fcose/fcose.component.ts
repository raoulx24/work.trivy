import { AfterViewInit, Component, ElementRef, ViewChild } from '@angular/core';

import cytoscape from 'cytoscape';
import fcose from 'cytoscape-fcose';
cytoscape.use(fcose);



@Component({
  selector: 'app-fcose',
  standalone: true,
  imports: [],
  templateUrl: './fcose.component.html',
  styleUrl: './fcose.component.scss'
})
export class FcoseComponent implements AfterViewInit {
  @ViewChild('graphContainer', { static: true }) graphContainer: ElementRef;

  constructor() { }

  ngAfterViewInit() {
    const cy = cytoscape({
      container: this.graphContainer.nativeElement,
      elements: [
        { data: { id: 'a' } },
        { data: { id: 'b' } },
        { data: { id: 'c' } },
        { data: { id: 'd' } },
        { data: { id: 'e' } },
        { data: { source: 'a', target: 'b' } },
        { data: { source: 'a', target: 'c' } },
        { data: { source: 'c', target: 'd' } },
        { data: { source: 'd', target: 'e' } }],
      layout: {
        name: 'fcose',
      },
      style: [
        {
          selector: 'node',
          style: { 'background-color': '#666', 'label': 'data(id)' }
        },
        {
          selector: 'edge',
          style: { 'width': 3, 'line-color': '#ccc', 'target-arrow-color': '#ccc', 'target-arrow-shape': 'triangle' }
        }]
    });
  }
}
