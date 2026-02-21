# STRG Editor

Desktop tool (Windows Forms) for opening, viewing, editing, and saving `.STRG` text files used in games such as **DKCR** and **Metroid Prime 3** for Nintendo Wii.

## What the tool does

* Opens **one or multiple STRG files** at the same time.
* Displays the **available languages** in the file.
* Shows and allows editing of texts in a grid with multi-line support.
* Saves changes to the original file (**Save**) or to a new file (**Save As**).
* Performs **text search** with navigation between results.
* Exports texts to `.txt` and imports texts from `.txt`.
* Allows closing a specific file or all loaded files.

## Main Features

* **Language-based editing:** Select a language and edit only that specific set of texts.
* **Smart search:** Finds occurrences in the selected language and allows navigation between them.
* **Change tracking:** Detects modified files and prompts before closing/importing.
* **TXT Import/Export:** Useful for translation and external proofreading.
* **Line break handling:** Properly converts `\n` between display and storage formats.

## Requirements

* **Windows**
* **.NET Framework 4.8**
* Visual Studio (recommended for building and running)

## How to Run

1. Open the `STRGeditor.sln` solution in Visual Studio.
2. Build the `STRGeditor` project.
3. Run the application (`F5` or "Start").

## Basic Usage Workflow

1. Go to `File > Open` to load one or more `.STRG` files.
2. Select the desired file and language.
3. Edit the texts in the table.
4. Use the search bar to locate specific texts.
5. Save using `Save`, `Save All`, or `Save As`.
6. Optionally, export/import texts via `TXT`.
