unbom
=====
Tool to remove UTF-8 BOM markers from files.

## why
UTF-8 BOMs are problematic and useless in common cases. A lot of tools don't support BOM markers; command-line
diff tools for instance. Tools that` `support UTF-8, support it without markers anyway.
They serve no purpose other than to annoy our diff outputs.

Visual Studio defaults to UTF-8+BOM causing the problems.

There are many shell scripts available for removing BOM markers. Many of them either slow or buggy. They assume BOM
marker can appear inside a file. A careless user can easily corrupt their files. 
I just wanted a simple and safer tool so I spent an hour on this. 

## install
Install unbom by running the command:

```
dotnet tool install --global unbom
```

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
