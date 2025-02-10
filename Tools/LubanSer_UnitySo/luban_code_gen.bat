set WORKSPACE=.
set LUBAN_DLL=%WORKSPACE%\LubanDlls\Luban.dll
set DOTNET_EXE=%WORKSPACE%\Env\dotnet-runtime-8.0.10-win-x64\dotnet.exe
set CONF_ROOT=.
set OUTPUT_DATA=..\..\Unity\Assets\Bundles\Config\TableData\

%DOTNET_EXE% %LUBAN_DLL% ^
    -t client ^
    -c cs-bin ^
    --conf %CONF_ROOT%\luban_code.conf ^
    -x outputCodeDir=%CONF_ROOT%\Generated\Code\CSharp

pause
