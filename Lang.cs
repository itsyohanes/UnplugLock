// Translations. Every piece of text the app and installer show goes through Lang.T, keyed by name.
// The English text lives in lang\en.cs; each other language has its own file in lang\ and only
// needs the keys it translates - anything missing falls back to English.
//
// The language follows the Windows display language. To add one: copy lang\en.cs, translate it,
// and register it in Languages below. The tests check that every translation has the same keys
// and {0} placeholders as English.

using System;
using System.Collections.Generic;
using System.Globalization;

namespace UnplugLock
{
    internal static partial class Lang
    {
        // culture name -> table. A Windows language matches its own name or any parent of it,
        // so "de" covers de-DE, de-AT and de-CH, "zh-Hans" covers zh-CN and zh-SG, and "zh-Hant"
        // covers zh-TW, zh-HK and zh-MO.
        private static readonly Dictionary<string, Func<Dictionary<string, string>>> Languages =
#if SETUP
            // setup (built with /define:SETUP) has only English built in, and borrows the other
            // languages from the app it installs - see SetupLanguages in Setup.cs
            SetupLanguages.Load();
#else
            new Dictionary<string, Func<Dictionary<string, string>>>(StringComparer.OrdinalIgnoreCase)
            {
                { "cs", Czech },
                { "de", German },
                { "es", Spanish },
                { "fr", French },
                { "id", Indonesian },
                { "it", Italian },
                { "ja", Japanese },
                { "ko", Korean },
                { "nl", Dutch },
                { "pl", Polish },
                { "pt", Portuguese },
                { "ru", Russian },
                { "th", Thai },
                { "tr", Turkish },
                { "uk", Ukrainian },
                { "vi", Vietnamese },
                { "zh-Hans", ChineseSimplified },
                { "zh-Hant", ChineseTraditional },
            };
#endif

        internal static readonly Dictionary<string, string> EnglishTable = English();

        private static Dictionary<string, string> _table; // null for English

        // the language in use ("en" when nothing matched) and the culture numbers and dates are written in
        public static string Name { get; private set; }
        public static CultureInfo Culture { get; private set; }

        static Lang()
        {
            Use(CultureInfo.CurrentUICulture);
        }

        internal static IEnumerable<string> Available { get { return Languages.Keys; } }

        internal static Dictionary<string, string> TableFor(string name)
        {
            return Languages[name]();
        }

        // a language picked in setup ("de", "en", ...). Null, empty or unknown follows Windows.
        public static void Use(string name)
        {
            if (string.IsNullOrEmpty(name) || (name != "en" && !Languages.ContainsKey(name)))
            {
                Use(CultureInfo.CurrentUICulture);
                return;
            }
            if (name == "en")
            {
                _table = null;
                Name = "en";
            }
            else
            {
                _table = Languages[name]();
                Name = name;
                foreach (var key in Languages.Keys)
                    if (key.Equals(name, StringComparison.OrdinalIgnoreCase))
                        Name = key; // as registered, so the plural rules recognise it
            }
            // write dates the Windows way if Windows is in that language too (de-AT, not de-DE)
            Culture = CultureInfo.GetCultureInfo(Name == "en" ? "en-US" : Name);
            for (var c = CultureInfo.CurrentCulture; c.Name.Length > 0; c = c.Parent)
                if (c.Name.Equals(Name, StringComparison.OrdinalIgnoreCase))
                {
                    Culture = CultureInfo.CurrentCulture;
                    break;
                }
        }

        // the language Windows is in, if there's a translation for it (else "en")
        public static string WindowsLanguage
        {
            get
            {
                for (var c = CultureInfo.CurrentUICulture; c != null && c.Name.Length > 0; c = c.Parent)
                    foreach (var key in Languages.Keys)
                        if (key.Equals(c.Name, StringComparison.OrdinalIgnoreCase))
                            return key;
                return "en";
            }
        }

        // every language with its own name for itself, English first, for the setup picker
        public static List<KeyValuePair<string, string>> Choices()
        {
            var list = new List<KeyValuePair<string, string>>();
            foreach (var name in Languages.Keys)
                list.Add(new KeyValuePair<string, string>(name, Languages[name]()["LanguageName"]));
            list.Sort(delegate(KeyValuePair<string, string> a, KeyValuePair<string, string> b)
            {
                return string.Compare(a.Value, b.Value, StringComparison.OrdinalIgnoreCase);
            });
            list.Insert(0, new KeyValuePair<string, string>("en", EnglishTable["LanguageName"]));
            return list;
        }

        // switches language: the first of ui and its parents that has a translation, else English
        public static void Use(CultureInfo ui)
        {
            for (var c = ui; c != null && c.Name.Length > 0; c = c.Parent)
            {
                Func<Dictionary<string, string>> make;
                if (Languages.TryGetValue(c.Name, out make))
                {
                    _table = make();
                    Name = c.Name;
                    Culture = ui.IsNeutralCulture ? CultureInfo.GetCultureInfo(c.Name) : ui;
                    return;
                }
            }
            _table = null;
            Name = "en";
            Culture = CultureInfo.GetCultureInfo("en-US");
        }

        public static string T(string key)
        {
            string text;
            if (_table != null && _table.TryGetValue(key, out text))
                return text;
            if (EnglishTable.TryGetValue(key, out text))
                return text;
            return key; // the tests make sure this never happens
        }

        public static string T(string key, params object[] args)
        {
            return string.Format(Culture, T(key), args);
        }

        // "1 second", "3 seconds" and so on, with the right plural form for the language
        public static string Seconds(int n)
        {
            string key = "Delay." + PluralOf(Name, n);
            string text;
            if (_table != null && !_table.TryGetValue(key, out text) && key == "Delay.Few")
                key = "Delay.Other";
            return T(key, n);
        }

        // CLDR plural categories, cut down to what whole numbers need
        internal static string PluralOf(string language, int n)
        {
            int mod10 = n % 10, mod100 = n % 100;
            switch (language)
            {
                case "id":
                case "ja":
                case "ko":
                case "th":
                case "vi":
                case "zh-Hans":
                case "zh-Hant":
                    return "Other"; // no plural forms
                case "ru":
                case "uk":
                    if (mod10 == 1 && mod100 != 11)
                        return "One";
                    return mod10 >= 2 && mod10 <= 4 && (mod100 < 12 || mod100 > 14) ? "Few" : "Other";
                case "pl":
                    if (n == 1)
                        return "One";
                    return mod10 >= 2 && mod10 <= 4 && (mod100 < 12 || mod100 > 14) ? "Few" : "Other";
                case "cs":
                    if (n == 1)
                        return "One";
                    return n >= 2 && n <= 4 ? "Few" : "Other";
                case "fr":
                case "pt":
                    return n == 0 || n == 1 ? "One" : "Other";
                default:
                    return n == 1 ? "One" : "Other";
            }
        }
    }
}
