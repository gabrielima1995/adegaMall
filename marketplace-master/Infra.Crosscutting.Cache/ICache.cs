using System;

namespace Infra.Crosscutting.Cache
{
    public interface ICache : IDisposable
    {
        void Add(string key, object value);

        void Add(string key, object value, TimeSpan timeout);

        bool Contains(string key);

        object Get(string key);

        T Get<T>(string key);

        void Remove(string key);

        void RemoveAll();
    }

    /*
     * StoreService.Get
     *      Retorna informações dos produtos
     *      Coloca no cache
     *      Faz o cache escutar alteração CRUD de produtos do vendedor
     *      
     * ProductService.Add / Update / Delete
     *      Sobe o evento OnAdd / OnUpdate / OnDelete (Expression<Action> methodCall)
     * 
     * 
     * 
     * 
     */
}
