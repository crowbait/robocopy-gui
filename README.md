<div align="center">
  <img src="Assets/icon.svg" width="256" />
</div>

# robocopy-gui

##### Windows GUI for generating and managing robocopy-based .bat-scripts for backup and automation purposes 

<div align="center">
  <img src="Assets/main-interface.png" width="75%" stlye="padding: 8px; float: left;" />
  <img src="Assets/exclusions.png" width="20%" stlye="padding: 8px; float: right;" />
</div>

## Features

- manages (technically any) Windows batch file (.bat) in a GUI
- support for Windows-style file and folder exclusions incl. wildcards
  - eg. `OLD-test.dll`, `*.dll`, `OLD*`
- support for arbitrary commands (excl. `echo`)
  - echo is ignored during parsing
- adding and removing files to/from on-logon startup "programs"
- enabling and disabling per-command (`REM`)

## Installation

Grab your executable from the [release page](https://github.com/crowbait/robocopy-gui/releases).

There are 2 versions published:
- framework-dependent: requires .NET-Framework 7.0 to be installed, but is a much smaller executable
- self-contained: does not have outside dependencies, but is much larger

Both assembly types are build for x64 Windows.

## To-do

- migrate to UWP or WinUI 3 to enable the following features:
  - drag-and-drop reordering of operations
  - visual grouping of operations for a less convoluted interface
