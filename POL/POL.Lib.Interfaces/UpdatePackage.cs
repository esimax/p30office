namespace POL.Lib.Interfaces
{
    public class UpdatePackage
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string NewVersion { get; set; }
        public string PrerequisiteVersion { get; set; }
        public string ReleaseDate { get; set; }
    }
}
