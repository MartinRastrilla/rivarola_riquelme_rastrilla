using MySql.Data.MySqlClient;

namespace rivarola_riquelme_rastrilla.Models;


public class RepositorioPagos
{
    string ConnectionString = "Server=localhost;Database=inmobiliaria;User=root;SslMode=none";

    public List<Pagos> ObtenerTodos()
    {

        List<Pagos> pagos = new List<Pagos>();

        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = $@"SELECT {nameof(Pagos.Id)}, {nameof(Pagos.ContratoId)}, {nameof(Pagos.Detalle)}, {nameof(Pagos.Importe)}, {nameof(Pagos.Fecha_pago)} FROM pagos";

            using (var command = new MySqlCommand(query, connection))
            {
                connection.Open();

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    pagos.Add(new Pagos
                    {
                        Id = reader.GetInt32("Id"),
                        ContratoId = reader.GetInt32("ContratoId"),
                        Detalle = reader.GetString("Detalle"),
                        Importe = reader.GetFloat("Importe"),
                        Fecha_pago = reader.GetDateTime("Fecha_pago")
                    });
                }
                connection.Close();
            }

            return pagos;
        }
    }

    public Pagos ObtenerPorId(int id)
    {
        Pagos pago = null;

        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = $@"SELECT {nameof(Pagos.Id)}, {nameof(Pagos.ContratoId)}, {nameof(Pagos.Detalle)}, {nameof(Pagos.Importe)}, {nameof(Pagos.Fecha_pago)} FROM pagos WHERE Id = @id";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();

                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    pago = new Pagos
                    {
                        Id = reader.GetInt32("Id"),
                        ContratoId = reader.GetInt32("ContratoId"),
                        Detalle = reader.GetString("Detalle"),
                        Importe = reader.GetFloat("Importe"),
                        Fecha_pago = reader.GetDateTime("Fecha_pago")
                    };
                }

                connection.Close();
            }
            return pago;
        }
    }

    public void Crear(Pagos pago)
    {

        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = $@"INSERT INTO pagos ({nameof(Pagos.ContratoId)}, {nameof(Pagos.Fecha_pago)}, {nameof(Pagos.Detalle)}, {nameof(Pagos.Importe)})
                           VALUES (@ContratoId, @Detalle, @Importe)";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@contratoId", pago.ContratoId);
                command.Parameters.AddWithValue("@detalle", pago.Detalle);
                command.Parameters.AddWithValue("@importe", pago.Importe);
                command.Parameters.AddWithValue("@fecha_pago", pago.Fecha_pago);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    public void Editar(Pagos pago)
    {

        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = $@"UPDATE pagos SET {nameof(Pagos.ContratoId)} = @ContratoId, {nameof(Pagos.Detalle)} = @Detalle, {nameof(Pagos.Importe)} = @Importe, {nameof(Pagos.Fecha_pago)} = @Fecha_pago
                           WHERE {nameof(Pagos.Id)} = @Id";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", pago.Id);
                command.Parameters.AddWithValue("@contratoId", pago.ContratoId);
                command.Parameters.AddWithValue("@detalle", pago.Detalle);
                command.Parameters.AddWithValue("@fecha_pago", pago.Fecha_pago);
                command.Parameters.AddWithValue("@importe", pago.Importe);
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
            var query = $@"DELETE FROM pagos WHERE {nameof(Pagos.Id)} = @id";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}
