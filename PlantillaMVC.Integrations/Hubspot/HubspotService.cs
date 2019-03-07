using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HubSpot.NET.Api.Contact.Dto;
using HubSpot.NET.Api.Deal.Dto;
using HubSpot.NET.Api.Engagement.Dto;
using HubSpot.NET.Core;
using HubSpot.NET.Core.Interfaces;
using Newtonsoft.Json;
using PlantillaMVC.Integrations.Hubspot;
using RestSharp;

namespace PlantillaMVC.Integrations
{
    public interface IHubspotService
    {
        object CreateContact();
        
        DealHubSpotResult ReadDeals(int limit, long offset);

        CompanyHubSpotResult GetCompanyById(long id);
        ContactHubSpotResult GetContactById(long id);

        string AssociateCompanyToTicket(long ticketId, long companyId);

        CompanyTicketCreateHubspotResponse CreateTicketToCompany(CompanyTicketHubspotSave ticketToSave);

        CompaniesHubSpotResult GetAllCompanies(int limit, long offset);
        IList<HubspotOwnerModel> GetOwners();
        IDictionary<int, string> GetDisctionaryEmailsOwner();
        PipelinesHubSpotResult GetDealsPipelines();
        PipelinesHubSpotResult GetAllPipelines(string objType);
        IDictionary<string, IDictionary<string, PipelineState>> GetDealsPipelinesStages();


    }
    public class HubspotService : IHubspotService
    {
        private IHubSpotApi apiService;
        private string apiKey;
        private string apiKeyMetrolab;
        private RestClient client;
        public HubspotService()
        {
            apiService = new HubSpotApi(System.Configuration.ConfigurationManager.AppSettings["HubspotApiKey"]);
            apiKey = System.Configuration.ConfigurationManager.AppSettings["HubspotApiKey"];
            apiKeyMetrolab = System.Configuration.ConfigurationManager.AppSettings["HubspotApiKey_Metrolab"];
            client = new RestClient("https://api.hubapi.com");
        }

        public object CreateContact()
        {
            // Create a contact
            var contact = apiService.Contact.Create(new ContactHubSpotModel()
            {
                Email = "john@gmail.com",
                FirstName = "John",
                LastName = "Smith",
                Phone = "00000 000000",
                Company = "Test Company"
            });

            return contact;
        }
        
        public DealHubSpotResult ReadDeals(int limit, long offset)
        {
            RestRequest request = new RestRequest("/deals/v1/deal/paged", Method.GET);
            request.AddParameter("hapikey", apiKey);
            request.AddParameter("includeAssociations", true);
            if (limit > 0) request.AddParameter("limit", limit);
            if (offset > 0) request.AddParameter("offset", offset);
            request.AddParameter("properties", "hubspot_owner_id");
            request.AddParameter("properties", "amount");
            request.AddParameter("properties", "closedate");
            request.AddParameter("properties", "dealstage");
            request.AddParameter("properties", "dealname");
            request.AddParameter("properties", "linea");
            request.AddParameter("properties", "factor");
            request.AddParameter("properties", "dealtype");
            request.AddParameter("properties", "rfc");
            request.AddParameter("properties", "pipeline");
            request.AddParameter("properties", "num_factura_epicor");

            IRestResponse response = client.Execute(request);
            DealHubSpotResult result = JsonConvert.DeserializeObject<DealHubSpotResult>(response.Content);
            return result;
        }
        public PipelinesHubSpotResult GetAllPipelines(string objType)
        {
            RestRequest request = new RestRequest("/crm-pipelines/v1/pipelines/{objType}", Method.GET);
            request.AddParameter("hapikey", apiKey);
            request.AddUrlSegment("objType", objType);
            IRestResponse response = client.Execute(request);
            PipelinesHubSpotResult result = JsonConvert.DeserializeObject<PipelinesHubSpotResult>(response.Content);
            return result;
        }
        public PipelinesHubSpotResult GetDealsPipelines()
        {
            return this.GetAllPipelines("deals");
        }
        public IDictionary<string, IDictionary<string, PipelineState>> GetDealsPipelinesStages()
        {
            PipelinesHubSpotResult pipelineResult = this.GetDealsPipelines();
            if (pipelineResult == null || pipelineResult.Pipelines == null || !pipelineResult.Pipelines.Any())
            {
                return null;
            }
            IEnumerable<Pipeline> pipelines = 
                pipelineResult.Pipelines.Where(x => x.Stages != null && x.Stages.Any(stage=> stage.Metadata != null));
            return pipelineResult.Pipelines.ToDictionary(x => x.PipelineId, y => (IDictionary<string, PipelineState>)y.Stages.ToDictionary(k=>k.StageId, v=>v));

        }


        public CompanyHubSpotResult GetCompanyById(long id)
        {

            RestRequest request = new RestRequest("/companies/v2/companies/{id}", Method.GET);
            request.AddParameter("hapikey", apiKey);
            request.AddUrlSegment("id", id.ToString());

            IRestResponse response = client.Execute(request);

            CompanyHubSpotResult result = JsonConvert.DeserializeObject<CompanyHubSpotResult>(response.Content);

            return result;
        }


        public IList<HubspotOwnerModel> GetOwners()
        {
            RestRequest request = new RestRequest("/owners/v2/owners", Method.GET);
            request.AddParameter("hapikey", apiKey);
            IRestResponse response = client.Execute(request);
            IList<HubspotOwnerModel> result = JsonConvert.DeserializeObject<IList<HubspotOwnerModel>>(response.Content);
            return result;
        }

