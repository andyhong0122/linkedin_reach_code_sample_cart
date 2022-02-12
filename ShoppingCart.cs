using System;
using System.Collections.Generic;
using System.Text;

namespace Sabio.Models.Domain
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public int ProviderServiceId { get; set; }
        public int ProviderId { get; set; }
        public decimal? Price { get; set; }
        public string ServiceName { get; set; }
        public string Cpt4Code { get; set; }
        public string ServiceType { get; set; }
        public int CreatedBy { get; set; }
    }
}
