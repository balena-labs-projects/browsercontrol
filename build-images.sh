#!/bin/bash
set -e

function build_and_push_image () {
  local DOCKER_REPO=$1
  local BALENA_MACHINE_NAME=$2
  local DOCKER_ARCH=$3
  local RID=$4

  echo "Building for machine name $BALENA_MACHINE_NAME, platform $DOCKER_ARCH, pushing to $DOCKER_REPO/browsercontrol"

  # resolve the template value for machine name
  sed "s/%%BALENA_MACHINE_NAME%%/$BALENA_MACHINE_NAME/g" ./Dockerfile.template > ./Dockerfile.temp

  # resolve the RID for dotnet publish
  sed "s/%%MICROSOFT_RID%%/$RID/g" ./Dockerfile.temp > ./Dockerfile.$BALENA_MACHINE_NAME
  
  docker buildx build -t $DOCKER_REPO/browsercontrol:$BALENA_MACHINE_NAME --platform $DOCKER_ARCH --file Dockerfile.$BALENA_MACHINE_NAME .

  echo "Publishing..."
  docker push $DOCKER_REPO/browsercontrol:$BALENA_MACHINE_NAME

  echo "Cleaning up..."
  rm Dockerfile.$BALENA_MACHINE_NAME
  rm Dockerfile.temp
}

function retag_and_push_image () {
  local DOCKER_REPO=$1
  local BUILT_TAG=$2
  local NEW_TAG=$3

  echo "Taging $DOCKER_REPO/browsercontrol:$BUILT_TAG as $DOCKER_REPO/browsercontrol:$NEW_TAG"
  docker tag $DOCKER_REPO/browsercontrol:$BUILT_TAG $DOCKER_REPO/browsercontrol:$NEW_TAG

  echo "Publishing..."
  docker push $DOCKER_REPO/browsercontrol:$NEW_TAG
}

function create_and_push_manifest() {
  docker pull $DOCKER_REPO/browsercontrol:raspberrypi3
  docker manifest rm $DOCKER_REPO/browsercontrol:latest
  docker manifest create $DOCKER_REPO/browsercontrol:latest --amend $DOCKER_REPO/browsercontrol:genericx86-64-ext --amend $DOCKER_REPO/browsercontrol:raspberrypi4-64  --amend $DOCKER_REPO/browsercontrol:raspberrypi3
  docker manifest push $DOCKER_REPO/browsercontrol:latest
}

# You can pass in a repo (such as a test docker repo) or accept the default
DOCKER_REPO=${1:-balenablocks}

# Have to manually build raspberrypi3 image on a pi - due to some dotnet docker limitation:
# https://github.com/dotnet/dotnet-docker/issues/1537

build_and_push_image $DOCKER_REPO "raspberrypi4-64" "linux/arm64" "linux-musl-arm64"
build_and_push_image $DOCKER_REPO "genericx86-64-ext" "linux/amd64" "linux-musl-x64"

create_and_push_manifest
