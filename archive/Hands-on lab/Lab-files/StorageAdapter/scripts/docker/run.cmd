:: Copyright (c) Microsoft. All rights reserved.

@ECHO off & setlocal enableextensions enabledelayedexpansion

:: Note: use lowercase names for the Docker images
SET DOCKER_IMAGE="azureiotpcs/pcs-storage-adapter-dotnet"

:: strlen("\scripts\docker\") => 16
SET APP_HOME=%~dp0
SET APP_HOME=%APP_HOME:~0,-16%
cd %APP_HOME%

:: Check dependencies
docker version > NUL 2>&1
IF %ERRORLEVEL% NEQ 0 GOTO MISSING_DOCKER

:: Check settings
call .\scripts\env-vars-check.cmd
IF %ERRORLEVEL% NEQ 0 GOTO FAIL

:: Start the application
echo Starting Storage Adapter ...
docker run -it -p 9022:9022 ^
    -e PCS_KEYVAULT_NAME ^
    -e PCS_AAD_APPID ^
    -e PCS_AAD_APPSECRET ^
    %DOCKER_IMAGE%:testing

:: - - - - - - - - - - - - - -
goto :END

:FAIL
    echo Command failed
    endlocal
    exit /B 1

:MISSING_DOCKER
    echo ERROR: 'docker' command not found.
    echo Install Docker and make sure the 'docker' command is in the PATH.
    echo Docker installation: https://www.docker.com/community-edition#/download
    exit /B 1

:END
endlocal
