using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TheFamilyFriend.Models
{
    [Table("ConsumptionExpenditure")]
    public partial class ConsumptionExpenditure
    { [Key]
        public int Id { get; set; }
        public string GoodName { get; set; }
        public Nullable<decimal> Money { get; set; }
        public string GoodType { get; set; }
        public Nullable<System.DateTime> RegisterTime { get; set; }
        public string PaymentMethod { get; set; }
        
        public string BackCard { get; set; }
        public string SpentPlace { get; set; }
    }
}