using Microsoft.Azure.Cosmos.Table;
using System.ComponentModel.DataAnnotations;

namespace ST10339829_CLDV6212_POE_FINAL.Models
{
    public class Product : TableEntity
    {
        [Required]
        public int? ProductID { get; set; }  

        [Required]
        public string Name { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public int Stock { get; set; }  

        public Product()
        {
            PartitionKey = "Product";
        }

        public void SetRowKey()
        {
            if (!ProductID.HasValue)
            {
                RowKey = Guid.NewGuid().ToString();
            }
            else
            {
                RowKey = ProductID.ToString();
            }
        }
    }
}
