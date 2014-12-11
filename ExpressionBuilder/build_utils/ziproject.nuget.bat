@echo off
echo Zipping solution
echo ================================================================
echo.
SET UTILS_ROOT=%CD%
cd ..
%UTILS_ROOT%\BuildCleaner -dp _ReSharper*.* -ip bin;obj;.git;TestResults -z
echo.
echo * Solution zipped with nuget packages
echo.
pause