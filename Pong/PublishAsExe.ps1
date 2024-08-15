# Change to the project directory
Set-Location "D:\All Code\Games\Pong"

# Run dotnet publish
dotnet publish -c Release -r win-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained

# Define the source directory and the paths
$sourceExePath = "D:\All Code\Games\Pong\Pong\bin\Release\net6.0\win-x64\publish\Pong.exe"
$destinationPath = "D:\All Code\Games\Pong\Pong_Release.exe"

# Copy the executable to the new location
if (Test-Path $sourceExePath) {
    Copy-Item $sourceExePath -Destination $destinationPath -Force
    Write-Output "Executable copied successfully to: $destinationPath"
} else {
    Write-Output "Executable not found at $sourceExePath. Publish may have failed."
}
