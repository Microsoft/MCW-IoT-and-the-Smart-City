#Fix RDP Issue
$HKLM = "HKLM:\SYSTEM\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp"
Set-ItemProperty -Path $HKLM -Name "SecurityLayer" -Value 0

#Enable Containers
Enable-WindowsOptionalFeature -Online -FeatureName Microsoft-Hyper-V-All -All -NoRestart
Enable-WindowsOptionalFeature -Online -FeatureName Containers -All -NoRestart

#Install Chocolatey
Set-ExecutionPolicy Bypass -Scope Process -Force; iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))

#Assign Packages to Install
$Packages = 'googlechrome',`
            'docker-desktop',`
            'postman',`
            'nodejs.install',`
            'vscode',`
            'git.install',`
            'visualstudio2019community',`
            'visualstudio2019-workload-azure',`
            'visualstudio2019-workload-manageddesktop',`
            'visualstudio2019-workload-netweb',`
            'dotnetcore-sdk --version=2.1.805'            

#Install Packages
ForEach ($PackageName in $Packages)
{choco install $PackageName -y}

#Install Visual Studio Code Extensions
Set-ExecutionPolicy Bypass -Scope Process -Force; & 'C:\Program Files\Microsoft VS Code\bin\code.cmd' --install-extension ms-vscode.csharp
Set-ExecutionPolicy Bypass -Scope Process -Force; & 'C:\Program Files\Microsoft VS Code\bin\code.cmd' --install-extension vsciot-vscode.azure-iot-edge

#Add Demo User to docker group
Add-LocalGroupMember -Member demouser -Group docker-users

#Bring down Desktop Shortcuts
$zipDownload = "https://github.com/microsoft/MCW-IoT-and-the-Smart-City/blob/master/Hands-on%20lab/Lab-files/LabVM/shortcuts.zip?raw=true"
$downloadedFile = "D:\shortcuts.zip"
$vmFolder = "C:\Users\Public\Desktop"
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
Invoke-WebRequest $zipDownload -OutFile $downloadedFile
Add-Type -assembly "system.io.compression.filesystem"
[io.compression.zipfile]::ExtractToDirectory($downloadedFile, $vmFolder)

#Reboot
Restart-Computer