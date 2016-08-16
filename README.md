unbom
=====
Tool to remove UTF-8 BOM markers from files

## usage

    Usage: unbom [options] <filespec>

    Options:
        -r, --recurse              recurse subdirectories
        -n, --nobackup             do not save a backup file

## example

    unbom -r *.cs

Remove UTF-8 BOM markers from all files with ".cs" extensions and subdirectories. Saves old versions of files in "filename.bak" file.

## license

MIT License. See [LICENSE!](LICENSE) file for details.