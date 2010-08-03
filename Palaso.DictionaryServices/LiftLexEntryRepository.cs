using System;
using System.Collections.Generic;
using System.Diagnostics;
using Palaso.Code;
using Palaso.Data;
using Palaso.DictionaryServices.Lift;
using Palaso.DictionaryServices.Model;
using Palaso.DictionaryServices.Queries;
using Palaso.Lift.Options;
using Palaso.UiBindings;
using Palaso.Progress;
using Palaso.Text;
using Palaso.WritingSystems;

#if MONO
using Palaso.Linq;
#else

#endif

namespace Palaso.DictionaryServices
{
	public class LiftLexEntryRepository : IDataMapper<LexEntry>, ICountGiver
	{
		public class EntryEventArgs : EventArgs
		{
			public readonly string label;

			public EntryEventArgs(LexEntry entry)
			{
				label = entry.LexicalForm.GetFirstAlternative();
			}
			public EntryEventArgs(RepositoryId repositoryId)
			{
				label = "?";//enhance: how do we get a decent label?
			}
		}
		public event EventHandler<EntryEventArgs> AfterEntryModified;
		public event EventHandler<EntryEventArgs> AfterEntryDeleted;
		//public event EventHandler<EntryEventArgs> AfterEntryAdded;  I (JH) don't know how to tell the difference between new and modified


		readonly ResultSetCacheManager<LexEntry> _caches;

		//hack to prevent sending nested Save calls, which was causing a bug when
		//the exporter caused an item to get a new id, which led eventually to the list thinking it was modified, etc...
		private bool _currentlySaving;

		private readonly IDataMapper<LexEntry> _decoratedDataMapper;

#if DEBUG
		private readonly StackTrace _constructionStackTrace;
#endif

		// review: this constructor is only used for tests, and causes grief with
		// the dispose pattern.  Remove and refactor tests to use the other constructor
		// in a using style. cp
		public LiftLexEntryRepository(string path)
		{
			_disposed = true;
#if DEBUG
			_constructionStackTrace = new StackTrace();
#endif
			_decoratedDataMapper = new LiftDataMapper(path, null, new string[] {}, new ProgressState());
			_caches = new ResultSetCacheManager<LexEntry>(_decoratedDataMapper);
			_disposed = false;
		}

		// review: may want to change LiftDataMapper to IDataMapper<LexEntry> but I (cp) am leaving
		// this for the moment as would also need to change the container builder.Register in WeSayWordsProject
		public LiftLexEntryRepository(LiftDataMapper decoratedDataMapper)
		{
			Guard.AgainstNull(decoratedDataMapper, "decoratedDataMapper");
#if DEBUG
			_constructionStackTrace = new StackTrace();
#endif
			_decoratedDataMapper = decoratedDataMapper;
			_disposed = false;
		}


		public DateTime LastModified
		{
			get { return _decoratedDataMapper.LastModified; }
		}

		public LexEntry CreateItem()
		{
			LexEntry item = _decoratedDataMapper.CreateItem();
			_caches.AddItemToCaches(item);
			return item;
		}

		public RepositoryId[] GetAllItems()
		{
			return _decoratedDataMapper.GetAllItems();
		}

		public int CountAllItems()
		{
			return _decoratedDataMapper.CountAllItems();
		}

		public RepositoryId GetId(LexEntry item)
		{
			return _decoratedDataMapper.GetId(item);
		}

		public LexEntry GetItem(RepositoryId id)
		{
			LexEntry item = _decoratedDataMapper.GetItem(id);
			return item;
		}

		public void SaveItems(IEnumerable<LexEntry> items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			var dirtyItems = new List<LexEntry>();
			foreach (LexEntry item in items)
			{
				if (item.IsDirty)
				{
					dirtyItems.Add(item);
					_caches.UpdateItemInCaches(item);
				}
			}
			_decoratedDataMapper.SaveItems(dirtyItems);
			foreach (LexEntry item in dirtyItems)
			{
				item.Clean();
			}
		}

		public ResultSet<LexEntry> GetItemsMatching(IQuery<LexEntry> query)
		{
			return _decoratedDataMapper.GetItemsMatching(query);
		}

