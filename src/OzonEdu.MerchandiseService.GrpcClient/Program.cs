using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
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
            int employeeId = 10;
            try
            {
                var orderItems = new List<MerchOrderItem>();
                orderItems.Add(new MerchOrderItem()
                {
                    Sku = 1232435567543443554,
                    SkuDescription = "TShirt Black Size L",
                    Quantity = 1
                });

                var request = new CreateMerchOrderRequest()
                {
                    EmployeeId = employeeId,
                    EmployeeFirstName = "Ivan",
                    EmployeeLastName = "Ivanov",
                    EmployeeMiddleName = "Jovanovich",
                    EmployeeEmail = "ivan@ivan.ivan",
                    OrderItems = {orderItems},
                    ManagerId = 6,
                    ManagerFirstName = "Ivan",
                    ManagerLastName = "Ivanov",
                    ManagerMiddleName = "Jovanovich",
                    ManagerEmail = "ivan@ivan.ivan"
                };
                var response = await client.CreateMerchOrderAsync(request);
                Console.WriteLine($"Create merch order response: {response}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            try
            {
                var response = await client.GetMerchListByEmployeeIdAsync(
                    new GetMerchListRequest() {EmployeeId = employeeId},
                    cancellationToken: CancellationToken.None);
                foreach (var item in response.MerchList)
                {
                    Console.WriteLine($"{item}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}