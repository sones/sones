#!/bin/bash
./clearDirectory.sh GraphDB *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh GraphFS *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh GraphDS *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
cd Lib
xbuild
cd ../StorageEngines
xbuild
cd ../GraphFS
xbuild
cd ../GraphDB
xbuild
cd ../GraphDS
xbuild
