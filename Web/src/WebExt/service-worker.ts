import Port from './utils/port';

let p = null;

const HOST_NAME='com.utticus.youtube.assist.host';
const promiseMap = {};


const sendMessageToPop = (action, data = undefined, error = undefined) => {
  const message = {
    type: 'SW_TO_POP',
    action,
    data,
  };
  chrome.runtime.sendMessage(message);
}
const disposePort = () => {
  if (p) {
    try {
      p.disconnect();
      p = null;
      sendMessageToPop('PORT_DISPOSE', 'port disposed');
    } catch (e) {
      console.log(`Error: ${e.message}`);
    }
  }
}

const createPort = async () => {
  if (p) {
    sendMessageToPop('PORT_CREATE', undefined, 'Port exists, can not re-create');
    return;
  }
  const port = new Port(HOST_NAME);
  p = await port.connect();
  console.log('port created', p);

  const handlePortNativeMessage = (action, data, error) => {
    switch (action) {
      case 'AUTH': {
        const UUID = data.UUID;
        const authResult = data.result;
        if (UUID) {
          // this is for WebApp
          const deferred = promiseMap[UUID];
          if (deferred) {
            deferred.resolve(authResult);
          }
          delete promiseMap[UUID];
          return;
        } else {
          // this is for Popup
          sendMessageToPop(action, authResult);
          return;
        }
      }
      case 'MESSAGE': {
        const message = data.message;
        sendMessageToPop(action, `From Native: ${message}`);
      }
    }
  }

  p.onMessage.addListener((msg) => {
    let {type, action, data, error} = msg;
    console.log('From Native', msg);
    switch (type) {
      case 'HOST_TO_EXT': {
        handlePortNativeMessage(action, data, error);
        break;
      }
    }
  }); 
  p.onDisconnect.addListener((msg) => {
    console.log('disconnected', msg);
    sendMessageToPop('PORT_CLOSE', undefined, msg);
  });

  sendMessageToPop('PORT_CREATE', 'port created');
};

const messagePort = (message) => {
  if (!p) {
    sendMessageToPop('MESSAGE', 'no port, can not send message')
    return;
  }

  const messageToNative = {
    type: 'EXT_TO_HOST',
    action: 'MESSAGE',
    // data to native must be a Dictionary<string, string>
    data: {
      message
    }
  }
  p.postMessage(messageToNative);
}


const handlePopUpMessage = (action, data, error) => {
  switch(action) {
    case 'PORT_CREATE':
      createPort();
      break;
    case 'PORT_DISPOSE':
      disposePort();
      break;
    case 'MESSAGE':
      messagePort(data);
      break;
    defaut:
      break;
  }
}
chrome.runtime.onMessage.addListener((message, sender, sendResponse) => {
  const {type, action, data, error} = message;
  switch(type) {
    case 'POP_TO_SW':
      handlePopUpMessage(action, data, error);
      break;
    defaut:
      break;
  }
});

chrome.runtime.onMessageExternal.addListener((message, sender, sendResponse) => {
  const {req, data} = message;
  const tabId = sender?.tab?.id;
  const origin = sender?.origin;
  const uuid = `${tabId}-${origin}`
  switch(req) {
    case 'webapp-port-message': {
      const dataExt = `${data}_${uuid}`
      messagePort(dataExt);
      const deferred = Promise.withResolvers();
      promiseMap[uuid] = deferred;
      deferred.promise.then((result) => {
        sendResponse(result);
      });
      return true;
    }
    defaut:
      break;
  }
});

if (chrome.runtime.lastError) {
  console.error(chrome.runtime.lastError);
}

