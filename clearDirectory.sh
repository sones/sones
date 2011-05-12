#!/bin/bash

echo "Cleaning directory: "$1
echo

for Dirs in $(ls -ad $1); do
	if [ -d $Dirs ] ; then
		for entry in $@; do
			if [ $entry != $1 ] ; then
				files=$(find $1 -name "$entry")
				for files in $files; do
					rm -f $files
					echo "the following file was removed:" $files
				done
			fi
		done
	fi
done
