# jpath
jpath is a command-line tool for processing json input using JSONPath expressions.

```powershell
Syntax: jpath.exe [options] [filter]

Options:
  -i, --input        The file that contains the JSON to read. If a file is not specified, the JSON is read from Standard Input instead.
  -c, --compact      Returns compacted JSON instead of formatted/indented json.
  -k, --keeparray    By default, if the result is an array with a single item, that item will be extracted from the array and returned.
                     You can prevent this behavior by using the --keeparray option.
  -h, --help         Shows this help message.
```