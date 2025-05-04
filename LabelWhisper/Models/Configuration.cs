namespace LabelWhisper.Models
{
    public sealed record Configuration
    {
        public string PrinterName { get; set; } = "Brother QL-800";
        public int LabelWidth { get; set; } = 100;
        public int LabelHeight { get; set; } = 62;
        public int LastUsedTextsize { get; set; }
    }
}
