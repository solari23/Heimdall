#!/bin/bash

pushd `dirname $0`

serviceDir=/home/heimdall/service
storageDir=/home/heimdall/storage

# Stop existing service (will fail if it doesn't exist)
systemctl stop heimdall.service
systemctl disable heimdall.service

# Create the service account (will just fail if it exists)
useradd --system --create-home heimdall

# Create storage directory (will do nothing if it exists)
mkdir -p $storageDir
chown -R heimdall:heimdall $storageDir

# Copy over the service binaries
rm -rf $serviceDir
mkdir $serviceDir
cp -r . $serviceDir

chmod +x $serviceDir/Heimdall.Server
chown -R heimdall:heimdall $serviceDir

# Copy over the new systemd config + start it
cp heimdall.service /etc/systemd/system/
systemctl daemon-reload
systemctl enable heimdall.service
systemctl start heimdall.service

# TODO: probe check here
# sleep 10

if [[ $1 == "--delete-staging" ]]
then
    echo "Deleting the staging directory contents"
    rm -rf *
fi

popd