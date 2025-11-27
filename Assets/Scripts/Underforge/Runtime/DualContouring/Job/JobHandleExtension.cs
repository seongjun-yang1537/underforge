using Unity.Jobs;
using Cysharp.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Underforge
{
    public static class JobHandleExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async UniTask ToUniTask(this JobHandle handle)
        {
            while (!handle.IsCompleted)
            {
                await UniTask.Yield();
            }

            handle.Complete();
        }
    }
}