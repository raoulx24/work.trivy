import { AfterViewInit, Component, ElementRef, ViewChild } from '@angular/core';

import cytoscape, { ElementDefinition } from 'cytoscape';
import fcose from 'cytoscape-fcose';
cytoscape.use(fcose);

// tests.sbom
import { ClusterSbomReportService } from '../../api/services/cluster-sbom-report.service';
import { ClusterSbomReportDto } from '../../api/models/cluster-sbom-report-dto';
//

@Component({
  selector: 'app-fcose',
  standalone: true,
  imports: [],
  templateUrl: './fcose.component.html',
  styleUrl: './fcose.component.scss'
})
export class FcoseComponent {
  @ViewChild('graphContainer', { static: true }) graphContainer: ElementRef;

  constructor(private service: ClusterSbomReportService) {
    // tests.sbom
    this.getTableDataDtos();
    //
  }


  // tests.sbom
  getTableDataDtos() {
    this.service.getClusterSbomReportDtos().subscribe({
      next: (res) => this.onGetDataDtos(res),
      error: (err) => console.error(err),
    });
  }

  onGetDataDtos(dtos: ClusterSbomReportDto[]) {
    this.dataDtos = dtos;

    const elements: ElementDefinition[] = [];

    this.dataDtos[0].details?.forEach(detail => {
      elements.push({ data: { id: detail.bomRef, label: detail.name } });
      detail.dependsOn?.forEach(depends => {
        elements.push({
          data: {
            source: detail.bomRef,
            target: depends
          }
        });
      });
    });

    const cy = cytoscape({
      container: this.graphContainer.nativeElement,
      elements: elements,
      layout: {
        name: 'fcose',
      },
      style: [
        {
          selector: 'node',
          style: { 'background-color': '#44A', 'label': 'data(label)' }
        },
        {
          selector: 'edge',
          style: { 'width': 3, 'line-color': '#ccc', 'target-arrow-color': '#ccc', 'target-arrow-shape': 'triangle' }
        }]
    });

    //cy.on('position', 'node', (event) => {
    //  console.log("mama");

    //  const sourceNode = event.target;
    //  console.log(sourceNode);

    //  const dependentNodes = sourceNode.connectedNodes();

    //  dependentNodes.forEach((node: any) => {
    //    console.log("tata");
    //    node.position({
    //      x: sourceNode.position().x + 10,
    //      y: sourceNode.position().y + 10 
    //    });
    //  });
    //});

  }

  private dataDtos: ClusterSbomReportDto[] = [];
  //
}
