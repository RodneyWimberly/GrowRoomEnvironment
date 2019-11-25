using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrowRoomEnvironment.DataAccess.Models
{
    public class EnumLookup : AuditableEntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Table { get; set; }

        public string EnumName { get; set; }

        public int EnumValue { get; set; }

        public string EnumDescription { get; set; }

    }
}
