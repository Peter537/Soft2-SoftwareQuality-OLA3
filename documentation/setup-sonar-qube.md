How to SonarQube C#

Run all commands from solution root.

1. dotnet tool install --global dotnet-sonarscanner
2. Go to https://sonarcloud.io/ make account
3. Go to https://sonarcloud.io/account/security Generate Token
4. dotnet sonarscanner begin /k:"Peter537_Soft2-SoftwareQuality-OLA3" /o:"ossi-1337" /d:sonar.host.url="https://sonarcloud.io" /d:sonar.login="YOUR_TOKEN" /d:sonar.exclusions="documentation/**"
5. dotnet build .\Soft2-SoftwareQuality-OLA3.sln
6. dotnet sonarscanner end /d:sonar.login="YOUR_TOKEN"