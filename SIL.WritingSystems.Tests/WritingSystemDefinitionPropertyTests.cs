using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Palaso.Data;
using Palaso.TestUtilities;
using SIL.WritingSystems.Migration.WritingSystemsLdmlV0To1Migration;

namespace SIL.WritingSystems.Tests
{
	public class WritingSystemDefinitionCloneableTests : CloneableTests<WritingSystemDefinition>
	{
		public override WritingSystemDefinition CreateNewCloneable()
		{
			return new WritingSystemDefinition();
		}

		protected override bool Equals(WritingSystemDefinition x, WritingSystemDefinition y)
		{
			if (x == null)
				return y == null;
			return x.ValueEquals(y);
		}

		public override string ExceptionList
		{
			// We do want to clone KnownKeyboards, but I don't think the automatic cloneable test for it can handle a list.
			get { return "|MarkedForDeletion|StoreID|_knownKeyboards|_localKeyboard|_defaultFont|_fonts|_spellCheckDictionaries|IsChanged|_matchedPairs|_punctuationPatterns|_quotationMarks|_defaultCollation|_collations|_characterSets|_variants|_language|_script|_region|_ignoreVariantChanges|PropertyChanged|"; }
		}

		protected override List<ValuesToSet> DefaultValuesForTypes
		{
			get
			{
				return new List<ValuesToSet>
							 {
								 new ValuesToSet(3.14f, 2.72f),
								 new ValuesToSet(false, true),
								 new ValuesToSet("to be", "!(to be)"),
								 new ValuesToSet(DateTime.Now, DateTime.MinValue),
								 new ValuesToSet(QuotationParagraphContinueType.None, QuotationParagraphContinueType.All),
							 };
			}
		}

		/// <summary>
		/// The generic test that clone copies everything can't handle lists.
		/// </summary>
		[Test]
		public void CloneCopiesKnownKeyboards()
		{
			var original = new WritingSystemDefinition();
			var kbd1 = new DefaultKeyboardDefinition("mine", string.Empty);
			var kbd2 = new DefaultKeyboardDefinition("yours", string.Empty);
			original.KnownKeyboards.Add(kbd1);
			original.KnownKeyboards.Add(kbd2);
			WritingSystemDefinition copy = original.Clone();
			Assert.That(copy.KnownKeyboards.Count, Is.EqualTo(2));
			Assert.That(copy.KnownKeyboards[0], Is.EqualTo(kbd1));
			Assert.That(copy.KnownKeyboards[0] == kbd1, Is.True);
		}

		[Test]
		public void CloneCopiesFonts()
		{
			var original = new WritingSystemDefinition();
			var fd1 = new FontDefinition("font1");
			var fd2 = new FontDefinition("font2");
			original.Fonts.Add(fd1);
			original.Fonts.Add(fd2);
			original.DefaultFont = fd2;
			WritingSystemDefinition copy = original.Clone();
			Assert.That(copy.Fonts.Count, Is.EqualTo(2));
			Assert.That(copy.Fonts[0].ValueEquals(fd1), Is.True);
			Assert.That(ReferenceEquals(copy.Fonts[0], fd1), Is.False);
			Assert.That(copy.DefaultFont.ValueEquals(fd2), Is.True);
			Assert.That(copy.DefaultFont == fd2, Is.False);
			Assert.That(copy.DefaultFont == copy.Fonts[1], Is.True);
		}

		[Test]
		public void CloneCopiesSpellCheckDictionaries()
		{
			var original = new WritingSystemDefinition();
			var scdd1 = new SpellCheckDictionaryDefinition(SpellCheckDictionaryFormat.Hunspell);
			var scdd2 = new SpellCheckDictionaryDefinition(SpellCheckDictionaryFormat.Lift);
			original.SpellCheckDictionaries.Add(scdd1);
			original.SpellCheckDictionaries.Add(scdd2);
			WritingSystemDefinition copy = original.Clone();
			Assert.That(copy.SpellCheckDictionaries.Count, Is.EqualTo(2));
			Assert.That(copy.SpellCheckDictionaries.First().ValueEquals(scdd1), Is.True);
			Assert.That(ReferenceEquals(copy.SpellCheckDictionaries.First(), scdd1), Is.False);
		}

		[Test]
		public void CloneCopiesMatchedPairs()
		{
			var original = new WritingSystemDefinition();
			var mp1 = new MatchedPair("(", ")", false);
			var mp2 = new MatchedPair("[", "]", false);
			original.MatchedPairs.Add(mp1);
			original.MatchedPairs.Add(mp2);
			WritingSystemDefinition copy = original.Clone();
			Assert.That(copy.MatchedPairs.Count, Is.EqualTo(2));
			Assert.That(copy.MatchedPairs, Is.EquivalentTo(new[] { mp1, mp2 }));
		}

		[Test]
		public void CloneCopiesPunctuationPatterns()
		{
			var original = new WritingSystemDefinition();
			var pp1 = new PunctuationPattern("?", PunctuationPatternContext.Final);
			var pp2 = new PunctuationPattern(":", PunctuationPatternContext.Final);
			original.PunctuationPatterns.Add(pp1);
			original.PunctuationPatterns.Add(pp2);
			WritingSystemDefinition copy = original.Clone();
			Assert.That(copy.PunctuationPatterns.Count, Is.EqualTo(2));
			Assert.That(copy.PunctuationPatterns, Is.EquivalentTo(new[] { pp1, pp2 }));
		}

		[Test]
		public void CloneCopiesQuotationMarks()
		{
			var original = new WritingSystemDefinition();
			var qm1 = new QuotationMark("�", "�", null, 1, QuotationMarkingSystemType.Narrative);
			var qm2 = new QuotationMark("�", "�", null, 3, QuotationMarkingSystemType.Normal);
			original.QuotationMarks.Add(qm1);
			original.QuotationMarks.Add(qm2);
			WritingSystemDefinition copy = original.Clone();
			Assert.That(copy.QuotationMarks.Count, Is.EqualTo(2));
			Assert.That(copy.QuotationMarks, Is.EqualTo(new[] { qm1, qm2 }));
		}

		[Test]
		public void CloneCopiesCollations()
		{
			var original = new WritingSystemDefinition();
			var cd1 = new CollationDefinition("standard");
			var cd2 = new CollationDefinition("other");
			original.Collations.Add(cd1);
			original.Collations.Add(cd2);
			original.DefaultCollation = cd2;
			WritingSystemDefinition copy = original.Clone();
			Assert.That(copy.Collations.Count, Is.EqualTo(2));
			Assert.That(copy.Collations[0].ValueEquals(cd1), Is.True);
			Assert.That(ReferenceEquals(copy.Collations[0], cd1), Is.False);
			Assert.That(copy.DefaultCollation.ValueEquals(cd2), Is.True);
			Assert.That(copy.DefaultCollation == cd2, Is.False);
			Assert.That(copy.DefaultCollation == copy.Collations[1], Is.True);
		}

		[Test]
		public void CloneCopiesCharacterSets()
		{
			var original = new WritingSystemDefinition();
			var cs1 = new CharacterSetDefinition("cs1");
			var cs2 = new CharacterSetDefinition("cs2");
			original.CharacterSets.Add(cs1);
			original.CharacterSets.Add(cs2);
			WritingSystemDefinition copy = original.Clone();
			Assert.That(copy.CharacterSets.Count, Is.EqualTo(2));
			Assert.That(copy.CharacterSets[0].ValueEquals(cs1), Is.True);
			Assert.That(ReferenceEquals(copy.CharacterSets[0], cs1), Is.False);
		}

		[Test]
		public void CloneCopiesVariants()
		{
			var original = new WritingSystemDefinition();
			original.Variants.Add("1901");
			original.Variants.Add("biske");
			WritingSystemDefinition copy = original.Clone();
			Assert.That(copy.Variants, Is.EqualTo(new VariantSubtag[] {"1901", "biske"}));
		}

		/// <summary>
		/// The generic test that ValueEquals compares everything can't handle lists.
		/// </summary>
		[Test]
		public void ValueEqualsComparesKnownKeyboards()
		{
			var first = new WritingSystemDefinition();
			var kbd1 = new DefaultKeyboardDefinition("mine", string.Empty);
			var kbd2 = new DefaultKeyboardDefinition("yours", string.Empty);
			first.KnownKeyboards.Add(kbd1);
			first.KnownKeyboards.Add(kbd2);
			var second = new WritingSystemDefinition();
			var kbd3 = new DefaultKeyboardDefinition("theirs", string.Empty);

			Assert.That(first.ValueEquals(second), Is.False, "ws with empty known keyboards should not equal one with some");
			second.KnownKeyboards.Add(kbd1);
			Assert.That(first.ValueEquals(second), Is.False, "ws's with different length known keyboard lists should not be equal");
			second.KnownKeyboards.Add(kbd2);
			Assert.That(first.ValueEquals(second), Is.True, "ws's with same known keyboard lists should be equal");

			second.KnownKeyboards.Clear();
			second.KnownKeyboards.Add(kbd1);
			second.KnownKeyboards.Add(kbd3);
			Assert.That(first.ValueEquals(second), Is.False, "ws with same-length lists of different known keyboards should not be equal");
		}

		[Test]
		public void ValueEqualsComparesFonts()
		{
			var first = new WritingSystemDefinition();
			var fd1 = new FontDefinition("font1");
			var fd2 = new FontDefinition("font2");
			first.Fonts.Add(fd1);
			first.Fonts.Add(fd2);
			var second = new WritingSystemDefinition();
			var fd3 = new FontDefinition("font1");
			var fd4 = new FontDefinition("font3");

			Assert.That(first.ValueEquals(second), Is.False, "ws with empty fonts should not equal one with some");
			second.Fonts.Add(fd3);
			Assert.That(first.ValueEquals(second), Is.False, "ws's with different length font lists should not be equal");
			second.Fonts.Add(fd2.Clone());
			Assert.That(first.ValueEquals(second), Is.True, "ws's with same font lists should be equal");
			second.DefaultFont = second.Fonts[0];
			Assert.That(first.ValueEquals(second), Is.True);
			second.DefaultFont = second.Fonts[1];
			Assert.That(first.ValueEquals(second), Is.False);

			second = new WritingSystemDefinition();
			second.Fonts.Add(fd3);
			second.Fonts.Add(fd4);
			Assert.That(first.ValueEquals(second), Is.False, "ws with same-length lists of different fonts should not be equal");
		}

		[Test]
		public void ValueEqualsComparesSpellCheckDictionaries()
		{
			var first = new WritingSystemDefinition();
			var scdd1 = new SpellCheckDictionaryDefinition(SpellCheckDictionaryFormat.Hunspell);
			var scdd2 = new SpellCheckDictionaryDefinition(SpellCheckDictionaryFormat.Lift);
			first.SpellCheckDictionaries.Add(scdd1);
			first.SpellCheckDictionaries.Add(scdd2);
			var second = new WritingSystemDefinition();
			var scdd3 = new SpellCheckDictionaryDefinition(SpellCheckDictionaryFormat.Hunspell);
			var scdd4 = new SpellCheckDictionaryDefinition(SpellCheckDictionaryFormat.Wordlist);

			Assert.That(first.ValueEquals(second), Is.False, "ws with empty dictionaries should not equal one with some");
			second.SpellCheckDictionaries.Add(scdd3);
			Assert.That(first.ValueEquals(second), Is.False, "ws's with different length dictionary lists should not be equal");
			second.SpellCheckDictionaries.Add(scdd2.Clone());
			Assert.That(first.ValueEquals(second), Is.True, "ws's with same dictionary lists should be equal");

			second = new WritingSystemDefinition();
			second.SpellCheckDictionaries.Add(scdd3);
			second.SpellCheckDictionaries.Add(scdd4);
			Assert.That(first.ValueEquals(second), Is.False, "ws with same-length lists of different dictionaries should not be equal");
		}

