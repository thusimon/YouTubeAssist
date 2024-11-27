@echo off
set BuildName=%1
set OutputDir=%2

echo running post build tasks

set OutputPath=%~dp0%OutputDir%%BuildName%.exe

set name=com.utticus.youtube.assist.host
set description=browser extension native host
set hostpath=%OutputPath:\=\\%
set uuid=kpkilehcpjaeglkmjmodnbijmcphnmfm
set HostConfigFile=%~dp0%OutputDir%%name%.json
rem Create host config json file
> "%HostConfigFile%" (
    echo {
    echo   "name": "%name%",
    echo   "description": "%description%",
    echo   "path": "%hostpath%",
    echo   "type": "stdio",
    echo   "allowed_origins": ["chrome-extension://%uuid%/"]
    echo }
)
echo The host config file has been written to %HostConfigFile%

%SystemRoot%\System32\reg.exe ADD "HKEY_CURRENT_USER\Software\Google\Chrome\NativeMessagingHosts\%name%" /ve /t REG_SZ /d "%HostConfigFile%" /f
%SystemRoot%\System32\reg.exe ADD "HKEY_CURRENT_USER\Software\Microsoft\Edge\NativeMessagingHosts\%name%" /ve /t REG_SZ /d "%HostConfigFile%" /f
echo HKEY_CURRENT_USER registry has been updated
