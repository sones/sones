#!/bin/bash

mkdir ./temp
cp ./Library/External/LuceneDotNet/*.dll ./temp/
./clearDirectory.sh Applications *.pidb *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh GraphDB *.pidb *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh GraphDS *.pidb *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh GraphFS *.pidb *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh GraphQL *.pidb *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh Library *.pidb *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh Plugins *.pidb *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
cp ./temp/*.dll ./Library/External/LuceneDotNet/
rm ./temp/*.dll
rmdir ./temp