        public IDictionary<int, string> GetDisctionaryEmailsOwner()
        {
            IList<HubspotOwnerModel> hubspotOwnerModels = this.GetOwners();
            if(hubspotOwnerModels == null)
            {
                return null;
            }
            return hubspotOwnerModels.ToDictionary(x => x.OwnerId, y => y.Email);
        }

        public ContactHubSpotResult GetContactById(long id)
        {

            RestRequest request = new RestRequest("/contacts/v1/contact/vid/{id}/profile", Method.GET);
            request.AddParameter("hapikey", apiKey);
            request.AddUrlSegment("id", id.ToString());

            IRestResponse response = client.Execute(request);

            ContactHubSpotResult result = JsonConvert.DeserializeObject<ContactHubSpotResult>(response.Content);

            return result;



        }

        public CompanyTicketCreateHubspotResponse CreateTicketToCompany(CompanyTicketHubspotSave ticket)
        {
            //https://developers.hubspot.com/docs/methods/crm-associations/crm-associations-overview
            //Ticket to company 	26
            CompanyTicketCreateHubspotResponse responseModel = new CompanyTicketCreateHubspotResponse();
            List<TicketProperty> ticketProperties = new List<TicketProperty>();
            ticketProperties.Add(new TicketProperty() { Name = "subject", Value = ticket.Subject.Trim() });
            ticketProperties.Add(new TicketProperty() { Name = "content", Value = ticket.Content.Trim() });
            ticketProperties.Add(new TicketProperty() { Name = "numero_de_operaci_n", Value = ticket.NumeroOperacion.ToString() });
            ticketProperties.Add(new TicketProperty() { Name = "monto", Value = ticket.Monto.ToString() });
            ticketProperties.Add(new TicketProperty() { Name = "hs_pipeline", Value = ticket.PipelineId });
            ticketProperties.Add(new TicketProperty() { Name = "hs_pipeline_stage", Value = ticket.PipelineStageId });
            string jsonToSend = JsonConvert.SerializeObject(ticketProperties);

            var request = new RestRequest("/crm-objects/v1/objects/tickets?hapikey=" + apiKeyMetrolab);
            request.Method = Method.POST;
            request.AddHeader("Accept", "application/json");
            request.Parameters.Clear();
            request.AddParameter("application/json", jsonToSend, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            TicketResponse ticketResponse = JsonConvert.DeserializeObject<TicketResponse>(response.Content);
            if (ticketResponse.ObjectId > 0)
            {
                Association association = new Association()
                {
                    FromObjectId = ticketResponse.ObjectId,
                    ToObjectId = ticket.CompanyId,
                    Category = "HUBSPOT_DEFINED",
                    DefinitionId = ticket.DefinitionId
                };
                jsonToSend = JsonConvert.SerializeObject(association);
                request = new RestRequest("/crm-associations/v1/associations?hapikey=" + apiKeyMetrolab);
                request.Method = Method.PUT;
                request.AddParameter("application/json", jsonToSend, ParameterType.RequestBody);
                request.AddHeader("Content-Type", "application/json");
                response = client.Put(request);
                if(response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    responseModel.IsCreated = true;
                    responseModel.TicketId = ticketResponse.ObjectId;
                }
            }
            responseModel.Content = response.Content;
            responseModel.StatusHttp = (int)response.StatusCode;
            return responseModel;
        }
        
        public CompaniesHubSpotResult GetAllCompanies(int limit, long offset)
        {
            RestRequest request = new RestRequest("/companies/v2/companies/paged", Method.GET);
            request.AddParameter("hapikey", apiKeyMetrolab);
            if (limit > 0) request.AddParameter("limit", limit);
            if (offset > 0) request.AddParameter("offset", offset);
            request.AddParameter("properties", "name");
            request.AddParameter("properties", "rfc");
            IRestResponse response = client.Execute(request);
            CompaniesHubSpotResult result = JsonConvert.DeserializeObject<CompaniesHubSpotResult>(response.Content);
            return result;
        }

        public string AssociateCompanyToTicket(long companyId, long ticketId)
        {
            //https://developers.hubspot.com/docs/methods/crm-associations/crm-associations-overview
            //Company to ticket   25
 
            Association association = new Association()
            {
                FromObjectId = companyId,
                ToObjectId = ticketId,
                Category = "HUBSPOT_DEFINED",
                DefinitionId = 25
            };
            string jsonToSend = JsonConvert.SerializeObject(association);
            var request = new RestRequest("/crm-associations/v1/associations?hapikey=" + apiKey);
            request.Method = Method.PUT;
            request.AddParameter("application/json", jsonToSend, ParameterType.RequestBody);
            request.AddHeader("Content-Type", "application/json");
            var response = client.Put(request);

            return response.Content;
        }

        public PipelinesHubSpotResult GetAllPipelines()
        {
            RestRequest request = new RestRequest("/crm-pipelines/v1/pipelines/deals", Method.GET);
            request.AddParameter("hapikey", apiKey);
            IRestResponse response = client.Execute(request);
            PipelinesHubSpotResult result = JsonConvert.DeserializeObject<PipelinesHubSpotResult>(response.Content);
            return result;
        }

    }
}
