import * as express from 'express';

const PORT = 3000;

const app = express()

app.use(express.static('build/WebApp/public'));

app.get('/', function(req, res) {
  res.sendFile('/index.html', { root: 'build/WebApp/public' });
});

app.listen(PORT, () => {
  console.log(`Server started on ${PORT}`);
});
