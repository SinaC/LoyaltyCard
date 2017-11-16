using System;
using System.Threading.Tasks;

namespace LoyaltyCard.Common
{
    public static class AsyncFake
    {
        public static async Task CallAsync<TBusinessInterface>(TBusinessInterface instance, Action<TBusinessInterface> method)
        {
            await Task.Run(() => method(instance));
        }

        public static async Task<TResult> CallAsync<TBusinessInterface,TResult>(TBusinessInterface instance, Func<TBusinessInterface, TResult> method)
        {
            return await Task.Run(() => method(instance));
        }
    }
}
