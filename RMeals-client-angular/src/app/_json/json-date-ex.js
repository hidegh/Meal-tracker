/*
 * JSON Date Extensions
 * Based on https://github.com/RickStrahl/json.date-extensions
 *
 * USAGE:
 *
 * Just "require" given module file inside app.component.ts:
 * const variable = require('json-date-ex')
 *
 * Inside the constructor set up the default for date-stringify (the one your architecture needs: 0 - UTC, 1 - local,  2 - unspecified time-zones):
 *  (<any>JSON).useDateStringifyMode = 2;
 */
(function (undefined) {
  "use strict";

  if (JSON && !JSON.dateExInitialized) {

      JSON.dateExInitialized = true;

      console.log('initializing JSON date handling');

      JSON.dateStringifyModeEnum = {
          defaultUTC: 0,
          local: 1,
          unspecifiedTimeZone: 2
      }

      JSON.useDateParser = true;
      JSON.useDateStringifyMode = JSON.dateStringifyModeEnum.defaultUTC;

      const reISO = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2}(?:\.{0,1}\d*))(?:Z|(\+|-)([\d|:]*))?$/;

      // store original stringify and parse
      JSON._parse = JSON.parse;
      JSON._stringify = JSON.stringify;

      // replace them with new function
      JSON.parse = function(jsonValue, reviverFn)
      {
          return JSON.useDateParser
              ? JSON._parse(jsonValue, function(key, value) {
                  return  dateExReviver.bind(this)(key, value, reviverFn);
              })
              : JSON._parse(jsonValue, reviverFn);
      }

      JSON.stringify = function(objectToConvert, replacerFn, space)
      {
          return JSON.useDateStringifyMode
              ? JSON._stringify(objectToConvert, function (key, convertedJsonValue) {
                  return dateExReplacer.bind(this)(JSON.useDateStringifyMode, key, convertedJsonValue, replacerFn);
                  },
                  space
              )
              : JSON._stringify(objectToConvert, replacerFn, space);
      }

      var dateExReviver = function(key, value, chainFilter)
      {
          // console.log('reviver', key, value, chainFilter);

          // NOTE: this is bound to the object on which the key property is being processed
          var parsedValue = value;

          if (typeof value === 'string') {
              var a = reISO.exec(value);
              if (a) {
                  // This handled ISO date's according docx: any date with time-zone will be converted (shifted) to local; date without time zone info will be assumed to be a local date
                  parsedValue = new Date(value);
              }
          }

          if (chainFilter && typeof chainFilter === "function")
              return chainFilter(key, parsedValue);
          else
              return parsedValue;
      }

      var dateExPad = function (num) {
          if (num < 10) {
              return '0' + num;
          }
          return num;
      }

      var dateExReplacer = function(dateStringifyMode, key, jsonifiedValue, chainFilter) {

          // console.log('replacer', dateStringifyMode, key, jsonifiedValue, chainFilter);

          // NOTE: this is bound to the object on which the key property is being processed - so we get rawValue (value)
          var value = this[key];

          var modifiedResult = jsonifiedValue;

          if (value instanceof Date) {

              var dateString = "";

              switch (dateStringifyMode) {
                  case 1: // return local with offset
                  case 2: // return local without offset

                      // create basic date string
                      dateString =
                          value.getFullYear() +
                          '-' +
                          dateExPad(value.getMonth() + 1) +
                          '-' +
                          dateExPad(value.getDate()) +
                          'T' +
                          dateExPad(value.getHours()) +
                          ':' +
                          dateExPad(value.getMinutes()) +
                          ':' +
                          dateExPad(value.getSeconds()) +
                          '.' +
                          (value.getMilliseconds() / 1000)
                              .toFixed(3)
                              .slice(2, 5);

                      if (dateStringifyMode == 2)
                          break;

                      // add local offset
                      var tzo = -1 * value.getTimezoneOffset();
                      var dif = tzo >= 0 ? '+' : '-';

                      dateString +=
                          dif
                          + dateExPad(tzo / 60) + ':' + dateExPad(tzo % 60);

                      break;

                  default:
                      // default behaviour, return GMT date
                      dateString = value.toISOString();
              }

              modifiedResult = dateString;

          }

          if (chainFilter && typeof chainFilter === "function")
              return chainFilter(key, modifiedResult);
          else
              return modifiedResult;

      }

  }
})();

