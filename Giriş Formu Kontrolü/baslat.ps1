$php = "C:\Users\yilmazdemircioglu\AppData\Local\Microsoft\WinGet\Packages\PHP.PHP.8.3_Microsoft.Winget.Source_8wekyb3d8bbwe\php.exe"
Set-Location $PSScriptRoot

if (-not (Test-Path $php)) {
    $cmd = Get-Command php -ErrorAction SilentlyContinue
    if ($cmd) { $php = $cmd.Source }
}

if (-not (Test-Path $php)) {
    Write-Host "PHP bulunamadi." -ForegroundColor Red
    pause
    exit 1
}

Write-Host "PHP: $php"
Write-Host ""
Write-Host "Tarayicida acin: http://localhost:8000" -ForegroundColor Green
Write-Host "Durdurmak icin Ctrl+C"
Write-Host ""
& $php -S localhost:8000
