using MySql.Data.MySqlClient;
using rivarola_riquelme_rastrilla.Models;
namespace rivarola_riquelme_rastrilla.Controllers
{
    public class RepositorioTipo
    {
        string Conexion = "Server=localhost;User=root;Password=;Database=inmobiliaria;SslMode=none";

        public List<Tipo> ObtenerTipos()
        {
            var tipos = new List<Tipo>();
            using (MySqlConnection connection = new MySqlConnection(Conexion))
            {
                string sqlquery = "SELECT id, nombre FROM tipos";
                using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tipos.Add(new Tipo
                            {
                                Id = reader.GetInt32("id"),
                                Nombre = reader.GetString("nombre")
                            });
                        }
                    }
                }
            }
            return tipos;
        }

        public Tipo ObtenerTipo(int id)
        {
            Tipo tipo = null;
            using (MySqlConnection connection = new MySqlConnection(Conexion))
            {
                string sqlquery = "SELECT id, nombre FROM tipos WHERE id = @Id";
                using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            tipo = new Tipo
                            {
                                Id = reader.GetInt32("id"),
                                Nombre = reader.GetString("nombre")
                            };
                        }
                    }
                }
            }
            return tipo;
        }

        public int AgregarTipo(Tipo tipo)
        {
            int resultado = -1;
            using (MySqlConnection connection = new MySqlConnection(Conexion))
            {
                string sqlquery = "INSERT INTO tipos (nombre) VALUES (@nombre)";
                using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
                {
                    command.Parameters.AddWithValue("@nombre", tipo.Nombre);
                    connection.Open();
                    resultado = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return resultado;
        }

        public int EditarTipo(Tipo tipo)
        {
            int resultado = -1;
            using (MySqlConnection connection = new MySqlConnection(Conexion))
            {
                string sqlquery = "UPDATE tipos SET nombre = @nombre WHERE id = @Id";
                using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
                {
                    command.Parameters.AddWithValue("@Id", tipo.Id);
                    command.Parameters.AddWithValue("@nombre", tipo.Nombre);
                    connection.Open();
                    resultado = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return resultado;
        }

        public int BorrarTipo(int id)
        {
            int resultado = -1;
            using (MySqlConnection connection = new MySqlConnection(Conexion))
            {
                string sqlquery = "DELETE FROM tipos WHERE id = @Id";
                using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    resultado = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return resultado;
        }
    }
}
