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

let p = null;
const HOST_NAME='com.utticus.youtube.assist.host'

const portControlClickHandler = async (event) => {
  const id = event.target.id;
  if (p != null && id === 'port-btn-connect') {
    console.log('port is still in use, can not connect');
    return;
  }
  if (p == null && id === 'port-btn-disconnect') {
    console.log('port is already disconnected, can not disconnect');
    return;
  }

  if (id === 'port-btn-connect') {
    const port = new Port(HOST_NAME);
    p = await port.connect();
    console.log('connection created', p);

    p.onMessage.addListener((msg) => {
      console.log('get native msg', msg);
      const {resp} = msg;
      log(resp);
    }); 
    p.onDisconnect.addListener((msg) => {
      console.log('disconnected', msg);
    });

    return;
  }

  if (id === 'port-btn-disconnect') {
    p.disconnect();
    p = null;
  }
  
}
portConnectBtnE.addEventListener('click', portControlClickHandler);
portDisconnectBtnE.addEventListener('click', portControlClickHandler);

btnToNativeE.addEventListener('click', () => {
  if (p == null) {
    console.log('port is null, can not send message');
    return;
  }
  const req = textToNativeE.value;

  if (!req) {
    return;
  }

  p.postMessage({req});
    
});

console.log('popup opened');
