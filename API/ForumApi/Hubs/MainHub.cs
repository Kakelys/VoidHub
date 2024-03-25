using SignalR.Modules;

namespace ForumApi.Hubs
{
    [SignalRModuleHub(typeof(SessionHub))]
    public partial class MainHub : ModulesEntryHub
    {
        public MainHub(ILogger<MainHub> logger, IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {

        }
    }
}