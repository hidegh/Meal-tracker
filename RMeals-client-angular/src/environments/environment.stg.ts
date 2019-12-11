import * as v from '../version.json';
import { UrlConsts } from './url-consts';

/** Changes FORBIDDEN */
export const environment = {
  name: 'STG',
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
