﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Palaso.IO;
using Palaso.TestUtilities;
using Palaso.WritingSystems;

namespace Palaso.Tests.WritingSystems
{
	[TestFixture]
	public class OrphanFinderTests
	{
		private class TestEnvironment:IDisposable
		{
			private TempFile _file = new TempFile();
			private string _writingSystemsPath;
			private LdmlInFolderWritingSystemRepository _writingSystemRepository;

			public TestEnvironment(string id1, string id2)
			{   _writingSystemsPath = new TemporaryFolder().Path;
				WritingSystemRepository = new LdmlInFolderWritingSystemRepository(_writingSystemsPath);
				File.WriteAllText(_file.Path, String.Format("{0}|{0}|{1}", id1, id2));
			}

			public LdmlInFolderWritingSystemRepository WritingSystemRepository
			{
				get { return _writingSystemRepository; }
				set { _writingSystemRepository = value; }
			}

			public void Dispose()
			{
				File.Delete(_file.Path);
			}

			public IEnumerable<string> GetIdsFromFile()
			{
				var fileContent = File.ReadAllText(_file.Path);
				return fileContent.Split('|').Distinct();
			}

			public void ReplaceIdInFile(string oldid, string newid)
			{
				var fileContent = File.ReadAllText(_file.Path);
				fileContent = fileContent.Replace(oldid, newid);
				File.WriteAllText(_file.Path, fileContent);
			}

			public string FileContent
			{
				get
				{
					return File.ReadAllText(_file.Path);
				}
			}
		}

		[Test]
		public void FindOrphans_NoOrphansFound_WritingSystemRepoAndFileUntouched()
		{
			using (var e = new TestEnvironment("en", "en"))
			{
				var englishWs = new WritingSystemDefinition("en");
				e.WritingSystemRepository.Set(englishWs);
				OrphanFinder.FindOrphans(e.GetIdsFromFile, e.ReplaceIdInFile, e.WritingSystemRepository);
				Assert.That(e.WritingSystemRepository.Count, Is.EqualTo(1));
				Assert.That(e.WritingSystemRepository.Get("en"), Is.EqualTo(englishWs));
				Assert.That(e.FileContent, Is.EqualTo("en|en|en"));
			}
		}

		[Test]
		public void FindOrphans_OrphanFoundIsValidRfcTag_WritingsystemIsAddedToWritingSystemRepoAndFileUntouched()
		{
			using (var e = new TestEnvironment("en", "de"))
			{
				var englishWs = new WritingSystemDefinition("en");
				e.WritingSystemRepository.Set(englishWs);
				OrphanFinder.FindOrphans(e.GetIdsFromFile, e.ReplaceIdInFile, e.WritingSystemRepository);
				Assert.That(e.WritingSystemRepository.Count, Is.EqualTo(2));
				Assert.That(e.WritingSystemRepository.Get("en"), Is.EqualTo(englishWs));
				Assert.That(e.WritingSystemRepository.Get("de"), Is.Not.Null);
				Assert.That(e.FileContent, Is.EqualTo("en|en|de"));
			}
		}

		[Test]
		public void FindOrphans_OrphanFoundIsNotValidRfcTag_OrphanIsMadeConformAndIsAddedWritingSystemRepoAndWritingSystemIsChangedInFile()
		{
			using (var e = new TestEnvironment("en", "bogusws"))
			{
				var englishWs = new WritingSystemDefinition("en");
				e.WritingSystemRepository.Set(englishWs);
				OrphanFinder.FindOrphans(e.GetIdsFromFile, e.ReplaceIdInFile, e.WritingSystemRepository);
				Assert.That(e.WritingSystemRepository.Count, Is.EqualTo(2));
				Assert.That(e.WritingSystemRepository.Get("en"), Is.EqualTo(englishWs));
				Assert.That(e.WritingSystemRepository.Get("x-bogusws"), Is.Not.Null);
				Assert.That(e.FileContent, Is.EqualTo("en|en|x-bogusws"));
			}
		}

		[Test]
		public void FindOrphans_OrphanFoundIsNotValidRfcTagButWritingSystemRepoKnowsAboutChange_WritingSystemIsChangedInFile()
		{
			using (var e = new TestEnvironment("en", "bogusws"))
			{
				var englishWs = new WritingSystemDefinition("en");
				e.WritingSystemRepository.Set(englishWs);
				e.WritingSystemRepository.Save();
				englishWs.Variant = "x-new";
				e.WritingSystemRepository.Set(englishWs);
				e.WritingSystemRepository.Save();
				OrphanFinder.FindOrphans(e.GetIdsFromFile, e.ReplaceIdInFile, e.WritingSystemRepository);
				Assert.That(e.WritingSystemRepository.Count, Is.EqualTo(2));
				Assert.That(e.WritingSystemRepository.Get("en-x-new"), Is.EqualTo(englishWs));
				Assert.That(e.WritingSystemRepository.Get("x-bogusws"), Is.Not.Null);
				Assert.That(e.FileContent, Is.EqualTo("en-x-new|en-x-new|x-bogusws"));
			}
		}
	}
}
