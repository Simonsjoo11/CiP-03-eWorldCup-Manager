import 'dotenv/config';
import { defineConfig } from 'orval';

export default defineConfig({
  eworldcup: {
    input: {
      target: './openapi.json',
    },
    output: {
      target: './src/lib/api/generated/eWorldCupApi.ts',
      client: 'react-query',
      httpClient: 'axios',
      mode: 'split',
      schemas: './src/lib/api/generated/model',
      clean: true,
      baseUrl: process.env.NEXT_PUBLIC_API_BASE_URL ?? 'http://localhost:5296',
    },
    hooks: {
      afterAllFilesWrite: ['prettier --write'],
    },
  },
});
