using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace API.Tests.Config
{
    public class IntegrationTestBase
    {
        protected HttpClient HttpClient;
        private TestServer _testServer;
        private IdentityServerSetup _identityServerSetup;

        protected IntegrationTestBase()
        {
            InitializeIdentityServer();
            InitializeClient();
        }


        private void InitializeClient()
        {
            var builder = new WebHostBuilder()
                .ConfigureServices(InitializeServices)
                .Configure(ConfigureApi);
            _testServer = new TestServer(builder);
            HttpClient = _testServer.CreateClient();
        }

        protected virtual void InitializeServices(IServiceCollection services)
        {
            services.AddAuthorization();
            services.AddAuthentication();
            services.AddMvc();
        }

        private void ConfigureApi(IApplicationBuilder app)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            app.UseIdentityServerAuthentication(_identityServerSetup.GetIdentityServerAuthenticationOptions());
            app.UseMvc();
        }

        private void InitializeIdentityServer()
        {
            _identityServerSetup = new IdentityServerSetup(null)
                .SetClients(Clients.Get())
                .SetUsers(Users.Get())
                .SetScopeClaims(new List<string> { JwtClaimTypes.Role })
                .AddLogging()
                .Initialize();
        }

        protected Task SetUpTokenFor(string username, string password)
        {
            return _identityServerSetup.SetAccessToken(HttpClient, username, password);
        }
    }
}
