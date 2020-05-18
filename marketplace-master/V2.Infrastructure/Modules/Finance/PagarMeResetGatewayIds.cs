using Dapper;
using V2.Application.Modules.Finance.Queries;
using V2.Infrastructure.Data;

namespace V2.Infrastructure.Modules.Finance
{
    public class PagarmeResetGatewayIds : IResetGatewayIds
    {
        public void RunQuery()
        {
            using (var dbCon = MyDbConnection.Create())
            {
                var results = dbCon.Execute("UPDATE Person SET SupplierGatewayId = NULL, CustomerGatewayId = NULL");
            }
        }
    }
}
