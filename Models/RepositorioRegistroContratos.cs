using MySql.Data.MySqlClient;
using rivarola_riquelme_rastrilla.Models;

namespace rivarola_riquelme_rastrilla.Controllers;

public class RepositorioRegistroContratos
{
    string Conexion = "Server=localhost;User=root;Password=;Database=inmobiliaria;SslMode=none";

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
                            Fecha_cancelacion = reader.GetDateTime("fecha_cancelacion"),
                            Cancelado_por = reader.GetInt32("cancelado_por")
                        };
                    }
                }
                connection.Close();
            }
            return registro;
        }
    }
}