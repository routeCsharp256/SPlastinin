using System;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace OzonEdu.MerchandiseService.Infrastructure.Extensions
{
    public static class HeaderDictionaryExtensions
    {
        public static string FormatHeaders(this IHeaderDictionary headers)
        {
            StringBuilder sb = new();
            if (headers != null && headers.Count > 0)
            {
                sb.Append("Headers:");
                foreach (var keyValuePair in headers)
                {
                    sb.Append(Environment.NewLine);
                    sb.Append(keyValuePair.Key);
                    sb.Append(": ");
                    sb.Append(keyValuePair.Value.ToString());
                }
            }

            return sb.Length == 0 ? "Headers are empty" : sb.ToString();
        }
    }
}