@echo off
setlocal EnableDelayedExpansion

REM Get parameters from user if they're not specified
REM if [%1]==[] set /P directory="Enter name of directory/site: "
REM if [%2]==[] set /P port="Enter port number: "
REM if [%3]==[] set /P sitenum="Enter site number: "

SET APPCMD=%WINDIR%\system32\inetsrv\appcmd.exe

@ECHO Cria o pool para o server 
%APPCMD% ADD APPPOOL /NAME:"M2M_Server" /managedPipelineMode:Integrated /managedRuntimeVersion:v4.0

@ECHO Cria o pool para o server 
%APPCMD% ADD APPPOOL /NAME:"M2M_Client" /managedPipelineMode:Integrated  /managedRuntimeVersion:v4.0

@ECHO Seta o APPLICATION POOL 
%APPCMD% SET APP "M2M_Server" /APPLICATIONPOOL:"M2M_Server"

REM Cria o site server-side WebAPI no IIS
%systemroot%\system32\inetsrv\appcmd add site /name:"M2M_Server" /id:88 /physicalPath:"C:\CompraCredito\PublishWebAPI" /bindings:http/*:50970:localhost

REM Cria o site client-side Www no IIS
%systemroot%\system32\inetsrv\appcmd add site /name:"M2M_Client" /id:89 /physicalPath:"C:\CompraCredito\PublishWww" /bindings:http/*:49575:localhost

REM Tenta migrar config IIS7 
%SystemRoot%\system32\inetsrv\appcmd migrate config "M2M_Server"

REM Tenta migrar config IIS7 
%SystemRoot%\system32\inetsrv\appcmd migrate config "M2M_Cliente"

REM Start no server-side
%SystemRoot%\system32\inetsrv\appcmd start site "M2M_Server"

REM Start no client-side
%SystemRoot%\system32\inetsrv\appcmd start site "M2M_Client"

echo site "M2M_Server" now running at http://localhost:50970

echo site "M2M_Client" now running at http://localhost:49575

REM interactive mode
if [%1]==[] (if [%2]==[] (if [%3]==[] (
    pause
    exit
)))