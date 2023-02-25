using BlazorPeliculas.Shared.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BlazorPeliculas.Server.Controllers {
    [ApiController, Route("api/notifications")]
    public class NotificationsController: ControllerBase {
        private readonly ApplicationDbContext context;

        public NotificationsController(ApplicationDbContext context) {
            this.context = context;
        }

        [HttpPost("suscribe")]
        public async Task<ActionResult> Suscribe(Notif notification) {
            context.Add(notification);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("unsuscribe")]
        public async Task<ActionResult> Unsuscribe(Notif notification) {
            var notificationDB = context.Notifs
                .FirstOrDefault(x => x.Auth == notification.Auth && x.P256dh == notification.P256dh);

            if (notificationDB == null) return NotFound();

            context.Remove(notificationDB);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
