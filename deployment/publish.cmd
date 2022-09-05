@echo off

pushd %~dp0..

echo Building..
dotnet publish -c Retail --runtime linux-arm64 --self-contained -o .\out src\Heimdall.Server\Heimdall.Server.csproj
copy deployment\heimdall.service out
copy deployment\deploy.sh out

echo Copying to staging directory on HomePi..
scp -r out/* pi@homepi.local:/home/pi/staging/heimdall/

echo Done publishing!
echo To deploy the service, ssh to HomePi and run deploy.sh
echo "cd ~/staging/heimdall && chmod +x deploy.sh && sudo ./deploy.sh"

popd