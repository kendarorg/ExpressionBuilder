SET CURRENT_DIR=%CD%
SET NUGET_DIR=%CURRENT_DIR%\.nuget
VisualStudioIdentifier 12.0 VS2013 vs.bat
call vs.bat

cd %VS2013%
cd..
SET VS2013=%CD%
CD %CURRENT_DIR%

call "%VS2013%\Tools\VsDevCmd.bat"

mkdir tmp_nuget