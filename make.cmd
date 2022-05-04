@echo off

set pcbs_path=%~1
set pcbs_path_store=pcbs_path.txt

set net_version=v3.5
set "path=%windir%\Microsoft.NET\Framework64\%net_version%;%path%"


if "%pcbs_path%" neq "" (
    echo %pcbs_path% > %pcbs_path_store%
) else (
    set /p pcbs_path= < "%pcbs_path_store%"
)


:: csc is .NET's C# compiler
csc ^
    /reference:"%pcbs_path%\BepInEx\core\0Harmony.dll" ^
    /reference:"%pcbs_path%\BepInEx\core\BepInEx.dll" ^
    /reference:"%pcbs_path%\PCBS_Data\Managed\Assembly-CSharp-firstpass.dll" ^
    /reference:"%pcbs_path%\PCBS_Data\Managed\UnityEngine.CoreModule.dll" ^
    /reference:"%pcbs_path%\PCBS_Data\Managed\UnityEngine.dll" ^
    /target:library ^
    /out:OCCTColors.dll ^
    OCCTColors.cs


if %errorlevel% equ 0 (
    echo.
    echo Compilation successfull. You can now copy OCCTColors.dll into
    echo.
    echo.    PC Building Simulator\BepInEx\plugins
    echo.
) else (
    echo.
    echo Compilation failed. Pass the path to PCBS's installation directory as first
    echo argument to make.cmd. It will be stored in
    echo.
    echo.    %pcbs_path_store%
    echo.
    echo and used next time you run make.cmd without command line arguments.
    echo.
)
