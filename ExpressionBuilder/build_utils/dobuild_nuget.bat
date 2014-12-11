@echo off >NUL 2>NUL

if "%VS_VERSION%"=="" (
    echo ERROR!!!
    echo This batch cannot be run alone!!
    pause
    exit 1
)    

SET PROJECT_NAME=%1
SET PROJECT_DIR=%SOLUTION_DIR%\%1
if "%2"=="" (SET PROJECT_DIR=%SOLUTION_DIR%\%1) ELSE (SET PROJECT_DIR=%SOLUTION_DIR%\%2)

echo Building nuget package for: %1
echo ================================================================

SET NUGET_VERBOSITY=quiet
if "%VERBOSITY%"=="TRUE" (SET NUGET_VERBOSITY=detailed) ELSE (SET NUGET_VERBOSITY=quiet)

"%NUGET_DIR%\NuGet.exe" pack "%PROJECT_DIR%\%PROJECT_NAME%.nuspec" -Verbosity %NUGET_VERBOSITY% -basepath "%SOLUTION_DIR%\tmp_nuget\%PROJECT_NAME%" -OutputDirectory "%SOLUTION_DIR%\tmp_nuget"

echo if "%VERBOSITY%"=="TRUE" ( >>cleanup.bat
echo     echo * NOT Cleaning up temporary nuget for %PROJECT_NAME% >>cleanup.bat
echo ) else ( >>cleanup.bat
echo     rd /q /s "%SOLUTION_DIR%\tmp_nuget\%PROJECT_NAME%" ^>NUL 2^>NUL  >>cleanup.bat
echo ) >>cleanup.bat

echo * Nuget package created
echo.