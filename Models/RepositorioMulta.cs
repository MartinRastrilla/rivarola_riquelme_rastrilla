using MySql.Data.MySqlClient;
using rivarola_riquelme_rastrilla.Models;

namespace rivarola_riquelme_rastrilla.Controllers;

public class RepositorioMulta
{
    string Conexion = "Server=localhost;User=root;Password=;Database=inmobiliaria;SslMode=none";

    public List<Multa> ObtenerMultas()
    {
        var multas = new List<Multa>();
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            string sqlquery = "SELECT id, contrato_id, monto, fecha_multa FROM multa;";
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        multas.Add(new Multa
                        {
                            Id = reader.GetInt32("id"),
                            Contrato_id = reader.GetInt32("contrato_id"),
                            Monto = reader.GetDecimal("monto"),
                            Fecha_multa = reader.GetDateTime("fecha_multa")
                        });
                    }
                }
                connection.Close();
            }
        }
        return multas;
    }

    public int AltaMulta(Multa multa)
    {
        int r = -1;
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            string sqlquery = "INSERT INTO multa (contrato_id, monto, fecha_multa) VALUES (@contrato_id, @monto, @fecha_multa);";
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@contrato_id", multa.Contrato_id);
                command.Parameters.AddWithValue("@monto", multa.Monto);
                command.Parameters.AddWithValue("@fecha_multa", multa.Fecha_multa);
                connection.Open();
                r = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return r;
    }

    public int BajaMulta(int Id)
    {
        int r = -1;
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            string sqlquery = "DELETE FROM multa WHERE id=@Id;";
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@Id", Id);
                connection.Open();
                r = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return r;
    }

    public int EditarMulta(Multa multa)
    {
        int r = -1;
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            string sqlquery = "UPDATE multa SET contrato_id=@contrato_id, monto=@monto, fecha_multa=@fecha_multa WHERE id=@id;";
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@id", multa.Id);
                command.Parameters.AddWithValue("@contrato_id", multa.Contrato_id);
                command.Parameters.AddWithValue("@monto", multa.Monto);
                command.Parameters.AddWithValue("@fecha_multa", multa.Fecha_multa);
                connection.Open();
                r = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return r;
    }
}