Welcome to 'sones GraphDB' !
============================

This file helps you to get started with the 'sones GraphDB v2.0 Community Edition'. It covers the following subjects:

* Requisites
* Installation
* Application overview
* First steps
* Configuration
* FAQ
* Further information and contact

Requisites
==========

'sones GraphDB' and associated products are developed in Microsoft C#. The following runtime is required to execute this kind of applications:

* .(dot)NET Framework 4

Installation
============

The Windows installation package comes as one single file which is named 'sones_GraphDB_v2.0_CE.msi'. Please be aware of that you need to be logged in as Administrator to install the 'sones GraphDB v2.0 Community Edition'.
Just double click on the MSI-file to start the installer. A typical installation means to install everything to 'C:\Program Files (x86)\Sones'. A custom installation allows you to change this path.


The following will become installed:

* sones GraphDB starter application
* ReadMe
* Desktop shortcuts and Program menu entries

Application overview
====================

The 'sones GraphDB v2.0 Community Edition' includes the following:

* sones GraphDB starter: A command line tool to start up sones GraphDB
* 'sones Webshell': A simple to use shell like application which runs in your browser. It allows you to execute GQL statements. 

--Note: For further details about the G(raph) Q(uery) L(anguage) please read the 'GQL cheat sheet', which can be found on our web site.

First steps
===========

At this point we assume that you successfully installed the 'sones GraphDB v2.0 Community Edition'. The following steps may help you to get started.

The following user/password is used by default:

* test/test

Here some useful steps:

* You should find a Desktop icon 'GraphDB Server' on your Desktop, so please double click on it to start up your server instance
* In the Program Menu you can find an entry 'All Programs -> Sones -> sones Webshell'. A click on this link opens your web browser. It now should show the 'sones Webshell'.
* Please enter the command 'DESCRIBE VERTEX TYPES' to get some information about the out of the box existent Vertex Types!

Configuration
=============

There is a configuration file which is named 'sonesGraphDBStarter.exe.cfg' located in the installation directory. 

FAQ
===

* What's the difference between this evaluation version and the Open Source version?: 'sones GraphDB' is developed under AGPL and a proprietary license. This installation package contains the Community Edition. The main difference is that the Community Edition only supports non persistent data storages.
* Where to find the source code?: 'https://github.com/sones/sones'
* Please check if there is another application using the required network ports.
* What are the default login credentials?: test/test 

Further information and contact
===============================

* 'http://www.sones.com'
* 'http://developers.sones.de/'

Feel free to contact us directly via email: 'info@sones.de'






