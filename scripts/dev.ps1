# Build and launch Commander Markers in Blish HUD using Properties/launchSettings.json.
#
# Usage:
#   .\scripts\dev.ps1
#   .\scripts\dev.ps1 -Profile powershell
#   .\scripts\dev.ps1 -Profile gw2 -Configuration Debug
param(
    [ValidateSet('gw2', 'powershell', 'mumblelink1', 'prerelease_test')]
    [string]$Profile = 'gw2',
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Debug',
    [switch]$NoBuild
)

$ErrorActionPreference = 'Stop'

$Root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$LaunchSettingsPath = Join-Path $Root 'Properties/launchSettings.json'
$ProjectFile = Join-Path $Root 'CommanderMarkers.csproj'

if (-not (Test-Path -LiteralPath $LaunchSettingsPath)) {
    Write-Error @"
launchSettings.json not found at $LaunchSettingsPath
Copy Properties/sample.launchSettings.json to Properties/launchSettings.json and edit paths for your GW2 install.
"@
    exit 1
}

$launch = Get-Content -LiteralPath $LaunchSettingsPath -Raw | ConvertFrom-Json
$profileConfig = $launch.profiles.$Profile
if (-not $profileConfig) {
    Write-Error "Profile '$Profile' not found in launchSettings.json"
    exit 1
}

$TargetName = 'CommanderMarkers'
$TargetFramework = 'net48'
$BaseOutputPath = 'bin\'
$ProjectDir = "$Root\"

function Expand-LaunchTokens {
    param([string]$Text)
    $Text = $Text -replace '\$\(ProjectDir\)', $ProjectDir
    $Text = $Text -replace '\$\(BaseOutputPath\)', $BaseOutputPath
    $Text = $Text -replace '\$\(Configuration\)', $Configuration
    $Text = $Text -replace '\$\(TargetFramework\)', $TargetFramework
    $Text = $Text -replace '\$\(TargetName\)', $TargetName
    return $Text
}

$blishExe = if ($env:BLISH_HUD_EXE) { $env:BLISH_HUD_EXE } else { $profileConfig.executablePath }
$blishDir = if ($env:BLISH_HUD_DIR) { $env:BLISH_HUD_DIR } else { $profileConfig.workingDirectory }

if (-not $blishExe -or -not (Test-Path -LiteralPath $blishExe)) {
    Write-Error "Blish HUD not found at '$blishExe'. Set BLISH_HUD_EXE or edit launchSettings.json."
    exit 1
}
if (-not $blishDir -or -not (Test-Path -LiteralPath $blishDir)) {
    Write-Error "Blish HUD working directory not found at '$blishDir'. Set BLISH_HUD_DIR or edit launchSettings.json."
    exit 1
}

if (-not $NoBuild) {
    Write-Host "-> Building $TargetName ($Configuration)..."
    & dotnet build $ProjectFile -c $Configuration --nologo -v minimal
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}

$modulePath = Join-Path $Root "$BaseOutputPath$Configuration\$TargetFramework\$TargetName.bhm"
$modulePath = [System.IO.Path]::GetFullPath($modulePath)
if (-not (Test-Path -LiteralPath $modulePath)) {
    Write-Error "Module not found at $modulePath (build may have failed)"
    exit 1
}

$commandLineArgs = Expand-LaunchTokens $profileConfig.commandLineArgs

Write-Host "-> Build:        $Configuration (Debug uses http://localhost:3000 for CM API)"
Write-Host "-> Blish HUD:    $blishExe"
Write-Host "-> Module:       $modulePath"
Write-Host "-> Working dir:  $blishDir"
Write-Host ''

$proc = Start-Process -FilePath $blishExe -ArgumentList $commandLineArgs -WorkingDirectory $blishDir -PassThru -Wait
exit $proc.ExitCode
