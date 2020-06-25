using Infrastructure.Data.Context;

namespace Infrastructure.Data.Repositories
{
    public abstract class EfRepository
    {
        private ScaffoldContext context;

        protected ScaffoldContext Context
        {
            get
            {
                /*
                 * AVISO!
                 * ======
                 * Algum lugar da aplicação está chamando Dispose no DbContext antes da hora.
                 * Isso resulta em algumas operações falhando no acesso ao banco de dados.
                 * Já verifiquei:
                 *      Configurações de dependency injection do Unity em V2.Infrastructure.Bootstrapper
                 *      Removi registros duplicados (ScaffoldContext, DbContext)
                 *      LifetimeManager utilizado para o DbContext
                 *      Busquei por todos os locais chamando Dispose em ScaffoldContext
                 *      Coloquei um breakpoint em ScaffoldContext.Dispose pra ver se encontrava
                 * Até encontrar onde está o problema, isso resolve...
                 * 
                 */

                if (context == null || context.IsDisposed)
                    context = UnityIocContainer.Instance.Resolve(typeof(ScaffoldContext), null) as ScaffoldContext;

                return context;
            }
        }

        public EfRepository(ScaffoldContext context)
        {
            this.context = context;
        }
    }
}
