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

let authStatus = 0; // 0: idle, 1: in progress
btnAuth.addEventListener('click', (evt) => {
  if (authStatus === 1) {
    console.log('still in auth progress, please wait');
    return;
  }
  authStatus = 1;
  //btnAuth.disabled = true;
  btnAuthText.textContent = 'Authenticating';
  btnAuthLoader.classList.replace('hide', 'show');
  chrome.runtime.sendMessage({req: 'pop-port-message', data: 'WebExt::Auth:request'}); 
})

chrome.runtime.onMessage.addListener((message, sender, sendResponse) => {
  const {req, data, error} = message;
  switch(req) {
    case 'sw-port-msg':
      log(`From Native: ${data}`);
      break;
    case 'sw-port-close':
      break;
    defaut:
      break;
  }
});

console.log('popup opened');
