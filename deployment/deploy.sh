#!/bin/bash

serviceDir=~heimdall/service

# Stop existing service (will fail if it doesn't exist)
systemctl enable kestrel-helloapp.service

# Create the service account (will just fail if it exists)
useradd --system --create-home heimdall

# TODO: Copy over the service binaries
mkdir $serviceDir
# TODO

# Copy over the new systemd config + start it
cp heimdall.service /etc/systemd/system/
systemctl enable heimdall.service