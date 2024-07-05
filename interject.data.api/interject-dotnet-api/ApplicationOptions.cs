namespace Interject.DataApi
{
    public class ApplicationOptions
    {
        public const string Application = "Application";

        public string Name { get; set; } = "Interject Data Api";
        public string Version { get; set; } = "1.1.7";
        public string Framework { get; set; } = "net8.0";
        public bool UseClientIdAsConnectionName { get; set; } = false;

        public ApplicationOptions() { }

        public ApplicationOptions(ApplicationOptions options)
        {
            this.Name = options.Name;
            this.Version = options.Version;
            this.Framework = options.Framework;
            this.UseClientIdAsConnectionName = options.UseClientIdAsConnectionName;
        }
    }
}