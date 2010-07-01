#!/bin/bash

./clearDirectory.sh Applications *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh GraphAlgorithms *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh GraphDB *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh GraphDS *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh GraphFS *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh Lib *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh Libraries *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh Notifications *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh StorageEngines *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt

xbuild