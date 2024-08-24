import {enableProdMode, StaticProvider} from '@angular/core';
import {platformBrowserDynamic} from '@angular/platform-browser-dynamic';

import {AppModule} from './app/app.module';
import {environment} from './environments/environment';

if (environment.production) {
  enableProdMode();
}

const providers: StaticProvider[] = [];
platformBrowserDynamic(providers).bootstrapModule(AppModule)
  .catch(err => console.log(err));
