namespace Interject.DataApi
{
    public class ApplicationOptions
    {
        public const string Application = "Application";

        public string Name { get; set; }
        public string Version { get; set; }

        public ApplicationOptions() { }

        public ApplicationOptions(ApplicationOptions options)
        {
            this.Name = options.Name;
            this.Version = options.Version;
        }
    }
}