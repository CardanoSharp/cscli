using CardanoSharp.Wallet.Enums;
using Refit;

namespace Cscli.ConsoleTool.Koios
{
    public static class BackendGateway
    {
        public static T GetBackendClient<T>(NetworkType networkType) =>
            RestService.For<T>(GetBaseUrlForNetwork(networkType));

        private static string GetBaseUrlForNetwork(NetworkType networkType) => networkType switch
        {
            NetworkType.Mainnet => "https://api.koios.rest/api/v0",
            NetworkType.Testnet => "https://testnet.koios.rest/api/v0",
            _ => throw new ArgumentException($"{nameof(networkType)} {networkType} is invalid", nameof(networkType))
        };
    }
}
