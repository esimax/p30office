namespace POL.Lib.Interfaces
{
    public class CallStat
    {
        public string Category { get; set; }

        public decimal CountCallIn { get; set; }
        public decimal CountCallOut { get; set; }
        public decimal CountCallAll { get; set; }

        public decimal DurationCallIn { get; set; }
        public decimal DurationCallOut { get; set; }
        public decimal DurationCallAll { get; set; }
    }
}