		public void SaveItem(LexEntry item)
		{
			if (_currentlySaving) //sometimes the process of saving leads modification which leads to a new save
			{
				return;
			}
			_currentlySaving = true;
			try
			{

				if (item == null)
				{
					throw new ArgumentNullException("item");
				}
				if (item.IsDirty)
				{
					_decoratedDataMapper.SaveItem(item);
					_caches.UpdateItemInCaches(item);
					item.Clean();

					//review: I (JH) don't know how to tell the difference between new and modified
					if (AfterEntryModified != null)
					{
						AfterEntryModified(this, new EntryEventArgs(item));
					}
				}
			}
			finally
			{
				_currentlySaving = false;
			}
		}

		public bool CanQuery
		{
			get { return _decoratedDataMapper.CanQuery; }
		}

		public bool CanPersist
		{
			get { return _decoratedDataMapper.CanPersist; }
		}

		public void DeleteItem(LexEntry item)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			var args = new EntryEventArgs(item);

			_caches.DeleteItemFromCaches(item);
			_decoratedDataMapper.DeleteItem(item);

			if(AfterEntryDeleted !=null)
			{
				AfterEntryDeleted(this, args);
			}
		}

		public void DeleteItem(RepositoryId repositoryId)
		{
			var args = new EntryEventArgs(repositoryId);

			_caches.DeleteItemFromCaches(repositoryId);
			_decoratedDataMapper.DeleteItem(repositoryId);

			if(AfterEntryDeleted !=null)
			{
				AfterEntryDeleted(this, args);
			}
		}

		public void DeleteAllItems()
		{
			_decoratedDataMapper.DeleteAllItems();
			_caches.DeleteAllItemsFromCaches();
		}

		public void NotifyThatLexEntryHasBeenUpdated(LexEntry updatedLexEntry)
		{
			if(updatedLexEntry == null)
			{
				throw new ArgumentNullException("updatedLexEntry");
			}
			//This call checks that the Entry is in the repository
			GetId(updatedLexEntry);
			_caches.UpdateItemInCaches(updatedLexEntry);
		}

