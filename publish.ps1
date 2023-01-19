param(
    [Parameter(Mandatory)]
    [ValidateSet('Debug','Release')]
    [System.String]$Target,
    
    [Parameter(Mandatory)]
    [System.String]$TargetPath,
    
    [Parameter(Mandatory)]
    [System.String]$TargetAssembly,

    [Parameter(Mandatory)]
    [System.String]$ValheimPath,

    [Parameter(Mandatory)]
    [System.String]$ProjectPath
)

# Make sure Get-Location is the script path
Push-Location -Path (Split-Path -Parent $MyInvocation.MyCommand.Path)

# Main Script
Write-Host "Publishing for $Target from $TargetPath"

if ($Target.Equals("Debug")) 
{
    Write-Host "Updating local installation in $ValheimPath"
    $name = "OhhLoz-$TargetAssembly" -Replace('.dll')

    $plug = New-Item -Type Directory -Path "$ValheimPath\BepInEx\plugins\$name" -Force
    Write-Host "Copy $TargetAssembly to $plug"
    Copy-Item -Path "$TargetPath\$TargetAssembly" -Destination "$plug" -Force

    # Set dnspy debugger env - after a relog in Windows mono runtime listens on port 56000 instead of 55555
    #$dnspy = '--debugger-agent=transport=dt_socket,server=y,address=127.0.0.1:56000,suspend=n,no-hide-debugger'
    #[Environment]::SetEnvironmentVariable('DNSPY_UNITY_DBG2',$dnspy,'User')
}

if($Target.Equals("Release")) 
{
    Write-Host "Packaging for ThunderStore..."
    $PackagePath="$ProjectPath\Package"

    Write-Host "$PackagePath\OhhLoz-$TargetAssembly"
    Copy-Item -Path "$TargetPath\$TargetAssembly" -Destination "$PackagePath\plugins\$TargetAssembly"
    Copy-Item -Path "$PackagePath\README.md" -Destination "$ProjectPath\README.md"
    Compress-Archive -Path "$PackagePath\*" -DestinationPath "$TargetPath\$TargetAssembly.zip" -Force
}

# Pop Location
Pop-Location