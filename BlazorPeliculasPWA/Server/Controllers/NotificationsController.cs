using BlazorPeliculas.Shared.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;

namespace BlazorPeliculas.Server.Controllers {
    [Microsoft.AspNetCore.Components.Route("api/notifications"), ApiController]
    public class NotificationsController: ControllerBase {
        private readonly ApplicationDbContext context;

        public NotificationsController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpPost("suscribe")]
        public async Task<ActionResult> Suscribe(Notification notification) {
            context.Add(notification);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("unsuscribe")]
        public async Task<ActionResult> Unsuscribe(Notification notification) {
            var notificationDB = context.Notifications
                .FirstOrDefault(x => x.Auth == notification.Auth && x.P256h == notification.P256h);

            if (notificationDB == null) return NotFound();

            context.Remove(notificationDB);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