		[Test]
		public void ValueEqualsComparesMatchedPairs()
		{
			var first = new WritingSystemDefinition();
			var mp1 = new MatchedPair("(", ")", false);
			var mp2 = new MatchedPair("[", "]", false);
			first.MatchedPairs.Add(mp1);
			first.MatchedPairs.Add(mp2);
			var second = new WritingSystemDefinition();
			var mp3 = new MatchedPair("{", "}", false);

			Assert.That(first.ValueEquals(second), Is.False, "ws with empty matched pairs should not equal one with some");
			second.MatchedPairs.Add(mp1);
			Assert.That(first.ValueEquals(second), Is.False, "ws's with different length matched pair lists should not be equal");
			second.MatchedPairs.Add(mp2);
			Assert.That(first.ValueEquals(second), Is.True, "ws's with same matched pair lists should be equal");

			second.MatchedPairs.Clear();
			second.MatchedPairs.Add(mp1);
			second.MatchedPairs.Add(mp3);
			Assert.That(first.ValueEquals(second), Is.False, "ws with same-length lists of different matched pairs should not be equal");
		}

		[Test]
		public void ValueEqualsComparesPunctuationPatterns()
		{
			var first = new WritingSystemDefinition();
			var pp1 = new PunctuationPattern("?", PunctuationPatternContext.Final);
			var pp2 = new PunctuationPattern(":", PunctuationPatternContext.Final);
			first.PunctuationPatterns.Add(pp1);
			first.PunctuationPatterns.Add(pp2);
			var second = new WritingSystemDefinition();
			var pp3 = new PunctuationPattern(",", PunctuationPatternContext.Final);

			Assert.That(first.ValueEquals(second), Is.False, "ws with empty punctuation patterns should not equal one with some");
			second.PunctuationPatterns.Add(pp1);
			Assert.That(first.ValueEquals(second), Is.False, "ws's with different length punctuation pattern lists should not be equal");
			second.PunctuationPatterns.Add(pp2);
			Assert.That(first.ValueEquals(second), Is.True, "ws's with same punctuation pattern lists should be equal");

			second.PunctuationPatterns.Clear();
			second.PunctuationPatterns.Add(pp1);
			second.PunctuationPatterns.Add(pp3);
			Assert.That(first.ValueEquals(second), Is.False, "ws with same-length lists of different punctuation patterns should not be equal");
		}

		[Test]
		public void ValueEqualsComparesQuotationMarks()
		{
			var first = new WritingSystemDefinition();
			var qm1 = new QuotationMark("�", "�", null, 3, QuotationMarkingSystemType.Narrative);
			var qm2 = new QuotationMark("�", "�", null, 1, QuotationMarkingSystemType.Normal);
			first.QuotationMarks.Add(qm1);
			first.QuotationMarks.Add(qm2);
			var second = new WritingSystemDefinition();
			var qm3 = new QuotationMark("{", "}", null, 3, QuotationMarkingSystemType.Narrative);

			Assert.That(first.ValueEquals(second), Is.False, "ws with empty quotation marks should not equal one with some");
			second.QuotationMarks.Add(qm1);
			Assert.That(first.ValueEquals(second), Is.False, "ws's with different length quotation mark lists should not be equal");
			second.QuotationMarks.Add(qm2);
			Assert.That(first.ValueEquals(second), Is.True, "ws's with same quotation mark lists should be equal");

			second.QuotationMarks.Clear();
			second.QuotationMarks.Add(qm1);
			second.QuotationMarks.Add(qm3);
			Assert.That(first.ValueEquals(second), Is.False, "ws with same-length lists of different quotation marks should not be equal");
		}

		[Test]
		public void ValueEqualsComparesCollations()
		{
			var first = new WritingSystemDefinition();
			var cd1 = new CollationDefinition("standard");
			var cd2 = new CollationDefinition("other1");
			first.Collations.Add(cd1);
			first.Collations.Add(cd2);
			var second = new WritingSystemDefinition();
			var cd3 = new CollationDefinition("standard");
			var cd4 = new CollationDefinition("other2");

			Assert.That(first.ValueEquals(second), Is.False, "ws with empty collations should not equal one with some");
			second.Collations.Add(cd3);
			Assert.That(first.ValueEquals(second), Is.False, "ws's with different length collation lists should not be equal");
			second.Collations.Add(cd2.Clone());
			Assert.That(first.ValueEquals(second), Is.True, "ws's with same collation lists should be equal");
			second.DefaultCollation = second.Collations[0];
			Assert.That(first.ValueEquals(second), Is.True);
			second.DefaultCollation = second.Collations[1];
			Assert.That(first.ValueEquals(second), Is.False);

			second = new WritingSystemDefinition();
			second.Collations.Add(cd3);
			second.Collations.Add(cd4);
			Assert.That(first.ValueEquals(second), Is.False, "ws with same-length lists of different collations should not be equal");
		}

		[Test]
		public void ValueEqualsComparesCharacterSets()
		{
			var first = new WritingSystemDefinition();
			var cs1 = new CharacterSetDefinition("cs1");
			var cs2 = new CharacterSetDefinition("cs2");
			first.CharacterSets.Add(cs1);
			first.CharacterSets.Add(cs2);
			var second = new WritingSystemDefinition();
			var cs3 = new CharacterSetDefinition("cs1");
			var cs4 = new CharacterSetDefinition("cs3");

			Assert.That(first.ValueEquals(second), Is.False, "ws with empty character sets should not equal one with some");
			second.CharacterSets.Add(cs3);
			Assert.That(first.ValueEquals(second), Is.False, "ws's with different length character set lists should not be equal");
			second.CharacterSets.Add(cs2.Clone());
			Assert.That(first.ValueEquals(second), Is.True, "ws's with same character set lists should be equal");

			second = new WritingSystemDefinition();
			second.CharacterSets.Add(cs3);
			second.CharacterSets.Add(cs4);
			Assert.That(first.ValueEquals(second), Is.False, "ws with same-length lists of different character sets should not be equal");
		}

		[Test]
		public void ValueEqualsComparesVariants()
		{
			var first = new WritingSystemDefinition();
			first.Variants.Add("1901");
			first.Variants.Add("biske");
			var second = new WritingSystemDefinition();

			Assert.That(first.ValueEquals(second), Is.False, "ws with empty variants should not equal one with some");
			second.Variants.Add("1901");
			Assert.That(first.ValueEquals(second), Is.False, "ws's with different length variant lists should not be equal");
			second.Variants.Add("biske");
			Assert.That(first.ValueEquals(second), Is.True, "ws's with same variant lists should be equal");

			second.Variants.Clear();
			second.Variants.Add("1901");
			second.Variants.Add("fonipa");
			Assert.That(first.ValueEquals(second), Is.False, "ws with same-length lists of different variants should not be equal");
		}
	}

	[TestFixture]
	public class WritingSystemDefinitionPropertyTests
	{
		[Test]
		public void Constructor_AllArgs_SetsOk()
		{
			var ws = new WritingSystemDefinition("en", "Latn", "US", "x-whatever");
			Assert.That(ws.Language, Is.EqualTo((LanguageSubtag) "en"));
			Assert.That(ws.Script, Is.EqualTo((ScriptSubtag) "Latn"));
			Assert.That(ws.Region, Is.EqualTo((RegionSubtag) "US"));
			Assert.That(ws.Variants, Is.EqualTo(new VariantSubtag[] {"whatever"}));
		}

		private void AssertWritingSystem(WritingSystemDefinition wsDef, string language, string script, string region, string variant)
		{
			Assert.That(wsDef.Language, Is.EqualTo((LanguageSubtag) language));
			Assert.That(wsDef.Script, Is.EqualTo((ScriptSubtag) script));
			Assert.That(wsDef.Region, Is.EqualTo((RegionSubtag) region));
			IEnumerable<VariantSubtag> variantSubtags;
			Assert.That(IetfLanguageTag.TryGetVariantSubtags(variant, out variantSubtags));
			Assert.That(wsDef.Variants, Is.EqualTo(variantSubtags));
		}

		[Test]
		public void Constructor_HasOnlyPrivateUse_WritingSystemHasExpectedFields()
		{
			var tag = new WritingSystemDefinition("x-privuse");
			AssertWritingSystem(tag, "privuse", string.Empty, string.Empty, string.Empty);
		}

		[Test]
		public void Constructor_HasMultiplePrivateUse_WritingSystemHasExpectedFields()
		{
			var tag = new WritingSystemDefinition("x-private-use");
			AssertWritingSystem(tag, "private", string.Empty, string.Empty, "x-use");
		}

		[Test]
		public void Constructor_HasLanguage_WritingSystemHasExpectedFields()
		{
			var tag = new WritingSystemDefinition("de");
			AssertWritingSystem(tag, "de", string.Empty, string.Empty, string.Empty);
		}

		[Test]
		public void Constructor_HasLanguageAndScript_WritingSystemHasExpectedFields()
		{
			var tag = new WritingSystemDefinition("en-Latn");
			AssertWritingSystem(tag, "en", "Latn", string.Empty, string.Empty);
		}

		[Test]
		public void Constructor_HasLanguageAndScriptAndRegion_WritingSystemHasExpectedFields()
		{
			var tag = new WritingSystemDefinition("en-Latn-US");
			AssertWritingSystem(tag, "en", "Latn", "US", string.Empty);
		}

		[Test]
		public void Constructor_HasLanguageAndScriptAndRegionAndVariant_WritingSystemHasExpectedFields()
		{
			var tag = new WritingSystemDefinition("de-Latn-DE-1901");
			AssertWritingSystem(tag, "de", "Latn", "DE", "1901");
		}

		[Test]
		public void Constructor_HasLanguageAndScriptAndRegionAndMultipleVariants_WritingSystemHasExpectedFields()
		{
			var tag = new WritingSystemDefinition("de-Latn-DE-1901-bauddha");
			AssertWritingSystem(tag, "de", "Latn", "DE", "1901-bauddha");
		}

		[Test]
		public void Constructor_HasLanguageAndScriptAndRegionAndMultipleVariantsAndPrivateUse_WritingSystemHasExpectedFields()
		{
			var tag = new WritingSystemDefinition("de-Latn-DE-1901-bauddha-x-private");
			AssertWritingSystem(tag, "de", "Latn", "DE", "1901-bauddha-x-private");
		}

		[Test]
		public void Constructor_HasLanguageAndScriptAndRegionAndMultipleVariantsAndMultiplePrivateUse_WritingSystemHasExpectedFields()
		{
			var tag = new WritingSystemDefinition("de-Latn-DE-1901-bauddha-x-private-use");
			AssertWritingSystem(tag, "de", "Latn", "DE", "1901-bauddha-x-private-use");
		}

		[Test]
		public void Constructor_HasLanguageAndScriptAndRegionAndVariantAndMultiplePrivateUse_WritingSystemHasExpectedFields()
		{
			var tag = new WritingSystemDefinition("de-Latn-DE-bauddha-x-private-use");
			AssertWritingSystem(tag, "de", "Latn", "DE", "bauddha-x-private-use");
		}

