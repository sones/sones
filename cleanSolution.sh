#!/bin/bash

./clearDirectory.sh Applications *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt  *.cache *.FileListAbsolute.txt *.vspscc *.vssscc *.tlog
./clearDirectory.sh GraphAlgorithms *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt  *.cache *.FileListAbsolute.txt *.vspscc *.vssscc *.tlog
./clearDirectory.sh GraphDB *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt  *.cache *.FileListAbsolute.txt *.vspscc *.vssscc *.tlog
./clearDirectory.sh GraphDS *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt  *.cache *.FileListAbsolute.txt *.vspscc *.vssscc *.tlog
./clearDirectory.sh GraphFS *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt  *.cache *.FileListAbsolute.txt *.vspscc *.vssscc *.tlog
./clearDirectory.sh Lib *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt  *.cache *.FileListAbsolute.txt *.vspscc *.vssscc *.tlog
./clearDirectory.sh Libraries *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt  *.cache *.FileListAbsolute.txt *.vspscc *.vssscc *.tlog
./clearDirectory.sh Notifications *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt *.cache *.FileListAbsolute.txt *.vspscc *.vssscc *.tlog
./clearDirectory.sh StorageEngines *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt *.cache *.FileListAbsolute.txt *.vspscc *.vssscc *.tlog
./clearDirectory.sh GraphIO *.dll *.exe *.mdb *.pdb *.FilesWrittenAbsolute.txt *.cache *.FileListAbsolute.txt *.vspscc *.vssscc *.tlog

rm -r GraphDS/GraphDSREST/obj/Debug
