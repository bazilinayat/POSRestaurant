using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace blazorapp.Data
{
    /// <summary>
    /// Class to represent the Tenant table in DB
    /// </summary>
    public class Tenant
    {
        /// <summary>
        /// PK of the table
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Name of the tenant
        /// This will be the entity name, in our case the restaurant name
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        /// <summary>
        /// This will be the name of the owner of the restaurant
        /// </summary>
        [Required]
        [StringLength(100)]
        public string OwnerName { get; set; }

        /// <summary>
        /// Different branches under this restaurant
        /// </summary>
        // Navigation properties
        public virtual ICollection<Branch> Branches { get; set; }
    }
}
