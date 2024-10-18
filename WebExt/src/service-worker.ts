import Port from './utils/port';

(async () => {
  const HOST_NAME='com.utticus.youtube.assist.host'
  const port = new Port(HOST_NAME);

  const connection = await port.connect();

  console.log('connection created', connection);
})();
