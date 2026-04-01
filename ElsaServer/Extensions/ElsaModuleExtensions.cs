using Elsa.Extensions;
using Elsa.Features.Services;


namespace ElsaServer.Extensions
{
    public static class ElsaModuleExtensions
    {
        /// <summary>
        /// Adds JavaScript and Liquid support with configuration binding.
        /// Uses explicit types to avoid ambiguity issues.
        /// </summary>
        public static IModule AddJavaScriptAndLiquid(
            this IModule module,
            IConfiguration configuration)
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            // Explicitly use modern Elsa.JavaScript to avoid ambiguity
            module.UseJavaScript((Elsa.JavaScript.Features.JavaScriptFeature js) =>
            {
                configuration.GetSection("JavaScript").Bind(js);
            });
            module.UseLiquid((Elsa.Liquid.Features.LiquidFeature liquid) =>
            {
                configuration.GetSection("Liquid").Bind(liquid);
            });

            return module;
        }
    }
}