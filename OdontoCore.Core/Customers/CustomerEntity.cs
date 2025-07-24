using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBarber.Core.Customers
{
    public class CustomerEntity
    {

        
        [Column("IdCliente")]
        public int Id { get; set; }
        public string ?Name { get; set; }

        public string?Email { get; set; }

        public string?Cpf { get; set; }

        public string? PhoneNumber { get; set; }

        public string ?DtBirthday { get; set;}

        public bool Active { get; set; }    


    }
}
