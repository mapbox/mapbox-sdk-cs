@ECHO OFF
SETLOCAL
SET EL=0
ECHO ~~~~~~~~~~~~~~~~~~~ %~f0 ~~~~~~~~~~~~~~~~~~~

SET ROOTDIR=%CD%
SET PATH=C:\Program Files (x86)\MSBuild\14.0\Bin;%PATH%
SET PATH=%CD%\scriptcs;%PATH%
SET PATH=%CD%\docfx;%PATH%

FOR /F "tokens=*" %%i in ('powershell Get-ExecutionPolicy') do SET PSPOLICY=%%i
ECHO Powershell execution policy^: %PSPOLICY%
IF NOT "%PSPOLICY%"=="Unrestricted" powershell Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy Unrestricted -Force
IF %ERRORLEVEL% NEQ 0 ECHO could not set poowershell execution policy GOTO ERROR
FOR /F "tokens=*" %%i in ('powershell Get-ExecutionPolicy') do SET PSPOLICY=%%i
ECHO Powershell execution policy^: %PSPOLICY%

::install scriptcs
IF NOT EXIST scriptcs (powershell .\scripts\get-scriptcs.ps1) ELSE (ECHO scriptcs already downloaded)
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
WHERE scriptcs >NUL
IF %ERRORLEVEL% NEQ 0 ECHO scriptcs not found && GOTO ERROR

scriptcs %ROOTDIR%\scripts\build.csx
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

GOTO DONE

:ERROR
SET EL=%ERRORLEVEL%
ECHO ~~~~~~~~~~~~~~~~~~~ ERROR %~f0 ~~~~~~~~~~~~~~~~~~~
ECHO ERRORLEVEL^: %EL%

:DONE
ECHO ~~~~~~~~~~~~~~~~~~~ DONE %~f0 ~~~~~~~~~~~~~~~~~~~

EXIT /b %EL%