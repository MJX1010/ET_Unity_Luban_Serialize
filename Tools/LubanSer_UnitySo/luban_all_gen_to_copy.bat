set WORKSPACE=.
set LUBAN_DLL=%WORKSPACE%\LubanDlls\Luban.dll
set DOTNET_EXE=%WORKSPACE%\Env\dotnet-runtime-8.0.10-win-x64\dotnet.exe
set CONF_ROOT=.
set OUTPUT_DATA=..\..\Unity\Assets\Bundles\Config\TableData\

echo Luban dll to generate csharp / bytes ...

%DOTNET_EXE% %LUBAN_DLL% ^
    -t client ^
    -c cs-bin ^
    -d bin ^
    --conf %CONF_ROOT%\luban_data.conf ^
    -x outputDataDir=%CONF_ROOT%\Generated\Data\Bytes ^
    -x outputCodeDir=%CONF_ROOT%\Generated\Code\CSharp

echo Luban dll to generate json / json ...

%DOTNET_EXE% %LUBAN_DLL% ^
    -t client ^
    -c cs-simple-json ^
    -d json ^
    --conf %CONF_ROOT%\luban_data.conf ^
    -x outputDataDir=%CONF_ROOT%\Generated\Data\Json ^
    -x outputCodeDir=%CONF_ROOT%\Generated\Code\Json

pause
