This is the readme file of the sones GraphDB Open Source Edition version 1.1 - Last updated on 30th of June 2010
(C) sones GmbH 2007 - 2010

The sones is an object-orientated graph data storage for a large amount of highly connected semi-structured data
in a distributed environment. In contrast to classical relational but also purely object orientated databases
this implies two very important consequences: First its main focus is no longer the data, objects or vertices
itself, but their (type-safe) interconnections or edges. This means we are interested in the name of an user 
within a large scale social network, but we are much more interested to know which films his friends-friends
watched last summer and thought that they were amazing. In the near future we will provide a large framework of 
graph algorithms for these problems and usage scenarios. 

To build this package you want to follow the instructions on: 

http://developers.sones.de/wiki/doku.php?id=quickreference:installationguide

1. Building on Windows

To use the sones GraphDB on Windows you need to have the current version of the Microsoft .NET Framework 
installed. The current version is .NET Framework 4.0 and it can be downloaded here:

http://www.microsoft.com/downloads/details.aspx?FamilyID=9cfb2d51-5ff4-4491-b0e5-b386f32c0992&displaylang=en

After having that installed you can use the command line to build the package:

C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe "sones GraphDB OpenSource Edition 1.1.sln"

If you want to use a graphical IDE you need to have at least Visual Studio 2010 Express Edition which you
can download for free here:

http://www.microsoft.com/express/Downloads/Download-2010.aspx

2. Building on Linux, MacOSX, ...

You need Mono as a .NET runtime platform on all operating systems that are not Windows.

Important Notice: Currently you need a specific version of Mono to compile and run this software. This version is
Revision 158624 which is downloadable from the official Mono subversion repository using these commands:

svn co -r 158624 svn://anonsvn.mono-project.com/source/trunk/mono
svn co -r 158624 svn://anonsvn.mono-project.com/source/trunk/mcs
svn co -r 158624 svn://anonsvn.mono-project.com/source/trunk/libgdiplus

after having done that you can use this command:

./autogen.sh --with-large-heap=yes --with-profile4=yes

to generate the build scripts which then can be used to start the build process using this command:

make

After an eventual "sudo make install" you can run the sones GraphDB using this line inside the sonesExample
bin\Debug folder:

mono --runtime=v4.0.30319 sonesExample.exe

3. Comments and Notes

If you want to leave us a comment or question please send an eMail to info@sones.com. Additional ways to
contact us are listet on our homepage: http://www.sones.com

