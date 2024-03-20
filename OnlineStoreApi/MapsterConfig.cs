using Mapster;

namespace OnlineStoreApi
{
    public class MapsterConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // for nested mapping
            TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);
        }
    }
}
