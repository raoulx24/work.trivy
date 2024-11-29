import { AfterViewInit, Component, ElementRef, ViewChild } from '@angular/core';

import cytoscape, { BaseLayoutOptions, EdgeSingular, ElementDefinition, NodeSingular } from 'cytoscape';
import fcose from 'cytoscape-fcose';
cytoscape.use(fcose);

// tests.sbom
import { ClusterSbomReportService } from '../../api/services/cluster-sbom-report.service';
import { ClusterSbomReportDto } from '../../api/models/cluster-sbom-report-dto';
//

interface FcoseLayoutOptions extends BaseLayoutOptions {
  name: 'fcose';
  padding: number;
  fit: boolean;
  nodeSeparation: number;
  nodeRepulsion: number;
  idealEdgeLength: number;
  edgeElasticity: number;
  numIter: 2500;
  randomize: boolean;
  componentSpacing: number;
  nodeOverlap: number;
  animate: boolean;
  nodeDimensionsIncludeLabels: boolean;
  quality: "draft" | "default" | "proof";
}

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
        name: "fcose",
        nodeRepulsion: 4500,
        idealEdgeLength: 100,
        edgeElasticity: 0.45,
        numIter: 2500,
        animate: true,
        fit: true,
        padding: 10,
        componentSpacing: 1000,
        nodeOverlap: 2000,
        nodeDimensionsIncludeLabels: true,
        quality: "default"
      } as FcoseLayoutOptions,
      style: [
        {
          selector: 'node',
          style: {
            'width': 'mapData(label.length, 1, 20, 20, 200)',
            'height': '20px',
            'shape': 'roundrectangle',
            'background-color': 'Aqua',
            'label': 'data(label)',
            'text-valign': 'center',
            'text-halign': 'center',
            'text-wrap': 'ellipsis',
            'text-max-width': '200px',
            'font-size': '10px',
            'border-width': 1,
            'border-color': '#000',
            'transition-property': 'width height font-size',
            'transition-duration': 0.2,
          }

        },
        {
          selector: 'edge',
          style: { 'width': 1, 'line-color': '#ccc', 'target-arrow-color': '#ccc', 'target-arrow-shape': 'triangle' }
        },
        {
          selector: '.hovered',
          style: {
            //'width': '120px', // Increase node width by 1.2
            'height': '24px', // Increase node height by 1.2
            'font-size': '14.4px', // Increase font size by 1.2
            'background-color': 'Silver',
            //'border-color': 'Gray'
          }
        },
        {
          selector: '.hoveredOutgoers',
          style: {
            //'width': '120px', // Increase node width by 1.2
            'height': '24px', // Increase node height by 1.2
            'font-size': '14.4px', // Increase font size by 1.2
            'background-color': 'DeepSkyBlue',
            //'border-color': 'skyblue'
          }
        },
        {
          selector: '.hoveredIncomers',
          style: {
            //'width': '120px', // Increase node width by 1.2
            'height': '24px', // Increase node height by 1.2
            'font-size': '14.4px', // Increase font size by 1.2
            'background-color': 'RoyalBlue',
            //'border-color': 'skyblue'
          }
        },
        {
          selector: '.highlighted-edge',
          style: {
            'line-color': '#ffa500',
            'width': 2,
            'target-arrow-color': '#ffa500'
          }
        },
      ]
    });

    cy.on('mouseover', 'node', function (event) {
      const node: NodeSingular = event.target;
      node.addClass('hovered');

      node.outgoers('node').forEach((depNode: NodeSingular) => {
        depNode.addClass('hoveredOutgoers');
      });
      node.incomers('node').forEach((depNode: NodeSingular) => {
        depNode.addClass('hoveredIncomers');
      });

      node.connectedEdges().forEach((edge: EdgeSingular)  => {
        edge.addClass('highlighted-edge');
      });
    });

    cy.on('mouseout', 'node', function (event: any) {
      const node: NodeSingular = event.target;
      node.removeClass('hovered');

      node.outgoers('node').forEach((depNode: NodeSingular) => {
        depNode.removeClass('hoveredOutgoers');
      });
      node.incomers('node').forEach((depNode: NodeSingular) => {
        depNode.removeClass('hoveredIncomers');
      });

      node.connectedEdges().forEach((edge: EdgeSingular) => {
        edge.removeClass('highlighted-edge');
      });
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
