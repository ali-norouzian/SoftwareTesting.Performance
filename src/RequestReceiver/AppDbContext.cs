using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;

namespace RequestReceiver
{

    public class AppDbContext : DbContext
    {
        public DbSet<SqlCounter> Counters { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer("Server=localhost;Database=TestDB;User Id=sa;Password=1234@Abcd; Max Pool Size=1000; TrustServerCertificate=True;");
        }
    }






    public class SqlCounter
    {
        public int Id { get; set; }
        public int Value { get; set; }
    }

    public class MongoCounter
    {
        public ObjectId Id { get; set; }
        public int Value { get; set; }
    }





    public static class Data
    {
        public static async Task IncreaseOneSqlAsync()
        {
            await using (var context = new AppDbContext())
            {
                await using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var counter = await context.Counters.FirstOrDefaultAsync(c => c.Id == 1);
                        //if (counter != null)
                        //{
                        counter.Value += 1;
                        await context.SaveChangesAsync();
                        //}

                        // Commit transaction
                        await transaction.CommitAsync();

                    }
                    catch (Exception)
                    {
                        // Rollback transaction if any error occurs
                        await transaction.RollbackAsync();
                    }

                }
            }
        }

        public static async Task IncreaseOneMongoAsync()
        {
            using var client = new MongoClient("mongodb://admin:admin@localhost:27017");
            var database = client.GetDatabase("TestDB");
            var collection = database.GetCollection<MongoCounter>("counters");

            var filter = Builders<MongoCounter>.Filter.Eq("_id", ObjectId.Parse("679fa3ba1a57cb1794c259c5"));
            var update = Builders<MongoCounter>.Update.Inc(c => c.Value, 1);

            await collection.UpdateOneAsync(filter, update);

        }
    }

}
