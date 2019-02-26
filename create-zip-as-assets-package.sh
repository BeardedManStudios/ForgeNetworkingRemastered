#!/usr/bin/env bash

##########################################################################################
##
## This script creates a tarball that can be easily dropped into your Unity
## Project's "Assets" folder to use Forge Networking from sources.
##
## For more information, visit this link:
## https://github.com/BeardedManStudios/ForgeNetworkingRemastered/wiki/Getting-Started
##
##########################################################################################

set -e
set -u

function cleanup() {
    rm -rf "$DIR_TMP_FN"
}

trap 'cleanup' EXIT

DIR_TMP_FN="$(mktemp -d 'ForgeNetworking.tmp.XXXXXXXXXX')"
DIR_ASSETS="$DIR_TMP_FN/Assets"

mkdir "$DIR_ASSETS"

FILE_TAR="ForgeNetworking-source-assets.tar"

echo "* Copying source files ..."

# 2) Copy the `ForgeNetworkingRemastered\Forge Networking Remastered Unity\Assets\Bearded Man Studios Inc\` folder into your assets
cp -a 'Forge Networking Remastered Unity/Assets/Bearded Man Studios Inc'  "$DIR_ASSETS/"

# 3) Remove the `Assets\Bearded Man Studios Inc\Plugins\` folder
rm -rf "$DIR_ASSETS/Bearded Man Studios Inc/"{Plugins,Plugins.meta}

# 4) Copy the `ForgeNetworkingRemastered\BeardedManStudios\Source` folder into `Assets\Bearded Man Studios Inc\`
cp -a 'BeardedManStudios/Source/'                                         "$DIR_ASSETS/Bearded Man Studios Inc/"

# 5) Remove the contents of `Assets\Bearded Man Studios Inc\Editor\`, leaving the `Resources\` folder
rm -rf "$DIR_ASSETS/Bearded Man Studios Inc/Editor/ForgeNetworkingUnityEditor."*

# 6) Copy the .cs files from `ForgeNetworkingRemastered\ForgeNetworkingUnityEditor\` into `Assets\Bearded Man Studios Inc\Editor\`
cp -a 'ForgeNetworkingUnityEditor/'*.cs                                   "$DIR_ASSETS/Bearded Man Studios Inc/Editor/"

# 7) Copy the `ForgeNetworkingRemastered\ForgeNetworkingCommon\Templating\` folder into `Assets\Bearded Man Studios Inc\`
cp -a 'ForgeNetworkingCommon/Templating/'                                 "$DIR_ASSETS/Bearded Man Studios Inc/"


echo "* Creating tarball ..."
tar cf "$FILE_TAR" -C "$DIR_TMP_FN" Assets

echo
echo -e "A file with name \`\033[0;33;1m$FILE_TAR\033[0m' has been created."
echo "Extract this file to your Unity Project. Now you should be ready to go using Forge Networking from sources."
echo
echo "If you need help using this script or Forge Networking in general, you can join our Discord Server here: https://discord.gg/yzZwEYm"
echo

