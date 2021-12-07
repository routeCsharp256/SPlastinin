using System.Collections.Generic;
using CSharpCourse.Core.Lib.Models;
using MediatR;

namespace OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Commands
{
    public class ProcessStockReplenishedCommand : IRequest
    {
        public ProcessStockReplenishedCommand(IEnumerable<StockReplenishedItem>  items)
        {
            Items = items;
        }

        public IEnumerable<StockReplenishedItem> Items { get; } 
    }
}