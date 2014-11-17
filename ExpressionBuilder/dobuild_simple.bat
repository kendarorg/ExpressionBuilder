SET PROJECT_NAME=%1
SET FRAMEWORK_VERSION=%2
SET FRAMEWORK_NUGET_VERSION=%3

del /q %PROJECT_DIR%\obj\Release\*.*

msbuild %PROJECT_NAME%/%PROJECT_NAME%.csproj /p:TargetFrameworkVersion=v%FRAMEWORK_VERSION%;Configuration=Release;Platform=AnyCPU