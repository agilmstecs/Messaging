using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MessagingCash
{
    class Search
    {
        const string myIndex = "realstate";
        const string searchServiceName = "[SEARCH RESOURCE NAME]";
        const string adminApiKey = "[ADMIN KEY]";
        const string queryApiKey = "[QUERY KEY]";
        SearchServiceClient serviceClient;
        public Search()
        {
            serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(adminApiKey));
        }
        public void CreateIndex()
        {
            Console.WriteLine("{0}", "Deleting index if exists...\n");
            if (serviceClient.Indexes.Exists(myIndex))
            {
                serviceClient.Indexes.Delete(myIndex);
            }
            Console.WriteLine("{0}", "Creating index...\n");
            var definition = new Index()
            {
                Name = myIndex,
                Fields = FieldBuilder.BuildForType<RealState>()
            };
            serviceClient.Indexes.Create(definition);
        }
        public void CreateDocument()
        {
            ISearchIndexClient indexClient = serviceClient.Indexes.GetClient(myIndex);
            Console.WriteLine("{0}", "Uploading documents...\n");
            ImportDocuments(indexClient);
        }
        private void ImportDocuments(ISearchIndexClient indexClient)
        {
            var actions = new IndexAction<RealState>[]
            {
                IndexAction.Upload(
                    new RealState()
                    {
                        ListingId = "1",
                        Name = "Madrid",
                        Beds = 2,
                        Baths = "1",
                        Description = "Meilleur hôtel en ville",
                        Status = "Available",
                        Type = "House",
                        City = "Bern",
                        Price = 1200
                    }),
                IndexAction.Upload(
                    new RealState()
                    {
                        ListingId = "2",
                        Name = "Real",
                        Beds = 3,
                        Baths = "2",
                        Description = "Hôtel le moins cher en ville",
                        Status = "Hired",
                        Type = "Room",
                        City = "Madrid",
                        Price = 1600
                    }),
                IndexAction.MergeOrUpload(
                    new RealState()
                    {
                        ListingId = "3",
                        Name = "Empire",
                        Beds = 3,
                        Baths = "2",
                        Description = "Mansion Le Blank",
                        Status = "Sold",
                        Type = "Castle",
                        City = "London",
                        Price = 1000
                    }),
                //IndexAction.Delete(new RealState() { ListingId = "6" })
            };
            var batch = IndexBatch.New(actions);
            try
            {
                indexClient.Documents.Index(batch);
            }
            catch (IndexBatchException e)
            {
                Console.WriteLine("Failed to index some of the documents: {0}", String.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key)));
            }
            Console.WriteLine("Waiting for documents to be indexed...\n");
            Thread.Sleep(2000);
        }
        public void SearchingQueries()
        {
            ISearchIndexClient indexClientForQueries = new SearchIndexClient(searchServiceName, myIndex, new SearchCredentials(queryApiKey));
            SearchParameters parameters;
            DocumentSearchResult<RealState> results;

            Console.WriteLine("Search the entire index for the term 'beds' and return only the status field:\n");
            parameters = new SearchParameters()
            {
                Select = new[] { "status", "description" }
            };
            results = indexClientForQueries.Documents.Search<RealState>("*", parameters);
            foreach (SearchResult<RealState> result in results.Results)
            {
                Console.WriteLine($"Status: {result.Document.Status}, Description: {result.Document.Description}");
            }
            Console.WriteLine();


            //Console.Write("Apply a filter to the index to find places cheaper than $1500, ");
            //Console.WriteLine("and return the listingId and description:\n");
            //parameters =
            //    new SearchParameters()
            //    {
            //        Filter = "price lt 1500",
            //        Select = new[] { "listingId", "price", "description" }
            //    };
            //results = indexClientForQueries.Documents.Search<RealState>("*", parameters);
            //foreach (SearchResult<RealState> result in results.Results)
            //{
            //    Console.WriteLine($"ListingId: {result.Document.ListingId}, Price: {result.Document.Price}, Description: {result.Document.Description}");
            //}
            //Console.WriteLine();

            //Console.Write("Search the entire index, order by a specific field (City) ");
            //Console.Write("in descending order, take the top two results, and show only status and ");
            //Console.WriteLine("city:\n");
            //parameters =
            //    new SearchParameters()
            //    {
            //        OrderBy = new[] { "city desc" },
            //        Select = new[] { "status", "city" },
            //        Top = 2
            //    };
            //results = indexClientForQueries.Documents.Search<RealState>("*", parameters);
            //foreach (SearchResult<RealState> result in results.Results)
            //{
            //    Console.WriteLine($"Status: {result.Document.Status}, City: {result.Document.City}, Description: {result.Document.Description}");
            //}
            //Console.WriteLine();

            //Console.WriteLine("Search the entire index for the term 'Madrid':\n");
            //parameters = new SearchParameters();
            //results = indexClientForQueries.Documents.Search<RealState>("madrid", parameters);
            //foreach (SearchResult<RealState> result in results.Results)
            //{
            //    Console.WriteLine($"ListingId: {result.Document.ListingId}, Beds: {result.Document.Beds}, Baths: {result.Document.Baths}, " +
            //        $"Price: {result.Document.Price}, Status: {result.Document.Status}, City: {result.Document.City}, Type: {result.Document.Type}, " +
            //        $"Description: {result.Document.Description}");
            //    Console.WriteLine();
            //}
            //Console.WriteLine();
        }
    }
    [SerializePropertyNamesAsCamelCase]
    public partial class RealState
    {
        [Key]
        [IsFilterable]
        public string ListingId { get; set; }

        [IsFilterable, IsSortable, IsSearchable]
        public string Name { get; set; }

        [IsFilterable, IsSortable, IsFacetable]
        public int Beds { get; set; }

        [IsSearchable]
        public string Baths { get; set; }

        [IsSearchable]
        public string Description { get; set; }

        [IsSearchable, IsFilterable, IsSortable]
        public string Status { get; set; }

        [IsSearchable, IsFilterable, IsSortable, IsFacetable]
        public string Type { get; set; }

        [IsSearchable, IsFilterable, IsSortable]
        public string City { get; set; }

        [IsFilterable, IsFacetable]
        public double Price { get; set; }
    }
}