		[Test]
		public void Constructor_HasLanguageAndRegionAndVariantAndMultiplePrivateUse_WritingSystemHasExpectedFields()
		{
			var tag = new WritingSystemDefinition("de-DE-bauddha-x-private-use");
			AssertWritingSystem(tag, "de", string.Empty, "DE", "bauddha-x-private-use");
		}

		[Test]
		public void Constructor_HasLanguageAndVariant_WritingSystemHasExpectedFields()
		{
			var tag = new WritingSystemDefinition("en-alalc97");
			AssertWritingSystem(tag, "en", string.Empty, string.Empty, "alalc97");
		}

		[Test]
		public void Constructor_HasBadSubtag_Throws()
		{
			Assert.Throws<ArgumentException>(() => new WritingSystemDefinition("qaa-dupl1"));
		}

		[Test]
		public void Constructor_HasLanguageAndMultipleVariants_WritingSystemHasExpectedFields()
		{
			var tag = new WritingSystemDefinition("en-alalc97-aluku");
			AssertWritingSystem(tag, "en", null, null, "alalc97-aluku");
		}

		[Test]
		public void Constructor_HasLanguageAndRegion_WritingSystemHasExpectedFields()
		{
			var tag = new WritingSystemDefinition("en-US");
			AssertWritingSystem(tag, "en", string.Empty, "US", string.Empty);
		}

		[Test]
		public void Constructor_HasLanguageAndPrivateUse_WritingSystemHasExpectedFields()
		{
			var tag = new WritingSystemDefinition("en-x-private-use");
			AssertWritingSystem(tag, "en", string.Empty, String.Empty, "x-private-use");
		}

		[Test]
		public void Constructor_HasLanguageAndVariantAndPrivateUse_WritingSystemHasExpectedFields()
		{
			var tag = new WritingSystemDefinition("en-1901-bauddha-x-private-use");
			AssertWritingSystem(tag, "en", string.Empty, String.Empty, "1901-bauddha-x-private-use");
		}

		[Test]
		public void Constructor_HasLanguageAndRegionAndPrivateUse_WritingSystemHasExpectedFields()
		{
			var tag = new WritingSystemDefinition("en-US-x-private-use");
			AssertWritingSystem(tag, "en", string.Empty, "US", "x-private-use");
		}

		[Test]
		public void Constructor_HasLanguageAndRegionAndVariant_WritingSystemHasExpectedFields()
		{
			var tag = new WritingSystemDefinition("en-US-1901-bauddha");
			AssertWritingSystem(tag, "en", string.Empty, "US", "1901-bauddha");
		}

		[Test]
		public void Constructor_HasLanguageAndRegionAndVariantAndPrivateUse_WritingSystemHasExpectedFields()
		{
			var tag = new WritingSystemDefinition("en-US-1901-bauddha-x-private-use");
			AssertWritingSystem(tag, "en", string.Empty, "US", "1901-bauddha-x-private-use");
		}

		[Test]
		public void Constructor_HasLanguageAndScriptAndPrivateUse_WritingSystemHasExpectedFields()
		{
			var tag = new WritingSystemDefinition("en-Latn-x-private-use");
			AssertWritingSystem(tag, "en", "Latn", String.Empty, "x-private-use");
		}

		[Test]
		public void Constructor_HasLanguageAndScriptAndRegionAndPrivateUse_WritingSystemHasExpectedFields()
		{
			var tag = new WritingSystemDefinition("en-Latn-US-x-private-use");
			AssertWritingSystem(tag, "en", "Latn", "US", "x-private-use");
		}

		[Test]
		public void DisplayLabelWhenUnknown()
		{
			var ws = new WritingSystemDefinition();
			Assert.AreEqual("???", ws.DisplayLabel);
		}

		[Test]
		public void DisplayLabel_NoAbbreviation_UsesRFC5646()
		{
			var ws = new WritingSystemDefinition();
			ws.Language = "en";
			ws.Variants.Add("1901");
			Assert.AreEqual("en-1901", ws.DisplayLabel);
		}

		[Test]
		public void DisplayLabel_LanguageTagIsDefaultHasAbbreviation_ShowsAbbreviation()
		{
			var ws = new WritingSystemDefinition();
			ws.Abbreviation = "xyz";
			Assert.AreEqual("xyz", ws.DisplayLabel);
		}

		[Test]
		public void Variant_ConsistsOnlyOfRfc5646Variant_VariantIsSetCorrectly()
		{
			var ws = new WritingSystemDefinition();
			ws.Variants.Add("fonipa");
			Assert.That(ws.Variants, Is.EqualTo(new VariantSubtag[] {"fonipa"}));
		}

		[Test]
		public void Variant_ConsistsOnlyOfRfc5646PrivateUse_VariantIsSetCorrectly()
		{
			var ws = new WritingSystemDefinition();
			ws.Variants.Add("test");
			Assert.That(ws.Variants, Is.EqualTo(new VariantSubtag[] {"test"}));
		}

		[Test]
		public void Variant_ConsistsOfBothRfc5646VariantandprivateUse_VariantIsSetCorrectly()
		{
			var ws = new WritingSystemDefinition();
			ws.Variants.Add("fonipa");
			ws.Variants.Add("etic");
			Assert.That(ws.Variants, Is.EqualTo(new VariantSubtag[] {"fonipa", "etic"}));
		}

		[Test]
		public void DisplayLabel_OnlyHasLanguageName_UsesFirstPartOfLanguageName()
		{
			var ws = new WritingSystemDefinition();
			ws.LanguageName = "abcdefghijk";
			Assert.AreEqual("abcd", ws.DisplayLabel);
		}

		[Test]
		public void InvalidTagOkWhenRequiresValidTagFalse()
		{
			var ws = new WritingSystemDefinition();
			ws.RequiresValidLanguageTag = false;
			ws.Language = "Kalaba";
			Assert.That(ws.Language, Is.EqualTo((LanguageSubtag) "Kalaba"));
		}

		[Test]
		public void DuplicatePrivateUseOkWhenRequiresValidTagFalse()
		{
			var ws = new WritingSystemDefinition();
			ws.RequiresValidLanguageTag = false;
			ws.Variants.Add("nong");
			ws.Variants.Add("nong");
			Assert.That(ws.Variants, Is.EqualTo(new VariantSubtag[] {"nong", "nong"}));
		}

		[Test]
		public void InvalidTagThrowsWhenRequiresValidTagSetToTrue()
		{
			var ws = new WritingSystemDefinition();
			ws.RequiresValidLanguageTag = false;
			ws.Language = "InvalidLanguage";
			Assert.Throws<ValidationException>(() => ws.RequiresValidLanguageTag = true);
		}

		[Test]
		public void DuplicatePrivateUseThrowsWhenRequiresValidTagSetToTrue()
		{
			var ws = new WritingSystemDefinition();
			ws.RequiresValidLanguageTag = false;
			ws.Variants.Add("nong");
			ws.Variants.Add("nong");
			Assert.Throws<ValidationException>(() => ws.RequiresValidLanguageTag = true);
		}

		[Test]
		public void LanguageName_Default_ReturnsUnknownLanguage()
		{
			var ws = new WritingSystemDefinition();
			Assert.AreEqual("Language Not Listed", ws.LanguageName);
		}

		[Test]
		public void LanguageName_SetLanguageEn_ReturnsEnglish()
		{
			var ws = new WritingSystemDefinition();
			ws.Language = "en";
			Assert.AreEqual("English", ws.LanguageName);
		}

		[Test]
		public void LanguageName_SetCustom_ReturnsCustomName()
		{
			var ws = new WritingSystemDefinition();
			ws.LanguageName = "CustomName";
			Assert.AreEqual("CustomName", ws.LanguageName);
		}

		[Test]
		public void Rfc5646_HasOnlyAbbreviation_ReturnsQaa()
		{
			var ws = new WritingSystemDefinition {Abbreviation = "hello"};
			Assert.AreEqual("qaa", ws.LanguageTag);
		}

		[Test]
		public void LanguageTagWhenJustIso()
		{
			var ws = new WritingSystemDefinition("en","","","");
			Assert.AreEqual("en", ws.LanguageTag);
		}
		[Test]
		public void LanguageTagWhenIsoAndScript()
		{
			var ws = new WritingSystemDefinition("en", "Zxxx", "", "");
			Assert.AreEqual("en-Zxxx", ws.LanguageTag);
		}

		[Test]
		public void LanguageTagWhenIsoAndRegion()
		{
			var ws = new WritingSystemDefinition("en", "", "US", "");
			Assert.AreEqual("en-US", ws.LanguageTag);
		}
		[Test]
		public void LanguageTagWhenIsoScriptRegionVariant()
		{
			var ws = new WritingSystemDefinition("en", "Zxxx", "US", "1901");
			Assert.AreEqual("en-Zxxx-US-1901", ws.LanguageTag);
		}

		[Test]
		public void Constructor_OnlyVariantContainingOnlyPrivateUseisPassedIn_LangTagConsistsOfOnlyPrivateUse()
		{
			var ws = new WritingSystemDefinition("", "", "", "x-private");
			Assert.That(ws.LanguageTag, Is.EqualTo("x-private"));
		}

		[Test]
		public void Constructor_OnlyPrivateUseIsPassedIn_LangTagConsistsOfOnlyPrivateUse()
		{
			var ws = new WritingSystemDefinition("x-private");
			Assert.AreEqual("x-private", ws.LanguageTag);
		}

		[Test]
		public void Constructor_OnlyVariantIsPassedIn_Throws()
		{
			Assert.Throws<ValidationException>(() => new WritingSystemDefinition("", "", "", "bogus"));
		}

		[Test]
		public void ReadsIsoRegistry()
		{
			Assert.Greater(StandardSubtags.Iso639Languages.Count, 100);
		}

