namespace beSmarter.BioTime.TransactionApi.Options
{
	public class LogOptions
	{
		public const string Logging = "Logging";
		public const string Options = "Options";

		public virtual string? FolderPath { get; set; }
		public virtual string? FilePath { get; set; }
		//public virtual bool UseStopwatch { get;	set; }

	}
}
