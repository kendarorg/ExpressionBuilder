@echo off >NUL 2>NUL

if "%VS_VERSION%"=="" (
    echo ERROR!!!
    echo This batch cannot be run alone!!
    pause
    exit 1
)  

SET PROJECT_NAME=%1
SET FRAMEWORK_VERSION=%2
SET FRAMEWORK_NUGET_VERSION=%3
SET PROJECT_DIR=%SOLUTION_DIR%\%1

echo Building: %1/.NET %2 with nuget
echo ================================================================

if "%4"=="" (SET PROJECT_DIR=%SOLUTION_DIR%\%1) ELSE (SET PROJECT_DIR=%SOLUTION_DIR%\%4)

if "%VERBOSITY%"=="TRUE" ( 
    echo * Cleaning up build directories for %PROJECT_NAME%...
    rd /s /q "%PROJECT_DIR%\obj"  
    rd /s /q "%PROJECT_DIR%\bin" 
) else ( 
    echo * Cleaning up build directories for %PROJECT_NAME%... 
    rd /s /q "%PROJECT_DIR%\obj"  >NUL 2>NUL 
    rd /s /q "%PROJECT_DIR%\bin"  >NUL 2>NUL 
)

echo if "%VERBOSITY%"=="TRUE" ( >>cleanup.bat
echo     echo * Not Cleaning up build directories for %PROJECT_NAME%/.Net %FRAMEWORK_VERSION%... >>cleanup.bat
echo ) else ( >>cleanup.bat
echo     rd /s /q "%PROJECT_DIR%\obj"  ^>NUL 2^>NUL >>cleanup.bat
echo     rd /s /q "%PROJECT_DIR%\bin"  ^>NUL 2^>NUL >>cleanup.bat
echo ) >>cleanup.bat

echo * Compiling...


if "%VERBOSITY%"=="TRUE" (
    msbuild %PROJECT_DIR%/%PROJECT_NAME%.csproj /verbosity:n /p:TargetFrameworkVersion=v%FRAMEWORK_VERSION%;Configuration=Release;Platform=AnyCPU /p:DefineConstants=%FRAMEWORK_NUGET_VERSION%
) ELSE (
    msbuild %PROJECT_DIR%/%PROJECT_NAME%.csproj /verbosity:q /p:TargetFrameworkVersion=v%FRAMEWORK_VERSION%;Configuration=Release;Platform=AnyCPU /p:DefineConstants=%FRAMEWORK_NUGET_VERSION%  >NUL 2>NUL
)

echo * Compilation completed
echo.