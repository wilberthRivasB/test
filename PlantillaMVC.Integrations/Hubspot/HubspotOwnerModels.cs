using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PlantillaMVC.Integrations.Hubspot
{
    [DataContract]
    public class HubspotRemoteOwnerListModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }
        [DataMember(Name = "portalId")]
        public int PortalId { get; set; }
        [DataMember(Name = "ownerId")]
        public int OwnerId { get; set; }
        [DataMember(Name = "remoteId")]
        public string RemoteId { get; set; }
        [DataMember(Name = "remoteType")]
        public string RemoteType { get; set; }
        [DataMember(Name = "active")]
        public bool Active { get; set; }
    }

    [DataContract]
    public class HubspotOwnerModel
    {
        [DataMember(Name = "portalId")]
        public int PortalId { get; set; }
        [DataMember(Name = "ownerId")]
        public int OwnerId { get; set; }
        [DataMember(Name = "type")]
        public string Type { get; set; }
        [DataMember(Name = "firstName")]
        public string FirstName { get; set; }
        [DataMember(Name = "lastName")]
        public string LastName { get; set; }
        [DataMember(Name = "email")]
        public string Email { get; set; }
        [DataMember(Name = "createdAt")]
        public object CreatedAt { get; set; }
        [DataMember(Name = "updatedAt")]
        public object UpdatedAt { get; set; }
        [DataMember(Name = "remoteList")]
        public List<HubspotRemoteOwnerListModel> RemoteList { get; set; }
        [DataMember(Name = "hasContactsAccess")]
        public bool HasContactsAccess { get; set; }
        [DataMember(Name = "signature")]
        public string Signature { get; set; }
    }
}
