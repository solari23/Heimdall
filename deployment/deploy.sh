#!/bin/bash

pushd `dirname $0`

serviceDir=/home/heimdall/service

# Stop existing service (will fail if it doesn't exist)
systemctl stop heimdall.service
systemctl disable heimdall.service

# Create the service account (will just fail if it exists)
useradd --system --create-home heimdall

# Copy over the service binaries
rm -rf $serviceDir
mkdir $serviceDir
cp -r . $serviceDir

chmod +x $serviceDir/Heimdall.Server
chown -R heimdall:heimdall $serviceDir

read -p "Clear systemctl logs? " -n 1 -r
echo # newline
if [[ $REPLY =~ ^[Yy]$ ]]
then
    journalctl --rotate
    journalctl --vacuum-time=1
fi

# Copy over the new systemd config + start it
cp heimdall.service /etc/systemd/system/
systemctl daemon-reload
systemctl enable heimdall.service
systemctl start heimdall.service

# TODO: probe check here
# sleep 10

read -p "Clean up staging directory? " -n 1 -r
echo # newline
if [[ $REPLY =~ ^[Yy]$ ]]
then
    rm -rf *
fi

popd