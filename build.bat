@echo off
cd /d "%~dp0"
echo Compilazione in corso...

set CSC=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe

if not exist "%CSC%" (
    set CSC=C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe
)

if not exist "%CSC%" (
    echo ERRORE: compilatore C# non trovato.
    pause
    exit /b 1
)

"%CSC%" /nologo /out:WifiPasswords.exe WifiPasswords.cs

if %ERRORLEVEL% == 0 (
    echo.
    echo OK! File creato: WifiPasswords.exe
) else (
    echo.
    echo ERRORE durante la compilazione.
)

pause
