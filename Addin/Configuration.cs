using MarkdownMonster.AddIns;

namespace AutoNumberAddin
{
	public class AutoNumberAddinConfiguration : BaseAddinConfiguration<AutoNumberAddinConfiguration>
	{
		public AutoNumberAddinConfiguration()
		{
			// uses this file for storing settings in `%appdata%\Markdown Monster`
			// to persist settings call `AutoNumberAddinConfiguration.Current.Write()`
			// at any time or when the addin is shut down
			ConfigurationFilename = "AutoNumberAddin.json";
		}

		public string FigureCaptionRegex { get; set; } = @".*?<figcaption>Figure (?<Number>\d{1,3})\..*";

		public string SeeFigureRegex { get; set; } = @"^.*?See Figure (?<Number>\d{1,3})\.?.*$";

		public string TableCaptionRegex { get; set; } = @"^\s*\*\*\s*Table (?<Number>\d{1,3})\.\s*\*\*.*$";

		public string SeeTableRegex { get; set; } = @"^.*?See Table (?<Number>\d{1,3})\.?.*$";

		public string ListingCaptionRegex { get; set; } = @"^\s*\*\*\s*Listing (?<Number>\d{1,3})\.\s*\*\*.*$";

		public string SeeListingRegex { get; set; } = @"^.*?See Listing (?<Number>\d{1,3})\.?.*$";

		public string IgnoreMarker { get; set; } = "[//]: # (AutoNumberIgnore)";

		public bool FigureCaptionEnabled { get; set; } = true;
		public bool ListingCaptionEnabled { get; set; } = true;
		public bool TableCaptionEnabled { get; set; } = true;

		public bool SeeFigureEnabled { get; set; } = true;
		public bool SeeListingEnabled { get; set; } = true;
		public bool SeeTableEnabled { get; set; } = true;

	}
}