		[Test]
		public void ModifyingDefinitionSetIsChangedFlag()
		{
			// Put any properties to ignore in this string surrounded by "|"
			// ObsoleteWindowsLcid has no public setter; it only gets a value by reading from an old file.
			const string ignoreProperties = "|MarkedForDeletion|StoreID|DateModified|RequiresValidLanguageTag|";
			// special test values to use for properties that are particular
			var firstValueSpecial = new Dictionary<string, object>();
			var secondValueSpecial = new Dictionary<string, object>();
			firstValueSpecial.Add("Iso639", "en");
			secondValueSpecial.Add("Iso639", "de");
			firstValueSpecial.Add("DuplicateNumber", 0);
			secondValueSpecial.Add("DuplicateNumber", 1);
			firstValueSpecial.Add("LocalKeyboard", new DefaultKeyboardDefinition("mine", string.Empty));
			secondValueSpecial.Add("LocalKeyboard", new DefaultKeyboardDefinition("yours", string.Empty));
			//firstValueSpecial.Add("SortUsing", "CustomSimple");
			//secondValueSpecial.Add("SortUsing", "CustomICU");
			// test values to use based on type
			var firstValueToSet = new Dictionary<Type, object>();
			var secondValueToSet = new Dictionary<Type, object>();
			firstValueToSet.Add(typeof (float), 2.18281828459045f);
			secondValueToSet.Add(typeof (float), 3.141592653589f);
			firstValueToSet.Add(typeof (bool), true);
			secondValueToSet.Add(typeof (bool), false);
			firstValueToSet.Add(typeof (string), "X");
			secondValueToSet.Add(typeof (string), "Y");
			firstValueToSet.Add(typeof (DateTime), new DateTime(2007, 12, 31));
			secondValueToSet.Add(typeof (DateTime), new DateTime(2008, 1, 1));
			firstValueToSet.Add(typeof(Rfc5646Tag), new Rfc5646Tag("de", "Latn", "", "1901","audio"));
			firstValueToSet.Add(typeof(IpaStatusChoices), IpaStatusChoices.IpaPhonemic);
			secondValueToSet.Add(typeof(IpaStatusChoices), IpaStatusChoices.NotIpa);
			firstValueToSet.Add(typeof(FontDefinition), new FontDefinition("font1"));
			secondValueToSet.Add(typeof(FontDefinition), new FontDefinition("font2"));
			firstValueToSet.Add(typeof(SpellCheckDictionaryDefinition), new SpellCheckDictionaryDefinition(SpellCheckDictionaryFormat.Hunspell));
			secondValueToSet.Add(typeof(SpellCheckDictionaryDefinition), new SpellCheckDictionaryDefinition(SpellCheckDictionaryFormat.Lift));
			firstValueToSet.Add(typeof(QuotationParagraphContinueType), QuotationParagraphContinueType.None);
			secondValueToSet.Add(typeof(QuotationParagraphContinueType), QuotationParagraphContinueType.All);
			firstValueToSet.Add(typeof(CollationDefinition), new CollationDefinition("standard"));
			secondValueToSet.Add(typeof(CollationDefinition), new SimpleCollationDefinition("standard"));
			firstValueToSet.Add(typeof(LanguageSubtag), new LanguageSubtag("en", false));
			secondValueToSet.Add(typeof(LanguageSubtag), new LanguageSubtag("de", false));
			firstValueToSet.Add(typeof(ScriptSubtag), new ScriptSubtag("Latn", false));
			secondValueToSet.Add(typeof(ScriptSubtag), new ScriptSubtag("Armi", false));
			firstValueToSet.Add(typeof(RegionSubtag), new RegionSubtag("US", false));
			secondValueToSet.Add(typeof(RegionSubtag), new RegionSubtag("GB", false));

			foreach (PropertyInfo propertyInfo in typeof(WritingSystemDefinition).GetProperties(BindingFlags.Public | BindingFlags.Instance))
			{
				// skip read-only or ones in the ignore list
				if (!propertyInfo.CanWrite || ignoreProperties.Contains("|" + propertyInfo.Name + "|"))
				{
					continue;
				}
				var ws = new WritingSystemDefinition();
				ws.AcceptChanges();
				// We need to ensure that all values we are setting are actually different than the current values.
				// This could be accomplished by comparing with the current value or by setting twice with different values.
				// We use the setting twice method so we don't require a getter on the property.
				try
				{
					if (firstValueSpecial.ContainsKey(propertyInfo.Name) && secondValueSpecial.ContainsKey(propertyInfo.Name))
					{
						propertyInfo.SetValue(ws, firstValueSpecial[propertyInfo.Name], null);
						propertyInfo.SetValue(ws, secondValueSpecial[propertyInfo.Name], null);
					}
					else if (firstValueToSet.ContainsKey(propertyInfo.PropertyType) && secondValueToSet.ContainsKey(propertyInfo.PropertyType))
					{
						propertyInfo.SetValue(ws, firstValueToSet[propertyInfo.PropertyType], null);
						propertyInfo.SetValue(ws, secondValueToSet[propertyInfo.PropertyType], null);
					}
					else
					{
						Assert.Fail("Unhandled property type - please update the test to handle type {0}",
									propertyInfo.PropertyType.Name);
					}
				}
				catch (Exception error)
				{
					Assert.Fail("Error setting property WritingSystemDefinition.{0},{1}", propertyInfo.Name, error);
				}
				Assert.IsTrue(ws.IsChanged, "Modifying WritingSystemDefinition.{0} did not change modified flag.", propertyInfo.Name);
			}
		}

		[Test]
		public void SetIsVoice_SetToTrue_SetsScriptRegionAndVariantCorrectly()
		{
			var ws = new WritingSystemDefinition
					 {
						 Script = "Latn",
						 Region = "US",
						 Variants = {"1901"},
						 IsVoice = true
					 };
			Assert.That(ws.Script, Is.EqualTo((ScriptSubtag) WellKnownSubtags.AudioScript));
			Assert.That(ws.Region, Is.EqualTo((RegionSubtag) "US"));
			Assert.That(ws.Variants, Is.EqualTo(new VariantSubtag[] {"1901", "audio"}));
			Assert.AreEqual("qaa-Zxxx-US-1901-x-audio", ws.LanguageTag);
		}

		[Test]
		public void SetIsVoice_ToTrue_LeavesIsoCodeAlone()
		{
			var ws = new WritingSystemDefinition
					 {
						 Language = "en",
						 IsVoice = true
					 };
			Assert.That(ws.Language, Is.EqualTo((LanguageSubtag) "en"));
		}

		[Test]
		public void SetVoice_FalseFromTrue_ClearsScript()
		{
			var ws = new WritingSystemDefinition();
			ws.IsVoice = true;
			ws.IsVoice = false;
			Assert.That(ws.Script, Is.Null);
			Assert.That(ws.Region, Is.Null);
			Assert.That(ws.Variants, Is.Empty);
		}

		[Test]
		public void Script_ChangedToSomethingOtherThanZxxxWhileIsVoiceIsTrue_Throws()
		{
			var ws = new WritingSystemDefinition
			{
				IsVoice = true
			};
			Assert.Throws<ValidationException>(() => ws.Script = "change!");
		}

		[Test]
		public void SetAllRfc5646LanguageTagComponents_ScriptSetToZxxxAndVariantSetToXDashAudio_SetsIsVoiceToTrue()
		{
			var ws = new WritingSystemDefinition();
			ws.SetAllComponents(
				"th",
				WellKnownSubtags.AudioScript,
				"",
				"x-audio"
			);
			Assert.IsTrue(ws.IsVoice);
		}

		[Test]
		public void SetAllRfc5646LanguageTagComponents_ScriptSetToZxXxAndVariantSetToXDashAuDiO_SetsIsVoiceToTrue()
		{
			var ws = new WritingSystemDefinition();
			ws.SetAllComponents(
				"th",
				"ZxXx",
				"",
				"x-AuDiO"
			);
			Assert.IsTrue(ws.IsVoice);
		}

		[Test]
		public void Variant_ChangedToSomethingOtherThanXDashAudioWhileIsVoiceIsTrue_IsVoiceIsChangedToFalse()
		{
			var ws = new WritingSystemDefinition();
			ws.IsVoice = true;
			ws.Variants.Clear();
			ws.Variants.Add("1901");
			Assert.That(ws.Variants, Is.EqualTo(new VariantSubtag[] {"1901"}));
			Assert.That(ws.IsVoice, Is.False);
		}

		[Test]
		public void Iso639_SetValidLanguage_IsSet()
		{
			var ws = new WritingSystemDefinition {Language = "th"};
			Assert.That(ws.Language, Is.EqualTo((LanguageSubtag) "th"));
		}

		[Test]
		public void IsVoice_ToggledAfterVariantHasBeenSet_DoesNotRemoveVariant()
		{
			var ws = new WritingSystemDefinition {Variants = {"1901"}};
			ws.IsVoice = true;
			ws.IsVoice = false;
			Assert.That(ws.Variants, Is.EqualTo(new VariantSubtag[] {"1901"}));
		}

		[Test]
		public void IsVoice_ToggledAfterRegionHasBeenSet_DoesNotRemoveRegion()
		{
			var ws = new WritingSystemDefinition {Region = "US"};
			ws.IsVoice = true;
			ws.IsVoice = false;
			Assert.That(ws.Region, Is.EqualTo((RegionSubtag) "US"));
		}

		[Test]
		public void IsVoice_ToggledAfterScriptHasBeenSet_ScriptIsCleared()
		{
			var ws = new WritingSystemDefinition {Script = "Latn"};
			ws.IsVoice = true;
			ws.IsVoice = false;
			Assert.That(ws.Script, Is.Null);
		}

		[Test]
		public void IsVoice_SetToTrueWhileIpaStatusIsIpa_IpaStatusIsSetToNotIpa()
		{
			var ws = new WritingSystemDefinition
			{
				IpaStatus = IpaStatusChoices.Ipa,
				IsVoice = true
			};
			Assert.AreEqual(IpaStatusChoices.NotIpa, ws.IpaStatus);
		}

		[Test]
		public void IsVoice_SetToTrueWhileIpaStatusIsIpaPhontetic_IpaStatusIsSetToNotIpa()
		{
			var ws = new WritingSystemDefinition
					 {
						 IpaStatus = IpaStatusChoices.IpaPhonetic,
						 IsVoice = true
					 };
			Assert.AreEqual(IpaStatusChoices.NotIpa, ws.IpaStatus);
		}

		[Test]
		public void IsVoice_SetToTrueWhileIpaStatusIsIpaPhonemic_IpaStatusIsSetToNotIpa()
		{
			var ws = new WritingSystemDefinition();
			ws.IpaStatus = IpaStatusChoices.IpaPhonemic;
			ws.IsVoice = true;
			Assert.AreEqual(IpaStatusChoices.NotIpa, ws.IpaStatus);
		}

		[Test]
		public void Variant_IsSetWithDuplicateTags_DontKnowWhatToDo()
		{
			var ws = new WritingSystemDefinition();
			ws.Language = "en";
			ws.Variants.Add("1901");
			Assert.Throws<ValidationException>(
				() => ws.Variants.Add("1901")
			);
		}

		[Test]
		public void Variant_SetToXDashAudioWhileScriptIsNotZxxx_Throws()
		{
			var ws = new WritingSystemDefinition();
			Assert.Throws<ValidationException>(() => ws.Variants.Add(WellKnownSubtags.AudioPrivateUse));
		}

		[Test]
		public void Script_SetToOtherThanZxxxWhileVariantIsXDashAudio_Throws()
		{
			var ws = new WritingSystemDefinition();
			ws.SetAllComponents(
				"th",
				WellKnownSubtags.AudioScript,
				"",
				"x-audio"
			);
			Assert.Throws<ValidationException>(() => ws.Script = "Ltn");
		}

		[Test]
		public void SetAllRfc5646LanguageTagComponents_VariantSetToPrivateUseOnly_VariantIsSet()
		{
			var ws = new WritingSystemDefinition();
			ws.SetAllComponents(
				"th",
				WellKnownSubtags.AudioScript,
				"",
				"x-audio"
			);
			Assert.That(ws.Variants, Is.EqualTo(new VariantSubtag[] {WellKnownSubtags.AudioPrivateUse}));
		}

		[Test]
		public void Variant_SetToxDashCapitalAUDIOWhileScriptIsNotZxxx_Throws()
		{
			var ws = new WritingSystemDefinition();
			Assert.Throws<ValidationException>(() => ws.Variants.Add("AUDIO"));
		}

		[Test]
		public void Script_SetToOtherThanZxxxWhileVariantIsxDashCapitalAUDIO_Throws()
		{
			var ws = new WritingSystemDefinition();
			ws.SetAllComponents(
				"th",
				WellKnownSubtags.AudioScript,
				"",
				"x-AUDIO"
			);
			Assert.Throws<ValidationException>(() => ws.Script = "Ltn");
		}

		[Test]
		public void IsVoice_VariantIsxDashPrefixaudioPostfix_ReturnsFalse()
		{
			var ws = new WritingSystemDefinition();
			ws.SetAllComponents("th", WellKnownSubtags.AudioScript, "", "x-paudiop");
			Assert.IsFalse(ws.IsVoice);
		}

