using Domain.Modules.Finance;
using Domain.Modules.Finance.Interfaces;
using Framework;
using Infrastructure.Data.Context;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;

namespace V2.Infrastructure.Modules.Finance.Repositories
{
    public class GatewayRegistrationLogRepository : IGatewayRegistrationLogRepository
    {
        private readonly ScaffoldContext context;

        public GatewayRegistrationLogRepository(ScaffoldContext context)
        {
            this.context = context;
        }

        public OperationResult Add(GatewayRegistrationLog registrationLog)
        {
            context.GatewayRegistrationLogs.Add(registrationLog);
            context.Entry(registrationLog).State = EntityState.Added;

            try
            {
                context.SaveChanges();
                return OperationResult.Success;
            }
            catch (DbEntityValidationException validationEx)
            {
                return validationEx.GetErrors();
            }
            catch (SqlException ex)
            {
                return OperationResult.Fail("", ex.Message);
            }
        }

        public bool HasAnyLogsForSeller(string sellerId)
        {
            return context.GatewayRegistrationLogs.Any(log => log.SellerId == sellerId);
        }
    }
}
