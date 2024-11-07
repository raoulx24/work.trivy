# Trivy Operator Dashboard

<div align="center">
  <img src="docs/imgs/logo.blurred.png" width="500">
</div>

## Intro

Welcome to the Trivy Operator Dashboard, a comprehensive security management tool for Kubernetes environments. Built on top of the powerful Trivy Operator from Aqua Security, this dashboard provides insights into your cluster's security posture. It offers detailed insights on vulnerability scans, policy validation scans and other ones. With an intuitive and user-friendly interface, you can easily monitor, detect, and respond to security threats, ensuring your Kubernetes environment remains robust and secure.

![](docs/imgs/combo.png)

## Features

The application exposes the following reports:
- Vulnerability Reports
- Config Audit Reports
- Cluster RBAC Assessment Reports
- Exposed Secret Reports

All of them consists in dashboards (for view at a glance), browse and inspect findings (with table filters, sorts), export data.

<img src="docs/imgs/app.gif">

This app is fully operational, with new features currently in development.

## Why we did it. The Story Behind Trivy Dashboard

1. **Security is Paramount** In our professional life, our dedication to security led us to create Trivy Dashboard. We needed a comprehensive solution to monitor, manage, and mitigate risks that existing open-source options couldn't provide.

2. **Bridging the Open-Source Gap** None of the available open-source dashboards met our specific needs. We developed Trivy Dashboard to fill this gap, offering a powerful and versatile tool tailored to our unique challenges.

3. **DevOps Curiosity** As a DevOps person, my curiosity drove us to build Trivy Dashboard. This project allowed me to gain a deeper and intimate understanding of the development process, enhancing my skills and fostering continuous learning.

Trivy Dashboard represents our commitment to security, bridging open-source gaps, and our relentless curiosity as IT dev professionals. We are happy to share this journey with the community.

## Considerations

Our goal with Trivy Dashboard is to ensure that it excels in a singular focus: being an effective and efficient dashboard. We've had extensive internal discussions and, for the time being, we’ve decided not to include enterprise-grade features such as authentication/authorization, Trivy reports history, email alerts, or direct configuration of the Trivy operator.

While we recognize the potential value these features could bring, our current aim is to maintain simplicity and focus on perfecting the core functionality of the dashboard. However, we remain open to the possibility of expanding its capabilities if the app gains significant traction and user demand increases.

For now, our priority is to deliver the rest of the provided features by Trivy (such as ClusterComplianceReport, ClusterConfigAuditReport, ClusterInfraAssessmentReport and so on), in order to have a robust and reliable dashboard that meets our immediate needs and serves the community effectively.

## Documentation

Main documentation of the app can be found [here](docs/main-doc.md)

## More Info

[Development Notes](DEV_NOTES.md)

[Contributing](CONTRIBUTING.md)

[Code of Conduct](CODE_OF_CONDUCT.md)

## Acknowledgements

I would like to give my sincere thanks to the following persons who have helped me to get here:
 - **Dănuț** - For his technical guidance and explanations. 
 - **Florin** - For his insights regarding the app and endurance with its bugs.
 - **Alina** - My better half, for tolerating my long programming evenings.
