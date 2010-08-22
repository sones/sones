#!/bin/bash

DIRECTORY=$(cd `dirname $0` && pwd)

echo Starting sonesExample in $DIRECTORY/Applications/sonesExample/bin/Debug/

cd $DIRECTORY/Applications/sonesExample/bin/Debug/
mono --runtime=v4.0.30319 sonesExample.exe
