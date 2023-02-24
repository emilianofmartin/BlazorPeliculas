using System.Text.Json;

namespace BlazorPeliculas.Client.Helpers {
    public class DbLocalRecord {
        public int Id { get; set; }
        public string Url { get; set; }
        public JsonElement Body {  get; set; }
    }

    public class DbLocalRecords {
        public List<DbLocalRecord> ObjectsToCreate { get; set; } = new List<DbLocalRecord>();
        public List<DbLocalRecord> ObjectsToDelete { get; set; } = new List<DbLocalRecord>();

        public int GetPending() {
            var count = 0;
            count += ObjectsToCreate.Count;
            count += ObjectsToDelete.Count;

            return count;
        }
    }
}
