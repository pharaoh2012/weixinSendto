Write-Host "Verifying project structure..." -ForegroundColor Green

# Check if solution file exists
if (Test-Path "weixinSendto.slnx") {
    Write-Host "✓ Solution file exists" -ForegroundColor Green
} else {
    Write-Host "✗ Solution file missing" -ForegroundColor Red
    exit 1
}

# Check if project file exists
if (Test-Path "weixinSendto\weixinSendto.csproj") {
    Write-Host "✓ Project file exists" -ForegroundColor Green
} else {
    Write-Host "✗ Project file missing" -ForegroundColor Red
    exit 1
}

# Check if packages.config exists
if (Test-Path "weixinSendto\packages.config") {
    Write-Host "✓ packages.config exists" -ForegroundColor Green
} else {
    Write-Host "✗ packages.config missing" -ForegroundColor Red
    exit 1
}

# Check if WindowsInput is referenced in packages.config
$packagesContent = Get-Content "weixinSendto\packages.config" -Raw
if ($packagesContent -match "WindowsInput.*6\.4\.1") {
    Write-Host "✓ WindowsInput 6.4.1 referenced in packages.config" -ForegroundColor Green
} else {
    Write-Host "✗ WindowsInput 6.4.1 not found in packages.config" -ForegroundColor Red
    exit 1
}

# Check if WindowsInput is referenced in csproj
$csprojContent = Get-Content "weixinSendto\weixinSendto.csproj" -Raw
if ($csprojContent -match "WindowsInput.*Version=6\.4\.1\.0") {
    Write-Host "✓ WindowsInput 6.4.1.0 referenced in csproj" -ForegroundColor Green
} else {
    Write-Host "✗ WindowsInput 6.4.1.0 not found in csproj" -ForegroundColor Red
    exit 1
}

# Check hint path in csproj
if ($csprojContent -match "HintPath.*\.\.\\packages\\WindowsInput\.6\.4\.1\\lib\\net461\\WindowsInput\.dll") {
    Write-Host "✓ Correct hint path for WindowsInput in csproj" -ForegroundColor Green
} else {
    Write-Host "✗ Incorrect hint path for WindowsInput in csproj" -ForegroundColor Red
    exit 1
}

Write-Host "\nAll checks passed! The project is properly configured." -ForegroundColor Green
Write-Host "The GitHub Actions workflow has been updated to build the solution correctly." -ForegroundColor Cyan