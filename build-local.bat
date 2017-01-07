@ECHO OFF
SETLOCAL
SET EL=0
ECHO ~~~~~~~~~~~~~~~~~~~ %~f0 ~~~~~~~~~~~~~~~~~~~

REM env vars that get set in appveyor.yml on remote builds
SET configuration=Debug
SET APPVEYOR_REPO_COMMIT_MESSAGE=local build


:::::::::::::: OVERRIDE PARAMETERS
:NEXT-ARG

IF '%1'=='' GOTO ARGS-DONE
ECHO setting %1
SET %1
SHIFT
GOTO NEXT-ARG

:ARGS-DONE
:::::::::::::: OVERRIDE PARAMETERS



SET PATH=C:\Program Files\7-Zip;%PATH%

IF DEFINED NUGET_API_KEY GOTO DOBUILD
ECHO !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
ECHO !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
ECHO no NUGET_API_KEY defined - cannot publish to nuget.org
ECHO !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
ECHO !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

:DOBUILD
IF EXIST src\Documentation\_site RD /Q /S src\Documentation\_site
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
nuget restore
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
CALL build-appveyor.bat
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

GOTO DONE

:ERROR
SET EL=%ERRORLEVEL%
ECHO ~~~~~~~~~~~~~~~~~~~ ERROR %~f0 ~~~~~~~~~~~~~~~~~~~
ECHO ERRORLEVEL^: %EL%

:DONE
ECHO ~~~~~~~~~~~~~~~~~~~ DONE %~f0 ~~~~~~~~~~~~~~~~~~~

EXIT /b %EL%