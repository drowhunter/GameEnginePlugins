REM SETLOCAL
ECHO OFF
SET GEFOLDER=E:\_Apps\GameEngine_2_6\Gameplugins
SET PLUGIN=GT7Plugin
SET VERSION=0.9.0.0
SET OUTFILE=%PLUGIN%_%VERSION%.dll

CD /D "%~dp0\%PLUGIN%"
ILRepack /out:..\%OUTFILE% %PLUGIN%.dll PDTools.Crypto.dll PDTools.Hashing.dll PDTools.SimulatorInterface.dll Syroot.BinaryData.dll Syroot.BinaryData.Core.dll Syroot.BinaryData.Memory.dll System.Buffers.dll System.Memory.dll System.Numerics.Vectors.dll System.Runtime.CompilerServices.Unsafe.dll

ECHO Installing to GameEngine folder...
XCOPY /Y ..\%OUTFILE% %GEFOLDER%
