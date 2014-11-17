SET PROJECT_NAME=%1
SET PROJECT_DIR=%1
if "%2"=="" (SET PROJECT_DIR=%1) ELSE (SET PROJECT_DIR=%2)

"%NUGET_DIR%\NuGet.exe" pack "%PROJECT_DIR%/%PROJECT_NAME%.nuspec" -Verbosity detailed -basepath "tmp_nuget/%PROJECT_NAME%" -OutputDirectory tmp_nuget