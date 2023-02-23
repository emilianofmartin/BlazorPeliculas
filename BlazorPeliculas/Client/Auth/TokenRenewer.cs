using System.Timers;
using Timer = System.Timers.Timer;

namespace BlazorPeliculas.Client.Auth {
    public class TokenRenewer : IDisposable {
        private readonly ILoginService loginService;

        public TokenRenewer(ILoginService loginService) {
            this.loginService = loginService;
        }

        Timer? timer;

        public void Start() {
            timer = new Timer();
            timer.Interval = 1000 * 60 * 4; // 4 minutos (debe ser menor al umbral definido (5 minutos)
            //timer.Interval = 1000 * 5; // 5 segundos (debe ser menor al umbral definido (1 minuto)
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e) {
            loginService.ManageTokenRenewal();
        }

        public void Dispose() {
            //Tenemos que limpiar el timer ya que utiliza recursos no manejados que tenemos que limpiar.
            timer?.Dispose();
        }
    }
}
