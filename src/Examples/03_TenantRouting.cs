﻿using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ProxyKit.Examples
{
    public class MultiTenantJwt : ExampleBase<MultiTenantJwt.Startup>
    {
        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddProxy();
            }

            public void Configure(IApplicationBuilder app)
            {
                app.RunProxy(
                    async (context, handle) =>
                    {
                        // Example of how to route a request to a backend host based on TenantId claim
                        // in a JWT Bearer token. Note: the backend service should still token validation.
                        // (Token validation can also be done here).
                        var authorization = context.IncomingRequest.Headers["Authorization"].SingleOrDefault();
                        if (string.IsNullOrWhiteSpace(authorization) || !authorization.StartsWith("Bearer"))
                        {
                            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                        }
                        var token = authorization.Substring(0, "Bearer ".Length);
                        var handler = new JwtSecurityTokenHandler();
                        var jwtToken = handler.ReadJwtToken(token);
                        var tenantIdClaim = jwtToken.Claims.SingleOrDefault(c => c.Type == "TenantId");

                        if (tenantIdClaim == null)
                        {
                            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                        }

                        var forwardContext = context
                            .ForwardTo($"http://{tenantIdClaim.Value}.internal:5001");

                        return await handle(forwardContext);
                    });
            }
        }
    }
}