#!/bin/bash

./clearDirectory.sh Applications *.pidb *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh GraphDB *.pidb *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh GraphDS *.pidb *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh GraphFS *.pidb *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh GraphQL *.pidb *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh Library *.pidb *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt
./clearDirectory.sh Plugins *.pidb *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt

