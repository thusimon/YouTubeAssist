import Port from '../utils/port';
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
    chrome.runtime.sendMessage({req: 'pop-port-create'});
  } else if (id === 'port-btn-disconnect') {
    chrome.runtime.sendMessage({req: 'pop-port-dispose'});
  }
  
}
portConnectBtnE.addEventListener('click', portControlClickHandler);
portDisconnectBtnE.addEventListener('click', portControlClickHandler);

btnToNativeE.addEventListener('click', () => {
  const msg = textToNativeE.value;

  if (!msg) {
    return;
  }

  chrome.runtime.sendMessage({req: 'pop-port-message', data: msg});    
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
  chrome.runtime.sendMessage({req: 'pop-port-message', data: 'WebExt::Auth:request'}); 
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

chrome.runtime.onMessage.addListener((message, sender, sendResponse) => {
  const {req, data, error} = message;
  switch(req) {
    case 'sw-port-msg': {
      log(`From Native: ${data}`);
      handleMessage(data);
      break;
    }
    case 'sw-port-create': {
      if (error === 'port_exists') {
        portConnectBtnE.disabled = true;
      }
      break;
    }
    case 'sw-port-dispose': {
      portConnectBtnE.disabled = false;
      break;
    }
    case 'sw-port-close': {
      break;
    }
    defaut:
      break;
  }
});

// create port when page is opened
chrome.runtime.sendMessage({req: 'pop-port-create'});

console.log('popup opened');
