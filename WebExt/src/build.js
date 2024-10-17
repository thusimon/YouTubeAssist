const esbuild = require('esbuild');
const {copy} = require('esbuild-plugin-copy');
const sassPlugin = require("esbuild-plugin-sass");


esbuild.build({
  entryPoints: [
    './src/popup/popup.ts'
  ],
  bundle: true,
  entryNames: '[dir]/[name]',
  outbase: 'src',
  outdir: './build',
  plugins: [
    copy({
      resolveFrom: 'cwd',
      assets: {
        from: ['./src/**/*.html', './src/manifest.json', './src/**/*.png'],
        to: ['./build'],
      },
    }),
    sassPlugin(),
  ],
});