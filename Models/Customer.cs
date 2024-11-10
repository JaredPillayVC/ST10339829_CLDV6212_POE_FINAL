using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Documents;
using System.ComponentModel.DataAnnotations;

namespace ST10339829_CLDV6212_POE_FINAL.Models
{
    public class Customer : TableEntity
    {
        [Required]
        public int? CID { get; set; }

        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        public Customer()
        {
            PartitionKey = "Customer";
        }

        public void SetRowKey()
        {
            if (!CID.HasValue)
            {
                RowKey = Guid.NewGuid().ToString();
            }
            else
            {
                RowKey = CID.ToString();
            }
        }
    }
}
