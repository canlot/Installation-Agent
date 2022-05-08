if(!(new-object System.Security.Principal.WindowsPrincipal([System.Security.Principal.WindowsIdentity]::GetCurrent())).IsInRole(544)){start powershell -Verb runas -ArgumentList '-File',$MyInvocation.MyCommand.Definition;exit}

$serviceName = "InstallationAgent"
$serviceDisplayName = "Installation Agent Service"
$executableRelativePath = "Installation.Service\bin\Debug\Installation.Service.exe"
$description = "Installation Service that executes application that are communicated from the gui"


$executableFilePath = [System.IO.Path]::Combine( $PSScriptRoot, $executableRelativePath)




Get-Service $serviceName | Stop-Service -ErrorAction Continue -Force


& "cmd" "/c", "sc delete $serviceName"

Sleep 2

New-Service -Name $serviceName -DisplayName $serviceDisplayName -BinaryPathName $executableFilePath -Description $description -StartupType Automatic
Get-Service $serviceName | Start-Service