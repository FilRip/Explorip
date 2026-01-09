@echo off
cd %1
echo.
echo Auto update will start automatically in 30sec (or press enter when you want, or Ctrl+C to stop now)
echo Do not forget to close ALL Explorip program :
echo - Explorip.exe,
echo - ExploripCopy.exe (in systray, the copy/paste interceptor program, if launched)
echo Before auto update start
timeout /t 30
move /Y *.* ../
cd ..
rmdir /S /Q %1
echo.
echo If no error(s) displayed, you can relaunch/start Explorip