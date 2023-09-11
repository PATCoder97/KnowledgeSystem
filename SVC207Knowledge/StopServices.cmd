@echo off
REM Dừng Service
C:\WINDOWS\system32\net.exe stop KnowledgeNotifyService
if %errorlevel% == 2 echo Could not start service.
if %errorlevel% == 0 echo Service started successfully.
echo Errorlevel: %errorlevel%

pause