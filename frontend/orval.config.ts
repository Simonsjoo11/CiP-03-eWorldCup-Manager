import { defineConfig } from 'orval';

export default defineConfig({
  eworldcup: {
    input: {
      target: './openapi.json',
    },
    output: {
      mode: 'split',
      target: './src/lib/api/generated/',
      schemas: './src/lib/api/generated/model',
      client: 'react-query',
      clean: true,
    },
    hooks: {
      afterAllFilesWrite: ['prettier --write'],
    },
  },
});
