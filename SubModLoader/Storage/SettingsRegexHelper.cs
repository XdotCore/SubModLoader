using System.Text.RegularExpressions;

namespace SubModLoader.Storage {
    internal static partial class SettingsRegexHelper {

        [GeneratedRegex("""(["\[\]\\#=\s\r])""")]
        private static partial Regex GetNeedsEscape();
        /// <summary>
        /// Gets ", [, ], \, #, =, \s, and \r, then replaces with \", \[, \], \\, \#, \=, \\s, and \\r
        /// </summary>
        internal static string GetEscaped(string value) => GetNeedsEscape().Replace(value, """\$1""");

        [GeneratedRegex("""\\(["\[\]\\#=\s\r])""")]
        private static partial Regex GetNeedsUnEscape();
        /// <summary>
        /// Gets \", \[, \], \\, \#, \=, \\s, and \\r, then replaces with ", [, ], \, #, =, \s, and \r
        /// </summary>
        internal static string GetUnEscaped(string value) => GetNeedsUnEscape().Replace(value, "$1");

        [GeneratedRegex("""((?:[^\[\]]|\\[\[\]])*)(?<!\\)\]([\s\S\r]*?)(?:(?<!\\)\[|\Z)""")]
        private static partial Regex GetAllSettings();
        /// <summary>
        /// Gets name until unescaped ], then matches until unescaped [
        /// </summary>
        internal static MatchCollection GetAllSettingsMatches(string save) => GetAllSettings().Matches(save);

        // needs to be on newline since ends with "
        [GeneratedRegex("""
            ((?:[^=\s\r]|\\[=\s\r])*)\s*=\s*"([\s\S\r]*?)(?<!\\)"
            """)]
        private static partial Regex GetAllItems();
        /// <summary>
        /// Gets name until = and ", then matches until unescaped "
        /// </summary>
        internal static MatchCollection GetAllItemsMatches(string categorySave) => GetAllItems().Matches(categorySave);

        [GeneratedRegex("""(.*)(?<!\\)##(.*)""")]
        private static partial Regex GetTwoNames();
        /// <summary>
        /// Gets name until last unescaped ## for first name, then second name after
        /// </summary>
        internal static (string name1, string name2) GetTwoNames(string nameString) {
            Match match = GetTwoNames().Match(nameString);
            if (match.Groups.Count > 2)
                return (match.Groups[1].Value, match.Groups[2].Value);
            else
                return (nameString, null);
        }

        [GeneratedRegex("""[\s\r]""")]
        private static partial Regex GetAllWhiteSpace();
        /// <summary>
        /// Gets whitespace, then replaces with nothing
        /// </summary>
        internal static string RemoveAllWhiteSpace(string s) => GetAllWhiteSpace().Replace(s, "");
    }
}
