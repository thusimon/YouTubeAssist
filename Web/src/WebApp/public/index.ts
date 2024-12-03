import './index.scss';

const WEBEXT_GUID = 'kpkilehcpjaeglkmjmodnbijmcphnmfm';

const captionE = document.getElementById('caption');
const btnAuth = document.getElementById('auth-win-btn');
const btnAuthText = document.getElementById('auth-btn-text');
const btnAuthLoader = document.getElementById('auth-btn-loader');

const setCaption = (authStatus) => {
  switch (authStatus) {
    case 1: {
      captionE.textContent = "Please complete challenge..."
      break;
    }
    case 2: {
      captionE.textContent = "Welcome!"
      break;
    }
    case 3: {
      captionE.textContent = "Access denied!"
      break;
    }
  }
}
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

/**
 * 0: idle
 * 1: in progress
 * 2: success
 * 3: fail
 */
let authStatus = 0;
btnAuth.addEventListener('click', async (evt) => {
  if (authStatus != 0) {
    console.log('still in auth progress, please wait');
    return;
  }
  authStatus = 1;
  //btnAuth.disabled = true;
  setCaption(authStatus);
  setBtnAuthStatus(authStatus);
  const authResult = await chrome.runtime.sendMessage(WEBEXT_GUID, {req: 'webapp-port-message', data: 'WebExt::Auth:request'});

  if (authResult === 'WebExt::Auth:True') {
    authStatus = 2
  } else if (authResult === 'WebExt::Auth:False') {
    authStatus = 3
  }
  setCaption(authStatus);
  setBtnAuthStatus(authStatus);
});
