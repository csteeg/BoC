using BoC.Helpers;
using Xunit;
using System.Collections;
using Xunit.Extensions;
using System.Collections.Generic;

namespace BoC.Tests.Helpers
{
	public class InflectorTestCase
	{
		#region Fixture Data

		private IDictionary<string,string> singularToPlural;

        public InflectorTestCase()
        {
            singularToPlural = new Dictionary<string,string>
                                   {
                                       {"search", "searches"},
                                       {"switch", "switches"},
                                       {"fix", "fixes"},
                                       {"box", "boxes"},
                                       {"process", "processes"},
                                       {"address", "addresses"},
                                       {"case", "cases"},
                                       {"stack", "stacks"},
                                       {"wish", "wishes"},
                                       {"fish", "fish"},
                                       {"category", "categories"},
                                       {"query", "queries"},
                                       {"ability", "abilities"},
                                       {"agency", "agencies"},
                                       {"movie", "movies"},
                                       {"archive", "archives"},
                                       {"index", "indices"},
                                       {"wife", "wives"},
                                       {"safe", "saves"},
                                       {"half", "halves"},
                                       {"move", "moves"},
                                       {"salesperson", "salespeople"},
                                       {"person", "people"},
                                       {"spokesman", "spokesmen"},
                                       {"man", "men"},
                                       {"woman", "women"},
                                       {"basis", "bases"},
                                       {"diagnosis", "diagnoses"},
                                       {"datum", "data"},
                                       {"medium", "media"},
                                       {"analysis", "analyses"},
                                       {"node_child", "node_children"},
                                       {"child", "children"},
                                       {"experience", "experiences"},
                                       {"day", "days"},
                                       {"comment", "comments"},
                                       {"foobar", "foobars"},
                                       {"newsletter", "newsletters"},
                                       {"old_news", "old_news"},
                                       {"news", "news"},
                                       {"series", "series"},
                                       {"species", "species"},
                                       {"quiz", "quizzes"},
                                       {"perspective", "perspectives"},
                                       {"ox", "oxen"},
                                       {"photo", "photos"},
                                       {"buffalo", "buffaloes"},
                                       {"tomato", "tomatoes"},
                                       {"dwarf", "dwarves"},
                                       {"elf", "elves"},
                                       {"information", "information"},
                                       {"equipment", "equipment"},
                                       {"bus", "buses"},
                                       {"status", "statuses"},
                                       {"status_code", "status_codes"},
                                       {"mouse", "mice"},
                                       {"louse", "lice"},
                                       {"house", "houses"},
                                       {"octopus", "octopi"},
                                       {"virus", "viri"},
                                       {"alias", "aliases"},
                                       {"portfolio", "portfolios"},
                                       {"vertex", "vertices"},
                                       {"matrix", "matrices"},
                                       {"axis", "axes"},
                                       {"testis", "testes"},
                                       {"crisis", "crises"},
                                       {"rice", "rice"},
                                       {"shoe", "shoes"},
                                       {"horse", "horses"},
                                       {"prize", "prizes"},
                                       {"edge", "edges"}
                                   };
        }

		#endregion

		[Fact]
		public void PluralizePlurals()
		{
			Assert.Equal("plurals", Inflector.Pluralize("plurals"));
			Assert.Equal("Plurals", Inflector.Pluralize("Plurals"));
		}

        [Fact]
		public void Pluralize()
		{
            foreach (var dictionaryEntry in singularToPlural)
			{
				Assert.Equal(dictionaryEntry.Value, Inflector.Pluralize((string) dictionaryEntry.Key));
				Assert.Equal(Inflector.Capitalize((string) dictionaryEntry.Value),
				                Inflector.Pluralize(Inflector.Capitalize((string) dictionaryEntry.Key)));
			}
		}

		[Fact]
		public void Singularize()
		{
            foreach (var dictionaryEntry in singularToPlural)
			{
				Assert.Equal(dictionaryEntry.Key, Inflector.Singularize((string) dictionaryEntry.Value));
				Assert.Equal(Inflector.Capitalize((string) dictionaryEntry.Key),
				                Inflector.Singularize(Inflector.Capitalize((string) dictionaryEntry.Value)));
			}
		}
	}
}
