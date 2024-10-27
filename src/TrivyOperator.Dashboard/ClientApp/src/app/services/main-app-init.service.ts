import { Injectable } from '@angular/core';

import { BackendSettingsService } from '../../api/services/backend-settings.service'
import { BackendSettingsDto } from '../../api/models/backend-settings-dto'


@Injectable({
  providedIn: 'root',
})
export class MainAppInitService {
  backendSettingsDto: BackendSettingsDto | null = null;
  constructor(private backendSettingsService: BackendSettingsService) {
  }

  initializeApp(): Promise<void> {
    return new Promise((resolve, reject) => {
      this.backendSettingsService.getBackendSettings().subscribe({
        next: (res) => {
          this.backendSettingsDto = res;
          resolve();
        },
        error: (err) => {
          console.error(err);
          reject(err);
        }
      });
    });
  }
}

export function initializeAppFactory(service: MainAppInitService) {
  return () => service.initializeApp();
}
