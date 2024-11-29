const esbuild = require('esbuild');
const {copy} = require('esbuild-plugin-copy');
const sassPlugin = require("esbuild-plugin-sass");

const base='./src/WebExt';

esbuild.build({
  entryPoints: [
    `${base}/popup/popup.ts`,
    `${base}/service-worker.ts`
  ],
  bundle: true,
  format: 'esm',
  entryNames: '[dir]/[name]',
  outdir: './build-ext',
  plugins: [
    copy({
      resolveFrom: 'cwd',
      assets: {
        from: [`${base}/**/*.html`, `${base}/manifest.json`, `${base}/**/*.png`],
        to: ['./build-ext']
      },
    }),
    sassPlugin(),
  ],
});
