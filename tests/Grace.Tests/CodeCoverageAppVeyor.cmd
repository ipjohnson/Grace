@echo off

SET dotnet="C:/Program Files/dotnet/dotnet.exe"  
SET opencover=..\..\tools\OpenCover.4.6.519\tools\OpenCover.Console.exe  
SET coveralls=..\..\tools\coveralls.net.0.412\tools\csmacnz.Coveralls.exe

SET targetargs="test"  
SET filter="+[*]Grace.* -[Grace.Tests]* -[xunit.*]* "  
SET coveragefile=Coverage.xml  
SET coveragedir=Coverage

nuget install OpenCover -Version 4.6.519 -OutputDirectory ..\..\tools
nuget install coveralls.net -Version 0.412.0 -OutputDirectory ..\..\tools

REM Run code coverage analysis  
set EXIT_CODE=
%opencover% -oldStyle -returntargetcode -register:user -target:%dotnet% -output:%coveragefile% -targetargs:%targetargs% -filter:%filter% -skipautoprops -hideskipped:All


REM publish
IF NOT "%COVERALLS_REPO_TOKEN%"=="" %coveralls% --serviceName appveyor --opencover -i .\Coverage.xml

exit /b %EXIT_CODE%