const esbuild = require('esbuild');
const {copy} = require('esbuild-plugin-copy');
const sassPlugin = require("esbuild-plugin-sass");

const base='./src/WebApp';

esbuild.build({
  entryPoints: [
    `${base}/public/index.ts`
  ],
  bundle: true,
  format: 'esm',
  entryNames: '[dir]/public/[name]',
  outdir: './build/WebApp',
  plugins: [
    copy({
      resolveFrom: 'cwd',
      assets: {
        from: [`${base}/**/*.html`, `${base}/**/*.png`],
        to: ['./build/WebApp']
      },
    }),
    sassPlugin(),
  ],
});
