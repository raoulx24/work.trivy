import {enableProdMode, StaticProvider} from '@angular/core';
import {platformBrowserDynamic} from '@angular/platform-browser-dynamic';

import {AppModule} from './app/app.module';
import {environment} from './environments/environment';

import {ModuleRegistry} from '@ag-grid-community/core';
import {ClientSideRowModelModule} from "@ag-grid-community/client-side-row-model";

if (environment.production) {
  enableProdMode();
}

const providers: StaticProvider[] = [];
ModuleRegistry.registerModules([ClientSideRowModelModule]);
platformBrowserDynamic(providers).bootstrapModule(AppModule)
  .catch(err => console.log(err));
