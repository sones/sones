sones GraphDB Community Edition version 2.0
Last updated on 19th of May 2011
(C) sones GmbH 2007 - 2011

sones is an object-orientated graph data storage for a large amount of highly connected semi-structured data
in a distributed environment. In contrast to classical relational but also purely object orientated databases
this implies two very important consequences: First its main focus is no longer the data, objects or vertices
itself, but their (type-safe) interconnections or edges. This means we are interested in the name of an user 
within a large scale social network, but we are much more interested to know which films his friends-friends
watched last summer and thought that they were amazing. In the near future we will provide a large framework of 
graph algorithms for these problems and usage scenarios. 

To build this package you want to follow the instructions on: 

http://developers.sones.de/wiki/doku.php?id=quickreference:installationguide

1. Running sones GraphDB precompiled binary

The only prerequisite for sones GraphDB 2.0 is a working MONO installation starting from
version 2.8 upwards.

The easiest way to download and install MONO on your machine is to point your browser to:

http://www.go-mono.com/mono-downloads/download.html 

and to follow the installation instructions. When the installation is done sones GraphDB can
built by running the build_solution.sh script. The sample application can be started
by running the run_sonesGraphDB.sh script.

Additional parameters can be configured using the .config file in the sonesGraphDB directory.

2. Comments and Notes

If you want to leave us a comment or question please send an eMail to info@sones.com. Additional ways to
contact us are listet on our homepage: http://www.sones.com


