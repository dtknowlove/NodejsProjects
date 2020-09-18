@echo off & setlocal enabledelayedexpansion 
set DIR=%~dp0
cd %DIR%

::变量后面不要加多余空格
set ProjectName=%1
set CopyCount=%2

if %CopyCount% LSS 1 echo "copy count can't less than 1"
set root_src=%DIR%%ProjectName%
set root_src_Library=%root_src%\Library

for /l %%J in (1,1,%CopyCount%) do (
    ::link Assets ProjectSettings
    set root_copy=%root_src%_%%J
    if not exist !root_copy! mkdir !root_copy!
    for %%i in (Assets,ProjectSettings) do (
        if not exist !root_copy!\%%i mklink /J !root_copy!\%%i %root_src%\%%i
    )
    ::link libaray
    set root_copy_Library=!root_copy!\Library
    if not exist !root_copy_Library! mkdir !root_copy_Library!
    cd %root_src_Library%
    :: link libaray floder
    for %%i in (AtlasCache,metadata,PackageCache,ScriptAssemblies,ShaderCache,StateCache) do (
        if not exist !root_copy_Library!\%%i mklink /J !root_copy_Library!\%%i %root_src_Library%\%%i
    )
    ::link libaray file
    for %%i in (*.*) do (
        if not exist !root_copy_Library!\%%i ( 
            if not %%i == EditorInstance.json mklink !root_copy_Library!\%%i %root_src_Library%\%%i
        )
    )
    cd %DIR%
)

