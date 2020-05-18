using Application.Avatar;
using Application.Modules.Portfolio.SocialMedias;
using Application.Modules.Ratings;
using Application.Sellers;
using Domain;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Modules.Ratings;
using Domain.Modules.Ratings.Queries;
using Domain.Sellers;
using Domain.Sellers.SocialMedias;
using Domain.Services;
using Infrastructure.Data.Repositories;
using Unity;
using Unity.Injection;
using V2.Application.Modules.UserAccounts;
using V2.Application.Modules.UserRatings;
using V2.Infrastructure.Auth;
using V2.Infrastructure.Modules.Ratings.Queries;
using V2.Infrastructure.Modules.UserRegistration.Queries;

namespace V2.Infrastructure.Bootstrapper.Internals
{
    internal class UserDependencies : IDependenciesRegistration
    {
        public void Register(IUnityContainer container)
        {
            container
                // Common
                .RegisterType<AccountService>()
                .RegisterType<PersonService>()
                .RegisterType<AvatarAppService>()

                .RegisterType<IPersonRepository, PersonRepository>(nameof(PersonRepository))
                .RegisterType<IPersonRepository, CachingPersonRepository>(
                    new InjectionConstructor(
                        new ResolvedParameter<IPersonRepository>(nameof(PersonRepository)),
                        new ResolvedParameter<ICache>()))

                .RegisterType<IUserAccountQueries, UserAccountQueries>()
                .RegisterType<IUserAuthProvider, UserAuthProvider>(new ScopedLifetimeManager())

                //Buyer
                .RegisterType<BuyerService>()
                .RegisterType<IBuyerRepository, BuyerRepository>()

                // Seller
                .RegisterType<SellerService_Obsolete>()
                .RegisterType<SellerSocialMediaService>()
                .RegisterType<SellerAppService>()
                .RegisterType<ISellerRepository, SellerRepository>()
                .RegisterType<ISocialMediaAppService, SocialMediaAppService>()

                //User Ratings
                .RegisterType<IUserRatingsRepository, UserRatingsRepository>()
                .RegisterType<IUserRatingAppService, UserRatingAppService>()
                .RegisterType<IRatingsQueries, RatingsQueries>()
                .RegisterType<CheckExpiredRatings, CheckExpiredRatings>();
        }
    }
}