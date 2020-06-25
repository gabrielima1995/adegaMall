using Application.Config;
using Application.Helpers;
using Domain.DTO;
using Domain.Entities;

namespace Application.Sellers
{
    public static class SellerHelper
    {
        private const int DefaultSize = 255;

        public static string GetAvatarUrl(this LoginProfile loginProfile, int size = DefaultSize, int height = DefaultSize /*refatorar para remover parâmetro nas chamadas*/)
        {
            return GetAvatarUrl(loginProfile.Avatar, size);
        }

        public static string GetAvatarUrl(this Person person, int size = DefaultSize, int height = DefaultSize /*refatorar para remover parâmetro nas chamadas*/)
        {
            return GetAvatarUrl(person.Avatar, size);
        }

        public static string GetAvatarUrl(string avatar, int size = DefaultSize, int height = DefaultSize /*refatorar para remover parâmetro nas chamadas*/)
        {
            return ImgHelper.Avatar(avatar, size);
        }

        public static string GetProfileUrl(this Seller seller)
        {
            return $"{MaaSConfig.Current.Environment.AppUrl}p/{seller.FriendlyUrl ?? seller.Nickname}/";
        }
    }
}