using MySql.Data.MySqlClient;

namespace rivarola_riquelme_rastrilla.Models
{
    public class RepositorioPago
    {
        string ConnectionString = "Server=localhost;Database=inmobiliaria;User=root;SslMode=none";

        public List<Pago> ObtenerTodos()
        {
            List<Pago> pagos = new List<Pago>();

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                var query = $@"SELECT {nameof(Pago.Id)} AS id, {nameof(Pago.Contrato_id)} AS contrato_id, 
                              fecha_pago AS {nameof(Pago.Fecha_pago)}, {nameof(Pago.Detalle)} AS detalle, 
                              {nameof(Pago.Importe)} AS importe FROM pagos";

                using (var command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        pagos.Add(new Pago
                        {
                            Id = reader.GetInt32(nameof(Pago.Id)),
                            Contrato_id = reader.IsDBNull(reader.GetOrdinal(nameof(Pago.Contrato_id))) ? null : reader.GetInt32(nameof(Pago.Contrato_id)),
                            Fecha_pago = reader.IsDBNull(reader.GetOrdinal(nameof(Pago.Fecha_pago))) ? null : reader.GetDateTime(nameof(Pago.Fecha_pago)),
                            Detalle = reader.IsDBNull(reader.GetOrdinal(nameof(Pago.Detalle))) ? null : reader.GetString(nameof(Pago.Detalle)),
                            Importe = reader.IsDBNull(reader.GetOrdinal(nameof(Pago.Importe))) ? null : reader.GetDecimal(nameof(Pago.Importe))
                        });
                    }
                    connection.Close();
                }
                return pagos;
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

        public void Agregar(Pago pago)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                var query = $@"INSERT INTO pagos ({nameof(Pago.Contrato_id)}, fecha_pago, 
                                  {nameof(Pago.Detalle)}, {nameof(Pago.Importe)}) 
                                  VALUES (@{nameof(Pago.Contrato_id)}, @{nameof(Pago.Fecha_pago)}, 
                                  @{nameof(Pago.Detalle)}, @{nameof(Pago.Importe)})";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue($"@{nameof(Pago.Contrato_id)}", pago.Contrato_id);
                    command.Parameters.AddWithValue($"@{nameof(Pago.Fecha_pago)}", pago.Fecha_pago);
                    command.Parameters.AddWithValue($"@{nameof(Pago.Detalle)}", pago.Detalle);
                    command.Parameters.AddWithValue($"@{nameof(Pago.Importe)}", pago.Importe);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
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
    }
}
