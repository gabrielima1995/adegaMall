using Domain.Entities.StoreModule;
using Framework;
using System.Collections.Generic;

namespace Domain.Interfaces.Repositories
{
    public interface ICartRepository
    {
        Cart GetFull(string buyerId);

        Cart GetWithItems(string buyerId);

        Cart GetCartById(params object[] idValues);

        List<Cart> ListCarts();

        OperationResult Add(Cart entity, bool saveChanges = false);

        OperationResult Update(Cart entity, bool saveChanges = false);

        OperationResult Delete(Cart entity, bool saveChanges = false);

        CartItem GetCartItemById(params object[] idValues);

        List<CartItem> ListCartItems();

        OperationResult Add(CartItem entity, bool saveChanges = false);

        OperationResult Update(CartItem entity, bool saveChanges = false);

        OperationResult Delete(CartItem entity, bool saveChanges = false);

        CartSeller GetCartSellerById(params object[] idValues);

        List<CartSeller> ListCartSellers();

        OperationResult Add(CartSeller entity, bool saveChanges = false);

        OperationResult Update(CartSeller entity, bool saveChanges = false);

        OperationResult Delete(CartSeller entity, bool saveChanges = false);
    }
}
