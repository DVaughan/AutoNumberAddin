using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using FontAwesome.WPF;
using MarkdownMonster;
using MarkdownMonster.AddIns;

namespace AutoNumberAddin
{
	public class AutoNumberAddin : MarkdownMonster.AddIns.MarkdownMonsterAddin
	{
		public override void OnApplicationStart()
		{
			base.OnApplicationStart();

			// Id - should match output folder name
			Id = "AutoNumberAddin";

			// a descriptive name - shows up on labels and tooltips for components
			Name = "Auto Number";

			// by passing in the add in you automatically
			// hook up OnExecute/OnExecuteConfiguration/OnCanExecute
			var menuItem = new AddInMenuItem(this)
			{
				Caption = "Auto Number",

				// if an icon is specified it shows on the toolbar
				// if not the add-in only shows in the add-ins menu
				FontawesomeIcon = FontAwesomeIcon.SortNumericAsc
			};

			// if you don't want to display config or main menu item clear handler
			menuItem.ExecuteConfiguration = null;

			// Must add the menu to the collection to display menu and toolbar items            
			MenuItems.Add(menuItem);
		}

		List<Regex> GetCaptionRegexes()
		{
			var config = AutoNumberAddinConfiguration.Current;
			var list = new List<Regex>();

			AddIfEnabled(() => new Regex(config.FigureCaptionRegex, RegexOptions.Compiled | RegexOptions.Singleline), list, config.FigureCaptionEnabled);
			AddIfEnabled(() => new Regex(config.TableCaptionRegex, RegexOptions.Compiled | RegexOptions.Singleline), list, config.TableCaptionEnabled);
			AddIfEnabled(() => new Regex(config.ListingCaptionRegex, RegexOptions.Compiled | RegexOptions.Singleline), list, config.ListingCaptionEnabled);

			return list;
		}

		List<Regex> GetSeeRegexes()
		{
			var config = AutoNumberAddinConfiguration.Current;
			var list = new List<Regex>();

			AddIfEnabled(() => new Regex(config.SeeFigureRegex, RegexOptions.Compiled | RegexOptions.Singleline), list, config.SeeFigureEnabled);
			AddIfEnabled(() => new Regex(config.SeeTableRegex, RegexOptions.Compiled | RegexOptions.Singleline), list, config.SeeTableEnabled);
			AddIfEnabled(() => new Regex(config.SeeListingRegex, RegexOptions.Compiled | RegexOptions.Singleline), list, config.SeeListingEnabled);

			return list;
		}

		void AddIfEnabled(Func<Regex> regex, List<Regex> list, bool enabled)
		{
			if (enabled)
			{
				list.Add(regex());
			}
		}

		public override void OnExecute(object sender)
		{
			MarkdownDocumentEditor editor = GetMarkdownEditor();
			
			var markdown = GetMarkdown();
			string[] lines = markdown.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
			var config = AutoNumberAddinConfiguration.Current;
			string ignoreMarker = config.IgnoreMarker;

			List<LineReplacement> replacements = new List<LineReplacement>();
			
			var captionRegexes = GetCaptionRegexes();
			var seeRegexes = GetSeeRegexes();

			int[] captionIndexes = new int[captionRegexes.Count];

			int lineIndex = -1;
			int ignoreCount = 0;

			foreach (var line in lines)
			{
				lineIndex++;
				bool hasCaption = false;

				if (line.Contains(ignoreMarker))
				{
					ignoreCount++;
					continue;
				}

				for (var i = 0; i < captionRegexes.Count; i++)
				{
					Regex regex = captionRegexes[i];
					var match = regex.Match(line);
					if (match.Success)
					{
						hasCaption = true;

						if (ignoreCount > 0)
						{
							ignoreCount--;
						}
						else
						{
							captionIndexes[i]++;

							var text = ReplaceNamedGroup(match, "Number", captionIndexes[i].ToString());
							replacements.Add(new LineReplacement(lineIndex, text, line.Length));
						}
						
						break;
					}
				}

				if (hasCaption)
				{
					continue;
				}

				for (var i = 0; i < seeRegexes.Count; i++)
				{
					Regex regex = seeRegexes[i];
					Match match = regex.Match(line);
					if (match.Success)
					{
						if (ignoreCount > 0)
						{
							ignoreCount--;
						}
						else
						{
							var text = ReplaceNamedGroup(match, "Number", (captionIndexes[i] + 1).ToString());
							replacements.Add(new LineReplacement(lineIndex, text, line.Length));
						}

						break;
					}
				}
			}

//			foreach (LineReplacement replacement in replacements)
//			{
//				MarkdownDocumentEditor editor = GetMarkdownEditor();
//								var range = new { start = new  { row = replacement.LineIndex, column = 0},
//													end = new { row = replacement.LineIndex, column = replacement.LineLength } };
//								editor.AceEditor.editor.session.replace(range, replacement.Text);
//				var range = editor.AceEditor.editor.getSelectionRange(); //getLineRange(replacement.LineIndex);
//
//			}
			
			foreach (LineReplacement replacement in replacements)
			{
				lines[replacement.LineIndex] = replacement.Text;
			}

			StringBuilder sb = new StringBuilder();

			foreach (var line in lines)
			{
				sb.AppendLine(line);
			}

			string newContent = sb.ToString();
			SetMarkdown(newContent);

			bool dirty = replacements.Any();
			if (dirty)
			{
				editor.SetDirty(true);
			}
		}

//		public override void OnExecuteConfiguration(object sender)
//		{
//			MessageBox.Show("Configuration for our sample Addin", "Markdown Addin Sample",
//							MessageBoxButton.OK, MessageBoxImage.Information);
//		}

		public override bool OnCanExecute(object sender)
		{
			return true;
		}

		static string ReplaceNamedGroup(Match match, string groupName, string replacement)
		{
			string capture = match.Value;
			capture = capture.Remove(match.Groups[groupName].Index - match.Index, match.Groups[groupName].Length);
			capture = capture.Insert(match.Groups[groupName].Index - match.Index, replacement);
			return capture;
		}

		class LineReplacement
		{
			public int LineIndex { get; }
			public string Text { get; }
			public int LineLength { get; }

			public LineReplacement(int lineIndex, string text, int lineLength)
			{
				LineIndex = lineIndex;
				Text = text;
				LineLength = lineLength;
			}
		}

	}
}
