#!/bin/bash
option="-d"
if [ $# -ne 1 ]; then
echo "Type -h for build options. Defaulting to debug."
elif [ $1 = "-r" ]; then
option=$1
fi


if [ $1 = "-h" ]; then
echo "-r run a release"
echo "-d run with debug symbols"
exit 0
elif [ $1 != "-d" -a $1 != "-r" ]; then
exit 0
fi

DIRECTORY=$(cd `dirname $0` && pwd)


if [ $option = "-r" ]; then
	echo Starting sones GraphDB 2.0 in $DIRECTORY/Applications/sonesGraphDB/bin/Release/
	cd $DIRECTORY/Applications/sonesGraphDB/bin/Release/
	mono --runtime=v4.0.30319 sonesGraphDBStarter.exe
elif [ $option = "-d" ]; then
	echo Starting sones GraphDB 2.0 in $DIRECTORY/Applications/sonesGraphDB/bin/Debug/
	cd $DIRECTORY/Applications/sonesGraphDB/bin/Debug/
	mono --runtime=v4.0.30319 sonesGraphDBStarter.exe
else
	exit 0
fi



