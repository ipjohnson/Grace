
xcopy /y %1\nuget\output\* %1\source\Grace.Templates\packages\

chdir %1\output\Tools\

Grace.VSIXPackageCreater.exe %2 %1\source\Grace.Templates\ %1\output\Grace.Templates.vsix