		[Test]
		public void Variant_ContainsXDashAudioDashFonipa_VariantIsSet()
		{
			var ws = new WritingSystemDefinition();
			ws.SetAllComponents(
				"th",
				WellKnownSubtags.AudioScript,
				"",
				"x-audio-fonipa"
			);
			Assert.That(ws.Variants, Is.EqualTo(new VariantSubtag[] {"audio", new VariantSubtag("fonipa", true)}));
		}

		[Test]
		public void Variant_ContainsXDashAudioAndFonipa_Throws()
		{
			var ws = new WritingSystemDefinition();
			Assert.Throws<ValidationException>(
				() => ws.SetAllComponents("qaa", WellKnownSubtags.AudioScript, "", WellKnownSubtags.IpaVariant + "-" + WellKnownSubtags.AudioPrivateUse));
		}

		[Test]
		public void Variant_ContainsXDashEticAndNotFonipaInVariant_Throws()
		{
			var ws = new WritingSystemDefinition();
			Assert.Throws<ValidationException>(
				() => ws.SetAllComponents("qaa", "", "", WellKnownSubtags.IpaPhoneticPrivateUse));
		}

		[Test]
		public void Variant_ContainsXDashEmicAndNotFonipaInVariant_Throws()
		{
			var ws = new WritingSystemDefinition();
			Assert.Throws<ValidationException>(
				() => ws.SetAllComponents("qaa", "", "", WellKnownSubtags.IpaPhonemicPrivateUse));
		}

		[Test]
		public void Variant_ContainsXDashEticAndFonipaInPrivateUse_Throws()
		{
			var ws = new WritingSystemDefinition();
			Assert.Throws<ValidationException>(
				() => ws.SetAllComponents("qaa", "", "", WellKnownSubtags.IpaPhoneticPrivateUse + '-' + WellKnownSubtags.IpaVariant));
		}

		[Test]
		public void Variant_ContainsXDashEmicAndAndFonipaInPrivateUse_Throws()
		{
			var ws = new WritingSystemDefinition();
			Assert.Throws<ValidationException>(
				() => ws.SetAllComponents("qaa", "", "", WellKnownSubtags.IpaPhonemicPrivateUse + '-' + WellKnownSubtags.IpaVariant));
		}

		[Test]
		public void Variant_ContainsXDashAudioAndPhoneticMarker_Throws()
		{
			var ws = new WritingSystemDefinition();
			Assert.Throws<ValidationException>(
				() => ws.SetAllComponents("qaa", WellKnownSubtags.AudioScript, "", WellKnownSubtags.AudioPrivateUse + "-" + "etic"));
		}

		[Test]
		public void Variant_ContainsXDashAudioAndPhonemicMarker_Throws()
		{
			var ws = new WritingSystemDefinition();
			Assert.Throws<ValidationException>(
				() => ws.SetAllComponents("qaa", WellKnownSubtags.AudioScript, "", WellKnownSubtags.AudioPrivateUse + "-" + "emic"));
		}

		[Test]
		public void IsVoice_SetToTrueWhileIpaStatusIsIpa_IsVoiceIsTrue()
		{
			var ws = new WritingSystemDefinition();
			ws.IpaStatus = IpaStatusChoices.Ipa;
			ws.IsVoice = true;
			Assert.IsTrue(ws.IsVoice);
		}

		[Test]
		public void IsVoice_SetToTrueWhileIpaStatusIsIpa_IpaStatusIsNotIpa()
		{
			var ws = new WritingSystemDefinition();
			ws.IpaStatus = IpaStatusChoices.Ipa;
			ws.IsVoice = true;
			Assert.AreEqual(IpaStatusChoices.NotIpa, ws.IpaStatus);
		}

		[Test]
		public void IsVoice_SetToFalseWhileIpaStatusIsIpa_IsVoiceIsFalse()
		{
			var ws = new WritingSystemDefinition();
			ws.IpaStatus = IpaStatusChoices.Ipa;
			ws.IsVoice = false;
			Assert.IsFalse(ws.IsVoice);
		}

		[Test]
		public void IsVoice_SetToTrueOnEntirelyPrivateUseLanguageTag_markerForUnlistedLanguageIsInserted()
		{
			var ws = new WritingSystemDefinition("x-private");
			Assert.That(ws.Language, Is.EqualTo((LanguageSubtag) "private"));
			ws.IsVoice = true;
			Assert.That(ws.Language, Is.EqualTo((LanguageSubtag) "private"));
			Assert.That(ws.Script, Is.EqualTo((ScriptSubtag) WellKnownSubtags.UnwrittenScript));
			Assert.That(ws.Region, Is.Null);
			Assert.That(ws.Variants, Is.EqualTo(new VariantSubtag[] {"audio"}));
		}

		[Test]
		public void IsVoice_SetToFalseWhileIpaStatusIsIpa_IpaStatusIsIpa()
		{
			var ws = new WritingSystemDefinition();
			ws.IpaStatus = IpaStatusChoices.Ipa;
			ws.IsVoice = false;
			Assert.AreEqual(IpaStatusChoices.Ipa, ws.IpaStatus);
		}

		[Test]
		public void IsVoice_SetToTrueWhileIpaStatusIsPhonetic_IsVoiceIsTrue()
		{
			var ws = new WritingSystemDefinition();
			ws.IpaStatus = IpaStatusChoices.IpaPhonetic;
			ws.IsVoice = true;
			Assert.IsTrue(ws.IsVoice);
		}

		[Test]
		public void IsVoice_SetToTrueWhileIpaStatusIsPhonetic_IpaStatusIsNotIpa()
		{
			var ws = new WritingSystemDefinition();
			ws.IpaStatus = IpaStatusChoices.IpaPhonetic;
			ws.IsVoice = true;
			Assert.AreEqual(IpaStatusChoices.NotIpa, ws.IpaStatus);
		}

		[Test]
		public void IsVoice_SetToTrueWhileIpaStatusIsPhonemic_IsVoiceIsTrue()
		{
			var ws = new WritingSystemDefinition();
			ws.IpaStatus = IpaStatusChoices.IpaPhonemic;
			ws.IsVoice = true;
			Assert.IsTrue(ws.IsVoice);
		}

		[Test]
		public void IsVoice_SetToTrueWhileIpaStatusIsPhonemic_IpaStatusIsNotIpa()
		{
			var ws = new WritingSystemDefinition();
			ws.IpaStatus = IpaStatusChoices.IpaPhonemic;
			ws.IsVoice = true;
			Assert.AreEqual(IpaStatusChoices.NotIpa, ws.IpaStatus);
		}

		[Test]
		public void Iso639_SetEmpty_ThrowsException()
		{
			var ws = new WritingSystemDefinition();
			Assert.Throws<ValidationException>(() => ws.Language = null);
		}

		[Test]
		public void Variant_ContainsUnderscore_Throws()
		{
			var ws = new WritingSystemDefinition();
			ws.Language = "de";
			Assert.Throws<ValidationException>(() => ws.Variants.Add("x_audio"));
		}

		[Test]
		public void Variant_ContainsxDashCapitalAUDIOAndScriptIsNotZxxx_Throws()
		{
			var ws = new WritingSystemDefinition();
			ws.Language = "de";
			ws.Script = "Latn";
			Assert.Throws<ValidationException>(() => ws.Variants.Add("AUDIO"));
		}

		[Test]
		public void Variant_IndicatesThatWsIsAudioAndScriptIsCapitalZXXX_ReturnsTrue()
		{
			var ws = new WritingSystemDefinition();
			ws.Language = "de";
			ws.Script = "ZXXX";
			ws.Variants.Add(WellKnownSubtags.AudioPrivateUse);
			Assert.IsTrue(ws.IsVoice);
		}

		[Test]
		public void IsValidWritingSystem_VariantIndicatesThatWsIsAudioButContainsotherThanJustTheNecassaryXDashAudioTagAndScriptIsNotZxxx_Throws()
		{
			var ws = new WritingSystemDefinition();
			ws.Language = "de";
			ws.Script = "latn";
			ws.Variants.Add("private");
			Assert.Throws<ValidationException>(() => ws.Variants.Add("audio"));
		}

		[Test]
		public void LanguageSubtag_ContainsXDashAudio_Throws()
		{
			var ws = new WritingSystemDefinition();
			Assert.Throws<ValidationException>(() => ws.Language = "de-x-audio");
		}

		[Test]
		public void Language_ContainsZxxx_Throws()
		{
			var ws = new WritingSystemDefinition();
			Assert.Throws<ValidationException>(() => ws.Language = "de-Zxxx");
		}

		[Test]
		public void LanguageSubtag_ContainsCapitalXDashAudio_Throws()
		{
			var ws = new WritingSystemDefinition();
			Assert.Throws<ValidationException>(() => ws.Language = "de-X-AuDiO");
		}

		public void Language_SetWithInvalidLanguageTag_Throws()
		{
			var ws = new WritingSystemDefinition();
			Assert.Throws<ValidationException>(() => ws.Language = "bogus");
		}

		[Test]
		public void Script_SetWithInvalidScriptTag_Throws()
		{
			var ws = new WritingSystemDefinition();
			ws.Language = "en";
			Assert.Throws<ValidationException>(() => ws.Script = "bogus");
		}

		[Test]
		public void Region_SetWithInvalidRegionTag_Throws()
		{
			var ws = new WritingSystemDefinition();
			ws.Language = "en";
			Assert.Throws<ValidationException>(() => ws.Region = "bogus");
		}

		[Test]
		public void Variant_SetWithPrivateUseTag_VariantisSet()
		{
			var ws = new WritingSystemDefinition();
			ws.Variants.Add("lalala");
			Assert.That(ws.Variants, Is.EqualTo(new VariantSubtag[] {"lalala"}));
		}

		[Test]
		public void SetRfc5646LanguageTagComponents_Language_IsSet()
		{
			var ws = new WritingSystemDefinition();
			ws.SetAllComponents("th", "", "", "");
			Assert.That(ws.Language, Is.EqualTo((LanguageSubtag) "th"));
		}

		[Test]
		public void SetRfc5646LanguageTagComponents_PrivateUseLanguage_IsSet()
		{
			var ws = new WritingSystemDefinition();
			Assert.Throws<ValidationException>(
				() => ws.SetAllComponents("BadLanguage", "", "", "")
			);
		}

		[Test]
		public void SetRfc5646LanguageTagComponents_Script_IsSet()
		{
			var ws = new WritingSystemDefinition();
			ws.SetAllComponents("th", "Thai", "", "");
			Assert.That(ws.Script, Is.EqualTo((ScriptSubtag) "Thai"));
		}

		[Test]
		public void SetRfc5646LanguageTagComponents_BadScript_Throws()
		{
			var ws = new WritingSystemDefinition();
			Assert.Throws<ValidationException>(
				() => ws.SetAllComponents("th", "BadScript", "", "")
			);
		}

		[Test]
		public void SetRfc5646LanguageTagComponents_Region_IsSet()
		{
			var ws = new WritingSystemDefinition();
			ws.SetAllComponents("th", "Thai", "TH", "");
			Assert.That(ws.Region, Is.EqualTo((RegionSubtag) "TH"));
		}

		[Test]
		public void SetRfc5646LanguageTagComponents_BadRegion_Throws()
		{
			var ws = new WritingSystemDefinition();
			Assert.Throws<ValidationException>(
				() => ws.SetAllComponents("th", "Thai", "BadRegion", "")
			);
		}

		[Test]
		public void SetRfc5646LanguageTagComponents_Variant_IsSet()
		{
			var ws = new WritingSystemDefinition();
			ws.SetAllComponents("th", "Thai", "TH", "1901");
			Assert.That(ws.Variants, Is.EqualTo(new VariantSubtag[] {"1901"}));
		}

