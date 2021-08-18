using System;
using System.Collections.Generic;
using System.Text;

namespace SBShared.Models
{
    public class AddressModel
    {
        public string StreetAddress { get; set; }
        public string PostalCode { get; set; }

        public AddressModel(string streetAddress, string postalCode)
        {
            StreetAddress = streetAddress;
            PostalCode = postalCode;
        }
    }
}
