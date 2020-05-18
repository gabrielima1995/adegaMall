using Domain.DTO;

namespace Presentation.Models
{
    public class ViewModelBase
    {
        public ViewModelBase()
        {
        }

        public ViewModelBase(LoginProfile loginProfile)
        {
            LoginProfile = loginProfile;
        }

        public LoginProfile LoginProfile { get; }
    }

    public sealed class ViewModelBase<T> : ViewModelBase
    {
        public ViewModelBase() : base()
        {
        }

        public ViewModelBase(LoginProfile loginProfile, T data) : base(loginProfile)
        {
            Data = data;
        }

        public T Data { get; set; }
    }
}