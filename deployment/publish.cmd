@echo off

pushd %~dp0..

dotnet publish -c Retail --runtime linux-arm64 --self-contained -o .\out src\Heimdall.Server\Heimdall.Server.csproj
copy deployment\heimdall.service out
copy deployment\deploy.sh out

scp -r out/* pi@homepi.local:/home/pi/staging/heimdall-v0/

popd