		[Test]
		public void SetRfc5646LanguageTagComponents_BadVariant_Throws()
		{
			var ws = new WritingSystemDefinition();
			Assert.Throws<ValidationException>(
				() => ws.SetAllComponents("th", "Thai", "TH", "BadVariant")
			);
		}

		[Test]
		public void Abbreviation_Sets_GetsSame()
		{
			var ws = new WritingSystemDefinition();
			ws.Abbreviation = "en";
			Assert.AreEqual("en", ws.Abbreviation);
		}

		[Test]
		public void Abbreviation_Uninitialized_ReturnsISO639()
		{
			var writingSystem = new WritingSystemDefinition("en");
			Assert.AreEqual("en", writingSystem.Abbreviation);
		}

		[Test]
		public void IpaStatus_SetToIpaLangTagStartsWithxDash_InsertsUnknownlanguagemarkerAsLanguageSubtag()
		{
			var writingSystem = new WritingSystemDefinition("x-bogus");
			writingSystem.IpaStatus = IpaStatusChoices.Ipa;
			Assert.That(writingSystem.Language, Is.EqualTo((LanguageSubtag) "bogus"));
			Assert.That(writingSystem.LanguageTag, Is.EqualTo("qaa-fonipa-x-bogus"));
		}

		[Test]
		public void IpaStatus_SetToPhoneticLangTagStartsWithxDash_InsertsUnknownlanguagemarkerAsLanguageSubtag()
		{
			var writingSystem = new WritingSystemDefinition("x-bogus") {IpaStatus = IpaStatusChoices.IpaPhonetic};
			Assert.That(writingSystem.Language, Is.EqualTo((LanguageSubtag) "bogus"));
			Assert.That(writingSystem.LanguageTag, Is.EqualTo("qaa-fonipa-x-bogus-etic"));
		}

		[Test]
		public void IpaStatus_SetToPhonemicLangTagStartsWithxDash_InsertsUnknownlanguagemarkerAsLanguageSubtag()
		{
			var writingSystem = new WritingSystemDefinition("x-bogus") {IpaStatus = IpaStatusChoices.IpaPhonemic};
			Assert.That(writingSystem.Language, Is.EqualTo((LanguageSubtag) "bogus"));
			Assert.That(writingSystem.LanguageTag, Is.EqualTo("qaa-fonipa-x-bogus-emic"));
		}

		[Test]
		public void CloneContructor_VariantStartsWithxDash_VariantIsCopied()
		{
			var writingSystem = new WritingSystemDefinition(new WritingSystemDefinition("x-bogus"));
			Assert.AreEqual("x-bogus", writingSystem.LanguageTag);
		}

		[Test]
		public void Language_Set_Idchanged()
		{
			var writingSystem = new WritingSystemDefinition("en", "Zxxx", "", "1901-x-audio");
			writingSystem.Language = "de";
			Assert.AreEqual("de-Zxxx-1901-x-audio", writingSystem.Id);
		}

		[Test]
		public void Script_Set_Idchanged()
		{
			var writingSystem = new WritingSystemDefinition("en", "Zxxx", "", "1901-x-bogus");
			writingSystem.Script = "Latn";
			Assert.AreEqual("en-Latn-1901-x-bogus", writingSystem.Id);
		}

		[Test]
		public void Region_Set_Idchanged()
		{
			var writingSystem = new WritingSystemDefinition("en", "Zxxx", "", "1901-x-bogus");
			writingSystem.Region = "US";
			Assert.AreEqual("en-Zxxx-US-1901-x-bogus", writingSystem.Id);
		}

		[Test]
		public void Variant_Set_Idchanged()
		{
			var writingSystem = new WritingSystemDefinition("en", "Zxxx", "", "1901-x-bogus");
			writingSystem.Variants.Clear();
			writingSystem.Variants.Add("audio");
			Assert.AreEqual("en-Zxxx-x-audio", writingSystem.Id);
		}

		[Test]
		public void Ctor1_IdIsSet()
		{
			var writingSystem = new WritingSystemDefinition();
			Assert.AreEqual("qaa", writingSystem.Id);
		}

		[Test]
		public void Ctor2_IdIsSet()
		{
			var writingSystem = new WritingSystemDefinition("en","Zxxx","","1901-x-audio","abb",true);
			Assert.AreEqual("en-Zxxx-1901-x-audio", writingSystem.Id);
		}

		[Test]
		public void Ctor3_IdIsSet()
		{
			var writingSystem = new WritingSystemDefinition("en-Zxxx-1901-x-audio");
			Assert.AreEqual("en-Zxxx-1901-x-audio", writingSystem.Id);
		}

		[Test]
		public void Ctor4_IdIsSet()
		{
			var writingSystem = new WritingSystemDefinition(new WritingSystemDefinition("en-Zxxx-1901-x-audio"));
			Assert.AreEqual("en-Zxxx-1901-x-audio", writingSystem.Id);
		}

		[Test]
		public void Parse_IdIsSet()
		{
			var writingSystem = new WritingSystemDefinition("en-Zxxx-1901-x-audio");
			Assert.AreEqual("en-Zxxx-1901-x-audio", writingSystem.Id);
		}

		[Test]
		public void FromLanguage_IdIsSet()
		{
			var writingSystem = new WritingSystemDefinition("en-Zxxx-1901-x-audio");
			Assert.AreEqual("en-Zxxx-1901-x-audio", writingSystem.Id);
		}

		[Test]
		public void FromRfc5646Subtags_IdIsSet()
		{
			var writingSystem = new WritingSystemDefinition("en", "Zxxx", "", "1901-x-audio");
			Assert.AreEqual("en-Zxxx-1901-x-audio", writingSystem.Id);
		}

		[Test]
		public void IpaStatus_Set_IdIsSet()
		{
			var writingSystem = new WritingSystemDefinition("en", "Zxxx", "", "1901-x-audio");
			writingSystem.IpaStatus = IpaStatusChoices.IpaPhonetic;
			Assert.AreEqual("en-Zxxx-1901-fonipa-x-etic", writingSystem.Id);
		}

		[Test]
		public void IsVoice_Set_IdIsSet()
		{
			var writingSystem = new WritingSystemDefinition("en", "Zxxx", "", "1901-x-audio");
			writingSystem.IsVoice = false;
			Assert.AreEqual("en-1901", writingSystem.Id);
		}

		[Test]
		public void AddToVariant_IdIsSet()
		{
			var writingSystem = new WritingSystemDefinition("en", "Zxxx", "", "1901-x-audio");
			writingSystem.Variants.Add("bauddha");
			Assert.AreEqual("en-Zxxx-1901-bauddha-x-audio", writingSystem.Id);
		}

		[Test]
		public void AddToVariant_NonRegisteredVariant_IdIsSet()
		{
			var writingSystem = new WritingSystemDefinition("en", "Zxxx", "", "1901-x-audio");
			writingSystem.Variants.Add("bogus");
			Assert.AreEqual("en-Zxxx-1901-x-audio-bogus", writingSystem.Id);
		}

		[Test]
		public void SetAllRfc5646LanguageTagComponents_IdIsSet()
		{
			var writingSystem = new WritingSystemDefinition("en", "Zxxx", "", "1901-x-audio");
			writingSystem.SetAllComponents("de","Latn","US","fonipa-x-etic");
			Assert.AreEqual("de-Latn-US-fonipa-x-etic", writingSystem.Id);
		}

		[Test]
		public void SetAllRfc5646LanguageTagComponents_IdChanged_ModifiedisTrue()
		{
			var writingSystem = new WritingSystemDefinition("en", "Zxxx", "", "1901-x-audio");
			writingSystem.SetAllComponents("de", "Latn", "US", "fonipa-x-etic");
			Assert.AreEqual(writingSystem.IsChanged, true);
		}

		[Test]
		public void SetAllRfc5646LanguageTagComponents_IdUnchanged_ModifiedisTrue()
		{
			var writingSystem = new WritingSystemDefinition("en", "Zxxx", "", "1901-x-audio");
			writingSystem.SetAllComponents("en", "Zxxx", "", "1901-x-audio");
			Assert.AreEqual(writingSystem.IsChanged, false);
		}

		[Test]
		public void MakeUnique_IsAlreadyUnique_NothingChanges()
		{
			var existingTags = new[] {"en-Zxxx-x-audio"};
			var ws = new WritingSystemDefinition("de");
			var newWs = WritingSystemDefinition.CreateCopyWithUniqueId(ws, existingTags);
			Assert.That(newWs.Id, Is.EqualTo("de"));
		}

		[Test]
		public void MakeUnique_IsNotUnique_DuplicateMarkerIsAppended()
		{
			var existingTags = new[] { "en-Zxxx-x-audio" };
			var ws = new WritingSystemDefinition("en-Zxxx-x-audio");
			var newWs = WritingSystemDefinition.CreateCopyWithUniqueId(ws, existingTags);
			Assert.That(newWs.Id, Is.EqualTo("en-Zxxx-x-audio-dupl0"));
		}

		[Test]
		public void MakeUnique_ADuplicateAlreadyExists_DuplicatemarkerWithHigherNumberIsAppended()
		{
			var existingTags = new[] { "en-Zxxx-x-audio", "en-Zxxx-x-audio-dupl0" };
			var ws = new WritingSystemDefinition("en-Zxxx-x-audio");
			var newWs = WritingSystemDefinition.CreateCopyWithUniqueId(ws, existingTags);
			Assert.That(newWs.Id, Is.EqualTo("en-Zxxx-x-audio-dupl1"));
		}

		[Test]
		public void MakeUnique_ADuplicatewithHigherNumberAlreadyExists_DuplicateMarkerWithLowNumberIsAppended()
		{
			var existingTags = new[] { "en-Zxxx-x-audio", "en-Zxxx-x-audio-dupl1" };
			var ws = new WritingSystemDefinition("en-Zxxx-x-audio");
			var newWs = WritingSystemDefinition.CreateCopyWithUniqueId(ws, existingTags);
			Assert.That(newWs.Id, Is.EqualTo("en-Zxxx-x-audio-dupl0"));
		}

		[Test]
		public void MakeUnique_StoreIdIsNull()
		{
			var existingTags = new[] { "en-Zxxx-x-audio" };
			var ws = new WritingSystemDefinition("de");
			var newWs = WritingSystemDefinition.CreateCopyWithUniqueId(ws, existingTags);
			Assert.That(newWs.StoreID, Is.EqualTo(null));
		}

		[Test]
		public void MakeUnique_IdAlreadyContainsADuplicateMarker_DuplicateNumberIsMaintainedAndNewOneIsIntroduced()
		{
			var existingTags = new[] { "en-Zxxx-x-dupl0-audio", "en-Zxxx-x-audio-dupl1" };
			var ws = new WritingSystemDefinition("en-Zxxx-x-dupl0-audio");
			var newWs = WritingSystemDefinition.CreateCopyWithUniqueId(ws, existingTags);
			Assert.That(newWs.Id, Is.EqualTo("en-Zxxx-x-dupl0-audio-dupl1"));
		}

		[Test]
		public void GetDefaultFontSizeOrMinimum_DefaultConstructor_GreaterThanSix()
		{
			Assert.That(new WritingSystemDefinition().GetDefaultFontSizeOrMinimum(), Is.GreaterThan(6));
		}

