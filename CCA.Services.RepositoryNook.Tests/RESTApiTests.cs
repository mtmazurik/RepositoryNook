using CCA.Services.RepositoryNook.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using models = CCA.Services.RepositoryNook.Models;
using RestSharp;

namespace CCA.Services.RepositoryNook.Tests
{
    [TestClass]
    public class RestApiTests
    {
        [TestMethod]
        public void TestCreate()
        {
            RestClient client = new RestClient("http://localhost:8902");

            for(int i = 1; i <= 100; i++)
            {
                {
                    RestRequest request = new RestRequest("/flight-db/flights");
                    request.AddHeader("Content-Type", "application/json");
                    request.AddJsonBody(PopulateRepositoryObject(i));
                    IRestResponse<Repository> response = client.Post<Repository>(request);
                }
            }
        }

        private static Repository PopulateRepositoryObject(int i)
        {
            Repository repo = new Repository();
            repo.key = "flightNumber" + i;
            repo.tags =  new string[2]{"planeType:777x","manufacturer:Boeing"};
            repo.createdBy = "RestApiTests";
            repo.app = "FlightBook";
            repo.validate = false;
            repo.schemaUri = "https://cloudcomputingassociates.com/schemas/Flight.schema.json";
            repo.data = "{ \"flight\" : { \"flightNumber\" : \"580\", \"dayOfWeek\" : \"Monday\",\"manufacturer\":\"Boeing\", \"planeType\":\"777x\" }}";
            return repo;
        }
    }
}
