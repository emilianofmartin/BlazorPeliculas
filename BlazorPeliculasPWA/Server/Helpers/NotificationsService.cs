using BlazorPeliculas.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebPush;

namespace BlazorPeliculas.Server.Helpers {
    public class NotificationsService {
        private readonly IConfiguration configuration;
        private readonly ApplicationDbContext context;

        public NotificationsService(IConfiguration configuration, ApplicationDbContext context)
        {
            this.configuration = configuration;
            this.context = context;
        }

        public async Task SendNotificationMovieOnBoard(Movie movie) {
            var notifications = await context.Notifs.ToListAsync();

            var publicKey = configuration.GetValue<string>("notifications:publicKey");
            var privateKey = configuration.GetValue<string>("notifications:privateKey");
            var subject = configuration.GetValue<string>("notifications:subject");

            var vapidDetails = new VapidDetails(subject, publicKey, privateKey);

            foreach(var notification in notifications) {
                var pushSubscription = new PushSubscription(notification.URL, notification.P256dh, notification.Auth);

                var webPushClient = new WebPushClient();

                try {
                    var payload = JsonSerializer.Serialize(new {
                        title = movie.Title,
                        image = movie.Poster,
                        url = $"movie/{movie.ID}/{movie.urlTitle()}"
                    });

                    await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
                }
                catch(Exception ex) {
                    throw ex;
                }
            }
        }
    }
}
