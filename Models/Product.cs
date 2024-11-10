using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Documents;
using System.ComponentModel.DataAnnotations;

namespace ST10339829_CLDV6212_POE_FINAL.Models
{
    public class Product : TableEntity
    {
        [Required]
        public int? PID { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }

        public Product()
        {
            PartitionKey = "Product";
        }

        public void SetRowKey()
        {
            if (!PID.HasValue)
            {
                RowKey = Guid.NewGuid().ToString();
            }
            else
            {
                RowKey = PID.ToString();
            }
        }
    }
}
