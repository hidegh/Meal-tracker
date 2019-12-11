import 'zone.js/dist/zone-error'; // For better debugging (avoid to use in PROD due perf. penalties)

import * as v from '../version.json';
import { UrlConsts } from './url-consts';

/** Changes FORBIDDEN */
export const environment = {
  name: 'QA',
  production: false,
  version: v.default.version,
  auth0 : {
  },
  security: {
  },
  urls: {
    [UrlConsts.API]: 'https://localhost:44383/'
  }
};
