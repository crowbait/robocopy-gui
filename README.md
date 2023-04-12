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

Binaries are built for x64 Windows.
Important: the program is targeting Windows 10.0.18362 and *might or might not* work on older versions. Theoretically, it should work down to Windows 7.

## To-do

- settings dialog per operation
  - move less important checkboxes (only newer, FAT file time)
  - options to set retry count and multithreading

#### Delayed indefinitely
These items seem either impossible or at least unfeasible. Further research might be required and ideas on tackling these problems have not been successful so far.
- drag-and-drop reordering of operations
- visual grouping of operations

## Development

- development using Visual Studio 2022 is recommended
- installation of [AvaloniaUI extension](https://avaloniaui.net/GettingStarted#installation) is recommended
