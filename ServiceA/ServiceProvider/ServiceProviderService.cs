
using ServiceA.ServiceProvider.Contract;
using ServiceAdapter;

namespace ServiceA.ServiceProvider
{
    public class ServiceProviderService
    {


        #region Get

        public static async Task<string> GetTestServiceByServiceB()
        {

            //var apiRequest = new ApiRequest { url = "/ServiceProvider/Values/item", arg = userInfo, httpMethod = ApiClient.Get };
            //apiRequest.SetServiceName("ServiceProvider");

            return await ApiClient.RequestApiAsync<string>("/ServiceB/api/Test/index2", null, ApiClient.Get);
        }

        #endregion



        #region Post

        public static async Task<UserInfo> PostTestServiceByServiceB(UserInfo userInfo)
        {
            return await ApiClient.RequestApiAsync<UserInfo>("/ServiceB/api/Test/index3", userInfo, ApiClient.Post);
        }
       
        #endregion


    }
}
