using AutoMapper;
using Domain.Entities;
using Admin = Presentation.Areas.Admin.Models;

namespace Presentation
{
    public class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            Mapper.Initialize(c =>
                c.CreateMap<Banner, Admin.BannerVM>().ReverseMap()
            );
        }
    }
}