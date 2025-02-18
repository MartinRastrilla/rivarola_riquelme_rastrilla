using MySql.Data.MySqlClient;
using rivarola_riquelme_rastrilla.Models;

namespace rivarola_riquelme_rastrilla.Controllers;

public class RepositorioRegistroPagos
{
    string Conexion = "Server=localhost;User=root;Password=;Database=inmobiliaria;SslMode=none";

    public List<RegistroPagos> ObtenerRegistroPagosPaginado(int page, int pageSize)
    {
        var registroPagos = new List<RegistroPagos>();
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            connection.Open();
            var sqlquery = @"
            SELECT
            rp.id,
            rp.fecha_creacion,
            rp.creado_por,
            rp.fecha_anulacion,
            rp.anulado_por,
            rp.pagos_id,
            u.id as creado_por_id,
            u.nombre as creado_por_nombre,
            u.apellido as creado_por_apellido,
            u.email as creado_por_email,
            u.avatar as creado_por_avatar,
            u2.id as anulado_por_id,
            u2.nombre as anulado_por_nombre,
            u2.apellido as anulado_por_apellido,
            u2.email as anulado_por_email,
            u2.avatar as anulado_por_avatar,
            p.id as pagos_id,
            p.detalle,
            p.importe,
            p.activo,
            p.fecha_pago,
            c.id as contrato_id,
            c.monto,
            c.fecha_inicio,
            c.fecha_fin,
            inm.direccion as inmueble_direccion,
            inm.coordenadas as inmueble_coordenadas,
            inq.dni as inquilino_dni,
            inq.nombre as inquilino_nombre,
            inq.apellido as inquilino_apellido,
            inq.telefono as inquilino_telefono,
            inq.email as inquilino_email
            FROM registro_pagos rp
            JOIN pagos p ON rp.pagos_id = p.id
            JOIN contratos c ON p.contrato_id = c.id
            JOIN inmuebles inm ON c.inmueble_id = inm.id
            JOIN inquilinos inq ON c.inquilino_dni = inq.dni
            JOIN usuarios u ON rp.creado_por = u.id
            LEFT JOIN usuarios u2 ON rp.anulado_por = u2.id
            ORDER BY rp.fecha_creacion DESC
            LIMIT @Offset, @PageSize;";
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
                command.Parameters.AddWithValue("@PageSize", pageSize);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        registroPagos.Add(new RegistroPagos
                        {
                            Id = reader.GetInt32("id"),
                            Fecha_creacion = reader.GetDateTime("fecha_creacion"),
                            Fecha_anulacion = reader.IsDBNull(reader.GetOrdinal("fecha_anulacion")) ? (DateTime?)null : reader.GetDateTime("fecha_anulacion"),
                            Usuario = new Usuarios
                            {
                                Id = reader.GetInt32("creado_por_id"),
                                Nombre = reader.GetString("creado_por_nombre"),
                                Apellido = reader.GetString("creado_por_apellido"),
                                Email = reader.GetString("creado_por_email"),
                                Avatar = reader.IsDBNull(reader.GetOrdinal("creado_por_avatar")) ? null : reader.GetString("creado_por_avatar"),
                            },
                            Usuario2 = reader.IsDBNull(reader.GetOrdinal("anulado_por")) ? null : new Usuarios
                            {
                                Id = reader.GetInt32("anulado_por_id"),
                                Nombre = reader.GetString("anulado_por_nombre"),
                                Apellido = reader.GetString("anulado_por_apellido"),
                                Email = reader.GetString("anulado_por_email"),
                                Avatar = reader.IsDBNull(reader.GetOrdinal("anulado_por_avatar")) ? null : reader.GetString("anulado_por_avatar"),
                            },
                            Pago = new Pago
                            {
                                Id = reader.GetInt32("pagos_id"),
                                Detalle = reader.GetString("detalle"),
                                Importe = reader.GetDecimal("importe"),
                                Fecha_pago = reader.GetDateTime("fecha_pago"),
                                Activo = reader.GetBoolean("activo"),
                                Contrato = new Contratos
                                {
                                    Id = reader.GetInt32("contrato_id"),
                                    Monto = reader.GetDecimal("monto"),
                                    Fecha_inicio = reader.GetDateTime("fecha_inicio"),
                                    Fecha_fin = reader.GetDateTime("fecha_fin"),
                                    Inmueble = new Inmueble
                                    {
                                        Direccion = reader.GetString("inmueble_direccion"),
                                        Coordenadas = reader.GetString("inmueble_coordenadas")
                                    },
                                    Inquilino = new Inquilino
                                    {
                                        Dni = reader.GetInt64("inquilino_dni"),
                                        Nombre = reader.GetString("inquilino_nombre"),
                                        Apellido = reader.GetString("inquilino_apellido"),
                                        Telefono = reader.GetInt64("inquilino_telefono"),
                                        Email = reader.GetString("inquilino_email")
                                    }
                                }
                            },
                        });
                    }

                }
                connection.Close();
            }
        }
        return registroPagos;
    }

    public int ObtenerTotalRegistrosPago()
    {
        int totalRegistros = 0;
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            connection.Open();
            using (MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM registro_pagos", connection))
            {
                totalRegistros = Convert.ToInt32(command.ExecuteScalar());
            }
            connection.Close();
        }
        return totalRegistros;
    }

    public int CrearRegistroPago(RegistroPagos registroPago)
    {
        int r = -1;
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            var sqlquery = @"INSERT INTO registro_pagos
            (fecha_creacion, creado_por, fecha_anulacion, anulado_por, pagos_id) VALUES (@fecha_creacion, @creado_por, @fecha_anulacion, @anulado_por, @pagos_id);";
            connection.Open();
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@fecha_creacion", registroPago.Fecha_creacion);
                command.Parameters.AddWithValue("@creado_por", registroPago.Creado_por);
                command.Parameters.AddWithValue("@pagos_id", registroPago.Pago_id);

                // Si los datos son NULL, asignar DBNull.Value
                command.Parameters.AddWithValue("@fecha_anulacion", registroPago.Fecha_anulacion ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@anulado_por", registroPago.Usuario2?.Id ?? (object)DBNull.Value);
                r = command.ExecuteNonQuery();
            }
            connection.Close();
        }
        return r;
    }

    public int CrearAnulacionRegistroPago(RegistroPagos registroPago)
    {
        int r = -1;
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            var sqlquery = @"UPDATE registro_pagos SET fecha_anulacion=@fecha_anulacion, anulado_por=@anulado_por
                        WHERE pagos_id = @pagos_id;";
            connection.Open();
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@pagos_id", registroPago.Pago_id);
                command.Parameters.AddWithValue("@fecha_creacion", registroPago.Fecha_creacion);
                command.Parameters.AddWithValue("@creado_por", registroPago.Creado_por);

                // Si los datos son NULL, asignar DBNull.Value 
                command.Parameters.AddWithValue("@fecha_anulacion", registroPago.Fecha_anulacion ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@anulado_por", registroPago.Anulado_por ?? (object)DBNull.Value);
                r = command.ExecuteNonQuery();
            }
            connection.Close();
        }
        return r;
    }

    public RegistroPagos ObtenerRegistroPorPago(int pagoId)
    {
        RegistroPagos registroPago = new RegistroPagos();
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            var sqlquery = @"SELECT id, fecha_creacion, fecha_anulacion, anulado_por, pagos_id, creado_por FROM registro_pagos WHERE pagos_id = @pagoId;";
            connection.Open();
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@pagoId", pagoId);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        registroPago = new RegistroPagos
                        {
                            Id = reader.GetInt32("id"),
                            Fecha_creacion = reader.GetDateTime("fecha_creacion"),
                            Fecha_anulacion = reader.IsDBNull(reader.GetOrdinal("fecha_anulacion")) ? null : reader.GetDateTime("fecha_anulacion"),
                            Creado_por = reader.GetInt32("creado_por"),
                            Anulado_por = reader.IsDBNull(reader.GetOrdinal("anulado_por")) ? null : reader.GetInt32("anulado_por"),
                            Pago_id = reader.GetInt32("pagos_id")
                        };
                    }
                }
            }
            connection.Close();
        }
        return registroPago;
    }
}