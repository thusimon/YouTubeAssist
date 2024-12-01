import * as express from 'express';

const PORT = 3000;

const app = express()

app.get('/', function (req, res) {
  res.send('Hello World');
});

app.listen(PORT, () => {
  console.log(`Server started on ${PORT}`);
});
