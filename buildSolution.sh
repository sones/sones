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

./clearDirectory.sh Applications *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh GraphAlgorithms *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh GraphDB *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh GraphDS *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh GraphFS *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh Lib *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh Libraries *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh Notifications *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh StorageEngines *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh GraphIO *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh GraphDSClient *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt

if [ $option = "-r" ]; then
	xbuild /property:Configuration=Release
elif [ $option = "-d" ]; then
	xbuild
else
	exit 0
fi