		[Test]
		public void GetDefaultFontSizeOrMinimum_SetAt0_GreaterThanSix()
		{
			var ws = new WritingSystemDefinition {DefaultFontSize = 0};
			Assert.That(ws.GetDefaultFontSizeOrMinimum(), Is.GreaterThan(6));
		}

		[Test]
		public void ListLabel_ScriptRegionVariantEmpty_LabelIsLanguage()
		{
			var ws = new WritingSystemDefinition("de");
			Assert.That(ws.ListLabel, Is.EqualTo("German"));
		}

		[Test]
		public void ListLabel_ScriptSet_LabelIsLanguageWithScriptInBrackets()
		{
			var ws = new WritingSystemDefinition("de") {Script = "Armi"};
			Assert.That(ws.ListLabel, Is.EqualTo("German (Armi)"));
		}


		[Test]
		public void ListLabel_RegionSet_LabelIsLanguageWithRegionInBrackets()
		{
			var ws = new WritingSystemDefinition("de") {Region = "US"};
			Assert.That(ws.ListLabel, Is.EqualTo("German (US)"));
		}

		[Test]
		public void ListLabel_ScriptRegionSet_LabelIsLanguageWithScriptandRegionInBrackets()
		{
			var ws = new WritingSystemDefinition("de") {Script = "Armi", Region = "US"};
			Assert.That(ws.ListLabel, Is.EqualTo("German (Armi-US)"));
		}

		[Test]
		public void ListLabel_ScriptVariantSet_LabelIsLanguageWithScriptandVariantInBrackets()
		{
			var ws = new WritingSystemDefinition("de") {Script = "Armi"};
			ws.Variants.Add("smth");
			Assert.That(ws.ListLabel, Is.EqualTo("German (Armi-x-smth)"));
		}

		[Test]
		public void ListLabel_RegionVariantSet_LabelIsLanguageWithRegionAndVariantInBrackets()
		{
			var ws = new WritingSystemDefinition("de") {Region = "US"};
			ws.Variants.Add("smth");
			Assert.That(ws.ListLabel, Is.EqualTo("German (US-x-smth)"));
		}

		[Test]
		public void ListLabel_VariantSetToIpa_LabelIsLanguageWithIPAInBrackets()
		{
			var ws = new WritingSystemDefinition("de") {Variants = {WellKnownSubtags.IpaVariant}};
			Assert.That(ws.ListLabel, Is.EqualTo("German (IPA)"));
		}

		[Test]
		public void ListLabel_VariantSetToPhonetic_LabelIsLanguageWithIPADashEticInBrackets()
		{
			var ws = new WritingSystemDefinition("de");
			ws.IpaStatus = IpaStatusChoices.IpaPhonetic;
			Assert.That(ws.ListLabel, Is.EqualTo("German (IPA-etic)"));
		}

		[Test]
		public void ListLabel_VariantSetToPhonemic_LabelIsLanguageWithIPADashEmicInBrackets()
		{
			var ws = new WritingSystemDefinition("de");
			ws.IpaStatus = IpaStatusChoices.IpaPhonemic;
			Assert.That(ws.ListLabel, Is.EqualTo("German (IPA-emic)"));
		}

		[Test]
		public void ListLabel_WsIsVoice_LabelIsLanguageWithVoiceInBrackets()
		{
			var ws = new WritingSystemDefinition("de");
			ws.IsVoice = true;
			Assert.That(ws.ListLabel, Is.EqualTo("German (Voice)"));
		}

		[Test]
		public void ListLabel_VariantContainsDuplwithNumber_LabelIsLanguageWithCopyAndNumberInBrackets()
		{
			var ws = new WritingSystemDefinition("de");
			var newWs = WritingSystemDefinition.CreateCopyWithUniqueId(ws, new[]{"de", "de-x-dupl0"});
			Assert.That(newWs.ListLabel, Is.EqualTo("German (Copy1)"));
		}

		[Test]
		public void ListLabel_VariantContainsDuplwithZero_LabelIsLanguageWithCopyAndNoNumberInBrackets()
		{
			var ws = new WritingSystemDefinition("de");
			var newWs = WritingSystemDefinition.CreateCopyWithUniqueId(ws, new[] { "de" });
			Assert.That(newWs.ListLabel, Is.EqualTo("German (Copy)"));
		}

		[Test]
		public void ListLabel_VariantContainsmulitpleDuplswithNumber_LabelIsLanguageWithCopyAndNumbersInBrackets()
		{
			var ws = new WritingSystemDefinition("de-x-dupl0");
			var newWs = WritingSystemDefinition.CreateCopyWithUniqueId(ws, new[] { "de", "de-x-dupl0" });
			Assert.That(newWs.ListLabel, Is.EqualTo("German (Copy-Copy1)"));
		}

		[Test]
		public void ListLabel_VariantContainsUnknownVariant_LabelIsLanguageWithVariantInBrackets()
		{
			var ws = new WritingSystemDefinition("de");
			ws.Variants.Add("garble");
			Assert.That(ws.ListLabel, Is.EqualTo("German (x-garble)"));
		}

		[Test]
		public void ListLabel_AllSortsOfThingsSet_LabelIsCorrect()
		{
			var ws = new WritingSystemDefinition("de-x-dupl0");
			var newWs = WritingSystemDefinition.CreateCopyWithUniqueId(ws, new[] { "de", "de-x-dupl0" });
			newWs.Region = "US";
			newWs.Script = "Armi";
			newWs.IpaStatus = IpaStatusChoices.IpaPhonetic;
			newWs.Variants.Add("garble");
			newWs.Variants.Add("1901");
			Assert.That(newWs.ListLabel, Is.EqualTo("German (IPA-etic-Copy-Copy1-Armi-US-1901-x-garble)"));
		}

		[Test]
		public void OtherAvailableKeyboards_DefaultsToAllAvailable()
		{
			var ws = new WritingSystemDefinition("de-x-dupl0");
			var kbd1 = new DefaultKeyboardDefinition("something", "en-US");
			var kbd2 = new DefaultKeyboardDefinition("somethingElse", "en-GB");
			var controller = new MockKeyboardController();
			var keyboardList = new List<IKeyboardDefinition> {kbd1, kbd2};
			controller.AllAvailableKeyboards = keyboardList;
			Keyboard.Controller = controller;

			var result = ws.OtherAvailableKeyboards;

			Assert.That(result, Has.Member(kbd1));
			Assert.That(result, Has.Member(kbd2));
		}

		[Test]
		public void OtherAvailableKeyboards_OmitsKnownKeyboards()
		{
			var ws = new WritingSystemDefinition("de-x-dupl0");
			var kbd1 = new DefaultKeyboardDefinition("something", "en-US");
			var kbd2 = new DefaultKeyboardDefinition("somethingElse", "en-GB");
			Keyboard.Controller = new MockKeyboardController {AllAvailableKeyboards = new[] {kbd1, kbd2}};
			ws.KnownKeyboards.Add(kbd1);

			List<IKeyboardDefinition> result = ws.OtherAvailableKeyboards.ToList();

			Assert.That(result, Has.Member(kbd2));
			Assert.That(result, Has.No.Member(kbd1));
		}

		class MockKeyboardController : IKeyboardController
		{
			/// <summary>
			/// Tries to get the keyboard with the specified <paramref name="layoutName"/>.
			/// </summary>
			/// <returns>
			/// Returns <c>KeyboardDescription.Zero</c> if no keyboard can be found.
			/// </returns>
			public IKeyboardDefinition GetKeyboard(string layoutName)
			{
				throw new NotImplementedException();
			}

			public IKeyboardDefinition GetKeyboard(string layoutName, string locale)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// Tries to get the keyboard for the specified <paramref name="writingSystem"/>.
			/// </summary>
			/// <returns>
			/// Returns <c>KeyboardDescription.Zero</c> if no keyboard can be found.
			/// </returns>
			public IKeyboardDefinition GetKeyboard(WritingSystemDefinition writingSystem)
			{
				throw new NotImplementedException();
			}

			public IKeyboardDefinition GetKeyboard(IInputLanguage language)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// Sets the keyboard
			/// </summary>
			public void SetKeyboard(IKeyboardDefinition keyboard)
			{
				throw new NotImplementedException();
			}

			public void SetKeyboard(string layoutName)
			{
				throw new NotImplementedException();
			}

			public void SetKeyboard(string layoutName, string locale)
			{
				throw new NotImplementedException();
			}

			public void SetKeyboard(WritingSystemDefinition writingSystem)
			{
				throw new NotImplementedException();
			}

			public void SetKeyboard(IInputLanguage language)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// Activates the keyboard of the default input language
			/// </summary>
			public void ActivateDefaultKeyboard()
			{
				throw new NotImplementedException();
			}

			public IEnumerable<IKeyboardDefinition> AllAvailableKeyboards { get; set; }

			public IKeyboardDefinition Default;
			public WritingSystemDefinition ArgumentPassedToDefault;
			public void UpdateAvailableKeyboards()
			{
				throw new NotImplementedException();
			}

			public IKeyboardDefinition DefaultForWritingSystem(WritingSystemDefinition ws)
			{
				ArgumentPassedToDefault = ws;
				return Default;
			}

			public IKeyboardDefinition LegacyForWritingSystem(WritingSystemDefinition ws)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// Creates and returns a keyboard definition object based on the ID.
			/// </summary>
			/// <remarks>The keyboard controller implementing this method will have to check the
			/// availability of the keyboard and what engine provides it.</remarks>
			public IKeyboardDefinition CreateKeyboardDefinition(string id, KeyboardFormat format, IEnumerable<string> urls)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// Gets or sets the currently active keyboard
			/// </summary>
			public IKeyboardDefinition ActiveKeyboard { get; set; }

			#region Implementation of IDisposable
			/// <summary>
			/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
			/// </summary>
			public void Dispose()
			{
				GC.SuppressFinalize(this);
			}
			#endregion
		}

		[Test]
		public void SettingLocalKeyboard_AddsToKnownKeyboards()
		{
			var ws = new WritingSystemDefinition("de-x-dupl0");
			var kbd1 = new DefaultKeyboardDefinition("something", "en-US");

			ws.LocalKeyboard = kbd1;

			Assert.That(ws.LocalKeyboard, Is.EqualTo(kbd1));
			Assert.That(ws.KnownKeyboards, Has.Member(kbd1));
		}

		/// <summary>
		/// This incidentally tests that AddKnownKeyboard sets the Modified flag when it DOES change something.
		/// </summary>
		[Test]
		public void AddKnownKeyboard_DoesNotMakeDuplicates()
		{
			var ws = new WritingSystemDefinition("de-x-dupl0");
			var kbd1 = new DefaultKeyboardDefinition("something", "en-US");

			ws.KnownKeyboards.Add(kbd1);
			Assert.That(ws.IsChanged, Is.True);
			ws.AcceptChanges();
			ws.KnownKeyboards.Add(kbd1);
			Assert.That(ws.IsChanged, Is.False);

			Assert.That(ws.KnownKeyboards.Count, Is.EqualTo(1));
		}

		[Test]
		public void SetLocalKeyboard_ToAlreadyKnownKeyboard_SetsModifiedFlag()
		{
			var ws = new WritingSystemDefinition("de-x-dupl0");
			var kbd1 = new DefaultKeyboardDefinition("something", "en-US");
			var kbd2 = new DefaultKeyboardDefinition("somethingElse", "en-US");

			ws.KnownKeyboards.Add(kbd1);
			ws.LocalKeyboard = kbd2;
			Assert.That(ws.IsChanged, Is.True); // worth checking, but it doesn't really prove the point, since it will have also changed KnownKeyboards
			ws.AcceptChanges();
			ws.LocalKeyboard = kbd1; // This time it's already a known keyboard so only the LocalKeyboard setter can be responsibe for setting the flag.
			Assert.That(ws.IsChanged, Is.True);
		}

