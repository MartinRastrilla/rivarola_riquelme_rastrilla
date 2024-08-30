using MySql.Data.MySqlClient;

namespace rivarola_riquelme_rastrilla.Controllers;


public class RepositorioPropietario
{
    string Conexion = "Server=localhost;User=root;Password=;Database=bdinmobiliaria;SslMode=none";


    public List<Propietario> ObtenerTodos()
    {

        List<Propietario> propie = new List<Propietario>();

        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            var query = $@"SELECT {nameof(Propietario.Nombre)} , {nameof(Propietario.Apellido)} , {nameof(Propietario.Dni)} , {nameof(Propietario.Telefono)} , {nameof(Propietario.Email)} FROM Propietarios";

            using (var command = new MySqlCommand(query, connection))
            {
                connection.Open();

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    propie.Add(new Propietario
                    {
                        Nombre = reader.GetString("Nombre"),
                        Apellido = reader.GetString("Apellido"),
                        Dni = reader.GetInt32("Dni"),
                        Telefono = reader.GetInt64("Telefono"),
                        Email = reader.GetString("Email")
                    });
                }
                connection.Close();
            }
            return propie;

        }
    }
}