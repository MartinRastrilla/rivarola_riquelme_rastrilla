using MySql.Data.MySqlClient;
using rivarola_riquelme_rastrilla.Models;

namespace rivarola_riquelme_rastrilla.Controllers;

public class RepositorioRegistroContratos
{
    string Conexion = "Server=localhost;User=root;Password=;Database=inmobiliaria;SslMode=none";

    public List<RegistroContratos> ObtenerRegistrosContratosPaginado(int page, int pageSize)
    {
        var registros = new List<RegistroContratos>();

        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            string sqlquery = @"
        SELECT
            rc.id,
            rc.contrato_id,
            rc.fecha_creacion,
            rc.fecha_cancelacion,
            rc.creado_por,
            rc.cancelado_por,
            u.id as creado_por_id,
            u.nombre as creado_por_nombre,
            u.apellido as creado_por_apellido,
            u.email as creado_por_email,
            u2.id as cancelado_por_id,
            u2.nombre as cancelado_por_nombre,
            u2.apellido as cancelado_por_apellido,
            u2.email as cancelado_por_email,
            c.id as contrato_id,
            c.estado,
            c.monto,
            c.fecha_inicio,
            c.fecha_fin,
            inm.direccion as inmueble_direccion,
            inm.precio as inmueble_precio,
            inm.coordenadas as inmueble_coordenadas,
            inq.dni as inquilino_dni,
            inq.nombre as inquilino_nombre,
            inq.apellido as inquilino_apellido
        FROM registro_contratos as rc
        JOIN contratos as c ON rc.contrato_id = c.id
        JOIN usuarios as u ON rc.creado_por = u.id
        JOIN inmuebles as inm ON c.inmueble_id = inm.id
        JOIN inquilinos as inq ON c.inquilino_dni = inq.dni
        LEFT JOIN usuarios as u2 ON rc.cancelado_por = u2.id
        ORDER BY rc.fecha_creacion DESC
        LIMIT @Offset, @PageSize;";

            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
                command.Parameters.AddWithValue("@PageSize", pageSize);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Contratos.EstadoContrato estado;
                        Enum.TryParse(reader.GetString("estado"), out estado);

                        registros.Add(new RegistroContratos
                        {
                            Id = reader.GetInt32("id"),
                            Contrato_id = reader.GetInt32("contrato_id"),
                            Fecha_creacion = reader.GetDateTime("fecha_creacion"),
                            Fecha_cancelacion = reader.IsDBNull(reader.GetOrdinal("fecha_cancelacion")) ? (DateTime?)null : reader.GetDateTime("fecha_cancelacion"),
                            Usuario = new Usuarios
                            {
                                Id = reader.GetInt32("creado_por_id"),
                                Nombre = reader.GetString("creado_por_nombre"),
                                Apellido = reader.GetString("creado_por_apellido"),
                                Email = reader.GetString("creado_por_email")
                            },
                            Usuario2 = reader.IsDBNull(reader.GetOrdinal("cancelado_por_id")) ? null : new Usuarios
                            {
                                Id = reader.GetInt32("cancelado_por_id"),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("cancelado_por_nombre")) ? null : reader.GetString("cancelado_por_nombre"),
                                Apellido = reader.IsDBNull(reader.GetOrdinal("cancelado_por_apellido")) ? null : reader.GetString("cancelado_por_apellido"),
                                Email = reader.IsDBNull(reader.GetOrdinal("cancelado_por_email")) ? null : reader.GetString("cancelado_por_email")
                            },
                            Contrato = new Contratos
                            {
                                Id = reader.GetInt32("contrato_id"),
                                Estado = estado,
                                Monto = reader.GetDecimal("monto"),
                                Fecha_inicio = reader.GetDateTime("fecha_inicio"),
                                Fecha_fin = reader.GetDateTime("fecha_fin"),
                                Inmueble = new Inmueble
                                {
                                    Direccion = reader.GetString("inmueble_direccion"),
                                    Precio = reader.GetDecimal("inmueble_precio"),
                                    Coordenadas = reader.GetString("inmueble_coordenadas")
                                },
                                Inquilino = new Inquilino
                                {
                                    Dni = reader.GetInt64("inquilino_dni"),
                                    Nombre = reader.GetString("inquilino_nombre"),
                                    Apellido = reader.GetString("inquilino_apellido")
                                }
                            }
                        });
                    }
                }

                connection.Close();
            }
        }

        return registros;
    }


    public List<RegistroContratos> ObtenerRegistrosContratos()
    {
        var registros = new List<RegistroContratos>();
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            string sqlquery = "SELECT id, contrato_id, fecha_creacion, creado_por, fecha_cancelacion, cancelado_por FROM registro_contratos;";
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        registros.Add(new RegistroContratos
                        {
                            Id = reader.GetInt32("id"),
                            Contrato_id = reader.GetInt32("contrato_id"),
                            Fecha_creacion = reader.GetDateTime("fecha_creacion"),
                            Creado_por = reader.GetInt32("creado_por"),
                            Fecha_cancelacion = reader.GetDateTime("fecha_cancelacion"),
                            Cancelado_por = reader.GetInt32("cancelado_por")
                        });
                    }
                }
                connection.Close();
            }
        }
        return registros;
    }

    public int CrearRegistroContratos(RegistroContratos registroContratos)
    {
        int r = -1;
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            string sqlquery = "INSERT INTO registro_contratos (contrato_id, fecha_creacion, creado_por, fecha_cancelacion, cancelado_por) " +
                "VALUES (@contrato_id, @fecha_creacion, @creado_por, @fecha_cancelacion, @cancelado_por);";
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@contrato_id", registroContratos.Contrato_id);
                command.Parameters.AddWithValue("@fecha_creacion", registroContratos.Fecha_creacion);
                command.Parameters.AddWithValue("@creado_por", registroContratos.Creado_por);

                // Si los datos son NULL, asignar DBNull.Value
                command.Parameters.AddWithValue("@fecha_cancelacion",
                registroContratos.Fecha_cancelacion ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@cancelado_por",
                registroContratos.Cancelado_por ?? (object)DBNull.Value);
                connection.Open();
                r = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return r;
    }

    public int CrearCancelacionRegistro(RegistroContratos registroContratos)
    {
        int r = -1;
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            string sqlquery = @"UPDATE registro_contratos SET fecha_cancelacion=@fecha_cancelacion, cancelado_por=@cancelado_por
                            WHERE contrato_id = @contrato_id;";
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@contrato_id", registroContratos.Contrato_id);
                command.Parameters.AddWithValue("@fecha_creacion", registroContratos.Fecha_creacion);
                command.Parameters.AddWithValue("@creado_por", registroContratos.Creado_por);

                // Si los datos son NULL, asignar DBNull.Value
                command.Parameters.AddWithValue("@fecha_cancelacion",
                registroContratos.Fecha_cancelacion ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@cancelado_por",
                registroContratos.Cancelado_por ?? (object)DBNull.Value);
                connection.Open();
                r = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return r;
    }

    public RegistroContratos ObtenerRegistroContratosPorContrato(int contratoId)
    {
        RegistroContratos registro = new RegistroContratos();
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            string sqlquery = @"SELECT id, contrato_id, fecha_creacion, creado_por, fecha_cancelacion, cancelado_por FROM registro_contratos WHERE contrato_id = @contrato_id;";
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@contrato_id", contratoId);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        registro = new RegistroContratos
                        {
                            Id = reader.GetInt32("id"),
                            Contrato_id = reader.GetInt32("contrato_id"),
                            Fecha_creacion = reader.GetDateTime("fecha_creacion"),
                            Creado_por = reader.GetInt32("creado_por"),
                            Fecha_cancelacion = reader.IsDBNull(reader.GetOrdinal("fecha_cancelacion")) ? (DateTime?)null : reader.GetDateTime("fecha_cancelacion"),
                            Cancelado_por = reader.IsDBNull(reader.GetOrdinal("cancelado_por")) ? (int?)null : reader.GetInt32("cancelado_por")

                        };
                    }
                }
                connection.Close();
            }
            return registro;
        }
    }

    public int ObtenerCantRegistrosContratos()
    {
        int cant = 0;
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            string sqlquery = "SELECT COUNT(*) FROM registro_contratos;";
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                connection.Open();
                cant = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
            }
        }
        return cant;
    }
}