import { AfterViewInit, Component, ElementRef, ViewChild } from '@angular/core';

import cytoscape, { BaseLayoutOptions, EdgeSingular, ElementDefinition, NodeSingular } from 'cytoscape';
import fcose from 'cytoscape-fcose';
cytoscape.use(fcose);

import { ButtonModule } from 'primeng/button';

// tests.sbom
import { ClusterSbomReportService } from '../../api/services/cluster-sbom-report.service';
import { ClusterSbomReportDto } from '../../api/models/cluster-sbom-report-dto';
//

interface FcoseLayoutOptions extends BaseLayoutOptions {
  name: 'fcose';
  quality: "draft" | "default" | "proof" | undefined;
  randomize: boolean | undefined;
  animate: boolean;
  animationDuration: number;
  animationEasing: undefined,
  fit: boolean;
  padding: number,
  nodeDimensionsIncludeLabels: boolean;
  uniformNodeDimensions: boolean;
  packComponents: boolean;
  step: "all" | "transformed" | "enforced" | "cose";
  /* spectral layout options */
  samplingType: boolean;
  sampleSize: number;
  nodeSeparation: number;
  piTol: number;
  /* incremental layout options */
  nodeRepulsion: (node: NodeSingular) => number;
  idealEdgeLength: (edge: EdgeSingular) => number;
  edgeElasticity: (edge: EdgeSingular) => number;
  nestingFactor: number;
  numIter: number;
  tile: boolean;
  tilingCompareBy: undefined;
  tilingPaddingVertical: number;
  tilingPaddingHorizontal: number;
  gravity: number;
  gravityRangeCompound: number;
  gravityCompound: number;
  gravityRange: number;
  initialEnergyOnIncremental: number;
  /* constraint options */
  // Fix desired nodes to predefined positions
  // [{nodeId: 'n1', position: {x: 100, y: 200}}, {...}]
  fixedNodeConstraint: undefined;
  // Align desired nodes in vertical/horizontal direction
  // {vertical: [['n1', 'n2'], [...]], horizontal: [['n2', 'n4'], [...]]}
  alignmentConstraint: undefined;
  // Place two nodes relatively in vertical/horizontal direction
  // [{top: 'n1', bottom: 'n2', gap: 100}, {left: 'n3', right: 'n4', gap: 75}, {...}]
  relativePlacementConstraint: undefined;

}

@Component({
  selector: 'app-fcose',
  standalone: true,
  imports: [ButtonModule],
  templateUrl: './fcose.component.html',
  styleUrl: './fcose.component.scss'
})
export class FcoseComponent {
  @ViewChild('graphContainer', { static: true }) graphContainer: ElementRef;
  private cy: cytoscape.Core;
  private hoveredNode: NodeSingular | null = null;

  private fcoseLayoutOptions = {
    name: "fcose",
    nodeRepulsion: (node: NodeSingular) => { return 20000 },
    numIter: 2500,
    animate: true,
    fit: true,
    padding: 10,
    sampleSize: 50,
    nodeSeparation: 4000,
    tilingPaddingHorizontal: 1000,
    tilingPaddingVertical: 1000,
    idealEdgeLength: (edge: EdgeSingular) => { return 150; },
    edgeElasticity: (edge: EdgeSingular) => { return .15; }
  };


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

    this.cy = cytoscape({
      container: this.graphContainer.nativeElement,
      elements: elements,
      layout: this.fcoseLayoutOptions as FcoseLayoutOptions,
      style: [
        {
          selector: 'node',
          style: {
            'width': 'mapData(label.length, 1, 30, 20, 200)',
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
            'transition-property': 'width height background-color font-size border-color',
            'transition-duration': 300,
          }
        },
        {
          selector: 'edge',
          style: {
            'width': 1,
            'line-color': '#ccc',
            'target-arrow-color': '#ccc',
            'target-arrow-shape': 'triangle',
            'transition-property': 'width line-color',
            'transition-duration': 300,
          }
        },
        {
          selector: '.hovered',
          style: {
            'width': 'mapData(label.length, 1, 30, 20, 240)',
            'height': '24px',
            'font-size': '12px',
            'background-color': 'Silver',
            'transition-property': 'width height background-color font-size border-color',
            'transition-duration': 300,
          }
        },
        {
          selector: '.hoveredOutgoers',
          style: {
            'width': 'mapData(label.length, 1, 30, 20, 240)',
            'height': '24px',
            'font-size': '12px',
            'background-color': 'DeepSkyBlue',
            'transition-property': 'width height background-color font-size border-color',
            'transition-duration': 300,
          }
        },
        {
          selector: '.hoveredIncomers',
          style: {
            'width': 'mapData(label.length, 1, 30, 20, 240)',
            'height': '24px',
            'font-size': '12px',
            'background-color': 'RoyalBlue',
            'transition-property': 'width height background-color font-size border-color',
            'transition-duration': 300,
          }
        },
        {
          selector: '.highlighted-edge',
          style: {
            'width': 3,
            "line-color": "Violet",
            'transition-property': 'line-color',
            'transition-duration': 300,
          }
        },
      ]
    });

