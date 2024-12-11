import { Component, ElementRef, ViewChild } from '@angular/core';

import cytoscape, { BaseLayoutOptions, EdgeSingular, ElementDefinition, NodeSingular } from 'cytoscape';
import fcose from 'cytoscape-fcose';
import { ButtonModule } from 'primeng/button';

// tests.sbom
import { ClusterSbomReportDto } from '../../api/models/cluster-sbom-report-dto';
import { ClusterSbomReportService } from '../../api/services/cluster-sbom-report.service';

cytoscape.use(fcose);

//

interface FcoseLayoutOptions extends BaseLayoutOptions {
  name: 'fcose';
  quality: 'draft' | 'default' | 'proof' | undefined;
  randomize: boolean | undefined;
  animate: boolean;
  animationDuration: number;
  animationEasing: undefined;
  fit: boolean;
  padding: number;
  nodeDimensionsIncludeLabels: boolean;
  uniformNodeDimensions: boolean;
  packComponents: boolean;
  step: 'all' | 'transformed' | 'enforced' | 'cose';
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
  styleUrl: './fcose.component.scss',
})
export class FcoseComponent {
  @ViewChild('graphContainer', { static: true }) graphContainer: ElementRef;
  private cy: cytoscape.Core;

  private get hoveredNode(): NodeSingular | null {
    return this._hoveredNode;
  }

  private set hoveredNode(node: NodeSingular | null) {
    this._hoveredNode = node;
    if (node) {
      const x = this.dataDtos[0].details?.find((x) => x.bomRef == node.id());
      if (x) {
        this.testText = `BomRef: ${x.bomRef} - Name: ${x.name} - Version: ${x.version} - purl: ${x.purl} - dependencies: ${x.dependsOn.length}`;
      }
    } else {
      this.testText = 'no info...';
    }
  }

  private _hoveredNode: NodeSingular | null = null;

  testText: string = '';

  private fcoseLayoutOptions = {
    name: 'fcose',
    nodeRepulsion: (node: NodeSingular) => {
      return 20000;
    },
    numIter: 2500,
    animate: true,
    fit: true,
    padding: 10,
    sampleSize: 50,
    nodeSeparation: 4000,
    tilingPaddingHorizontal: 1000,
    tilingPaddingVertical: 1000,
    idealEdgeLength: (edge: EdgeSingular) => {
      return 150;
    },
    edgeElasticity: (edge: EdgeSingular) => {
      return 0.15;
    },
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

    //const groupMap = new Map();
    //this.dataDtos[0].details?.filter(x => x.purl?.startsWith("pkg:nuget/"))
    //  .forEach(detail => {
    //    const potentialNs = detail.name.split('.')[0];
    //    if (!groupMap.has(potentialNs)) {
    //      groupMap.set(potentialNs, { data: { id: potentialNs, label: potentialNs }, classes: 'nodeCommon' });
    //    };
    //  });

    //const elements: ElementDefinition[] = [...groupMap.values()];

    //this.dataDtos[0].details?.forEach(detail => {
    //  elements.push({
    //    data: {
    //      id: detail.bomRef,
    //      label: detail.name,
    //      parent: (detail.purl?.startsWith("pkg:nuget/") && !detail.name.includes("Runtime.linux-x64"))
    //        ? detail.name.split('.')[0]
    //        : undefined
    //    },
    //    classes: `nodeCommon nodePackage ${detail.dependsOn?.length ? 'nodeBranch' : 'nodeLeaf'}`
    //  });
    //  detail.dependsOn?.forEach(depends => {
    //    elements.push({
    //      data: {
    //        source: detail.bomRef,
    //        target: depends
    //      },
    //      classes: 'edgeCommon'
    //    });
    //  });
    //});

    const elements: ElementDefinition[] = this.getElementsByNodeId('00000000-0000-0000-0000-000000000000');

    this.cy = cytoscape({
      container: this.graphContainer.nativeElement,
      elements: elements,
      layout: this.fcoseLayoutOptions as FcoseLayoutOptions,
      style: [
        {
          selector: '$node > .nodeCommon', // Compound (parent) node selector
          style: {
            'background-color': 'gray',
            'background-opacity': 0.2,
            //'label': 'data(label)',
            'text-valign': 'top',
            'text-halign': 'center',
            'text-background-color': 'aqua',
            'font-size': '14px',
            'font-weight': 'bold',
          },
        },
        {
          selector: '.nodeCommon',
          style: {
            'border-width': 1,
          },
        },
        {
          selector: '.nodePackage',
          style: {
            label: 'data(label)',
            width: 'mapData(label.length, 1, 30, 20, 200)',
            height: '20px',
            'background-color': 'Aqua',
            'text-valign': 'center',
            'text-halign': 'center',
            'text-wrap': 'ellipsis',
            'text-max-width': '200px',
            'font-size': '10px',
            'border-color': '#000',
            'transition-property': 'width height background-color font-size border-color',
            'transition-duration': 300,
          },
        },
        {
          selector: '.nodeBranch',
          style: {
            shape: 'roundrectangle',
          },
        },
        {
          selector: '.nodeLeaf',
          style: {
            shape: 'cut-rectangle',
          },
        },
        {
          selector: '.edgeCommon',
          style: {
            width: 1,
            'line-color': '#ccc',
            'target-arrow-color': '#ccc',
            'target-arrow-shape': 'triangle',
            'transition-property': 'width line-color',
            'transition-duration': 300,
          },
        },
        {
          selector: '.hoveredCommon',
          style: {
            width: 'mapData(label.length, 1, 30, 20, 240)',
            height: '24px',
            'font-size': '12px',
            'transition-property': 'width height background-color font-size border-color',
            'transition-duration': 300,
          },
        },
        {
          selector: '.hovered',
          style: {
            'background-color': 'Silver',
          },
        },
        {
          selector: '.hoveredOutgoers',
          style: {
            'background-color': 'DeepSkyBlue',
          },
        },
        {
          selector: '.hoveredIncomers',
          style: {
            'background-color': 'RoyalBlue',
          },
        },
        {
          selector: '.hoveredHighlight',
          style: {
            'overlay-opacity': 0.5,
            'overlay-color': 'RoyalBlue',
            'font-style': 'italic',
          },
        },
        {
          selector: '.highlighted-edge',
          style: {
            width: 3,
            'line-color': 'Violet',
            'transition-property': 'line-color',
            'transition-duration': 300,
          },
        },
      ],
    });

    this.setupCyEvents();
  }

