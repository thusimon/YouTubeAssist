# Introduction
This project demonstrate Chrome/Edge browser extension native messaging to native apps on Windows OS

Mainly contains these sections
* YoutubeAssist visual studio project (YouTubeAssist.sln)
  - YoutubeAssist WPF app, this is the native app
  - ClientHost console app, this is the standard IO app that browser extension can connect to, then this app will use IPC(NamedPipe) to connect to the native app, like a broker between the native app and browser extension 
* Web app and browser extension (in the Web directory)
  - WebApp, a web server for demo purpose
  - WebExt, the browser extension

## Diagram
```mermaid
sequenceDiagram
autonumber

participant External as WebApp/WebExt script
participant WebExt_SW as WebExt ServiceWorker
participant WebExt_Host as WebExt Host(StdIO)
participant Native

alt WebExt Popup open or click connect button
WebExt_SW ->> WebExt_Host: Invoke native Host
Note over WebExt_SW,WebExt_Host: Port created
WebExt_Host ->> Native: Establish IPC
Note over WebExt_Host, Native: NamedPipeStream created
end

alt Message flow
External ->> WebExt_SW: Send JSON message
WebExt_SW ->> WebExt_Host: Post JSON message<br>to native host
Note over WebExt_Host: Read message from StdIO
WebExt_Host ->> Native: Write message string to pipe
Note over Native: Read message from pipe<br>And handle message
Native ->> WebExt_Host:  Write response string to pipe
Note over WebExt_Host: Read message from pipe<br>And write message to StdIO
WebExt_Host ->> WebExt_SW: Send response to SeriveWorker
WebExt_SW ->> External: Send JSON response
end
```

# Build

## YouTubeAssist.sln
Open the `YouTubeAssist.sln` in visual studio and build the solution

### YoutubeAssist WPF
This project need windows 11 SDK(10.0.26100.0), which can be installed from visual studio installer -> Individual components
* Please create a `App.Secrets.config` file at the same level of the `App.config`, the content is
  ```
  <appSettings>
    <add key="ApiKey" value="Youtube data API key" />
  </appSettings>
  ```

### ClientHost console
This app will use a `post-build.bat` file to write registry key according to this [browser extension nativeMessaging documentation](https://developer.chrome.com/docs/extensions/develop/concepts/native-messaging#native-messaging-host-location)

## Web
* Install Nodejs 20.10.0+
* Install yarn 4.5 according to [installation doc](https://yarnpkg.com/getting-started/install)
* Go to the `Web` directory, run these command
  - `yarn install`
  - `yarn build-ext` #build brower extension
  - `yarn build-app` #build web app

The browser extension and web app will be in the `build` folder, then load the extension from chrome://extensions/ developer mode

# Usage
* Build the ClientHost app, YoutubeAssist WPF, WebExtension, WebApp
* load WebExtension in Chrome/Edge, and run `yarn start-server` under the `Web` directory
* Launch the YoutubeAssist WPF
* Open browser extension popup, a port would be created by using `chrome.runtime.connectNative` and connect to the WPF.
  - In windows task manager, a `ClientHost` process will also be created
* Send messages between WPF and browser extension
* Click the `Authenticate by Windows Hello` button on popup, windows hello challenge will prompt
* Open a new tab and go to `http://localhost:3000`, click the Click the `Authenticate by Windows Hello` button on the web page, windows hello challenge will prompt

## Demo

### WebExt Popup messaging and authentication with Windows Hello
  <img src="Web/demo/message_passing_win_hello_auth_WebExtPopup.png"
    width="600" height="auto"
    title="Messaging and authentication with windows hello on extension popup"
    alt="Messaging and authentication with windows hello on extension popup"/>

  <img src="Web/demo/win_hello_success_WebExtPopup.png"
    width="600" height="auto"
    title="Windows hello authentication success on extension popup"
    alt="Windows hello authentication success on extension popup"/>

### WebApp authentication with Windows Hello
  <img src="Web/demo/message_passing_win_hello_auth_WebApp.png"
    width="600" height="auto"
    title="Authentication with windows hello on web app"
    alt="Authentication with windows hello on web app"/>

  <img src="Web/demo/win_hello_success_WebApp.png"
    width="600" height="auto"
    title="Windows hello authentication success on web app"
    alt="Windows hello authentication success on web app"/>