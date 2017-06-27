namespace FaceYourFace.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Person")]
    public partial class Person
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public Guid PersonID { get; set; }

        [StringLength(50)]
        public string EnterpriseID { get; set; }

        [StringLength(50)]
        public string PersonGroupID { get; set; }

        public string PersonUserData { get; set; }
    }
}
