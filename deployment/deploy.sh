#!/bin/bash

pushd `dirname $0`

serviceDir=/home/heimdall/service
storageDir=/home/heimdall/storage

# Stop existing services (will fail if they don't exist)
systemctl stop heimdall.service
systemctl disable heimdall.service
systemctl stop heimdall-webhookproxy.service
systemctl disable heimdall-webhookproxy.service

# Create the service account (will just fail if it exists)
useradd --system --create-home heimdall

# Create storage directory (will do nothing if it exists)
mkdir -p $storageDir

# Create the secret key file
if [[ ! -f $storageDir/SecretKey ]]
then
  echo "Creating secret key file"
  openssl rand -hex -out /dev/stdout 32 | tr -d '\n' > $storageDir/SecretKey
  
  # Read-only perms, only for file owner (Heimdall user)
  chmod 400 $storageDir/SecretKey
fi

chown -R heimdall:heimdall $storageDir

# Copy over the service binaries
rm -rf $serviceDir
mkdir $serviceDir
cp -r . $serviceDir

chmod +x $serviceDir/Heimdall.Server
chmod +x $serviceDir/Heimdall.WebhookProxy
chown -R heimdall:heimdall $serviceDir

# Copy over the new systemd configs + start them
cp heimdall.service /etc/systemd/system/
cp heimdall-webhookproxy.service /etc/systemd/system/
systemctl daemon-reload

systemctl enable heimdall.service
systemctl start heimdall.service
systemctl enable heimdall-webhookproxy.service
systemctl start heimdall-webhookproxy.service

# TODO: probe check here
# sleep 10

if [[ $1 == "--delete-staging" ]]
then
    echo "Deleting the staging directory contents"
    rm -rf *
fi

popd