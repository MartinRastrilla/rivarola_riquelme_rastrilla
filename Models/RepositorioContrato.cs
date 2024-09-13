using MySql.Data.MySqlClient;

namespace rivarola_riquelme_rastrilla.Models;

public class RepositorioContrato
{
    string Conexion = "Server=localhost;User=root;Password=;Database=inmobiliaria;SslMode=none";
    public List<Contratos> ObtenerContratos()
    {
        List<Contratos> contratos = new List<Contratos>();
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            var sqlquery = $@"SELECT {nameof(Contratos.Id)},{nameof(Contratos.Inquilino_dni)},{nameof(Contratos.Inmueble_id)},{nameof(Contratos.Estado)},{nameof(Contratos.Monto)},{nameof(Contratos.Fecha_inicio)},{nameof(Contratos.Fecha_fin)} 
            FROM contratos";
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Contratos.EstadoContrato estadoContrato;
                    Enum.TryParse(reader.GetString("estado"), out estadoContrato);
                    contratos.Add(new Contratos
                    {
                        Id = reader.GetInt32(nameof(Contratos.Id)),
                        Inquilino_dni = reader.GetInt64(nameof(Contratos.Inquilino_dni)),
                        Inmueble_id = reader.GetInt32(nameof(Contratos.Inmueble_id)),
                        Estado = estadoContrato,
                        Monto = reader.GetDecimal(nameof(Contratos.Monto)),
                        Fecha_inicio = reader.GetDateTime(nameof(Contratos.Fecha_inicio)),
                        Fecha_fin = reader.GetDateTime(nameof(Contratos.Fecha_fin))
                    });
                }
                return contratos;
            }
        }
    }

    public Contratos? Obtener(long Id)
    {
        Contratos? contrato = null;
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            var sqlquery = "SELECT Id,inquilino_dni,inmueble_id,estado,monto,fecha_inicio,fecha_fin FROM contratos WHERE Id=@Id;";
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@Id", Id);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        contrato = new Contratos
                        {
                            Id = reader.GetInt32("Id"),
                            Inquilino_dni = reader.GetInt64("inquilino_dni"),
                            Inmueble_id = reader.GetInt32("inmueble_id"),
                            Estado = (Contratos.EstadoContrato)Enum.Parse(typeof(Contratos.EstadoContrato), reader.GetString("estado")),
                            Monto = reader.GetDecimal("monto"),
                            Fecha_inicio = reader.GetDateTime("fecha_inicio"),
                            Fecha_fin = reader.GetDateTime("fecha_fin")
                        };
                    }
                }
                connection.Close();
                return contrato;
            }
        }
    }

    public int AltaContrato(Contratos contrato)
    {
        int r = 0;
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            var sqlquery = @"INSERT INTO contratos (inquilino_dni, inmueble_id, estado, monto, fecha_inicio, fecha_fin) 
                            VALUES (@inquilino_dni, @inmueble_id, @estado, @monto, @fecha_inicio, @fecha_fin);";
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@inquilino_dni", contrato.Inquilino_dni);
                command.Parameters.AddWithValue("@inmueble_id", contrato.Inmueble_id);
                command.Parameters.AddWithValue("@estado", contrato.Estado.ToString());
                command.Parameters.AddWithValue("@monto", contrato.Monto);
                command.Parameters.AddWithValue("@fecha_inicio", contrato.Fecha_inicio);
                command.Parameters.AddWithValue("@fecha_fin", contrato.Fecha_fin);
                connection.Open();
                r = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return r;
    }

    public int Guardar(Contratos contrato)
    {
        int r = 0;
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            var sqlquery = @"UPDATE contratos SET inquilino_dni=@inquilino_dni, inmueble_id=@inmueble_id, estado=@estado, monto=@monto, fecha_inicio=@fecha_inicio, fecha_fin=@fecha_fin 
            WHERE Id=@Id;";
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@inquilino_dni", contrato.Inquilino_dni);
                command.Parameters.AddWithValue("@inmueble_id", contrato.Inmueble_id);
                command.Parameters.AddWithValue("@estado", contrato.Estado.ToString());
                command.Parameters.AddWithValue("@monto", contrato.Monto);
                command.Parameters.AddWithValue("@fecha_inicio", contrato.Fecha_inicio);
                command.Parameters.AddWithValue("@fecha_fin", contrato.Fecha_fin);
                command.Parameters.AddWithValue("@Id", contrato.Id);
                connection.Open();
                r = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return r;
    }

    public int BorrarContrato(long Id)
    {
        int r = -1;
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            var sqlquery = @"DELETE FROM Contratos WHERE Id=@Id;";
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

    public List<Contratos> verContratoInmueble(long Id)
    {
        List<Contratos> contratos = new List<Contratos>();
        int r = -1;
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            var sqlquery = @"SELECT Id,inquilino_dni,inmueble_id,estado,monto,fecha_inicio,fecha_fin FROM contratos WHERE inmueble_id=@inmueble_id";
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@inmueble_id", Id);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Contratos contrato = new Contratos
                        {
                            Id = reader.GetInt32("Id"),
                            Inquilino_dni = reader.GetInt64("inquilino_dni"),
                            Inmueble_id = reader.GetInt32("inmueble_id"),
                            Estado = (Contratos.EstadoContrato)Enum.Parse(typeof(Contratos.EstadoContrato), reader.GetString("estado")),
                            Monto = reader.GetDecimal("monto"),
                            Fecha_inicio = reader.GetDateTime("fecha_inicio"),
                            Fecha_fin = reader.GetDateTime("fecha_fin")
                        };
                        contratos.Add(contrato);
                    }
                }
                connection.Close();
                return contratos;
            }
        }
    }
}
