using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AzureMvcWithAuthenAuthor.Models
{
    [Index(nameof(Code), IsUnique = true)]
    [Index(nameof(Description), IsUnique = true)]
    public class Product
    {
        [Display(Name = "Product ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ID { get; set; }

        [Display(Name = "Product Code")]
        [Required()]
        public string Code { get; set; }

        [Display(Name = "Product Description")]
        [Required()]
        public string Description { get; set; }

        [Display(Name = "Unit Price")]
        [Required()]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12,2)")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Updated Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime UpdatedDate { get; set; }

        [Display(Name = "Updated By")]
        public string UpdatedBy { get; set; }
    }
}

