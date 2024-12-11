# Backend

## Windows with Visual Studio

- Install [Visual Studio 2022 version 17.11.5 or newer](https://visualstudio.microsoft.com/vs/):
    - Select the following workloads:
        - `ASP.NET and web development` workload.

## Prerequisites

- a valid kube config file should be available for Kubernetes cluster access
- if needed, the [k8s.create.kubeconfig.for.sa.ps1](tools/k8s.create.kubeconfig.for.sa.ps1) script can be used for
  creation of custom kube config file(s)

# Frontend

## Windows with nvm

- Install [nvm version 1.1.12](https://github.com/coreybutler/nvm-windows/releases/tag/1.1.12):
    - Install Node.js LTS:
        - `nvm install 22.11.0`.
    - Use Node.js LTS:
        - `nvm use 22.11.0`.

## Development server

Run `ng serve` for a dev server. Navigate to `http://localhost:4200/`. The app will automatically reload if you change
any of the source files.

## Code scaffolding

### New components

Run `ng generate component component-name` to generate a new component. You can also use
`ng generate directive|pipe|service|class|guard|interface|enum|module`.

```sh
ng g component fetch-data-new --skip-tests --style=scss
```

### Update OpenAPI specification for new endpoints

```sh
npm run update-endpoints:local
```

### Update npm packages

```sh
ncu
ncu -u
npm install
```

### Linter

```sh
npm run lint
npm run lint:fix
```

## Build

Run `ng build` to build the project. The build artifacts will be stored in the `dist/` directory.

## Running unit tests

Run `ng test` to execute the unit tests via [Karma](https://karma-runner.github.io).

## Running end-to-end tests

Run `ng e2e` to execute the end-to-end tests via a platform of your choice. To use this command, you need to first add a
package that implements end-to-end testing capabilities.

## Further help

To get more help on the Angular CLI use `ng help` or go check out
the [Angular CLI README](https://github.com/angular/angular-cli/blob/master/README.md).
