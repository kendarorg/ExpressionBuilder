@echo off >NUL 2>NUL

if "%VS_VERSION%"=="" (
    echo ERROR!!!
    echo This batch cannot be run alone!!
    pause
    exit 1
)  

echo Cleanup
echo ================================================================
echo * Cleanup started
call cleanup
del /q cleanup.bat >NUL 2>NUL
echo * Cleanup completed
echo.