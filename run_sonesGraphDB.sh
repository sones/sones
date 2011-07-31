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
# This is the run script for sones GraphDB 2.0 Community Edition Starter Application
# 
# There are several commandline options available:
#
# -h 	will display a help
# -d	run the DEBUG build (DEFAULT)
# -r	run the RELEASE build
#
# Last changes: 31-July-2011, Daniel Kirstenpfad
#
echo "sones GraphDB 2.0 Starter Script (C) sones GmbH 2007-2011";
echo "";

option=$1

if [ $# -lt 1 ]; then
	echo "Type -h for options. Defaulting to debug.";
	option="-d";
fi

if [ $option = "-h" ]; then
	echo "-r run a release build"
	echo "-d run with debug build"
	exit 0
fi

DIRECTORY=$(cd `dirname $0` && pwd)


if [ $option == "-r" ]; then
	echo "Starting sones GraphDB 2.0 (RELEASE) in $DIRECTORY/Applications/sonesGraphDB/bin/Release/";
	echo "";
	cd $DIRECTORY/Applications/sonesGraphDB/bin/Release/

	
	if [ -x sonesGraphDBStarter.exe	]; then
		if which "mono-sgen" >/dev/null; then
   	 	 mono-sgen --runtime=v4.0.30319 sonesGraphDBStarter.exe	
			else
				if which "mono" >/dev/null; then
	   			 mono --runtime=v4.0.30319 sonesGraphDBStarter.exe
	  			else
 	   			 echo "You need to have MONO (>2.8) installed and in your path. Mono-SGEN is recommended.";
	  			fi
	 		fi
	else
   	 echo "You need to build before you run. Please use the buildSolution.sh script to do that first.";
   	 exit 1;
	fi

elif [ $option == "-d" ]; then
	echo "Starting sones GraphDB 2.0 (DEBUG) in $DIRECTORY/Applications/sonesGraphDB/bin/Debug/";
	echo "";
	cd $DIRECTORY/Applications/sonesGraphDB/bin/Debug/

	if [ -x sonesGraphDBStarter.exe	]; then
			if which "mono-sgen" >/dev/null; then
			echo "Using MONO-sgen";
   	 		mono-sgen --runtime=v4.0.30319 sonesGraphDBStarter.exe	
			else
				if which "mono" >/dev/null; then
					echo "Using MONO (with boehm-gc)";
	   				mono --runtime=v4.0.30319 sonesGraphDBStarter.exe
	  			else
 	   				echo "You need to have MONO (>2.8) installed and in your path. Mono-SGEN is recommended.";
	  			fi
	 fi
	else
   	 echo "You need to build before you run. Please use the buildSolution.sh script to do that first.";
   	 exit 1;
	fi


else
	exit 0
fi



