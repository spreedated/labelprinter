using LabelWhisper.Models;
using neXn.Lib.ConfigurationHandler;
using System.Reflection;

namespace LabelWhisper.Logic
{
    internal static class Globals
    {
        public static Assembly Assembly { get; } = typeof(Globals).Assembly;
        public static ConfigurationHandler<Configuration> UserConfig { get; set; }
    }
}
