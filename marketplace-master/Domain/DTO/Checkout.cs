using Domain.Entities;
using Domain.Entities.StoreModule;
using Framework;
using System.Collections.Generic;
using System.Linq;

namespace Domain.DTO
{
    public class Checkout
    {
        public Cart Cart { get; set; }

        public PaymentOptions Payment { get; set; }

        public ErrorDictionary CartValidation { get; set; }

        public bool HasShippingError => CartValidation.HasErrors;

        public Checkout()
        {
            CartValidation = new ErrorDictionary();
            Payment = new PaymentOptions();
        }

        public Checkout(Cart cart, ErrorDictionary cartValidation) : this()
        {
            Cart = cart;
            if (cartValidation != null)
                CartValidation = cartValidation;
        }
    }
}