    this.setupCyEvents();
  }

  setupCyEvents() {
    this.cy.on('mouseover', 'node', (event) => {
      if (this.hoveredNode) {
        return;
      }
      this.hoveredNode = event.target;
      this.hoveredNode.addClass('hovered');

      this.hoveredNode.outgoers('node').forEach((depNode: NodeSingular) => {
        depNode.addClass('hoveredOutgoers');
      });
      this.hoveredNode.incomers('node').forEach((depNode: NodeSingular) => {
        depNode.addClass('hoveredIncomers');
      });

      this.hoveredNode.connectedEdges().forEach((edge: EdgeSingular) => {
        edge.addClass('highlighted-edge');
      });
    });

    this.cy.on('mouseout', 'node', (event) => {
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
      this.hoveredNode = null;
    });
  }

  onZoomIn(_event: MouseEvent) {
    this.cy.animate({
      zoom: this.cy.zoom() + 0.1,
      duration: 300,
    });
  }

  onZoomOut(_event: MouseEvent) {
    this.cy.animate({
      zoom: this.cy.zoom() - 0.1,
      duration: 300
    });
  }

  onZoomFit(_event?: MouseEvent) {
    this.cy.animate({
      fit: {
        eles: this.cy.elements(),
        padding: 10,
      },
      duration: 300
    });
  }

  onDiveIn(_event: MouseEvent) {
    // Select the node to keep (e.g., node with id 'root')
    const rootNode = this.cy.$('#78f660ea-c2f6-49e8-b116-c93884ad68bf');
    //this.cy.elements().not(rootNode).animate({ style: { opacity: 0 }, duration: 300 });
    const newElements = this.getElementsByNodeId("78f660ea-c2f6-49e8-b116-c93884ad68bf");
    this.cy.remove(this.cy.elements().filter(x => newElements.includes(x.id)))
    
    this.cy.add(newElements);
    //this.cy.elements().animate({ style: { opacity: 1 }, duration: 300 });
    this.cy.layout(this.fcoseLayoutOptions as FcoseLayoutOptions).run();
    //this.cy.fit();
  }

  private getElementsByNodeId(nodeId: string): ElementDefinition[] {
    const nodeIds: string[] = [nodeId];
    this.getNodeIds(nodeId, nodeIds);

    const elements: ElementDefinition[] = [];
    nodeIds.forEach(id => {
      const sbomDetail = this.dataDtos[0].details?.find(x => x.bomRef == id);
      if (sbomDetail) {
        elements.push({ data: { id: id, label: sbomDetail.name ?? "" } });
        sbomDetail.dependsOn?.forEach(depends => {
            elements.push({
              data: {
                source: sbomDetail.bomRef,
                target: depends
              }
            });
          });
      }

    });

    return elements;
  }

  private getNodeIds(nodeId: string, nodeIds: string[]) {
    const detail = this.dataDtos[0].details?.find(x => x.bomRef == nodeId);
    if (!detail) {
      return;
    }
    const detailRefIds = detail.dependsOn;
    if (!detailRefIds) {
      return;
    }

    const newIds: string[] = detailRefIds.filter(id => !nodeIds.includes(id));
    nodeIds.push(...newIds);
    newIds.forEach(id => this.getNodeIds(id, nodeIds))
  }

  // tests sbom
  private dataDtos: ClusterSbomReportDto[] = [];
  //
}
