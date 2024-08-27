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

}