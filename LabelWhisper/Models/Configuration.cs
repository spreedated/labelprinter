using System.Collections.Generic;

namespace LabelWhisper.Models
{
    public sealed record Configuration
    {
        public string PrinterName { get; set; } = "Brother QL-800";
        public int LabelWidth { get; set; } = 100;
        public int LabelHeight { get; set; } = 62;
        public int LastUsedTextsize { get; set; }
        public string LastUsedFreetext { get; set; }
        public List<string> LastUsedRowText { get; set; } = ["", "", "", ""];
        public bool LastUsedDrawDefault { get; set; }
        public bool LastUsedTextOption { get; set; }
    }
}
