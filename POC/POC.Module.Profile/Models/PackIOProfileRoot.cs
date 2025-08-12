namespace POC.Module.Profile.Models
{
    public class PackIOProfileRoot
    {
        public string Title { get; set; }
        public int Order { get; set; }
        public string RoleEdit { get; set; }
        public string RoleView { get; set; }
        public string[] Cats { get; set; }

        public PackIOProfileGroup[] Groups { get; set; }
    }
}
