#!/bin/bash

./clearDirectory.sh Applications *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt  *.cache *.FileListAbsolute.txt *.vspscc *.vssscc *.tlog *.pidb
./clearDirectory.sh GraphAlgorithms *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt  *.cache *.FileListAbsolute.txt *.vspscc *.vssscc *.tlog *.pidb
./clearDirectory.sh GraphDB *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt  *.cache *.FileListAbsolute.txt *.vspscc *.vssscc *.tlog *.pidb
./clearDirectory.sh GraphDS *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt  *.cache *.FileListAbsolute.txt *.vspscc *.vssscc *.tlog *.pidb
./clearDirectory.sh GraphFS *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt  *.cache *.FileListAbsolute.txt *.vspscc *.vssscc *.tlog *.pidb
./clearDirectory.sh Lib *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt  *.cache *.FileListAbsolute.txt *.vspscc *.vssscc *.tlog *.pidb
./clearDirectory.sh Libraries *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt  *.cache *.FileListAbsolute.txt *.vspscc *.vssscc *.tlog *.pidb
./clearDirectory.sh Notifications *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt *.cache *.FileListAbsolute.txt *.vspscc *.vssscc *.tlog *.pidb
./clearDirectory.sh StorageEngines *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt *.cache *.FileListAbsolute.txt *.vspscc *.vssscc *.tlog *.pidb
./clearDirectory.sh GraphIO *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt *.cache *.FileListAbsolute.txt *.vspscc *.vssscc *.tlog *.pidb

rm -r GraphDS/GraphDSREST/obj/Debug
