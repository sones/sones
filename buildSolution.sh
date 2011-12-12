#!/bin/bash
#
# sones GraphDB - Community Edition - http://www.sones.com
# Copyright (C) 2007-2011 sones GmbH
#
# This file is part of sones GraphDB Community Edition.
#
# sones GraphDB is free software: you can redistribute it and/or modify
# it under the terms of the GNU Affero General Public License as published by
# the Free Software Foundation, version 3 of the License.
# 
# sones GraphDB is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
# GNU Affero General Public License for more details.
#
# You should have received a copy of the GNU Affero General Public License
# along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
# 
# This is the build script for sones GraphDB 2.0 Community Edition
# 
# There are several commandline options available:
#
# -h 	will display a help
# -d	do a DEBUG build (DEFAULT)
# -r	do a RELEASE build
# -p	do a partial build (can be combined with -d/-r)
#
# Last changes: 31-July-2011, Daniel Kirstenpfad
#

echo "sones GraphDB 2.0 Build Script (C) sones GmbH 2007-2011";

option=$1
option2=$2

if [ $# -lt 1 ]; then
echo "Type -h for build options.";
option="-d";
option2="-d";
fi

if [ $option == "-p" ] && [ -z "$option2" ]; then
option2="-d";
fi

if [ $option == "-h" ]; then
echo "-r build a release";
echo "-d build with debug symbols";
echo "-p build partially (can be combined with -r and -d)";
exit 0
fi

if [ $option == "-p" ] || [ $option2 == "-p" ]; then
echo "Doing a partially build";
else
./cleanSolution.sh
fi

if [ $option == "-r" ] || [ $option2 == "-r" ]; then
	echo "Doing a release build";
	xbuild /property:Configuration=Release;
elif [ $option == "-d" ] || [ $option2 == "-d" ]; then
	echo "Doing a debug build";
	xbuild;
else
	exit 0
fi

