#!/bin/bash

DLL="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )/Sage.dll"
dotnet $DLL "$@"