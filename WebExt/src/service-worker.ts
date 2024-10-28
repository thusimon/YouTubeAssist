import Port from './utils/port';

let p;
(async () => {
  // const HOST_NAME='com.utticus.youtube.assist.host'
  // const port = new Port(HOST_NAME);

  // p = await port.connect();
  // console.log('connection created', p);

  // p.onMessage.addListener((msg) => {
  //   console.log('get native msg', msg);
  // });
  // p.onDisconnect.addListener((msg) => {
  //   console.log('disconnected', msg);
  // });

  // p.postMessage({text: 'Hello, my_application'});

  // chrome.runtime.sendNativeMessage(
  //   HOST_NAME,
  //   {msg: 'hello'},
  //   function (response) {
  //     if (chrome.runtime.lastError) {
  //       console.error("Error:", chrome.runtime.lastError.message);
  //     } else {
  //         console.log("Received response:", response);
  //     }
  //   }
  // );

  if (chrome.runtime.lastError) {
    console.error(chrome.runtime.lastError);
  }
})();