  setupCyEvents() {
    this.cy.on('mouseover', 'node', (event) => {
      if (this.hoveredNode || event.target.isParent()) {
        return;
      }
      this.hoveredNode = event.target;
      this.hoveredNode.addClass('hoveredCommon hovered');
      this.hoveredNode.incomers('node').forEach((depNode: NodeSingular) => {
        depNode.addClass('hoveredCommon ');
        if (this.hoveredNode.outgoers('node').has(depNode)) {
          depNode.addClass('hoveredHighlight');
        } else {
          depNode.addClass('hoveredIncomers');
        }
      });
      this.hoveredNode.outgoers('node').forEach((depNode: NodeSingular) => {
        depNode.addClass('hoveredCommon hoveredOutgoers');
      });

      this.hoveredNode.connectedEdges().forEach((edge: EdgeSingular) => {
        edge.addClass('highlighted-edge');
      });
    });

    this.cy.on('mouseout', 'node', (event) => {
      const node: NodeSingular = event.target;
      node.removeClass('hoveredCommon hovered');

      node.outgoers('node').forEach((depNode: NodeSingular) => {
        depNode.removeClass('hoveredCommon hoveredOutgoers hoveredHighlight');
      });
      node.incomers('node').forEach((depNode: NodeSingular) => {
        depNode.removeClass('hoveredCommon hoveredIncomers');
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
      duration: 300,
    });
  }

  onZoomFit(_event?: MouseEvent) {
    this.cy.animate({
      fit: {
        eles: this.cy.elements(),
        padding: 10,
      },
      duration: 300,
    });
  }

  onDiveInUbuntu(_event: MouseEvent) {
    this.onDiveIn('78f660ea-c2f6-49e8-b116-c93884ad68bf');
  }

  onDiveInDotnet(_event: MouseEvent) {
    this.onDiveIn('404f2067-003e-47cd-aade-16875d0c6899');
  }

  onDiveIn(nodeId: string) {
    const rootNode = this.cy.$(`#${nodeId}`);
    //this.cy.elements().not(rootNode).animate({ style: { opacity: 0 }, duration: 300 });
    this.cy.remove(this.cy.elements().not(rootNode));
    const newElements = this.getElementsByNodeId(nodeId);
    this.cy.add(newElements);
    //this.cy.elements().animate({ style: { opacity: 1 }, duration: 300 });
    this.cy.layout(this.fcoseLayoutOptions as FcoseLayoutOptions).run();
    this.cy.fit();
  }

  private getElementsByNodeId(nodeId: string): ElementDefinition[] {
    const nodeIds: string[] = [nodeId];
    this.getNodeIds(nodeId, nodeIds);

    const groupMap: string[] = [];

    const elements: ElementDefinition[] = [];
    nodeIds.forEach((id) => {
      const sbomDetail = this.dataDtos[0].details?.find((x) => x.bomRef == id);
      if (sbomDetail) {
        if (sbomDetail.purl?.startsWith('pkg:nuget/')) {
          const potentialNs = sbomDetail.name.split('.')[0];
          if (!groupMap.includes(potentialNs)) {
            groupMap.push(potentialNs);
            elements.push({ data: { id: potentialNs, label: potentialNs }, classes: 'nodeCommon' });
          }
        }
        elements.push({
          data: {
            id: id,
            label: sbomDetail.name ?? '',
            parent:
              sbomDetail.purl?.startsWith('pkg:nuget/') && !sbomDetail.name.includes('Runtime.linux-x64')
                ? sbomDetail.name.split('.')[0]
                : undefined,
          },
          classes: `nodeCommon nodePackage ${sbomDetail.dependsOn?.length ? 'nodeBranch' : 'nodeLeaf'}`,
        });
        sbomDetail.dependsOn?.forEach((depends) => {
          elements.push({
            data: {
              source: sbomDetail.bomRef,
              target: depends,
            },
            classes: 'edgeCommon',
          });
        });
      }
    });

    return elements;
  }

  private getNodeIds(nodeId: string, nodeIds: string[]) {
    const detail = this.dataDtos[0].details?.find((x) => x.bomRef == nodeId);
    if (!detail) {
      return;
    }
    const detailRefIds = detail.dependsOn;
    if (!detailRefIds) {
      return;
    }

    const newIds: string[] = detailRefIds.filter((id) => !nodeIds.includes(id));
    nodeIds.push(...newIds);
    newIds.forEach((id) => this.getNodeIds(id, nodeIds));
  }

  //this.dataDtos[0].details?.forEach(detail => {
  //  elements.push({ data: { id: detail.bomRef, label: detail.name } });
  //  detail.dependsOn?.forEach(depends => {
  //    elements.push({
  //      data: {
  //        source: detail.bomRef,
  //        target: depends
  //      }
  //    });
  //  });
  //});

  // tests sbom
  private dataDtos: ClusterSbomReportDto[] = [];
  //
}
