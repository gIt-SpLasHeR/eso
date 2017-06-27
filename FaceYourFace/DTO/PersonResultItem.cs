using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FaceYourFace.DTO
{
    public class PersonResultItem
    {
        public String EnterpriseID { get; set; }
        public String PersonGroupId { get; set; }
        public String UserData { get; set; }
        public Guid PersonId { get; set; }
        public List<Guid> PersonFaceId { get; set; }
        
    }
}