		[Test]
		public void LocalKeyboard_DefaultsToFirstKnownAvailable()
		{
			var ws = new WritingSystemDefinition("de-x-dupl0");
			var kbd1 = new DefaultKeyboardDefinition("something", "en-US");
			var kbd2 = new DefaultKeyboardDefinition("somethingElse", "en-US");

			ws.KnownKeyboards.Add(kbd1);
			ws.KnownKeyboards.Add(kbd2);

			Keyboard.Controller = new MockKeyboardController {AllAvailableKeyboards = new[] {kbd2}};

			Assert.That(ws.LocalKeyboard, Is.EqualTo(kbd2));
		}

		[Test]
		public void LocalKeyboard_DefersToController_WhenNoKnownAvailable()
		{
			var ws = new WritingSystemDefinition("de-x-dupl0");
			var kbd1 = new DefaultKeyboardDefinition("something", "en-US");

			var controller = new MockKeyboardController();
			var keyboardList = new List<IKeyboardDefinition>();
			controller.AllAvailableKeyboards = keyboardList;
			controller.Default = kbd1;
			Keyboard.Controller = controller;

			Assert.That(ws.LocalKeyboard, Is.EqualTo(kbd1));
			Assert.That(controller.ArgumentPassedToDefault, Is.EqualTo(ws));
		}

		[Test]
		public void LocalKeyboard_ResetWhenRemovedFromKnownKeyboards()
		{
			var ws = new WritingSystemDefinition("de-x-dupl0");
			var kbd1 = new DefaultKeyboardDefinition("something", "en-US");
			var kbd2 = new DefaultKeyboardDefinition("somethingElse", "en-US");
			var kbd3 = new DefaultKeyboardDefinition("somethingEntirelyElse", "en-US");

			var controller = new MockKeyboardController {AllAvailableKeyboards = new[] {kbd1, kbd2, kbd3}, Default = kbd3};
			Keyboard.Controller = controller;

			ws.KnownKeyboards.Add(kbd1);
			ws.KnownKeyboards.Add(kbd2);
			ws.LocalKeyboard = kbd2;

			Assert.That(ws.LocalKeyboard, Is.EqualTo(kbd2));
			ws.KnownKeyboards.RemoveAt(1);
			Assert.That(ws.LocalKeyboard, Is.EqualTo(kbd1));
			ws.KnownKeyboards.Clear();
			Assert.That(ws.LocalKeyboard, Is.EqualTo(kbd3));
		}

		[Test]
		public void DefaultFont_DefaultsToFirstFont()
		{
			var ws = new WritingSystemDefinition("de-x-dupl0");
			var fd1 = new FontDefinition("font1");
			var fd2 = new FontDefinition("font2");

			ws.Fonts.Add(fd1);
			ws.Fonts.Add(fd2);

			Assert.That(ws.DefaultFont, Is.EqualTo(fd1));
		}

		[Test]
		public void DefaultFont_ResetWhenRemovedFromFonts()
		{
			var ws = new WritingSystemDefinition("de-x-dupl0");
			var fd1 = new FontDefinition("font1");
			var fd2 = new FontDefinition("font2");

			ws.Fonts.Add(fd1);
			ws.Fonts.Add(fd2);
			ws.DefaultFont = fd2;

			Assert.That(ws.DefaultFont, Is.EqualTo(fd2));
			ws.Fonts.RemoveAt(1);
			Assert.That(ws.DefaultFont, Is.EqualTo(fd1));
			ws.Fonts.Clear();
			Assert.That(ws.DefaultFont, Is.Null);
		}

		[Test]
		public void DefaultCollation_DefaultsToFirstCollation()
		{
			var ws = new WritingSystemDefinition("de-x-dupl0");
			var cd1 = new CollationDefinition("standard");
			var cd2 = new CollationDefinition("other");

			ws.Collations.Add(cd1);
			ws.Collations.Add(cd2);

			Assert.That(ws.DefaultCollation, Is.EqualTo(cd1));
		}

		[Test]
		public void DefaultCollation_ResetWhenRemovedFromCollations()
		{
			var ws = new WritingSystemDefinition("de-x-dupl0");
			var cd1 = new CollationDefinition("standard");
			var cd2 = new CollationDefinition("other");

			ws.Collations.Add(cd1);
			ws.Collations.Add(cd2);
			ws.DefaultCollation = cd2;

			Assert.That(ws.DefaultCollation, Is.EqualTo(cd2));
			ws.Collations.RemoveAt(1);
			Assert.That(ws.DefaultCollation, Is.EqualTo(cd1));
			ws.Collations.Clear();
			Assert.That(ws.DefaultCollation, Is.Not.EqualTo(cd1));
		}

		private void VerifySubtagCodes(WritingSystemDefinition ws, string langCode, string scriptCode, string regionCode, string variantCode, string id)
		{
			Assert.That(ws.Language.Code, Is.EqualTo(langCode));
			if (scriptCode == null)
				Assert.That(ws.Script, Is.Null);
			else
				Assert.That(ws.Script.Code, Is.EqualTo(scriptCode));
			if (regionCode == null)
				Assert.That(ws.Region, Is.Null);
			else
				Assert.That(ws.Region.Code, Is.EqualTo(regionCode));
			if (variantCode == null)
				Assert.That(ws.Variants, Is.Empty);
			else
				Assert.That(IetfLanguageTag.GetVariantCodes(ws.Variants), Is.EqualTo(variantCode));

			// Now check that we can get the same tags by parsing the ID.
			LanguageSubtag languageSubtag;
			ScriptSubtag scriptSubtag;
			RegionSubtag regionSubtag;
			IEnumerable<VariantSubtag> variantSubtags;
			IetfLanguageTag.TryGetSubtags(id, out languageSubtag, out scriptSubtag, out regionSubtag, out variantSubtags);
			Assert.That(languageSubtag.Code, Is.EqualTo(langCode));
			if (scriptCode == null)
				Assert.That(scriptSubtag, Is.Null);
			else
				Assert.That(scriptSubtag.Code, Is.EqualTo(scriptCode));
			if (regionCode == null)
				Assert.That(regionSubtag, Is.Null);
			else
				Assert.That(regionSubtag.Code, Is.EqualTo(regionCode));
			if (variantCode == null)
				Assert.That(variantSubtags, Is.Empty);
			else
				Assert.That(IetfLanguageTag.GetVariantCodes(variantSubtags), Is.EqualTo(variantCode));
		}

		[Test]
		public void LanguageAndVariantTags()
		{
			// A new writing system has a Language tag of qaa. This is also its language tag. The others are null.
			var ws = new WritingSystemDefinition();
			VerifySubtagCodes(ws, "qaa", null, null, null, "qaa");

			ws.Language = "en";
			VerifySubtagCodes(ws, "en", null, null, null, "en");
			Assert.That(ws.LanguageName, Is.EqualTo("English"));

			ws.Language = new LanguageSubtag("kal", "Kalaba", true, "");
			Assert.That(ws.LanguageName, Is.EqualTo("Kalaba"));
			VerifySubtagCodes(ws, "kal", null, null, null, "qaa-x-kal");
			Assert.That(ws.Language.Name, Is.EqualTo("Kalaba"));

			// This is a region code that is valid, so we don't store it in the private-use area of our code.
			ws.Region = "QN";
			VerifySubtagCodes(ws, "kal", null, "QN", null, "qaa-QN-x-kal");

			// This is a standard region (Norway).
			ws.Region = "NO";
			VerifySubtagCodes(ws, "kal", null, "NO", null, "qaa-NO-x-kal");

			// A private region
			ws.Region = "ZD";
			VerifySubtagCodes(ws, "kal", null, "ZD", null, "qaa-QM-x-kal-ZD");

			// Add a private script
			ws.Script = "Zfdr";
			VerifySubtagCodes(ws, "kal", "Zfdr", "ZD", null, "qaa-Qaaa-QM-x-kal-Zfdr-ZD");

			// Change it to a standard one
			ws.Script = "Phnx";
			VerifySubtagCodes(ws, "kal", "Phnx", "ZD", null, "qaa-Phnx-QM-x-kal-ZD");

			// To the standard private-use marker
			ws.Script = "Qaaa";
			VerifySubtagCodes(ws, "kal", "Qaaa", "ZD", null, "qaa-Qaaa-QM-x-kal-Qaaa-ZD");

			// Back to the special one
			ws.Script = "Zfdr";
			VerifySubtagCodes(ws, "kal", "Zfdr", "ZD", null, "qaa-Qaaa-QM-x-kal-Zfdr-ZD");

			// Add a standard variant
			ws.Variants.Add("fonipa");
			VerifySubtagCodes(ws, "kal", "Zfdr", "ZD", "fonipa", "qaa-Qaaa-QM-fonipa-x-kal-Zfdr-ZD");

			// Change it to a combination one
			ws.Variants.Clear();
			ws.Variants.Add("fonipa");
			ws.Variants.Add("etic");
			VerifySubtagCodes(ws, "kal", "Zfdr", "ZD", "fonipa-x-etic", "qaa-Qaaa-QM-fonipa-x-kal-Zfdr-ZD-etic");

			// Back to no variant.
			ws.Variants.Clear();
			VerifySubtagCodes(ws, "kal", "Zfdr", "ZD", null, "qaa-Qaaa-QM-x-kal-Zfdr-ZD");

			// Try a double combination
			ws.Variants.Clear();
			ws.Variants.Add("fonipa");
			ws.Variants.Add("1996");
			ws.Variants.Add("etic");
			ws.Variants.Add("emic");
			VerifySubtagCodes(ws, "kal", "Zfdr", "ZD", "fonipa-1996-x-etic-emic", "qaa-Qaaa-QM-fonipa-1996-x-kal-Zfdr-ZD-etic-emic");

			// Drop a piece out of each
			ws.Variants.Clear();
			ws.Variants.Add("fonipa");
			ws.Variants.Add("etic");
			VerifySubtagCodes(ws, "kal", "Zfdr", "ZD", "fonipa-x-etic", "qaa-Qaaa-QM-fonipa-x-kal-Zfdr-ZD-etic");

			// Soemthing totally unknown
			ws.Variants.Clear();
			ws.Variants.Add("fonipa");
			ws.Variants.Add("blah");
			VerifySubtagCodes(ws, "kal", "Zfdr", "ZD", "fonipa-x-blah", "qaa-Qaaa-QM-fonipa-x-kal-Zfdr-ZD-blah");

			// Drop just the standard part
			ws.Variants.Clear();
			ws.Variants.Add("blah");
			VerifySubtagCodes(ws, "kal", "Zfdr", "ZD", "x-blah", "qaa-Qaaa-QM-x-kal-Zfdr-ZD-blah");

			// No longer a custom language
			ws.Language = "en";
			VerifySubtagCodes(ws, "en", "Zfdr", "ZD", "x-blah", "en-Qaaa-QM-x-Zfdr-ZD-blah");

			// No longer a custom script
			ws.Script = null;
			VerifySubtagCodes(ws, "en", null, "ZD", "x-blah", "en-QM-x-ZD-blah");

			// No longer a custom region
			ws.Region = null;
			VerifySubtagCodes(ws, "en", null, null, "x-blah", "en-x-blah");

			// No more variant
			ws.Variants.Clear();
			VerifySubtagCodes(ws, "en", null, null, null, "en");
		}
	}
}