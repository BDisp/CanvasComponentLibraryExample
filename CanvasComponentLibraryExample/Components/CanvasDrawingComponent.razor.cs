using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CanvasComponentLibraryExample.Components
{
    public partial class CanvasDrawingComponent : ComponentBase
    {
        private int screenLeft = 20;
        private int screenTop = 20;
        private int screenWidth = 800;
        private int screenHeight = 800;
        private double mouseX;
        private double mouseY;

        private Canvas2DContext? currentCanvasContext;

        [Inject]
        private IJSRuntime? jSRuntime { get; set; }

        [Parameter]
        public ElementReference divCanvas { get; set; }

        [Parameter]
        public BECanvasComponent? myCanvas { get; set; }

        class PieChartSlice
        {
            public string Name { get; set; }
            public double PercentageOfSlice { get; set; }
            public string ColorOfSlice { get; set; }
            public double LabelX { get; set; }
            public double LabelY { get; set; }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                currentCanvasContext = await myCanvas.CreateCanvas2DAsync();

                //var canvasDrawing = new CanvasDrawingInterop(jSRuntime);
                //await canvasDrawing.InitConsole(DotNetObjectReference.Create(this));
                await jSRuntime!.InvokeVoidAsync("initConsole", DotNetObjectReference.Create(this));
                // this will make sure that the viewport is correctly initialized
                await jSRuntime!.InvokeAsync<object>("consoleWindowResize", DotNetObjectReference.Create(this));
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task DrawChart()
        {
            await currentCanvasContext!.ClearRectAsync(0, 0, screenWidth, screenHeight);
            await currentCanvasContext.SetFillStyleAsync("Red");
            await currentCanvasContext.FillRectAsync(mouseX, mouseY, 5, 5);
            await currentCanvasContext.StrokeTextAsync("ClientX: " + mouseX + " ClientY: " + mouseY, screenLeft, screenTop);

            List<PieChartSlice> pieChartData = new List<PieChartSlice>();

            pieChartData.Add(new PieChartSlice
            {
                Name = "Akshay's Company",
                PercentageOfSlice = 50,
                ColorOfSlice = "Blue",
                LabelX = 287,
                LabelY = 459
            });
            pieChartData.Add(new PieChartSlice
            {
                Name = "John Doe's Company",
                PercentageOfSlice = 30,
                ColorOfSlice = "Red",
                LabelX = 200,
                LabelY = 260
            });
            pieChartData.Add(new PieChartSlice
            {
                Name = "Jane Doe's Company",
                PercentageOfSlice = 20,
                ColorOfSlice = "Green",
                LabelX = 400,
                LabelY = 280
            });

            if (myCanvas != null && currentCanvasContext != null)
            {
                await currentCanvasContext.SaveAsync();
                await currentCanvasContext.SetFillStyleAsync("Black");
                await currentCanvasContext.SetFontAsync("15pt Arial");
                await currentCanvasContext.FillTextAsync("Widget Industry Share Breakup %", 160, 120);
                await currentCanvasContext.SetFontAsync("12pt Arial");
                await currentCanvasContext.SetStrokeStyleAsync("Black");
                double currangle = 0;
                double lastangle = 0;
                for (var i = 0; i < pieChartData.Count; i++)
                {
                    currangle += (pieChartData[i].PercentageOfSlice * 360) / 100;
                    await currentCanvasContext.SetFillStyleAsync(pieChartData[i].ColorOfSlice);
                    await currentCanvasContext.BeginPathAsync();
                    await currentCanvasContext.MoveToAsync(350, 350);
                    await currentCanvasContext.ArcAsync(350, 350, 200, (Math.PI / 180) * lastangle,
                        (Math.PI / 180) * currangle, false);
                    await currentCanvasContext.ClosePathAsync();
                    await currentCanvasContext.FillAsync();
                    await currentCanvasContext.StrokeTextAsync(pieChartData[i].Name, pieChartData[i].LabelX,
                        pieChartData[i].LabelY);
                    lastangle = currangle;
                }
                await currentCanvasContext.RestoreAsync();
            }
        }

        [JSInvokable]
        public async ValueTask OnResize(int screenWidth, int screenHeight)
        {
            if (currentCanvasContext == null) return;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            await DrawChart();
        }

        [JSInvokable]
        public async void OnClick(MouseEventArgs eventArgs)
        {
            if (divCanvas.Id?.Length > 0)
            {
                //string data = await jSRuntime.InvokeAsync<string>("getDivCanvasOffset", new object[] { divCanvas });
                //string data = await DivCanvasJsInterop.GetDivCanvasOffset(jSRuntime, new object[] { divCanvas });
                var divCanvasJsInterop = new DivCanvasJsInterop(jSRuntime!);
                string data = await divCanvasJsInterop.GetDivCanvasOffset(new object[] { divCanvas });
                JObject offsets = (JObject)JsonConvert.DeserializeObject(data)!;
                mouseX = eventArgs.ClientX - offsets.Value<double>("offsetLeft");
                mouseY = eventArgs.ClientY - offsets.Value<double>("offsetTop");

                await DrawChart();
            }
        }

        [JSInvokable]
        public async void OnKeyDown(KeyboardEventArgs eventArgs)
        {
            switch (eventArgs.Code)
            {
                case "ArrowLeft":
                    screenLeft = Math.Max(screenLeft - 20, 0);
                    await DrawChart();
                    break;
                case "ArrowRight":
                    screenLeft = Math.Min(screenLeft + 20, screenWidth - 20);
                    await DrawChart();
                    break;
                case "ArrowUp":
                    screenTop = Math.Max(screenTop - 20, 0);
                    await DrawChart();
                    break;
                case "ArrowDown":
                    screenTop = Math.Min(screenTop + 20, screenHeight - 20);
                    await DrawChart();
                    break;
            }
        }
    }
}
