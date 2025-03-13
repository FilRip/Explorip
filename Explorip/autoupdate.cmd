@echo off
cd %1
timeout /t 3
move /Y *.* ../
cd ..
rmdir /S /Q %1
