#!/bin/bash

set -e

cd "$(dirname "$0")"

dotnet publish --os linux --arch x64 -p:PublishProfile=DefaultContainer -c Release

####
# Install nginx - might already be installed?
helm upgrade --atomic --install ingress-nginx ingress-nginx   --repo https://kubernetes.github.io/ingress-nginx   --namespace ingress-nginx --create-namespace
####

cd helm
helm upgrade --install --atomic --timeout 1m \
    --create-namespace --namespace sigterm-test \
    --debug \
    --force \
    good-website .

