using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace MichaelAllenMiller.ExtensionMethods {
    public static class StringExtensions {
        public static Boolean In(this String input, params String[] values) {
            return input.In(StringComparison.CurrentCultureIgnoreCase, values);
        }
        public static Boolean In(this String input, StringComparison comparison, params String[] values) {
            if (input == null) { return false; }
            return values.Any(x => x.Equals(input, comparison));
        }
    }
}