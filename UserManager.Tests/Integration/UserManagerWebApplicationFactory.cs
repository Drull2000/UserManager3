using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Text;
using UserManager.Services;

namespace UserManager.Tests.Integration
{
    public class UserManagerWebApplicationFactory: WebApplicationFactory<Program>
    {

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(service =>
            {
                var userRepo = service.SingleOrDefault(
                    d => d.ServiceType == typeof(IUserService));
            });
        }
    }
}
