using BlazorPeliculas.Shared.Entities;
using Microsoft.JSInterop;
using System.Runtime.CompilerServices;

namespace BlazorPeliculas.Client.Helpers {
    public static class IJSRuntimeExtensionMethods {
        public static async ValueTask<bool> Confirm(this IJSRuntime js, string message) {
            await js.InvokeVoidAsync("console.log", $"Asking: «{message}»");
            return await js.InvokeAsync<bool>("confirm", message);
        }
        public static ValueTask<object> SetInLocalStorage(this IJSRuntime js,
            string key, string value) {
            return js.InvokeAsync<object>("localStorage.setItem", key, value);
        }

        public static ValueTask<object> GetFromLocalStorage(this IJSRuntime js,
            string key) {
            return js.InvokeAsync<object>("localStorage.getItem", key);
        }
        public static ValueTask<object> RemoveFromLocalStorage(this IJSRuntime js,
            string key) {
            return js.InvokeAsync<object>("localStorage.removeItem", key);
        }
    }
}
