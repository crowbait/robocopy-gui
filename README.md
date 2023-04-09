<img src="Assets/icon.svg" width="256" />

<br>

# robocopy-gui

##### Windows GUI for generating and managing robocopy-based .bat-scripts for backup and automation purposes 

<div align="center">
  <img src="Assets/main-interface.png" width="40%" stlye="padding: 8px; float: left;" />
  <img src="Assets/exclusions.png" width="40%" stlye="padding: 8px; float: right;" />
</div>

## Features

- manages (technically any) Windows batch file (.bat) in a GUI
- support for file and folder exclusions, moving instead of copying, etc.
- support for arbitrary commands (excl. `echo`)
  - echo is ignored during parsing
- adding and removing files to/from on-logon startup "programs"
- enabling and disabling per-command (`REM`)

## To-do

- visual grouping of operations for a less convoluted interface
- drag-and-drop reordering of operations

* both will most likely need a migration to UWP or WinUI 3 *
