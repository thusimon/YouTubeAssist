import Port from './utils/port';

let p = null;

const HOST_NAME='com.utticus.youtube.assist.host';

const disposePort = () => {
  if (p) {
    try {
      p.disconnect();
      p = null;
      chrome.runtime.sendMessage({req: 'sw-port-dispose', data: 'port disposed'});
    } catch (e) {
      console.log(`Error: ${e.message}`);
    }
  }
}

const createPort = async () => {
  if (p) {
    chrome.runtime.sendMessage({
      req: 'sw-port-create',
      error: 'port_exists',
      detail:'Port exists, can not re-create'
    });
    return;
  }
  const port = new Port(HOST_NAME);
  p = await port.connect();
  console.log('port created', p);

  p.onMessage.addListener((msg) => {
    const {resp} = msg;
    console.log('From Native', msg);
    chrome.runtime.sendMessage({req: 'sw-port-msg', data: resp});
  }); 
  p.onDisconnect.addListener((msg) => {
    console.log('disconnected', msg);
    chrome.runtime.sendMessage({req: 'sw-port-close', error: msg});
  });

  chrome.runtime.sendMessage({req: 'sw-port-create', data: 'port created'});
};

const messagePort = (msg) => {
  if (!p) {
    chrome.runtime.sendMessage({req: 'sw-port-msg', error: 'no port, can not send message'});
    return;
  }

  p.postMessage({req: msg});
}

chrome.runtime.onMessage.addListener((message, sender, sendResponse) => {
  const {req, data} = message;
  switch(req) {
    case 'pop-port-create':
      createPort();
      break;
    case 'pop-port-dispose':
      disposePort();
      break;
    case 'pop-port-message':
      messagePort(data);
      break;
    defaut:
      break;
  }
});

chrome.runtime.onMessageExternal.addListener((message, sender, sendResponse) => {
  const {req, data} = message;
  switch(req) {
    case 'webapp-port-message':
      //messagePort(data);
      break;
    defaut:
      break;
  }
});

if (chrome.runtime.lastError) {
  console.error(chrome.runtime.lastError);
}

