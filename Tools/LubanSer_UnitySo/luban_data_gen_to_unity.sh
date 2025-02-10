#!/bin/bash

WORKSPACE=$(cd "$(dirname "$0")" && pwd)

LUBAN_DLL="$WORKSPACE/LubanDlls/Luban.dll"
DOTNET_EXE="$WORKSPACE/Env/dotnet-runtime-8.0.11-osx-x64/dotnet"
CONF_ROOT="$WORKSPACE"
OUTPUT_DATA="$WORKSPACE/../../Unity/Assets/Bundles/Config/TableData/"

if [[ ! -f "$LUBAN_DLL" ]]; then
    echo "Error: Luban.dll not found at $LUBAN_DLL"
    exit 1
fi

if [[ ! -f "$DOTNET_EXE" ]]; then
    echo "Error: Luban.dll not found at $DOTNET_EXE"
    exit 1
fi

"$DOTNET_EXE" "$LUBAN_DLL" \
    -t client \
    -c cs-bin \
    -d bin \
    --conf "$CONF_ROOT/luban_data.conf" \
    -x outputDataDir="$OUTPUT_DATA" \
    -x outputCodeDir="$CONF_ROOT/Generated/Code/CSharp"