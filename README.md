# KalymnosBT
Portable issue tracking tool for individuals / works in Windows
![KalymnosBT Main Window](/Downloads/KalymnosBT.1.5.png)

## Download & version history

Version 1.8 (Released on August 29, 2021) - [\[download\]](/Downloads/KalymnosBTPortable_1_8.zip?raw=true)

- Bug fixed: the votes panel moves outside the screen on long comments in Issue Details Dialog
- Bug fixed: the comments panel moves outside the screen on long description in sidebar of the Main Window
- Target .NET Framework switched to 4.8

Version 1.7 (Released on March 04, 2020)

- Fixed the ["bad dates format"](https://github.com/windracer/KalymnosBT/issues/3) issue.
- Added two shortcuts CTRL+L and F3 to set focus to the Search box.
- Search feature improved: now KalymnosBT finds entries containing ALL words in any order in any field. 

## How to use
Download the precompiled binaries [\[here\]](/Downloads/KalymnosBTPortable_1_8.zip?raw=true) and unpack it anywhere on your computer. This is the portable version, you can save it inside a local Dropbox or Google Drive folder. Start "KalymnosBTPortable.exe" to launch the program.

## The story
For several years a free local copy of [YouTrack](https://www.jetbrains.com/youtrack/) was used to maintain suggestions and issues for [WinCatalog Disk Catalog Software](https://www.wincatalog.com). With no doubts, YouTrack can be recommended for everyone as the best issue tracking software, but once, after upgrading a system hard drive, all the collected issues and suggestions were lost without an ability to recover them (and all the existing backups didn't work). It was possible to restore most of the issues from conversations with the clients, but a Google Spreadsheet was used since then. This project is the replacement of that Google Spreadsheet and a result of learning the WPF.

KalymnosBT stores all the data inside a single JSON file packed by Gzip. Once a day it creates a backup copy of the data file. The open file format (JSON text) and a lot of backups should make it reliable in terms of data loss.

### More about the functionality
In addition to the issue number, title and description, each issue may have comments and votes. Votes help to track most wanted features. All field are optional, the current version does not check issue numbers collisions.

Each issue may be starred, marked as important and backlogged (added to the pool for the next version). 

Deleted issues are moving to Trash, where they can be deleted permanently or restored. Deleted projects are also moving to Trash, but the mechanism of restoring wasn't yet implemented. This is a kind of an emergency and you can edit the JSON file to recover the occasionally deleted project.

## Portability
KalymnosBT was designed as a portable app. The executable should contain the `portable` word in the filename. In that case KalymnosBT will use the same folder for storing all the data and settings. Otherwise, the data and the settings will be stored in `%appdata%\OrangeCat Software` folder.

## Disclaimer
The app was created for internal needs, it does not contain all the planned features and may loss all the data it keeps. Please use it at your own risk. If you don't agree with this disclaimer, you should stop using the app.

## Requirements
.Net Framework 4.8 or newer

## Other products
For other products please visit https://www.wincatalog.com and https://www.securesafepro.com
