import {mergeApplicationConfig, ApplicationConfig, importProvidersFrom} from '@angular/core';
import { appConfig } from './app.config';
import {HttpClient} from "@angular/common/http";
import {TranslateHttpLoader} from "@ngx-translate/http-loader";
import {TranslateLoader, TranslateModule} from "@ngx-translate/core";

const browserConfig: ApplicationConfig = {
  providers: [
    importProvidersFrom(
      TranslateModule.forRoot({
        loader: {
          provide: TranslateLoader,
          useFactory: (http: HttpClient) => {
            return new TranslateHttpLoader(http);
          },
          deps: [
            HttpClient
          ]
        }
      })
    )
  ]
};

export const config = mergeApplicationConfig(appConfig, browserConfig);
