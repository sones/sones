#!/bin/bash
option="-d"
if [ $# -ne 1 ]; then
echo "Type -h for build options. Defaulting to debug."
elif [ $1 = "-r" ]; then
option=$1
fi


if [ $1 = "-h" ]; then
echo "-r build a release"
echo "-d build with debug symbols"
exit 0
elif [ $1 != "-d" -a $1 != "-r" ]; then
exit 0
fi

./clearDirectory.sh Applications *.pidb *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh GraphDB *.pidb *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh GraphDS *.pidb *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh GraphFS *.pidb *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh GraphQL *.pidb *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh Library *.pidb *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh Plugins *.pidb *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt

if [ $option = "-r" ]; then
	xbuild /property:Configuration=Release
elif [ $option = "-d" ]; then
	xbuild
else
	exit 0
fi
