# Change to the project directory
Set-Location "D:\All Code\Games\Pong"

# Run dotnet publish
dotnet publish -c Release -r win-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained

# Define the source directory and the output zip file path
$sourceDir = "D:\All Code\Games\Pong\Pong\bin\Release\net6.0\win-x64\publish"
$zipFilePath = "D:\All Code\Games\Pong\Pong_Release.zip"

# Create a zip file from the published output
Compress-Archive -Path $sourceDir\* -DestinationPath $zipFilePath -Update

# Write completion status
Write-Output "Publish and zip process completed. Zip file located at: $zipFilePath"
