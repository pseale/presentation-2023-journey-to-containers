#!/bin/bash

set -e

cd "$(dirname "$0")"

cargo install --path .
cargo build --release

docker build -t bad-website:latest .

# re-tag the docker image

####
# Install nginx - might already be installed?
helm upgrade --atomic --install ingress-nginx ingress-nginx   --repo https://kubernetes.github.io/ingress-nginx   --namespace ingress-nginx --create-namespace
####

cd helm
helm upgrade --install --atomic --timeout 1m \
    --create-namespace --namespace sigterm-test \
    --debug \
    bad-website .

