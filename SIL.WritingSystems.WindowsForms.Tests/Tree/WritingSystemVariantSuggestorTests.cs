﻿using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SIL.WritingSystems.WindowsForms.WSTree;

namespace SIL.WritingSystems.WindowsForms.Tests.Tree
{
	[TestFixture]
	public class WritingSystemVariantSuggestorTests
	{
		[Test, Ignore("Only works if there is an ipa keyboard installed")]
		public void GetSuggestions_HasNormalLacksIpa_IpaSuggestedWhichCopiesAllRelevantFields()
		{
			var etr = new WritingSystemDefinition("etr", string.Empty, "region", "variant", "edo", true) {DefaultFont = new FontDefinition("font") {DefaultSize = 33}};
			var list = new List<WritingSystemDefinition>(new[] {etr });
			var suggestor = new WritingSystemSuggestor();
			var suggestions = suggestor.GetSuggestions(etr, list);

			WritingSystemDefinition ipa = ((WritingSystemSuggestion)suggestions.First(defn => ((WritingSystemSuggestion)defn).TemplateDefinition.Script == "ipa")).TemplateDefinition;

			Assert.That(ipa.Language, Is.EqualTo((LanguageSubtag) "etr"));
			Assert.That(ipa.Variants, Is.EqualTo(new VariantSubtag[] {"fonipa"}));
			Assert.That(ipa.LanguageName, Is.EqualTo("Edolo"));
			Assert.That(string.IsNullOrEmpty(ipa.NativeName), Is.True);
			Assert.That(ipa.Region, Is.EqualTo((RegionSubtag) "region"));
			//Assert.AreEqual("arial unicode ms", ipa.DefaultFontName); this depends on what fonts are installed on the test system
			Assert.That(ipa.DefaultFont.DefaultSize, Is.EqualTo(33));

			Assert.That(ipa.Keyboard.ToLower().Contains("ipa"), Is.True);
		}

		[Test] // ok
		public void GetSuggestions_HasNormalAndIPA_DoesNotIncludeItemToCreateIPA()
		{
			var etr = new WritingSystemDefinition("etr", string.Empty, string.Empty, string.Empty, "edo", false);
			var etrIpa = new WritingSystemDefinition("etr", string.Empty, string.Empty,  "fonipa", "edo", false);
			var list = new List<WritingSystemDefinition>(new[] { etr, etrIpa });
			var suggestor = new WritingSystemSuggestor();
			var suggestions = suggestor.GetSuggestions(etr, list);

			Assert.That(suggestions.Any(defn => ((WritingSystemSuggestion) defn).TemplateDefinition.Variants.Contains("fonipa")), Is.False);
		}

		[Test]
		public void OtherKnownWritingSystems_TokPisinDoesNotAlreadyExist_HasTokPisin()
		{
			var suggestor = new WritingSystemSuggestor();

			var existingWritingSystems = new List<WritingSystemDefinition>();
			Assert.That(suggestor.GetOtherLanguageSuggestions(existingWritingSystems).Any(ws=>ws.Label == "Tok Pisin"), Is.True);
		}

		[Test]
		public void OtherKnownWritingSystems_TokPisinAlreadyExists_DoesNotHaveTokPisin()
		{
			var suggestor = new WritingSystemSuggestor();

			var existingWritingSystems = new List<WritingSystemDefinition>{new WritingSystemDefinition("tpi")};
			Assert.That(suggestor.GetOtherLanguageSuggestions(existingWritingSystems).Any(ws => ws.Label == "Tok Pisin"), Is.False);
		}


		/// <summary>
		/// For English, it's very unlikely that they'll want to add IPA, in a app like wesay
		/// </summary>
		[Test, Category("KnownMonoIssue")]
		public void GetSuggestions_MajorWorlLanguage_SuggestsOnlyIfSuppressSuggesstionsForMajorWorldLanguagesIsFalse()
		{
			var english = new WritingSystemDefinition("en", string.Empty, string.Empty, string.Empty, "eng", false);
			var list = new List<WritingSystemDefinition>(new[] { english });
			var suggestor = new WritingSystemSuggestor();
			suggestor.SuppressSuggestionsForMajorWorldLanguages =false;
			var suggestions = suggestor.GetSuggestions(english, list);
			Assert.That(suggestions.Any(defn => ((WritingSystemSuggestion) defn).TemplateDefinition.Variants.Contains("fonipa")), Is.True);

			suggestor.SuppressSuggestionsForMajorWorldLanguages =true;
			suggestions = suggestor.GetSuggestions(english, list);
			Assert.That(suggestions.Any(defn => ((WritingSystemSuggestion) defn).TemplateDefinition.Variants.Contains("fonipa")), Is.False);
		}
	}
}
