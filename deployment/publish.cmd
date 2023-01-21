@echo off

pushd %~dp0..

echo Building..
dotnet publish -c Retail --runtime linux-arm64 --self-contained -o .\out src\Server\Heimdall.Server.csproj
dotnet publish -c Retail --runtime linux-arm64 --self-contained -o .\out src\WebhookProxy\Heimdall.WebhookProxy.csproj
copy deployment\heimdall.service out
copy deployment\heimdall-webhookproxy.service out
copy deployment\deploy.sh out

choice /M "Copy to HomePi staging?"
IF ERRORLEVEL ==2 GOTO END

echo Copying to staging directory on HomePi..
scp -r out/* pi@homepi.local:/home/pi/staging/heimdall/

echo Done publishing!

choice /M "Deploy service?"
IF ERRORLEVEL ==2 GOTO END

echo Deploying the service on HomePi..
ssh pi@homepi.local "cd ~/staging/heimdall && chmod +x deploy.sh && sudo ./deploy.sh --delete-staging"
echo Deployment complete!

:END
popd