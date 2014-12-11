SET UTILS_ROOT=%CD%
cd ..
%UTILS_ROOT%\BuildCleaner -dp _ReSharper*.* -ip bin;obj;.git;packages;.nuget;TestResults; -z
pause