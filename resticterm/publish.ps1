# recticterm : Powershell script for package generation
# Win x64 & Linux x64

# Fucntions
function Write-ZipUsing7Zip([string]$FilesToZip, [string]$ZipOutputFilePath, [string]$Password, [ValidateSet('7z','zip','gzip','bzip2','tar','iso','udf')][string]$CompressionType = 'zip', [switch]$HideWindow)
{
    # Look for the 7zip executable.
    $pathTo32Bit7Zip = "C:\Program Files (x86)\7-Zip\7z.exe"
    $pathTo64Bit7Zip = "C:\Program Files\7-Zip\7z.exe"
    $THIS_SCRIPTS_DIRECTORY = Split-Path $script:MyInvocation.MyCommand.Path
    $pathToStandAloneExe = Join-Path $THIS_SCRIPTS_DIRECTORY "7za.exe"
    if (Test-Path $pathTo64Bit7Zip) { $pathTo7ZipExe = $pathTo64Bit7Zip }
    elseif (Test-Path $pathTo32Bit7Zip) { $pathTo7ZipExe = $pathTo32Bit7Zip }
    elseif (Test-Path $pathToStandAloneExe) { $pathTo7ZipExe = $pathToStandAloneExe }
    else { throw "Could not find the 7-zip executable." }

    # Delete the destination zip file if it already exists (i.e. overwrite it).
    if (Test-Path $ZipOutputFilePath) { Remove-Item $ZipOutputFilePath -Force }

    $windowStyle = "Normal"
    if ($HideWindow) { $windowStyle = "Hidden" }

    # Create the arguments to use to zip up the files.
    # Command-line argument syntax can be found at: http://www.dotnetperls.com/7-zip-examples
    # -mx9 for extra compression
    $arguments = "a -t$CompressionType ""$ZipOutputFilePath"" ""$FilesToZip"" "
    if (!([string]::IsNullOrEmpty($Password))) { $arguments += " -p$Password" }

    # Zip up the files.
    $p = Start-Process $pathTo7ZipExe -ArgumentList $arguments -Wait -PassThru -WindowStyle $windowStyle

    # If the files were not zipped successfully.
    if (!(($p.HasExited -eq $true) -and ($p.ExitCode -eq 0)))
    {
        throw "There was a problem creating the zip file '$ZipFilePath'."
    }
}


### Publish : Windows

# Folders
if (-not (Test-Path -Path ".\bin\Publish" -PathType Container))
{
    New-Item -Path ".\bin\Publish" -ItemType Directory -ErrorAction Stop
}
if (-not (Test-Path -Path ".\bin\Publish\Win" -PathType Container))
{
    New-Item -Path ".\bin\Publish\Win" -ItemType Directory -ErrorAction Stop
}
dotnet publish resticterm.csproj -r win-x64 /p:PublishSingleFile=true -o bin/Publish/Win --self-contained --configuration Release --verbosity n

# Create ZIP
Write-ZipUsing7Zip "./bin/Publish/Win/*"  -ZipOutputFilePath "./bin/Publish/resticterm_Win_x64.zip" 

# Version
$ver=(Get-Command "./bin/Publish/Win/resticterm.exe").FileVersionInfo.FileVersion
if(Test-Path -Path "./bin/Publish/resticterm_Win_x64_V$ver.zip" -PathType Leaf)
{
    Remove-Item -Path "./bin/Publish/resticterm_Win_x64_V$ver.zip" 
}
Rename-Item -Path "./bin/Publish/resticterm_Win_x64.zip" -NewName "resticterm_Win_x64_V$ver.zip" 


### Linux

# Folders
if (-not (Test-Path -Path ".\bin\Publish\Linux" -PathType Container))
{
    New-Item -Path ".\bin\Publish\Linux" -ItemType Directory -ErrorAction Stop
}
dotnet publish resticterm.csproj -r linux-x64 /p:PublishSingleFile=false -o bin/Publish/Linux --self-contained --configuration Release --verbosity n
#dotnet publish resticterm.csproj -os linux /p:PublishSingleFile=true -o bin/Publish --self-contained --configuration Release --verbosity n

# Create ZIP
Write-ZipUsing7Zip "./bin/Publish/Linux/*"  -ZipOutputFilePath "./bin/Publish/resticterm_Linux_x64.zip" 

# Version
if(Test-Path -Path "./bin/Publish/resticterm_Linux_x64_V$ver.zip" -PathType Leaf)
{
    Remove-Item -Path "./bin/Publish/resticterm_Linux_x64_V$ver.zip" 
}
Rename-Item -Path "./bin/Publish/resticterm_Linux_x64.zip" -NewName "resticterm_Linux_x64_V$ver.zip" 

# Clear 
Remove-Item './bin/Publish/Linux' -Recurse
Remove-Item './bin/Publish/Win' -Recurse

echo 'Done Windows & Linux version !'
