@echo off

echo Setting copyright on solution
echo ================================================================
SET UTILS_ROOT=%CD%
cd ..
call "%UTILS_ROOT%\CommentsHeader" -e cs -t "%UTILS_ROOT%\license.cs"
echo * Copyright set
echo.
pause