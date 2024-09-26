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
            var sqlquery = @"
                SELECT 
                    c.id AS ContratoId,
                    c.inquilino_dni AS InquilinoDni,
                    c.inmueble_id AS InmuebleId,
                    c.estado AS ContratoEstado,
                    c.monto AS ContratoMonto,
                    c.fecha_inicio AS ContratoFechaInicio,
                    c.fecha_fin AS ContratoFechaFin,
                    i.nombre AS InquilinoNombre,
                    i.apellido AS InquilinoApellido,
                    i.telefono AS InquilinoTelefono,
                    i.email AS InquilinoEmail,
                    inm.direccion AS InmuebleDireccion,
                    t.id AS TipoId,
                    t.nombre AS TipoNombre
                FROM contratos c
                JOIN inquilinos i ON c.inquilino_dni = i.dni
                JOIN inmuebles inm ON c.inmueble_id = inm.id
                JOIN tipos t ON inm.tipo_id = t.id"; // Relación con tipos

            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    // Parseamos el estado del contrato
                    Contratos.EstadoContrato estadoContrato;
                    Enum.TryParse(reader.GetString("ContratoEstado"), out estadoContrato);

                    // Creamos el objeto Inquilino y lo mapeamos
                    var inquilino = new Inquilino
                    {
                        Dni = reader.GetInt64("InquilinoDni"),
                        Nombre = reader.GetString("InquilinoNombre"),
                        Apellido = reader.GetString("InquilinoApellido"),
                        Telefono = reader.GetInt64("InquilinoTelefono"),
                        Email = reader.GetString("InquilinoEmail")
                    };

                    // Creamos el objeto Tipo y lo mapeamos
                    var tipo = new Tipo
                    {
                        Id = reader.GetInt32("TipoId"),
                        Nombre = reader.GetString("TipoNombre")
                    };

                    // Creamos el objeto Inmueble y lo mapeamos
                    var inmueble = new Inmueble
                    {
                        Id = reader.GetInt32("InmuebleId"),
                        Direccion = reader.GetString("InmuebleDireccion"),
                        Tipo = tipo // Asignamos el objeto Tipo
                    };

                    // Creamos el objeto Contratos y lo mapeamos
                    contratos.Add(new Contratos
                    {
                        Id = reader.GetInt32("ContratoId"),
                        Inquilino_dni = reader.GetInt64("InquilinoDni"),
                        Inquilino = inquilino,
                        Inmueble_id = reader.GetInt32("InmuebleId"),
                        Inmueble = inmueble,
                        Estado = estadoContrato,
                        Monto = reader.GetDecimal("ContratoMonto"),
                        Fecha_inicio = reader.GetDateTime("ContratoFechaInicio"),
                        Fecha_fin = reader.GetDateTime("ContratoFechaFin")
                    });
                }
                connection.Close();
                return contratos;
            }
        }
    }

    public Contratos? Obtener(long Id)
    {
        Contratos? contrato = null;
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            var sqlquery = @"
                SELECT 
                    c.id AS ContratoId,
                    c.inquilino_dni AS InquilinoDni,
                    c.inmueble_id AS InmuebleId,
                    c.estado AS ContratoEstado,
                    c.monto AS ContratoMonto,
                    c.fecha_inicio AS ContratoFechaInicio,
                    c.fecha_fin AS ContratoFechaFin,
                    inm.direccion AS InmuebleDireccion,
                    t.id AS TipoId,
                    t.nombre AS TipoNombre
                FROM contratos c
                JOIN inmuebles inm ON c.inmueble_id = inm.id
                JOIN tipos t ON inm.tipo_id = t.id
                WHERE c.id=@Id"; // Unimos también la tabla tipos

            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@Id", Id);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var tipo = new Tipo
                        {
                            Id = reader.GetInt32("TipoId"),
                            Nombre = reader.GetString("TipoNombre")
                        };

                        var inmueble = new Inmueble
                        {
                            Id = reader.GetInt32("InmuebleId"),
                            Direccion = reader.GetString("InmuebleDireccion"),
                            Tipo = tipo
                        };

                        contrato = new Contratos
                        {
                            Id = reader.GetInt32("ContratoId"),
                            Inquilino_dni = reader.GetInt64("InquilinoDni"),
                            Inmueble_id = reader.GetInt32("InmuebleId"),
                            Inmueble = inmueble,
                            Estado = (Contratos.EstadoContrato)Enum.Parse(typeof(Contratos.EstadoContrato), reader.GetString("ContratoEstado")),
                            Monto = reader.GetDecimal("ContratoMonto"),
                            Fecha_inicio = reader.GetDateTime("ContratoFechaInicio"),
                            Fecha_fin = reader.GetDateTime("ContratoFechaFin")
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
}
