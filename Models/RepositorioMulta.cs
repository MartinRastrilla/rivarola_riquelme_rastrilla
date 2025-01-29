using MySql.Data.MySqlClient;
namespace rivarola_riquelme_rastrilla.Models;

public class RepositorioMulta
{
    string Conexion = "Server=localhost;User=root;Password=;Database=inmobiliaria;SslMode=none";

    public void AgregarMulta(Multa multa)
    {
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            string query = "INSERT INTO multa (Contrato_id, monto, fecha_multa) VALUES (@Contrato_id, @monto, @fecha_multa);";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Contrato_id", multa.Id);
                command.Parameters.AddWithValue("@monto", multa.Monto);
                command.Parameters.AddWithValue("@fecha_multa", DateTime.Now);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }    
        }
    }

    public List<Multa> ObtenerMultas(){
        List<Multa> multas = new List<Multa>();
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            string query = "SELECT * FROM multa;";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        multas.Add(new Multa
                        {
                            Id = reader.GetInt32("Id"),
                            Contrato_id = reader.GetInt32("Contrato_id"),
                            Monto = reader.GetDecimal("Monto"),
                            Fecha_multa = reader.GetDateTime("Fecha_multa")
                        });
                    }
                }
                connection.Close();
            }
        }
        return multas;
    }

}