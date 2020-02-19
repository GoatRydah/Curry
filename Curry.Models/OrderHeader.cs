using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Curry.Models
{
    public class OrderHeader
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        [Display(Name = "Order Total")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double OrderTotal { get; set; }
        [Required]
        [Display(Name = "Delivery Time")]
        public DateTime DeliveryTime { get; set; }

        [Required]
        [NotMapped]
        public DateTime DeliveryDate { get; set; }

        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public string Comments { get; set; }
        [Display(Name = "Delivery Name")]
        public string DeliveryName { get; set; }
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        public int TransactionId { get; set; }
    }
}
