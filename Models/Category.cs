using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductsAndCategories.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }
        [Required(ErrorMessage = "Input cannot be blank")]
        [MaxLength(45)]
        public string Name { get; set; }

        public List<Association> Products {get; set;}

        [Required]
        public DateTime Created_At {get; set;} = DateTime.Now;
        [Required]
        public DateTime Updated_At {get; set;} = DateTime.Now;
    }
}