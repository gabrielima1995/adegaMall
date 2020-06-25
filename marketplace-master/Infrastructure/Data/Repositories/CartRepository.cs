using Domain.Entities.StoreModule;
using Domain.Interfaces.Repositories;
using Framework;
using Infrastructure.Data.Context;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Infrastructure.Data.Repositories
{
    public class CartRepository : EfCrudRepository, ICartRepository
    {
        public CartRepository(ScaffoldContext context) : base(context)
        {
        }

        public OperationResult Add(Cart entity, bool saveChanges = false)
        {
            return Add<Cart>(entity, saveChanges);
        }

        public OperationResult Add(CartItem entity, bool saveChanges = false)
        {
            return Add<CartItem>(entity, saveChanges);
        }

        public OperationResult Add(CartSeller entity, bool saveChanges = false)
        {
            return Add<CartSeller>(entity, saveChanges);
        }

        public OperationResult Delete(Cart entity, bool saveChanges = false)
        {
            return Delete<Cart>(entity, saveChanges);
        }

        public OperationResult Delete(CartItem entity, bool saveChanges = false)
        {
            return Delete<CartItem>(entity, saveChanges);
        }

        public OperationResult Delete(CartSeller entity, bool saveChanges = false)
        {
            return Delete<CartSeller>(entity, saveChanges);
        }

        public Cart GetCartById(params object[] idValues)
        {
            return GetById<Cart>(idValues);
        }

        public CartItem GetCartItemById(params object[] idValues)
        {
            return GetById<CartItem>(idValues);
        }

        public CartSeller GetCartSellerById(params object[] idValues)
        {
            return GetById<CartSeller>(idValues);
        }

        public Cart GetFull(string buyerId)
        {
            return CacheHelper.Get(Context.Carts
                .Include(x => x.ShippingAddress)
                .Include(x => x.Buyer)
                .Include(x => x.Buyer.Address)
                .Include(x => x.Items)
                .Include(x => x.Items.Select(i => i.Product))
                .Include(x => x.Items.Select(i => i.Product.PrivateFleetParams))
                .Include(x => x.Items.Select(i => i.Product.Category))
                .Include(x => x.Items.Select(i => i.Product.StockVariantType))
                .Include(x => x.Items.Select(i => i.Product.StockVariants))
                .Include(x => x.Items.Select(i => i.Product.StockVariants.Select(j => j.Variant)))
                .Include(x => x.Items.Select(i => i.Product.Store))
                .Include(x => x.Items.Select(i => i.Product.Store.SingleFreightParams))
                .Include(x => x.Items.Select(i => i.Product.Store.Seller))
                .Include(x => x.Items.Select(i => i.Product.Store.Seller.Address))
                .Include(x => x.Sellers)
                .Include(x => x.Sellers.Select(s => s.Seller))
                .Include(x => x.Sellers.Select(s => s.Seller.Store))
                .Include(x => x.Sellers.Select(s => s.Seller.Store.SingleFreightParams))
                .Where(x => x.Buyer.Id == buyerId && !x.Completed));
        }

        public Cart GetWithItems(string buyerId)
        {
            return CacheHelper.Get(Context.Carts
                .Include(x => x.Items)
                .Where(x => x.Buyer.Id == buyerId));
        }

        public List<CartItem> ListCartItems()
        {
            return List<CartItem>();
        }

        public List<Cart> ListCarts()
        {
            return List<Cart>();
        }

        public List<CartSeller> ListCartSellers()
        {
            return List<CartSeller>();
        }

        public OperationResult Update(Cart entity, bool saveChanges = false)
        {
            return Update<Cart>(entity, saveChanges);
        }

        public OperationResult Update(CartItem entity, bool saveChanges = false)
        {
            return Update<CartItem>(entity, saveChanges);
        }

        public OperationResult Update(CartSeller entity, bool saveChanges = false)
        {
            return Update<CartSeller>(entity, saveChanges);
        }
    }
}