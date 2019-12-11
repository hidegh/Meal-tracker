/**
 * Must be excluded from build due the usage of "require"!
 * Plus it's just a build related script.
 */
(() => {
  console.log('Updating app. version!');

  const fs = require('fs');

  /**
   * Originally we had deployment/version.json and src/version.json
   * We updated both
   * But we read the version from the deployment/version.json
   */
  const versionPath = 'src/version.json';

  const getNextVersion = (version) => {

    const parts = version.split('.');

    if (parts.length < 3) {
      throw new Error('Version number should be in #.#.# format');
    }

    // tslint:disable-next-line: radix
    let currentMajorVersion = parseInt(parts[0]);
    // tslint:disable-next-line: radix
    let currentMinorVersion = parseInt(parts[1]);
    // tslint:disable-next-line: radix
    let currentMinorIteration = parseInt(parts[2]);

    currentMinorIteration++;

    if (currentMinorIteration > 999) {
      currentMinorIteration = 1;
      currentMinorVersion++;
    }

    if (currentMinorVersion > 999) {
      currentMinorVersion = 1;
      currentMajorVersion++;
    }

    return currentMajorVersion + '.' + currentMinorVersion + '.' + currentMinorIteration;
  };

  fs.readFile(versionPath, 'utf8', (err, fileContents) => {

    // tslint:disable-next-line: curly
    if (err)
      throw err;

    try {

      let data = JSON.parse(fileContents);

      if (data && data.version) {
        const nextVersion = getNextVersion(data.version);

        data.version = nextVersion;

        fs.writeFile(versionPath, JSON.stringify(data, null, 4), err => {
          // tslint:disable-next-line: curly
          if (err) throw err;
        });

      }

    } catch (err) {
      throw err;
    }

  });

})();
