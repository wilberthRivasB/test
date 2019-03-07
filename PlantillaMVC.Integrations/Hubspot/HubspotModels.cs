using HubSpot.NET.Api.Deal.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PlantillaMVC.Integrations.Hubspot
{
    public class HubspotDealModel : DealHubSpotModel
    {

        public string linea_de_negocio { set; get; }


        public int RelatedCompanies { set; get; }

        public int RelatedContacts { set; get; }
        [DataMember(Name = "associations")]
        public HubSpotAssociationsDeals AssociationsModel { get; set; }
    }
    public class HubSpotAssociationsDeals : DealHubSpotAssociations
    {
        [DataMember(Name = "associatedDealIds")]
        public long[] AssociatedDealIds { get; set; }
    }
    [DataContract]
    public class HubspotDealsResult
    {
        public long PortalId { set; get; }

        public long DealId { set; get; }
        [DataMember(Name = "deals")]
        public List<HubspotDeal> Deals { set; get; }
    }

    public class HubspotDeal
    {
        public List<HubspotDataEntity> Properties { set; get; }
        
    }

    public class HubspotDataEntity
    {
    }

    public class HubspotDataEntityProp
    {
        public string Property { get; set; }

        public object Value { get; set; }

        public object Name { get; set; }
    }
    [DataContract]
    public class Associations
    {
        [DataMember(Name = "associatedVids")]
        public List<long> AssociatedVids { get; set; }
        [DataMember(Name = "associatedCompanyIds")]
        public List<long> associatedCompanyIds { get; set; }
        [DataMember(Name = "associatedDealIds")]
        public List<long> AssociatedDealIds { get; set; }
    }
    [DataContract]
    public class Version
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "value")]
        public string Value { get; set; }
        [DataMember(Name = "timestamp")]
        public long Timestamp { get; set; }
        [DataMember(Name = "source")]
        public string source { get; set; }
        [DataMember(Name = "sourceVid")]
        public List<long> SourceVid { get; set; }
    }
    [DataContract]
    public class Property
    {
        [DataMember(Name = "value")]
        public string Value { get; set; }
        [DataMember(Name = "timestamp")]
        public string Timestamp { get; set; }
        [DataMember(Name = "source")]
        public string source { get; set; }
        [DataMember(Name = "sourceId")]
        public string SourceId { get; set; }
        [DataMember(Name = "versions")]
        public List<Version> Versions { get; set; }
    }
   
    [DataContract]
    public class Properties
    {
        [DataMember(Name = "dealname")]
        public Property Dealname { get; set; }
        [DataMember(Name = "pipeline")]
        public Property Pipeline { get; set; }
        [DataMember(Name = "amount")]
        public Property Amount { get; set; }
        [DataMember(Name = "hubspot_owner_id")]
        public Property HubspotOwnerId { get; set; }
        [DataMember(Name = "closedate")]
        public Property CloseDate { get; set; }
        [DataMember(Name = "linea")]
        public Property LineaDeNegocio { get; set; }
        [DataMember(Name = "factor")]
        public Property Factor { get; set; }
        [DataMember(Name = "dealstage")]
        public Property DealStage { get; set; }
        [DataMember(Name = "num_factura_epicor")]
        public Property NumFacturaEpicor { get; set; }
        [DataMember(Name = "hs_object_id")]
        public Property HsObjectId { get; set; }

        //Propiedades de Compania
        [DataMember(Name = "name")]
        public Property Name { get; set; }

        [DataMember(Name = "rfc")]
        public Property RFC { get; set; }
    }
    [DataContract]
    public class Deal
    {
        [DataMember(Name = "portalId")]
        public int PortalId { get; set; }
        [DataMember(Name = "dealId")]
        public int DealId { get; set; }
        [DataMember(Name = "isDeleted")]
        public bool IsDeleted { get; set; }
        [DataMember(Name = "associations")]
        public Associations Associations { get; set; }
        [DataMember(Name = "properties")]
        public Properties Properties { get; set; }
        [DataMember(Name = "imports")]
        public List<object> Imports { get; set; }
    }

    [DataContract]
    public class DealHubSpotResult
    {
        [DataMember(Name = "deals")]
        public List<Deal> Deals { get; set; }
        [DataMember(Name = "hasMore")]
        public bool HasMore { get; set; }
        [DataMember(Name = "offset")]
        public long Offset { get; set; }
    }

    [DataContract]
    public class CompanyHubSpotResult
    {
        [DataMember(Name = "portalId")]
        public int PortalId { get; set; }
        [DataMember(Name = "companyId")]
        public int CompanyId { get; set; }
        [DataMember(Name = "properties")]
        public CompanyProperties Properties { get; set; }
    }

    [DataContract]
    public class CompanyProperties
    {
        [DataMember(Name = "name")]
        public Property Name { get; set; }
        [DataMember(Name = "domain")]
        public Property Domain { get; set; }
        [DataMember(Name = "rfc")]
        public Property RFC { get; set; }
    }

    [DataContract]
    public class ContactHubSpotResult
    {
        [DataMember(Name = "portalId")]
        public int PortalId { get; set; }
        [DataMember(Name = "companyId")]
        public int CompanyId { get; set; }
        [DataMember(Name = "properties")]
        public ContactProperties Properties { get; set; }
    }

    [DataContract]
    public class ContactProperties
    {
        [DataMember(Name = "firstname")]
        public Property FirstName { get; set; }

        [DataMember(Name = "email")]
        public Property Email { get; set; }
    }

    [DataContract]
    public class Association
    {
        [DataMember(Name = "fromObjectId")]
        public long FromObjectId { get; set; }
        [DataMember(Name = "toObjectId")]
        public long ToObjectId { get; set; }
        [DataMember(Name = "category")]
        public string Category { get; set; }
        [DataMember(Name = "definitionId")]
        public int DefinitionId { get; set; }
    }

    [DataContract]
    public class TicketProperty
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "value")]
        public string Value { get; set; }
    }

    [DataContract]
    public class TicketResponse
    {
        [DataMember(Name = "objectId")]
        public long ObjectId { get; set; }
        [DataMember(Name = "objectType")]
        public string ObjectType { get; set; }
    }

    [DataContract]
    public class CompaniesHubSpotResult
    {
        [DataMember(Name = "companies")]
        public List<Company> Companies { get; set; }
        [DataMember(Name = "has-more")]
        public bool HasMore { get; set; }
        [DataMember(Name = "offset")]
        public long Offset { get; set; }
    }

    [DataContract]
    public class Company
    {
        [DataMember(Name = "portalId")]
        public int PortalId { get; set; }
        [DataMember(Name = "companyId")]
        public int CompanyId { get; set; }
        [DataMember(Name = "isDeleted")]
        public bool IsDeleted { get; set; }
        [DataMember(Name = "properties")]
        public Properties Properties { get; set; }
    }

    [DataContract]
    public class  PipelinesHubSpotResult
    {
        [DataMember(Name = "results")]
        public List<Pipeline> Pipelines { get; set; }
    }
    [DataContract]
    public class Pipeline
    {
        [DataMember(Name = "pipelineId")]
        public string PipelineId { get; set; }
        [DataMember(Name = "stages")]
        public IList<PipelineState> Stages { get; set; }
    }
    [DataContract]
    public class PipelineState
    {
        [DataMember(Name = "stageId")]
        public string StageId { get; set; }
        [DataMember(Name = "label")]
        public string Label { get; set; }
        [DataMember(Name = "metadata")]
        public PipelineStateMetadata Metadata { get; set; }
    }
    [DataContract]
    public class PipelineStateMetadata
    {
        [DataMember(Name = "probability")]
        public decimal Probability { get; set; }
    }
    public class CompanyTicketHubspotSave
    {
        public string Subject { get; set; }
        public string Content { get; set; }
        public long CompanyId { get; set; }
        public decimal? Monto { get; set; }
        public int NumeroOperacion { get; set; }
        public string PipelineId { get; set; }
        public string PipelineStageId { get; set; }
        public int DefinitionId { get; set; }
    }

    public class CompanyTicketCreateHubspotResponse : HubspotRespose
    {
        public bool IsCreated { get; set; }
        public long? TicketId { get; set; }
    }
    public class HubspotRespose
    {

        public int StatusHttp { get; set; }
        public string Content { get; set; }
    }
}
