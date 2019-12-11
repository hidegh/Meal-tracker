/*
PERMIT TO CHANGE GRANTED (but avoid pushing changes, in case of doing so, use QA api values to restore original state)!
*/
import * as v from '../version.json';
import { UrlConsts } from './url-consts';

export const environment = {
  name: 'DEV',
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
