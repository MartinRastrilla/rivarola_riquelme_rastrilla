using MySql.Data.MySqlClient;

namespace rivarola_riquelme_rastrilla.Models;

public class RepositorioPago
{
    string ConnectionString = "Server=localhost;Database=inmobiliaria;User=root;SslMode=none";

    public List<Pago> ObtenerTodos()
    {
        List<Pago> pagos = new List<Pago>();

        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = $@"
        SELECT 
            p.{nameof(Pago.Id)} AS id, 
            p.{nameof(Pago.Contrato_id)} AS contrato_id, 
            p.fecha_pago AS {nameof(Pago.Fecha_pago)}, 
            p.{nameof(Pago.Detalle)} AS detalle, 
            p.{nameof(Pago.Importe)} AS importe, 
            i.{nameof(Inmueble.Id)} AS inmueble_id, 
            i.{nameof(Inmueble.Direccion)} AS inmueble_direccion, 
            inq.{nameof(Inquilino.Dni)} AS inquilino_dni, 
            inq.{nameof(Inquilino.Nombre)} AS inquilino_nombre, 
            inq.{nameof(Inquilino.Apellido)} AS inquilino_apellido,
            inq.{nameof(Inquilino.Email)} AS inquilino_email,
            inq.{nameof(Inquilino.Telefono)} AS inquilino_telefono
        FROM pagos p
        JOIN contratos c ON p.{nameof(Pago.Contrato_id)} = c.{nameof(Contratos.Id)}
        JOIN inmuebles i ON c.{nameof(Contratos.Inmueble_id)} = i.{nameof(Inmueble.Id)}
        JOIN inquilinos inq ON c.{nameof(Contratos.Inquilino_dni)} = inq.{nameof(Inquilino.Dni)}";

            using (var command = new MySqlCommand(query, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var inquilino = new Inquilino
                    {
                        Dni = reader.GetInt64("inquilino_dni"),
                        Nombre = reader.GetString("inquilino_nombre"),
                        Apellido = reader.GetString("inquilino_apellido"),
                        Email = reader.GetString("inquilino_email"),
                        Telefono = reader.GetInt64("inquilino_telefono")
                    };

                    var inmueble = new Inmueble
                    {
                        Id = reader.GetInt32("inmueble_id"),
                        Direccion = reader.GetString("inmueble_direccion")
                    };

                    var contrato = new Contratos
                    {
                        Id = reader.GetInt32(nameof(Contratos.Id)),
                        Inquilino_dni = reader.GetInt64(nameof(Contratos.Inquilino_dni)),
                        Inmueble_id = reader.GetInt32(nameof(Contratos.Inmueble_id)),
                        Inquilino = inquilino,
                        Inmueble = inmueble
                    };

                    pagos.Add(new Pago
                    {
                        Id = reader.GetInt32(nameof(Pago.Id)),
                        Contrato_id = reader.GetInt32(nameof(Pago.Contrato_id)),
                        Fecha_pago = reader.GetDateTime(nameof(Pago.Fecha_pago)),
                        Detalle = reader.GetString(nameof(Pago.Detalle)),
                        Importe = reader.GetDecimal(nameof(Pago.Importe)),
                        Contrato = contrato
                    });
                }
            }

            connection.Close();
            return pagos;
        }
    }

    public int ObtenerTotalPagos()
    {
        int totalPagos = 0;

        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = "SELECT COUNT(*) FROM pagos";
            using (var command = new MySqlCommand(query, connection))
            {
                connection.Open();
                totalPagos = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
            }
        }
        return totalPagos;
    }

    public List<Pago> ObtenerPaginado(int page, int pageSize)
    {
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            connection.Open();
            var query = $@"
        SELECT 
            p.{nameof(Pago.Id)} AS id, 
            p.{nameof(Pago.Contrato_id)} AS contrato_id, 
            p.fecha_pago AS {nameof(Pago.Fecha_pago)}, 
            p.{nameof(Pago.Detalle)} AS detalle, 
            p.{nameof(Pago.Importe)} AS importe,
            p.activo AS activo, 
            i.{nameof(Inmueble.Id)} AS inmueble_id, 
            i.{nameof(Inmueble.Direccion)} AS inmueble_direccion, 
            inq.{nameof(Inquilino.Dni)} AS inquilino_dni, 
            inq.{nameof(Inquilino.Nombre)} AS inquilino_nombre, 
            inq.{nameof(Inquilino.Apellido)} AS inquilino_apellido,
            inq.{nameof(Inquilino.Email)} AS inquilino_email,
            inq.{nameof(Inquilino.Telefono)} AS inquilino_telefono
        FROM pagos p
        JOIN contratos c ON p.{nameof(Pago.Contrato_id)} = c.{nameof(Contratos.Id)}
        JOIN inmuebles i ON c.{nameof(Contratos.Inmueble_id)} = i.{nameof(Inmueble.Id)}
        JOIN inquilinos inq ON c.{nameof(Contratos.Inquilino_dni)} = inq.{nameof(Inquilino.Dni)}
        LIMIT @Offset, @PageSize";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
                command.Parameters.AddWithValue("@PageSize", pageSize);
                var pagos = new List<Pago>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var inquilino = new Inquilino
                        {
                            Dni = reader.GetInt64("inquilino_dni"),
                            Nombre = reader.GetString("inquilino_nombre"),
                            Apellido = reader.GetString("inquilino_apellido"),
                            Email = reader.GetString("inquilino_email"),
                            Telefono = reader.GetInt64("inquilino_telefono")
                        };

                        var inmueble = new Inmueble
                        {
                            Id = reader.GetInt32("inmueble_id"),
                            Direccion = reader.GetString("inmueble_direccion")
                        };

                        var contrato = new Contratos
                        {
                            Id = reader.GetInt32(nameof(Contratos.Id)),
                            Inquilino_dni = reader.GetInt64(nameof(Contratos.Inquilino_dni)),
                            Inmueble_id = reader.GetInt32(nameof(Contratos.Inmueble_id)),
                            Inquilino = inquilino,
                            Inmueble = inmueble
                        };

                        pagos.Add(new Pago
                        {
                            Id = reader.GetInt32(nameof(Pago.Id)),
                            Contrato_id = reader.GetInt32(nameof(Pago.Contrato_id)),
                            Fecha_pago = reader.GetDateTime(nameof(Pago.Fecha_pago)),
                            Detalle = reader.GetString(nameof(Pago.Detalle)),
                            Importe = reader.GetDecimal(nameof(Pago.Importe)),
                            Activo = reader.GetBoolean(nameof(Pago.Activo)),
                            Contrato = contrato
                        });
                    }
                }
                return pagos;
            }
        }
    }

    public Pago? ObtenerPorId(int id)
    {
        Pago? pago = null;

        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = $@"SELECT {nameof(Pago.Id)} AS id, {nameof(Pago.Contrato_id)} AS contrato_id, 
                              fecha_pago AS {nameof(Pago.Fecha_pago)}, {nameof(Pago.Detalle)} AS detalle, 
                              {nameof(Pago.Importe)} AS importe FROM pagos WHERE {nameof(Pago.Id)} = @id";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();

                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    pago = new Pago
                    {
                        Id = reader.GetInt32(nameof(Pago.Id)),
                        Contrato_id = reader.IsDBNull(reader.GetOrdinal(nameof(Pago.Contrato_id))) ? null : reader.GetInt32(nameof(Pago.Contrato_id)),
                        Fecha_pago = reader.IsDBNull(reader.GetOrdinal(nameof(Pago.Fecha_pago))) ? null : reader.GetDateTime(nameof(Pago.Fecha_pago)),
                        Detalle = reader.IsDBNull(reader.GetOrdinal(nameof(Pago.Detalle))) ? null : reader.GetString(nameof(Pago.Detalle)),
                        Importe = reader.IsDBNull(reader.GetOrdinal(nameof(Pago.Importe))) ? null : reader.GetDecimal(nameof(Pago.Importe))
                    };
                }
                connection.Close();
            }
            return pago;
        }
    }

    public int DesactivarPago(int id)
    {
        int r = 0;
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = $@"UPDATE pagos SET activo = 0 WHERE id = @id";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                r = command.ExecuteNonQuery();
                connection.Close();
            }
            return r;
        }
    }

    public int Agregar(Pago pago)
    {
        int r = 0;
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = $@"INSERT INTO pagos ({nameof(Pago.Contrato_id)}, fecha_pago, 
                                  {nameof(Pago.Detalle)}, {nameof(Pago.Importe)}) 
                                  VALUES (@{nameof(Pago.Contrato_id)}, @{nameof(Pago.Fecha_pago)}, 
                                  @{nameof(Pago.Detalle)}, @{nameof(Pago.Importe)});
                                  SELECT LAST_INSERT_ID();"; // Devuelve el ID del último registro insertado
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Pago.Contrato_id)}", pago.Contrato_id);
                command.Parameters.AddWithValue($"@{nameof(Pago.Fecha_pago)}", pago.Fecha_pago);
                command.Parameters.AddWithValue($"@{nameof(Pago.Detalle)}", pago.Detalle);
                command.Parameters.AddWithValue($"@{nameof(Pago.Importe)}", pago.Importe);
                connection.Open();
                r = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
            }
        }
        return r;
    }

    public void Editar(Pago pago)
    {
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = $@"UPDATE pagos SET {nameof(Pago.Contrato_id)} = @{nameof(Pago.Contrato_id)}, 
                                  fecha_pago = @{nameof(Pago.Fecha_pago)}, 
                                  {nameof(Pago.Detalle)} = @{nameof(Pago.Detalle)}, 
                                  {nameof(Pago.Importe)} = @{nameof(Pago.Importe)} 
                                  WHERE {nameof(Pago.Id)} = @{nameof(Pago.Id)}";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Pago.Contrato_id)}", pago.Contrato_id);
                command.Parameters.AddWithValue($"@{nameof(Pago.Fecha_pago)}", pago.Fecha_pago);
                command.Parameters.AddWithValue($"@{nameof(Pago.Detalle)}", pago.Detalle);
                command.Parameters.AddWithValue($"@{nameof(Pago.Importe)}", pago.Importe);
                command.Parameters.AddWithValue($"@{nameof(Pago.Id)}", pago.Id);
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
            var query = $@"DELETE FROM pagos WHERE {nameof(Pago.Id)} = @{nameof(Pago.Id)}";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Pago.Id)}", id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    public List<Pago> ObtenerPagosPorContrato(int contratoId, int page, int pageSize)
    {
        List<Pago> pagos = new List<Pago>();
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            connection.Open();
            var query = $@"SELECT {nameof(Pago.Id)} AS id, {nameof(Pago.Contrato_id)} AS contrato_id, 
                              fecha_pago AS {nameof(Pago.Fecha_pago)}, {nameof(Pago.Detalle)} AS detalle, 
                              {nameof(Pago.Importe)} AS importe FROM pagos WHERE {nameof(Pago.Contrato_id)} = @contrato_id
                              LIMIT @offset, @pageSize;";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@contrato_id", contratoId);
                command.Parameters.AddWithValue("@offset", (page - 1) * pageSize);
                command.Parameters.AddWithValue("@pageSize", pageSize);

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    pagos.Add(new Pago
                    {
                        Id = reader.GetInt32("id"),
                        Contrato_id = reader.GetInt32("contrato_id"),
                        Fecha_pago = reader.GetDateTime("fecha_pago"),
                        Detalle = reader.GetString("detalle"),
                        Importe = reader.GetDecimal("importe")
                    });
                }
                connection.Close();
            }
        }
        return pagos;
    }

    public int ObtenerTotalPagosPorContrato(int contratoId)
    {
        int totalPagos = 0;
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = $"SELECT COUNT(*) FROM pagos WHERE {nameof(Pago.Contrato_id)} = @contrato_id;";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@contrato_id", contratoId);
                connection.Open();
                totalPagos = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
            }
        }
        return totalPagos;
    }
}


