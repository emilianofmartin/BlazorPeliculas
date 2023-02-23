using BlazorPeliculas.Client.Helpers;
using MathNet.Numerics.Statistics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace BlazorPeliculas.Client.Pages {
    public partial class Counter {
        [Inject] IJSRuntime js { get; set; } = null!;
        [CascadingParameter] private Task<AuthenticationState> authStateTask { get; set; } = null!;

        private int currentCount = 0;

        public async Task IncrementCount() {
            var arr = new double[]{ 1, 2, 3, 4, 5};
            var max = arr.Maximum();
            var min = arr.Minimum();

            //await js.InvokeVoidAsync("alert", $"The max value is {max}  and the min es {min}");

            var authState = await authStateTask;
            var userIsAuthenticated = authState.User.Identity!.IsAuthenticated;

            if (userIsAuthenticated)
                currentCount++;
            else
                currentCount--;
        }
    }
}
