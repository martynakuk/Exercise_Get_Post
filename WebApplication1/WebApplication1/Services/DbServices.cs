using Kolokwium_Poprawa.Models;
using System.Data.SqlClient;

namespace Kolokwium_Poprawa.Services
{
    public interface IDbServices
    {
        Task<bool> OwnerExists(int ownerId);
        Task<ICollection<OwnerDTO>> OwnerList(int ownerId);

        Task<bool> ObjectExists(int objectId);
        Task AddObjectToOwner(int objectId, int ownerId);
    }
    public class DbServices : IDbServices
    {
        private readonly IConfiguration _config;

        public DbServices(IConfiguration config)
        {
            _config = config;
        }

        public async Task AddObjectToOwner(int objectId, int ownerId)
        {
            using (var connection = new SqlConnection(_config.GetConnectionString("Default")))
            {
                var command = connection.CreateCommand();
                command.CommandText = "Insert into Object_Owner (Object_ID,Owner_ID) values (@1, @2)";
                command.Parameters.AddWithValue("@1", objectId);
                command.Parameters.AddWithValue("@2", ownerId);
                await connection.OpenAsync();
                await command.ExecuteScalarAsync();
            }

        }

        public async Task<bool> ObjectExists(int objectId)
        {
            using (var connection = new SqlConnection(_config.GetConnectionString("Default")))
            {
                var command = connection.CreateCommand();
                command.CommandText = "Select * from Object where id = @1";
                command.Parameters.AddWithValue("@1", objectId);
                await connection.OpenAsync();
                return await command.ExecuteScalarAsync() is not null;
            }
        }

        public async Task<bool> OwnerExists(int ownerId)
        {
            using (var connection = new SqlConnection(_config.GetConnectionString("Default")))
            {
                var command = connection.CreateCommand();
                command.CommandText = "Select * from Owner where id = @1";
                command.Parameters.AddWithValue("@1", ownerId);
                await connection.OpenAsync();
                return await command.ExecuteScalarAsync() is not null;
            }
        }

        public async Task<ICollection<OwnerDTO>> OwnerList(int ownerId)
        {
            var results = new List<OwnerDTO>();
            using (var connection = new SqlConnection(_config.GetConnectionString("Default")))
            {
                var command = connection.CreateCommand();
                command.CommandText = "Select Owner.FirstName, Owner.LastName, Owner.PhoneNumber, Object.Id, Object.Width, Object.Height, Object_Type.Name, Warehouse.Name " +
                    "from Owner " +
                    "join Object_Owner on Owner.id = Object_Owner.Owner_id " +
                    "join Object on Object_owner.Object_id = Object.id " +
                    "join Object_Type on Object.Object_type_id = object_type.id " +
                    "join warehouse on object.warehouse_id = warehouse.id " +
                    "where Owner.id = @1;";
                command.Parameters.AddWithValue("@1", ownerId);

                await connection.OpenAsync();

                var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var FirstName = reader.GetString(0);
                    if (results.Find(e => e.FirstName == FirstName) is null)
                    {
                        results.Add(new OwnerDTO
                        {
                            FirstName = FirstName,
                            LastName = reader.GetString(1),
                            PhoneNumber = reader.GetString(2),
                            OwnerObjects = new List<ObjectOwnerDTO>
                            {
                                new ObjectOwnerDTO
                                {
                                    Id = reader.GetInt32(3),
                                    Width = reader.GetDecimal(4),
                                    Height = reader.GetDecimal(5),
                                    Type = reader.GetString(6),
                                    WareHouse = reader.GetString(7)
                                }
                            }
                        });
                    }
                    else
                    {
                        results.Find(e => e.FirstName == FirstName)!.OwnerObjects.Add(
                            new ObjectOwnerDTO
                            {
                                Id = reader.GetInt32(3),
                                Width = reader.GetDecimal(4),
                                Height = reader.GetDecimal(5),
                                Type = reader.GetString(6),
                                WareHouse = reader.GetString(7)
                            });
                    }
                }

            }

            return results;
        }
    }
}
