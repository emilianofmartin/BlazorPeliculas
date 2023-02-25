using BlazorPeliculas.Shared.Entities;
using Microsoft.JSInterop;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace BlazorPeliculas.Client.Helpers {
    public static class IJSRuntimeExtensionMethods {
        
        public static async ValueTask<string> GetStatusNotificationGrant(this IJSRuntime js) {
            return await js.InvokeAsync<string>("getStatusNotificationGrant");
        }

        public static async ValueTask<Notif> SuscribeUser(this IJSRuntime js) {
            return await js.InvokeAsync<Notif>("suscribeUser");
        }

        public static async ValueTask<Notif> UnsuscribeUser(this IJSRuntime js) {
            return await js.InvokeAsync<Notif>("unsuscribeUser");
        }
        public static async ValueTask<DbLocalRecords> GetPendingRecords(this IJSRuntime js) {
            return await js.InvokeAsync<DbLocalRecords>("getPendingRecords");
        }

        public static async ValueTask deleteRecord(this IJSRuntime js, string table, int id) {
            await js.InvokeVoidAsync("deleteRecord", table, id);
        }

        public static async ValueTask saveCreateRecord<T>(this IJSRuntime js, string url, T entity) {
            var body = JsonSerializer.Serialize(entity);
            await js.InvokeVoidAsync("saveCreateRecord", url, body);
        }

        public static async ValueTask saveDeleteRecord(this IJSRuntime js, string url) {
            await js.InvokeVoidAsync("saveDeleteRecord", url);
        }

        public static async ValueTask<int> GetPendingRecordCount(this IJSRuntime js) {
            return await js.InvokeAsync<int>("getPendingRecordCount");
        }

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
