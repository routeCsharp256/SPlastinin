using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Net.Client;
using OzonEdu.MerchandiseService.Grpc;

namespace OzonEdu.MerchandiseService.GrpcClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new MerchandiseGrpc.MerchandiseGrpcClient(channel);

            try
            {
                var response = await client.GetMerchListByEmployeeIdAsync(new GetMerchListRequest(){ EmployeeId = 10 }, 
                    cancellationToken: CancellationToken.None);
                foreach (var item in response.MerchList)
                {
                    Console.WriteLine($"MerchList item data");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            try
            {
                var response = await client.CreateMerchOrderAsync(new CreateMerchOrderRequest());
                Console.WriteLine($"Create merch order response: {response.NotImplemented}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}