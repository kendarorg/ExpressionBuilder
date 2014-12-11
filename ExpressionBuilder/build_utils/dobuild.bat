@echo off
call dobuild_env.bat

call dobuild_single ExpressionBuilder 4.0 net40
call dobuild_single ExpressionBuilder 4.5 net45
call dobuild_nuget ExpressionBuilder

call dobuild_clean
pause
