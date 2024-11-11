using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Documents;
using System.ComponentModel.DataAnnotations;

namespace ST10339829_CLDV6212_POE_FINAL.Models
{
    public class Customer : TableEntity
    {
       
        public int? CustomerID { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public Customer()
        {
            PartitionKey = "Customer";
        }

        public void SetRowKey()
        {
            if (!CustomerID.HasValue)
            {
                RowKey = Guid.NewGuid().ToString();
            }
            else
            {
                RowKey = CustomerID.ToString();
            }
        }
    }
}
