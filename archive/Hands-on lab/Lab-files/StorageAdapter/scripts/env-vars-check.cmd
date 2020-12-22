:: Copyright (c) Microsoft. All rights reserved.

@ECHO off & setlocal enableextensions enabledelayedexpansion

IF "%PCS_KEYVAULT_NAME%" == "" (
    echo Error: the PCS_KEYVAULT_NAME environment variable is not defined.
    exit /B 1
)

IF "%PCS_AAD_APPID%" == "" (
    echo Error: the PCS_AAD_APPID environment variable is not defined.
    exit /B 1
)

IF "%PCS_AAD_APPSECRET%" == "" (
    echo Error: the PCS_AAD_APPSECRET environment variable is not defined.
    exit /B 1
)

endlocal
