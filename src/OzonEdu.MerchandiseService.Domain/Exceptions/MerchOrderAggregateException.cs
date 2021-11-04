using System;

namespace OzonEdu.MerchandiseService.Domain.Exceptions
{
    public class MerchOrderAggregateException : Exception
    {
        public MerchOrderAggregateException()
        {
        }

        public MerchOrderAggregateException(string message) : base(message)
        {
        }

        public MerchOrderAggregateException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}