		public int GetHomographNumber(LexEntry entry, WritingSystemDefinition headwordWritingSystem)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("entry");
			}
			if (headwordWritingSystem == null)
			{
				throw new ArgumentNullException("headwordWritingSystem");
			}
			ResultSet<LexEntry> resultSet = GetAllEntriesSortedByHeadword(headwordWritingSystem);
			RecordToken<LexEntry> first = resultSet.FindFirst(entry);
			if (first == null)
			{
				throw new ArgumentOutOfRangeException("entry", entry, "Entry not in repository");
			}
			if ((bool) first["HasHomograph"])
			{
				return (int) first["HomographNumber"];
			}
			return 0;
		}

		/// <summary>
		/// Gets a ResultSet containing all entries sorted by citation if one exists and otherwise
		/// by lexical form.
		/// Use "Form" to access the headword in a RecordToken.
		/// </summary>
		/// <param name="writingSystemDefinition"></param>
		/// <returns></returns>
		public ResultSet<LexEntry> GetAllEntriesSortedByHeadword(WritingSystemDefinition writingSystemDefinition)
		{
			if (writingSystemDefinition == null)
			{
				throw new ArgumentNullException("writingSystemDefinition");
			}

			HeadwordQuery headWordQuery = new HeadwordQuery(writingSystemDefinition);

			ResultSet<LexEntry> resultsFromCache = GetResultsFromCache(headWordQuery);

			string previousHeadWord = null;
			int homographNumber = 1;
			RecordToken<LexEntry> previousToken = null;
			foreach (RecordToken<LexEntry> token in resultsFromCache)
			{
				// A null Form indicates there is no HeadWord in this writing system.
				// However, we need to ensure that we return all entries, so the AtLeastOne in the query
				// above ensures that we keep it in the result set with a null Form and null WritingSystemId.
				var currentHeadWord = (string) token["Form"];
				if (string.IsNullOrEmpty(currentHeadWord))
				{
					token["HasHomograph"] = false;
					token["HomographNumber"] = 0;
					continue;
				}
				if (currentHeadWord == previousHeadWord)
				{
					homographNumber++;
				}
				else
				{
					previousHeadWord = currentHeadWord;
					homographNumber = 1;
				}
				// only used to get our sort correct --This comment seems nonsensical --TA 2008-08-14!!!
				token["HomographNumber"] = homographNumber;
				switch (homographNumber)
				{
					case 1:
						token["HasHomograph"] = false;
						break;
					case 2:
						Debug.Assert(previousToken != null);
						previousToken["HasHomograph"] = true;
						token["HasHomograph"] = true;
						break;
					default:
						token["HasHomograph"] = true;
						break;
				}
				previousToken = token;
			}

			return resultsFromCache;
		}

		/// <summary>
		/// Gets a ResultSet containing all entries sorted by lexical form for a given writing system.
		/// If a lexical form for a given writingsystem does not exist we substitute one from another writingsystem.
		/// Use "Form" to access the lexical form in a RecordToken.
		/// </summary>
		/// <param name="writingSystemDefinition"></param>
		/// <returns></returns>
		public ResultSet<LexEntry> GetAllEntriesSortedByLexicalFormOrAlternative(WritingSystemDefinition writingSystemDefinition)
		{
			if (writingSystemDefinition == null)
			{
				throw new ArgumentNullException("writingSystemDefinition");
			}
			LexicalFormOrAlternativeQuery lexicalFormOrAlternativeQuery = new LexicalFormOrAlternativeQuery(writingSystemDefinition);
			return GetResultsFromCache(lexicalFormOrAlternativeQuery);
		}

		/// <summary>
		/// Gets a ResultSet containing all entries sorted by lexical form for a given writing system.
		/// Use "Form" to access the lexical form in a RecordToken.
		/// </summary>
		/// <param name="writingSystemDefinition"></param>
		/// <returns></returns>
		private ResultSet<LexEntry> GetAllEntriesSortedByLexicalForm(WritingSystemDefinition writingSystemDefinition)
		{
			if (writingSystemDefinition == null)
			{
				throw new ArgumentNullException("writingSystemDefinition");
			}
			LexicalFormQuery lexicalFormQuery = new LexicalFormQuery(writingSystemDefinition);

			return GetResultsFromCache(lexicalFormQuery);
		}

		private ResultSet<LexEntry> GetAllEntriesSortedByGuid()
		{
			GuidQuery guidQuery = new GuidQuery();
			return GetResultsFromCache(guidQuery);
		}

		private ResultSet<LexEntry> GetResultsFromCache(IQuery<LexEntry> query)
		{
			if (_caches[query.UniqueLabel] == null)
			{
				ResultSet<LexEntry> results = _decoratedDataMapper.GetItemsMatching(query);
				_caches.Add(query,results);
			}
			ResultSet<LexEntry> resultsFromCache = _caches[query.UniqueLabel].GetResultSet();

			return resultsFromCache;
		}

		private ResultSet<LexEntry> GetAllEntriesSortedById()
		{
			IdQuery idQuery = new IdQuery();
			return GetResultsFromCache(idQuery);
		}

		/// <summary>
		/// Gets a ResultSet containing all entries sorted by definition and gloss. It will return both the definition
		/// and the gloss if both exist and are different.
		/// Use "Form" to access the Definition/Gloss in RecordToken.
		/// </summary>
		/// <param name="writingSystemDefinition"></param>
		/// <returns>Definition and gloss in "Form" field of RecordToken</returns>
		public ResultSet<LexEntry> GetAllEntriesSortedByDefinitionOrGloss(WritingSystemDefinition writingSystemDefinition)
		{
			if (writingSystemDefinition == null)
			{
				throw new ArgumentNullException("writingSystemDefinition");
			}
			IQuery<LexEntry> definitionQuery = new DefinitionQuery(writingSystemDefinition);
			IQuery<LexEntry> glossQuery = new GlossQuery(writingSystemDefinition);
			KeyMap glossToFormMap = new KeyMap(){{"Gloss","Form"}};
			IQuery<LexEntry> definitionOrGlossQuery = definitionQuery.GetAlternative(glossQuery, glossToFormMap);

			KeyMap formToGlossMap = new KeyMap() { {"Form", "Gloss"} };
			IQuery<LexEntry> glossOrDefinitionQuery = glossQuery.GetAlternative(definitionQuery, formToGlossMap);
			IQuery<LexEntry> finalQuery = (definitionOrGlossQuery.Merge(glossOrDefinitionQuery, glossToFormMap)).StripDuplicates();
			return GetResultsFromCache(finalQuery);
		}

		/// <summary>
		/// Gets a ResultSet containing entries that contain a semantic domain assigned to them
		/// sorted by semantic domain.
		/// Use "SemanticDomain" to access the semantic domain in a RecordToken.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public ResultSet<LexEntry> GetEntriesWithSemanticDomainSortedBySemanticDomain()
		{
			SemanticDomainQuery semanticDomainQuery = new SemanticDomainQuery();
			return GetResultsFromCache(semanticDomainQuery);
		}


		private ResultSet<LexEntry> GetAllEntriesWithGlossesSortedByLexicalForm(WritingSystemDefinition lexicalUnitWritingSystemDefinition)
		{
			if (lexicalUnitWritingSystemDefinition == null)
			{
				throw new ArgumentNullException("lexicalUnitWritingSystemDefinition");
			}
		   LexicalFormsWithGlossesQuery lexicalFormWithGlossesQuery = new LexicalFormsWithGlossesQuery(lexicalUnitWritingSystemDefinition);
			return GetResultsFromCache(lexicalFormWithGlossesQuery);
		}

		/// <summary>
		/// Gets a ResultSet containing entries whose gloss match glossForm sorted by the lexical form
		/// in the given writingsystem.
		/// Use "Form" to access the lexical form and "Gloss/Form" to access the Gloss in a RecordToken.
		/// </summary>
		/// <param name="glossForm"></param>
		/// <param name="lexicalUnitWritingSystemDefinition"></param>
		/// <returns></returns>
		public ResultSet<LexEntry> GetEntriesWithMatchingGlossSortedByLexicalForm(
			LanguageForm glossForm, WritingSystemDefinition lexicalUnitWritingSystemDefinition)
		{

			if (null==glossForm || string.IsNullOrEmpty(glossForm.Form))
			{
				throw new ArgumentNullException("glossForm");
			}
			if (lexicalUnitWritingSystemDefinition == null)
			{
				throw new ArgumentNullException("lexicalUnitWritingSystemDefinition");
			}
			var allGlossesResultSet = GetAllEntriesWithGlossesSortedByLexicalForm(lexicalUnitWritingSystemDefinition);
			var filteredResultSet = new List<RecordToken<LexEntry>>();
			foreach (RecordToken<LexEntry> recordToken in allGlossesResultSet)
			{
				if (((string) recordToken["Gloss"] == glossForm.Form)
					&& ((string) recordToken["GlossWritingSystem"] == glossForm.WritingSystemId))
				{
					filteredResultSet.Add(recordToken);
				}
			}

			return new ResultSet<LexEntry>(this, filteredResultSet);
		}

		/// <summary>
		/// Gets the LexEntry whose Id matches id.
		/// </summary>
		/// <returns></returns>
		public LexEntry GetLexEntryWithMatchingId(string id)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			if (id == string.Empty)
			{
				throw new ArgumentOutOfRangeException("id", "The Id should not be empty.");
			}
			ResultSet<LexEntry> allGlossesResultSet = GetAllEntriesSortedById();
			var filteredResultSet = new List<RecordToken<LexEntry>>();
			foreach (RecordToken<LexEntry> recordToken in allGlossesResultSet)
			{
				if (((string)recordToken["Id"] == id))
				{
					filteredResultSet.Add(recordToken);
				}
			}
			if (filteredResultSet.Count > 1)
			{
				throw new ApplicationException("More than one entry exists with the guid " + id);
			}
			if (filteredResultSet.Count == 0)
			{
				return null;
			}
			return filteredResultSet[0].RealObject;
		}

		/// <summary>
		/// Gets the LexEntry whose Guid matches guid.
		/// </summary>
		/// <returns></returns>
		public LexEntry GetLexEntryWithMatchingGuid(Guid guid)
		{
			if(guid == Guid.Empty)
			{
				throw new ArgumentOutOfRangeException("guid", "Guids should not be empty!");
			}
			ResultSet<LexEntry> allGlossesResultSet = GetAllEntriesSortedByGuid();
			var filteredResultSet = new List<RecordToken<LexEntry>>();
			foreach (RecordToken<LexEntry> recordToken in allGlossesResultSet)
			{
				if (((Guid) recordToken["Guid"] == guid))
				{
					filteredResultSet.Add(recordToken);
				}
			}
			if(filteredResultSet.Count > 1)
			{
				throw new ApplicationException("More than one entry exists with the guid " + guid);
			}
			if(filteredResultSet.Count == 0)
			{
				return null;
			}
			return filteredResultSet[0].RealObject;
		}

		/// <summary>
		/// Gets a ResultSet containing entries whose lexical form is similar to lexicalForm
		/// sorted by the lexical form in the given writingsystem.
		/// Use "Form" to access the lexical form in a RecordToken.
		/// </summary>
		/// <returns></returns>
		public ResultSet<LexEntry> GetEntriesWithSimilarLexicalForm(string lexicalForm,
																	WritingSystemDefinition writingSystemDefinition,
																	ApproximateMatcherOptions
																		matcherOptions)
		{
			if (lexicalForm == null)
			{
				throw new ArgumentNullException("lexicalForm");
			}
			if (writingSystemDefinition == null)
			{
				throw new ArgumentNullException("writingSystemDefinition");
			}
			return new ResultSet<LexEntry>(this,
										   ApproximateMatcher.FindClosestForms
											   <RecordToken<LexEntry>>(
											   GetAllEntriesSortedByLexicalForm(writingSystemDefinition),
											   GetFormForMatchingStrategy,
											   lexicalForm,
											   matcherOptions));
		}

		private static string GetFormForMatchingStrategy(object item)
		{
			return (string) ((RecordToken<LexEntry>) item)["Form"];
		}

		/// <summary>
		/// Gets a ResultSet containing entries whose lexical form match lexicalForm
		/// Use "Form" to access the lexical form in a RecordToken.
		/// </summary>
		/// <param name="lexicalForm"></param>
		/// <param name="writingSystemDefinition"></param>
		/// <returns></returns>
		public ResultSet<LexEntry> GetEntriesWithMatchingLexicalForm(string lexicalForm,
																	 WritingSystemDefinition writingSystemDefinition)
		{
			if (lexicalForm == null)
			{
				throw new ArgumentNullException("lexicalForm");
			}
			if (writingSystemDefinition == null)
			{
				throw new ArgumentNullException("writingSystemDefinition");
			}
			ResultSet<LexEntry> allGlossesResultSet = GetAllEntriesSortedByLexicalForm(writingSystemDefinition);
			var filteredResultSet = new List<RecordToken<LexEntry>>();
			foreach (RecordToken<LexEntry> recordToken in allGlossesResultSet)
			{
				if (((string)recordToken["Form"] == lexicalForm))
				{
					filteredResultSet.Add(recordToken);
				}
			}

			return new ResultSet<LexEntry>(this, filteredResultSet);
		}

//
//        private string MakeSafeForFileName(string fileName)
//        {
//            foreach (char invalChar in Path.GetInvalidFileNameChars())
//            {
//                fileName = fileName.Replace(invalChar.ToString(), "");
//            }
//            return fileName;
//        }

		#region IDisposable Members

#if DEBUG
		~LiftLexEntryRepository()
		{
			if (!_disposed)
			{
				throw new ApplicationException(
					"Disposed not explicitly called on LexEntryRepository." + "\n" + _constructionStackTrace
					);
			}
		}
#endif

		private bool _disposed = true;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					// dispose-only, i.e. non-finalizable logic
					_decoratedDataMapper.Dispose();
				}

				// shared (dispose and finalizable) cleanup logic
				_disposed = true;
			}
		}

		protected void VerifyNotDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException("LexEntryRepository");
			}
		}

		#endregion

		public int Count
		{
			get { return CountAllItems(); }
		}
	}
}