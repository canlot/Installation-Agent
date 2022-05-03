$serviceName = "InstallationAgent"
$serviceDisplayName = "Installation Agent Service"
$executablePath = "\Installation.Service\bin\Debug\Installation.Service.exe"
$description = "Installation Service that executes application that are communicated from the gui"

$executableFilePath = [System.IO.Path]::Combine( $PSScriptRoot, $executablePath)

$executableFilePath


Get-Service $serviceName | Stop-Service


& "cmd" "/c", "sc delete $serviceName"

Sleep 2

New-Service -Name $serviceName -DisplayName $serviceDisplayName -BinaryPathName $executablePath -Description $description -StartupType Automatic
Get-Service $serviceName | Start-Service