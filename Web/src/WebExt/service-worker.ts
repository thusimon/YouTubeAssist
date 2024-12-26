import Port from './utils/port';

let p = null;

const HOST_NAME='com.utticus.youtube.assist.host';
const promiseMap = {};

type MessageType = {
  type: string; // direction of the message, e.g POPUP_TO_SW(Serive Worker), NATIVE_TO_HOST, HOST_TO_SW
  action: string; // purpose of the message, e.g MESSAGE, AUTH
  data?: { [key: string]: string; }; // data of the message, optional
  error?: { [key: string]: string; }; // error of the message, optional
}

const sendMessageToPop = (action, data = undefined, error = undefined) => {
  const message: MessageType = {
    type: 'SW_TO_POP',
    action,
    data,
    error
  };
  chrome.runtime.sendMessage(message);
}
const disposePort = () => {
  if (p) {
    try {
      p.disconnect();
      p = null;
      sendMessageToPop('PORT_DISPOSE', {message: 'port disposed'});
    } catch (e) {
      console.log(`Error: ${e.message}`);
    }
  }
}

const createPort = async () => {
  if (p) {
    sendMessageToPop('PORT_CREATE', undefined, {message: 'Port exists, can not re-create'});
    return;
  }
  const port = new Port(HOST_NAME);
  p = await port.connect();
  console.log('port created', p);

  const handlePortNativeMessage = (action, data, error) => {
    switch (action) {
      case 'AUTH': {
        const {from, uuid} = data;
        if (from === 'webapp') {
          // this is for WebApp
          const deferred = promiseMap[uuid];
          if (deferred) {
            deferred.resolve(data);
          }
          delete promiseMap[uuid];
          return;
        } else if (from === 'popup') {
          // this is for Popup
          sendMessageToPop(action, data);
          return;
        }
      }
      case 'MESSAGE': {
        const message = data.message;
        sendMessageToPop(action, {message: `MSG From Native: ${message}`});
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
  p.onDisconnect.addListener((p) => {
    console.log('disconnected', p);
    const errorMessage = p.error?.message || chrome.runtime.lastError?.message
    sendMessageToPop('PORT_CLOSE', undefined, {message: errorMessage});
  });

  sendMessageToPop('PORT_CREATE', {message: 'port created'});
};

const messagePort = (action, data) => {
  if (!p) {
    sendMessageToPop('MESSAGE', {message: 'no port, can not send message'})
    return;
  }

  const messageToNative = {
    type: 'EXT_TO_HOST',
    action: action,
    // data or error to native must be a Dictionary<string, string>
    data: data
  }
  p.postMessage(messageToNative);
}


const handlePopUpMessage = (action, data) => {
  switch(action) {
    case 'PORT_CREATE':
      createPort();
      break;
    case 'PORT_DISPOSE':
      disposePort();
      break;
    case 'MESSAGE':
      messagePort(action, data);
      break;
    case 'AUTH':
      messagePort(action, data);
      break;
    defaut:
      break;
  }
}

const handleWebMessage = (action, data, sender, sendResponse) => {
  switch (action) {
    case 'AUTH':
      const tabId = sender?.tab?.id;
      const url = sender?.url;
      const uuid = `${url}-${tabId}`;
      data.uuid = uuid;
      data.from = 'webapp';
      const deferred = Promise.withResolvers();
      promiseMap[uuid] = deferred;
      deferred.promise.then((result) => {
        sendResponse(result);
      });
      messagePort(action, data);
      break;
    default:
      break;
  }
}

chrome.runtime.onMessage.addListener((message: MessageType, sender, sendResponse) => {
  const {type, action, data} = message;
  switch(type) {
    case 'POP_TO_SW':
      handlePopUpMessage(action, data);
      break;
    defaut:
      break;
  }
});

chrome.runtime.onMessageExternal.addListener((message: MessageType, sender, sendResponse) => {
  const {type, action, data} = message;
  switch(type) {
    case 'WEB_TO_SW': {
      handleWebMessage(action, data, sender, sendResponse);
      return true;
    }
    defaut:
      break;
  }
});

if (chrome.runtime.lastError) {
  console.error(chrome.runtime.lastError);
}

