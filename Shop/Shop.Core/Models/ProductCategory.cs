using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Core.Models
{
   public class ProductCategory : BaseEntity
    {
       
        [DisplayName("Product Category Name")]
        public string Category { get; set; }

        
    }
}
