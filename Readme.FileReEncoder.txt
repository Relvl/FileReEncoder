EncodingFixer
============

Small console utility for checking and converting text file encodings recursively
from the current working directory.

Public repository:
<REPOSITORY_URL_HERE>


Usage
-----

EncodingFixer.exe -ext:sql -target:windows-1251
EncodingFixer.exe -ext:sql -target:windows-1251 -fix

Multiple extensions can be specified with commas:

EncodingFixer.exe -ext:sql,txt,csv -target:utf-8
EncodingFixer.exe -ext:sql,txt,csv -target:utf-8 -fix


Arguments
---------

-ext:<extensions>

    Required.
    File extensions to process, separated by commas.
    Extensions are written without a leading dot.

    Examples:
        -ext:sql
        -ext:sql,txt
        -ext:sql,txt,csv


-target:<encoding>

    Optional.
    Target encoding that files should have.

    Default:
        utf-8

    Examples:
        -target:utf-8
        -target:windows-1251
        -target:windows-1252


-fix

    Optional flag.

    If omitted, the tool only prints files whose detected encoding differs
    from the target encoding.

    If specified, the tool converts those files to the target encoding.


Behavior
--------

The tool scans all files in the current directory recursively.

For each matching file, it:

    1. Reads the file as bytes.
    2. Detects the current encoding.
    3. Skips empty and ASCII-only files.
    4. Compares detected encoding with the target encoding.
    5. Prints files with a different detected encoding.
    6. Converts them only when -fix is specified.

ASCII-only files are skipped because their byte representation is usually valid
for many encodings at the same time, including UTF-8, Windows-1251 and
Windows-1252.


Examples
--------

Check all .sql files and report files that are not Windows-1251:

    EncodingFixer.exe -ext:sql -target:windows-1251

Convert all detected non-Windows-1251 .sql files to Windows-1251:

    EncodingFixer.exe -ext:sql -target:windows-1251 -fix

Check .sql and .txt files against UTF-8:

    EncodingFixer.exe -ext:sql,txt -target:utf-8

Convert .sql and .txt files to UTF-8:

    EncodingFixer.exe -ext:sql,txt -target:utf-8 -fix


Notes
-----

Encoding detection is probabilistic.

UTF-8 and UTF-16 files with BOM are usually detected reliably.
Single-byte encodings such as Windows-1251 and Windows-1252 can be ambiguous,
especially in short files or files without Cyrillic text.

Before running with -fix on important data, make a backup or run the tool once
without -fix and review the reported files.
