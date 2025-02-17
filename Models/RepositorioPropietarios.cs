using MySql.Data.MySqlClient;

namespace rivarola_riquelme_rastrilla.Models;


public class RepositorioPropietario
{
    string ConnectionString = "Server=localhost;Database=inmobiliaria;User=root;SslMode=none";

    public List<Propietarios> ObtenerTodos()
    {

    List<Propietarios> propietarios = new List<Propietarios>();

    using (MySqlConnection connection = new MySqlConnection(ConnectionString))
    {
        var query = $@"SELECT {nameof(Propietarios.Id)}, {nameof(Propietarios.Nombre)} , {nameof(Propietarios.Apellido)} , {nameof(Propietarios.Dni)} , {nameof(Propietarios.Telefono)} , {nameof(Propietarios.Email)} FROM propietarios";

        using (var command = new MySqlCommand(query, connection))
        {
            connection.Open();

            var reader = command.ExecuteReader();
            
                while (reader.Read())
                {
                    propietarios.Add(new Propietarios
                    {
                        Id = reader.GetInt32("Id"),
                        Nombre = reader.GetString("Nombre"),
                        Apellido = reader.GetString("Apellido"),
                        Dni = reader.GetInt32("Dni"),
                        Telefono = reader.GetString("Telefono"),
                        Email = reader.GetString("Email")
                    });
                }
            connection.Close();    
        }
        return propietarios;

        }
    }

    public int ObtenerTotalPropietarios(string search = "")
    {
        int totalPropietarios = 0;

        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = $@"SELECT COUNT(*) FROM propietarios
                        WHERE (@Search IS NULL OR Nombre LIKE @Search OR Apellido LIKE @Search OR Dni LIKE @Search);";

            using (var command = new MySqlCommand(query, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@Search", string.IsNullOrEmpty(search) ? (object)DBNull.Value : $"%{search}%");
                totalPropietarios = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
            }
        }
        return totalPropietarios;
    }

    public List<Propietarios> ObtenerPaginado(int page, int pageSize,string search = "")
    {
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            connection.Open();
            var sqlquery = @"SELECT * FROM propietarios 
                            WHERE (@Search IS NULL OR Nombre LIKE @Search OR Apellido LIKE @Search OR Dni LIKE @Search) 
                            LIMIT @Offset, @PageSize;";
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@Search", string.IsNullOrEmpty(search) ? (object)DBNull.Value : $"%{search}%");
                command.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
                command.Parameters.AddWithValue("@PageSize", pageSize);

                var propietarios = new List<Propietarios>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        propietarios.Add(new Propietarios
                        {
                            Id = reader.GetInt32("Id"),
                            Nombre = reader.GetString("Nombre"),
                            Apellido = reader.GetString("Apellido"),
                            Dni = reader.GetInt32("Dni"),
                            Telefono = reader.GetString("Telefono"),
                            Email = reader.GetString("Email")
                        });
                    }
                    return propietarios;
                }
            }
        }
    }

    public Propietarios? ObtenerPorId(int id)
    {
        Propietarios? propietario = null;

        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = $@"SELECT {nameof(Propietarios.Id)}, {nameof(Propietarios.Nombre)}, {nameof(Propietarios.Apellido)}, 
                           {nameof(Propietarios.Dni)}, {nameof(Propietarios.Telefono)}, {nameof(Propietarios.Email)} 
                           FROM propietarios WHERE Id = @id";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();

                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    propietario = new Propietarios
                    {
                        Id = reader.GetInt32("Id"),
                        Nombre = reader.GetString("Nombre"),
                        Apellido = reader.GetString("Apellido"),
                        Dni = reader.GetInt32("Dni"),
                        Telefono = reader.GetString("Telefono"),
                        Email = reader.GetString("Email")
                    };
                }
                connection.Close();
            }
        }
        return propietario;
    }

    public Propietarios? ObtenerPorDni(int dni)
    {
        Propietarios? propietario = null;

        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = $@"SELECT {nameof(Propietarios.Id)}, {nameof(Propietarios.Nombre)}, {nameof(Propietarios.Apellido)}, 
                           {nameof(Propietarios.Dni)}, {nameof(Propietarios.Telefono)}, {nameof(Propietarios.Email)} 
                           FROM propietarios WHERE Dni = @dni";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@dni", dni);
                connection.Open();

                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    propietario = new Propietarios
                    {
                        Id = reader.GetInt32("Id"),
                        Nombre = reader.GetString("Nombre"),
                        Apellido = reader.GetString("Apellido"),
                        Dni = reader.GetInt32("Dni"),
                        Telefono = reader.GetString("Telefono"),
                        Email = reader.GetString("Email")
                    };
                }
                connection.Close();
            }
        }
        return propietario;
    }

    public void Editar(Propietarios propietario)
    {
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = $@"UPDATE propietarios SET {nameof(Propietarios.Nombre)} = @nombre, {nameof(Propietarios.Apellido)} = @apellido, 
                           {nameof(Propietarios.Dni)} = @dni, {nameof(Propietarios.Telefono)} = @telefono, 
                           {nameof(Propietarios.Email)} = @email WHERE {nameof(Propietarios.Id)} = @id";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", propietario.Id);
                command.Parameters.AddWithValue("@nombre", propietario.Nombre);
                command.Parameters.AddWithValue("@apellido", propietario.Apellido);
                command.Parameters.AddWithValue("@dni", propietario.Dni);
                command.Parameters.AddWithValue("@telefono", propietario.Telefono);
                command.Parameters.AddWithValue("@email", propietario.Email);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    public void Eliminar(int id)
    {
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = $@"DELETE FROM propietarios WHERE {nameof(Propietarios.Id)} = @id";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    public void Crear(Propietarios propietario)
    {
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = $@"INSERT INTO propietarios ({nameof(Propietarios.Nombre)}, {nameof(Propietarios.Apellido)}, 
                           {nameof(Propietarios.Dni)}, {nameof(Propietarios.Telefono)}, {nameof(Propietarios.Email)}) 
                           VALUES (@nombre, @apellido, @dni, @telefono, @email)";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@nombre", propietario.Nombre);
                command.Parameters.AddWithValue("@apellido", propietario.Apellido);
                command.Parameters.AddWithValue("@dni", propietario.Dni);
                command.Parameters.AddWithValue("@telefono", propietario.Telefono);
                command.Parameters.AddWithValue("@email", propietario.Email);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }



}