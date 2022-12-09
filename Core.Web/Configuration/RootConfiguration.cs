using BeCoreApp.Web.Configuration.Interfaces;

namespace BeCoreApp.Web.Configuration
{
    public class RootConfiguration : IRootConfiguration
    {
        public GameConfiguration GameConfiguration { get; } = new GameConfiguration();
    }
}
