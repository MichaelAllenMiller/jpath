using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MichaelAllenMiller.ExtensionMethods;

namespace MichaelAllenMiller.jpath {
    class Program {
        static void Main(string[] args) {
            try {
                var commandLineOptions = args.WithContext();

                if (args.Any(x => x.In("-?", "-h", "--help"))) {
                    ShowHelp(args);
                } else {
                    JToken token = null;

                    if (args.Any(x => x.In("-i", "--input"))) {
                        var fileName = commandLineOptions.FirstOrDefault(x => x.Current.In("-i", "--input")).Next;
                        if (String.IsNullOrEmpty(fileName)) {
                            Console.Error.WriteLine("You must specify an input file.");
                            Environment.Exit((Int32)ExitCodes.MissingInputFile);
                        } else {
                            var currentDirectory = Environment.CurrentDirectory;
                            var fileInfo = new System.IO.FileInfo(fileName);
                            if (!fileInfo.Exists) {
                                fileInfo = new System.IO.FileInfo(System.IO.Path.GetFullPath(fileName, currentDirectory));
                            }

                            if (!fileInfo.Exists) {
                                Console.Error.WriteLine("The input file you supplied does not exist.");
                                Environment.Exit((Int32)ExitCodes.MissingInputFile);
                            } else {
                                var json = System.IO.File.ReadAllText(fileInfo.FullName);
                                token = Newtonsoft.Json.Linq.JToken.Parse(json);
                            }
                        }
                    } else {

                        // Load the json from standard input.
                        using var reader = new Newtonsoft.Json.JsonTextReader(Console.In);
                        token = JToken.ReadFrom(reader);
                    }

                    WriteOutput(token, commandLineOptions);
                }
            } catch (JsonReaderException ex) {
                Console.Error.WriteLine(ex.Message);
                Environment.Exit((Int32)ExitCodes.InvalidJson);
            } catch (System.Exception ex) {
                Console.Error.WriteLine(ex.Message);
                Environment.Exit((Int32)ExitCodes.Failed);
            }
        }

        static void ShowHelp(string[] args) {
            Console.Out.WriteLine($"Syntax: jpath.exe [options] [filter]");
            Console.Out.WriteLine($"");
            Console.Out.WriteLine($"Options:");
            Console.Out.WriteLine($"  -i, --input        The file that contains the JSON to read. If a file is not specified, the JSON is read from Standard Input instead.");
            Console.Out.WriteLine($"  -c, --compact      Returns compacted JSON instead of formatted/indented json.");
            Console.Out.WriteLine($"  -k, --keeparray    By default, if the result is an array with a single item, that item will be extracted from the array and returned.");
            Console.Out.WriteLine($"                     You can prevent this behavior by using the --keeparray option.");
            Console.Out.WriteLine($"  -h, --help         Shows this help message.");
        }

        static void WriteOutput(JToken token, IEnumerable<IEnumberableExtensions.ElementWithContext<String>> args) {
            var indented = !args.Any(x => x.Current.In("-c", "--compact"));
            var path = args.Where(x => !x.Current.StartsWith("-", StringComparison.CurrentCulture) && (x.Previous == null || !x.Previous.StartsWith("-", StringComparison.CurrentCulture))).Select(x => x.Current).LastOrDefault() ?? "";

            var settings = new Newtonsoft.Json.JsonSerializerSettings {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
                Formatting = (indented ? Newtonsoft.Json.Formatting.Indented : Newtonsoft.Json.Formatting.None)
            };

            if (String.IsNullOrEmpty(path)) {
                Console.Out.Write(Newtonsoft.Json.JsonConvert.SerializeObject(token, settings));
            } else {
                var keepArray = args.Any(x => x.Current.In("-k", "--keeparray"));

                var matches = token.SelectTokens(path);
                if (!keepArray && matches.Count() == 1) {
                    Console.Out.Write(Newtonsoft.Json.JsonConvert.SerializeObject(matches.First(), settings));
                } else {
                    Console.Out.Write(Newtonsoft.Json.JsonConvert.SerializeObject(matches, settings));
                }
            }

            Environment.Exit((Int32)ExitCodes.Success);
        }

        [Flags]
        private enum ExitCodes { Success = 0, Failed = 1, InvalidJson = 2, MissingInputFile = 4 }
    }
}
