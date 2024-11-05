import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

import { BackendSettingsDto } from '../../api/models/backend-settings-dto';
import { BackendSettingsService } from '../../api/services/backend-settings.service';
import { LocalStorageUtils } from '../utils/local-storage.utils'

@Injectable({
  providedIn: 'root',
})
export class MainAppInitService {
  defaultBackendSettingsDto: BackendSettingsDto | null = null;
  isDarkMode: boolean = false;
  private backendSettingsDtoSubject: BehaviorSubject<BackendSettingsDto> = new BehaviorSubject<BackendSettingsDto>({
    trivyReportConfigDtos: [],
  });
  backendSettingsDto$ = this.backendSettingsDtoSubject.asObservable();

  constructor(private backendSettingsService: BackendSettingsService) {}

  initializeApp(): Promise<void> {
    return new Promise((resolve, reject) => {
      this.backendSettingsService.getBackendSettings().subscribe({
        next: (res) => {
          this.defaultBackendSettingsDto = res;
          this.mergeBackendSettingsDto(res);
          this.isDarkMode = this.getDarkMode();
          this.setDarkMode();
          resolve();
        },
        error: (err) => {
          console.error(err);
          reject(err);
        },
      });
    });
  }

  updateBackendSettingsTrivyReportConfigDto(newIds: string[]) {
    const newTrivyReportConfig = (
      this.defaultBackendSettingsDto ?? { trivyReportConfigDtos: [] }
    ).trivyReportConfigDtos?.map((dto) => {
      if (dto.enabled) {
        return { ...dto, enabled: newIds.includes(dto.id ?? '') };
      }
      return dto;
    });

    // const clone = JSON.parse(JSON.stringify(original)) as typeof original;
    this.backendSettingsDtoSubject.next({ trivyReportConfigDtos: newTrivyReportConfig });
    localStorage.setItem('backendSettings.trivyReportConfig', newIds.join(','));
    localStorage.setItem(
      'backendSettings.trivyReportConfig.defaultsPreviousSession',
      (this.defaultBackendSettingsDto?.trivyReportConfigDtos?.filter((x) => x.enabled).map((x) => x.id) ?? []).join(
        ',',
      ),
    );
  }

  switchLightDarkMode() {
    this.isDarkMode = !this.isDarkMode;
    this.setDarkMode();
  }

  private getDarkMode(): boolean {
    return (
      LocalStorageUtils.getBoolKeyValue('mainSettings.isDarkMode') ??
      (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches)
    );
  }

  private setDarkMode() {
    const primengThemeLink = document.getElementById('primeng-theme') as HTMLLinkElement | null;
    if (primengThemeLink == null) {
      return;
    }
    primengThemeLink.href = this.isDarkMode ? 'primeng-dark.css' : 'primeng-light.css';
    localStorage.setItem('mainSettings.isDarkMode', this.isDarkMode.toString());
  }

  

  private mergeBackendSettingsDto(backendSettingsDto: BackendSettingsDto) {
    const previousItems: string[] =
      localStorage.getItem('backendSettings.trivyReportConfig.defaultsPreviousSession')?.split(',') ?? [];
    const itemsToAdd = (
      backendSettingsDto.trivyReportConfigDtos?.filter((x) => x.enabled)?.map((x) => x.id ?? '') ?? []
    ).filter((x) => !previousItems.includes(x));
    const savedItems: string[] =
      localStorage.getItem('backendSettings.trivyReportConfig')?.split(',') ??
      backendSettingsDto.trivyReportConfigDtos?.filter((x) => x.enabled).map((x) => x.id ?? '') ??
      [];
    const mergedItems = [...savedItems, ...itemsToAdd.filter((item) => !savedItems.includes(item))];
    this.updateBackendSettingsTrivyReportConfigDto(mergedItems);
  }
}

export function initializeAppFactory(service: MainAppInitService) {
  return () => service.initializeApp();
}
