const $RefParser = require('@apidevtools/json-schema-ref-parser');
const NgOpenApiGen = require('ng-openapi-gen').NgOpenApiGen;

generateApi();

async function generateApi() {
  const options = {
    input: 'backend-api.yaml',
    output: 'src/api',
  };
  // load the openapi-spec and resolve all $refs
  const RefParser = new $RefParser();
  const openApi = await RefParser.bundle(options.input, {
    dereference: {circular: false},
  });
  const ngOpenGen = new NgOpenApiGen(openApi, options);
  ngOpenGen.generate();
}
