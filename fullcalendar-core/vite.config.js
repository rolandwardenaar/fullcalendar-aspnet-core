import { defineConfig } from 'vite';
import { resolve } from 'path';

export default defineConfig({
  root: '.',
  publicDir: 'public',
  build: {
    outDir: 'wwwroot',
    emptyOutDir: false,
    manifest: false,
    rollupOptions: {
      input: {
        calendar: resolve(__dirname, 'Scripts/main.js')
      },
      output: {
        entryFileNames: 'js/[name].js',
        chunkFileNames: 'js/[name]-[hash].js',
        assetFileNames: (assetInfo) => {
          if (assetInfo.name.endsWith('.css')) {
            return 'css/[name][extname]';
          }
          return 'assets/[name]-[hash][extname]';
        }
      }
    },
    sourcemap: false,
    minify: 'esbuild'
  },
  css: {
    preprocessorOptions: {
      scss: {
        api: 'modern-compiler',
        silenceDeprecations: ['legacy-js-api']
      }
    },
    postcss: false
  },
  server: {
    port: 5173,
    strictPort: false,
    open: false,
    hmr: {
      protocol: 'ws',
      host: 'localhost'
    }
  }
});
