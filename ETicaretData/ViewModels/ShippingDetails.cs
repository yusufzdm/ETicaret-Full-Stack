using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretData.ViewModels
{
    public class ShippingDetails
    {
        [Required(ErrorMessage ="Lütfen boş geçmeyiniz...")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Lütfen boş geçmeyiniz...")]
        public string AdressTitle { get; set; }
        [Required(ErrorMessage = "Lütfen boş geçmeyiniz...")]
        public string Adress { get; set; }
        [Required(ErrorMessage = "Lütfen boş geçmeyiniz...")]
        public string City { get; set; }
    

    }
}
