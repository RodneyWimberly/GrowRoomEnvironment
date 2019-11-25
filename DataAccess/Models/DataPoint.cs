using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GrowRoomEnvironment.DataAccess.Models
{
    public class DataPoint : AuditableEntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int Id { get; set; }
        public string Caption { get; set; }
        public string Icon { get; set; }
        public string Template { get; set; }
        public bool ShowInUI { get; set; }
    }
}
