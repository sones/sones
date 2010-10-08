#!/bin/bash
if [ $# -ne 1 ]; then
echo "Type -h for build options."
exit 0
fi

if [ $1 = "-h" ]; then
echo "-r build a release"
echo "-d build with debug symbols"
exit 0
elif [ $1 != "-d" -a $1 != "-r" ]; then
exit 0
fi

function build {
	if [ $1 = "-r" ]; then
		xbuild /property:Configuration=Release
	elif [ $1 = "-d" ]; then
		xbuild
	else
		exit 0
	fi
}

./clearDirectory.sh GraphDB *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh GraphFS *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh GraphDS *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
cd Lib
build $1
cd ../StorageEngines
build $1
cd ../GraphFS
build $1
cd ../GraphDB
build $1
cd ../GraphDS
build $1
