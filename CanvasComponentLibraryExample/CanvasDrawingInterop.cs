using CanvasComponentLibraryExample.Components;
using Microsoft.JSInterop;

namespace CanvasComponentLibraryExample
{
    public class CanvasDrawingInterop : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;

        public CanvasDrawingInterop(IJSRuntime jsRuntime)
        {
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
               "import", "./_content/CanvasComponentLibraryExample/Scripts/canvasDrawingInterop.js").AsTask());
        }

        public async ValueTask<object> InitConsole(DotNetObjectReference<CanvasDrawingComponent> divCanvas)
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<object>("initConsole", divCanvas);
        }

        public async ValueTask DisposeAsync()
        {
            if (moduleTask.IsValueCreated)
            {
                var module = await moduleTask.Value;
                await module.DisposeAsync();
            }
        }
    }
}
