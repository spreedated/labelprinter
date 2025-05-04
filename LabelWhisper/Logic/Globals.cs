using LabelWhisper.Models;
using neXn.Lib.ConfigurationHandler;
using neXn.Ui;
using System.Collections.Generic;
using System.Reflection;

namespace LabelWhisper.Logic
{
    internal static class Globals
    {
        public static Assembly Assembly { get; } = typeof(Globals).Assembly;
        public static ConfigurationHandler<Configuration> UserConfig { get; set; }
        public static HashSet<string> IntalledPrinters { get; } = [];
        public static AppStatus<string> AppStatus { get; } = new("System ready");
    }
}
