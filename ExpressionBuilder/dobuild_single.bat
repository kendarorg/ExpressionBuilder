SET PROJECT_NAME=%1
SET FRAMEWORK_VERSION=%2
SET FRAMEWORK_NUGET_VERSION=%3
SET PROJECT_DIR=%1
if "%4"=="" (SET PROJECT_DIR=%1) ELSE (SET PROJECT_DIR=%4)

mkdir tmp_nuget\%PROJECT_NAME%
mkdir tmp_nuget\%PROJECT_NAME%\bin
mkdir tmp_nuget\%PROJECT_NAME%\bin\%FRAMEWORK_NUGET_VERSION%

del /q %PROJECT_DIR%\obj\Release\*.*

msbuild %PROJECT_DIR%/%PROJECT_NAME%.csproj /p:TargetFrameworkVersion=v%FRAMEWORK_VERSION%;Configuration=Release;Platform=AnyCPU
copy /Y %PROJECT_DIR%\bin\Release\*.dll tmp_nuget\%PROJECT_NAME%\bin\%FRAMEWORK_NUGET_VERSION%
copy /Y %PROJECT_DIR%\bin\Release\*.exe tmp_nuget\%PROJECT_NAME%\bin\%FRAMEWORK_NUGET_VERSION%
copy /Y %PROJECT_DIR%\bin\Release\*.config tmp_nuget\%PROJECT_NAME%\bin\%FRAMEWORK_NUGET_VERSION%
copy /Y %PROJECT_DIR%\bin\Release\*.txt tmp_nuget\%PROJECT_NAME%\bin\%FRAMEWORK_NUGET_VERSION%
copy /Y %PROJECT_DIR%\bin\Release\*.md tmp_nuget\%PROJECT_NAME%\bin\%FRAMEWORK_NUGET_VERSION%