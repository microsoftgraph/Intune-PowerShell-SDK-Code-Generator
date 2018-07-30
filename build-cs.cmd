
rem Initialize the developer environment just like a developer box. Note that 'call' keyword that ensures that the script does not exist after 
rem calling the other batch file.
call "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\Tools\VsDevCmd.bat" -arch=amd64 -host_arch=amd64 -winsdk=10.0.16299.0

rem Run self-check on the environment that was initialized by the above.
call "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\Tools\VsDevCmd.bat" -arch=amd64 -host_arch=amd64 -winsdk=10.0.16299.0 -test

rem Save working directory so that we can restore it back after building everything. This will make developers happy and then 
rem switch to the folder this script resides in. Don't assume absolute paths because on the build host and on the dev host the locations may be different.
pushd "%~dp0"

rem Initialize.
%~dp0init.cmd

rem Save exit code for dotnet restore.
set EX=%ERRORLEVEL%

rem Check exit code and exit with non-zero exit code so that build will fail.
if "%EX%" neq "0" (
    popd
    echo "Initialization failed."
	exit /b %EX%
)

rem Restore working directory of user so this works fine in dev box.
popd

rem Exit with explicit 0 code so that build does not fail.
exit /B %EX%
