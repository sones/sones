#!/bin/bash
if [ $# -ne 1 ]; then
echo "Type -h for build options. Defaulting to debug."
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


./clearDirectory.sh Applications *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh GraphAlgorithms *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh GraphDB *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh GraphDS *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh GraphFS *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh Lib *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh Libraries *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh Notifications *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh StorageEngines *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt

if [ $# -ne 1 ]; then
    build -d
exit 0
fi

xbuild
