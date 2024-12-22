import "./popup.scss";

const titleLinkE = document.getElementById('popup-link');

titleLinkE?.addEventListener('click', () => {
  const href = chrome.runtime.getURL('popup/popup.html');
  chrome.tabs.create({url: href})
});

const tabLinksE = document.getElementsByClassName('tab-link');
const portControlsE = document.getElementById('port-controls');
const portConnectBtnE = document.getElementById('port-btn-connect');
const portDisconnectBtnE = document.getElementById('port-btn-disconnect');
const textFromNativeE = document.getElementById('from-native-text');
const textToNativeE = document.getElementById('to-native-text');
const btnToNativeE = document.getElementById('to-native-btn');
const btnAuth = document.getElementById('auth-win-btn');
const btnAuthText = document.getElementById('auth-btn-text');
const btnAuthLoader = document.getElementById('auth-btn-loader');

const log = (message) => {
  textFromNativeE.value += message + '\n';
};

const sendMessageToSW = (action, data = undefined, error = undefined) => {
  const message = {
    type: 'POP_TO_SW',
    action,
    data,
    error
  };
  chrome.runtime.sendMessage(message);
}

const tabLinkClickHandler = (event) => {
  for (const tabLinkE of tabLinksE) {
    tabLinkE.classList.remove('active');
  }
  event.target.classList.add('active')
  if (event.target.id === 'tab-port') {
    portControlsE.classList.remove('hide');
  } else {
    portControlsE.classList.add('hide');
  }
}

for (const tabLink of tabLinksE) {
  tabLink.addEventListener('click', tabLinkClickHandler);
}

const portControlClickHandler = async (event) => {
  const id = event.target.id;
  if (id === 'port-btn-connect') {
    sendMessageToSW('PORT_CREATE');
  } else if (id === 'port-btn-disconnect') {
    sendMessageToSW('PORT_DISPOSE');
  }
  
}
portConnectBtnE.addEventListener('click', portControlClickHandler);
portDisconnectBtnE.addEventListener('click', portControlClickHandler);

btnToNativeE.addEventListener('click', () => {
  const msg = textToNativeE.value;

  if (!msg) {
    return;
  }
  sendMessageToSW('MESSAGE', msg);
});

/**
 * 0: idle
 * 1: in progress
 * 2: success
 * 3: fail
 */
let authStatus = 0;
btnAuth.addEventListener('click', (evt) => {
  if (authStatus != 0) {
    console.log('still in auth progress, please wait');
    return;
  }
  authStatus = 1;
  //btnAuth.disabled = true;
  setBtnAuthStatus(authStatus);

  sendMessageToSW('MESSAGE', 'WebExt::Auth:request') 
})

const setBtnAuthStatus = (authStatus) => {
  switch (authStatus) {
    case 0: {
      btnAuth.className = 'normal';
      btnAuthText.textContent = 'Authenticate by Windows Hello';
      btnAuthLoader.classList.replace('show', 'hide');
      break;
    }
    case 1: {
      btnAuth.className = 'normal';
      btnAuthText.textContent = 'Authenticating';
      btnAuthLoader.classList.replace('hide', 'show');
      break;
    }
    case 2: {
      btnAuth.className = 'success';
      btnAuthText.textContent = 'Authenication Success✔';
      btnAuthLoader.classList.replace('show', 'hide');
      break;
    }
    case 3: {
      btnAuth.className = 'fail';
      btnAuthText.textContent = 'Authenication Failed✘';
      btnAuthLoader.classList.replace('show', 'hide');
      break;
    }
    default:
      break;
  }
} 

const handleMessage = (msg) => {
  switch (msg) {
    case 'WebExt::Auth:True':
      authStatus = 2
      setBtnAuthStatus(authStatus);
      break;
    case 'WebExt::Auth:False':
      authStatus = 3
      setBtnAuthStatus(authStatus);
      break;
    default:
      break;
  }
};

const handleSWMessage = (action, data, error) => {
  switch(action) {
    case 'MESSAGE': {
      log(data);
      break;
    }
    case 'PORT_CREATE': {
      portConnectBtnE.disabled = true;
      if (error) {
        log(error);
      }
      break;
    }
    case 'PORT_DISPOSE': {
      portConnectBtnE.disabled = false;
      break;
    }
    case 'PORT_CLOSE': {
      break;
    }
    defaut:
      break;
  }
}
chrome.runtime.onMessage.addListener((message, sender, sendResponse) => {
  const {type, action, data, error} = message;
  switch (type) {
    case 'SW_TO_POP': {
      handleSWMessage(action, data, error);
      break;
    }
  }
});

// create port when page is opened
sendMessageToSW('PORT_CREATE');

console.log('popup opened');
