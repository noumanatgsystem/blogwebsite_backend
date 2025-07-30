using Application.Interface.Blog;
using Application.Interface.User;
using Infrastructure.Services.Blog;
using Infrastructure.Services.User;

namespace blogging_Website.InjectedServices
{
    public static class CustomeService
    {
        public static void AddCustomeService(this IServiceCollection Services)
        {
            Services.AddScoped<IBlog, BlogService>();
            Services.AddScoped<IAppUser, AppUserService>();
        }
    }
}
