@echo off

REM Xoá Service
set path=%~dp0
C:\Windows\Microsoft.NET\Framework\v4.0.30319\installutil.exe -u "%path%SVC207Knowledge.exe"

REM Cài Service
C:\Windows\Microsoft.NET\Framework\v4.0.30319\installutil.exe "%path%SVC207Knowledge.exe"

REM Cấu hình service Automatic
C:\WINDOWS\system32\sc.exe config KnowledgeNotifyService start=auto
if %errorlevel% == 2 echo Could not start service.
if %errorlevel% == 0 echo Service started successfully.
echo Errorlevel: %errorlevel%

REM Chạy Service
C:\WINDOWS\system32\net.exe start KnowledgeNotifyService
if %errorlevel% == 2 echo Could not start service.
if %errorlevel% == 0 echo Service started successfully.
echo Errorlevel: %errorlevel%

pause