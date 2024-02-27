import {mergeApplicationConfig, ApplicationConfig, importProvidersFrom} from '@angular/core';
import { provideServerRendering } from '@angular/platform-server';
import { appConfig } from './app.config';
import {of} from "rxjs";
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';

import * as translationEn from '../assets/i18n/en.json';
import * as translationFr from '../assets/i18n/fr.json';
class TranslateJsonLoader implements TranslateLoader {
public getTranslation(lang: string) {
    switch (lang) {
      case 'nl': return of(translationFr);
      default: return of(translationEn);
    }
  }
}

const serverConfig: ApplicationConfig = {
  providers: [
    provideServerRendering(),
    importProvidersFrom(
      TranslateModule.forRoot({
        loader: {
          provide: TranslateLoader,
          useFactory: () => {
            return new TranslateJsonLoader();
          }
        }
      }))
  ]
};

export const config = mergeApplicationConfig(appConfig, serverConfig);
