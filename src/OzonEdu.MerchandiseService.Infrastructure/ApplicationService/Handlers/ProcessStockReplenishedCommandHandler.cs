using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Commands;

namespace OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Handlers
{
    public class ProcessStockReplenishedCommandHandler : IRequestHandler<ProcessStockReplenishedCommand>
    {
        public ProcessStockReplenishedCommandHandler()
        {
            
        }

        public async Task<Unit> Handle(ProcessStockReplenishedCommand request, CancellationToken cancellationToken)
        {
            foreach (var item in request.Items)
            {
                await Task.Delay(100);
            }
            
            return Unit.Value;
        }
    }
}