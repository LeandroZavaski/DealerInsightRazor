using System.Text;
using System.Threading.Tasks;
using DealerInsightRazor.Authentication;
using DealerInsightRazor.DataAccess;
using DealerInsightRazor.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace DealerInsightRazor
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Events = new CookieAuthenticationEvents
                    {
                        OnSignedIn = context => Task.CompletedTask,
                        OnSigningOut = context => Task.CompletedTask,
                        OnValidatePrincipal = context => Task.CompletedTask,
                    };
                    options.SlidingExpiration = true;
                })
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.Authority = Configuration["OpenId:Authority"];
                    options.RequireHttpsMetadata = false;
                    options.ClientId = Configuration["OpenId:ClientId"];
                    options.ResponseType = OpenIdConnectResponseType.IdToken;
                    options.SaveTokens = true;
                    options.TokenValidationParameters.ValidateIssuer = false;
                    options.GetClaimsFromUserInfoEndpoint = true;
                });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddDistributedMemoryCache();
            services.AddSession();

            var azureConfig = Configuration.GetSection("AzureAd");
            services.Configure<AzureAdSettings>(azureConfig);

            var powerbiConfig = Configuration.GetSection("PowerBi");
            services.Configure<PowerBiSettings>(powerbiConfig);

            services.AddScoped<AuthenticationHandler>();
            services.AddScoped<ReportRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(Configuration["Error"]);
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseSession();

            app.UseAuthentication();
            app.UseMiddleware(typeof(AuthenticationPingMiddleware));

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    var error = context.Features.Get<IExceptionHandlerFeature>();
                    if (error != null)
                    {
                        var ex = error.Error;

                        await context.Response.WriteAsync(new ErrorViewModel()
                        {
                            Code = ex.HResult,
                            Message = ex.Message
                        }.ToString(), Encoding.UTF8);
                    }
                });
            });

            app.UseMvc();
        }
    }
}
