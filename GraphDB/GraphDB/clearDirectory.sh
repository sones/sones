#!/bin/bash

echo "Löscht die im angegebenen Verzeichnis liegenden Dateien."
echo

for Dirs in $(ls -ad $1); do
	if [ -d $Dirs ] ; then
		for entry in $@; do
			if [ $entry != $1 ] ; then
				files=$(find . -name "$entry")
				for files in $files; do
					rm -f $files
					echo "Folgende Datei wurde gelöscht:" $files
				done
			fi
		done
	fi
done
