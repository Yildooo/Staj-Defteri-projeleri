@echo off
chcp 65001 >nul
cd /d "%~dp0"

set "PHP=C:\Users\yilmazdemircioglu\AppData\Local\Microsoft\WinGet\Packages\PHP.PHP.8.3_Microsoft.Winget.Source_8wekyb3d8bbwe\php.exe"

if not exist "%PHP%" (
    where php >nul 2>&1 && set "PHP=php"
)
if not exist "%PHP%" if exist "C:\xampp\php\php.exe" set "PHP=C:\xampp\php\php.exe"

if not exist "%PHP%" (
    echo PHP bulunamadi.
    pause
    exit /b 1
)

echo PHP: %PHP%
echo.
echo Tarayicida acin: http://localhost:8000
echo Durdurmak icin Ctrl+C
echo.
"%PHP%" -S localhost:8000
