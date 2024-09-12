using MySql.Data.MySqlClient;
using rivarola_riquelme_rastrilla.Models;
namespace rivarola_riquelme_rastrilla.Controllers;
public class RepositorioInmueble
{
    string Conexion = "Server=localhost;User=root;Password=;Database=inmobiliaria;SslMode=none";
    public List<Inmueble> ObtenerInmueble()
    {
        List<Inmueble> inmueble = new List<Inmueble>();
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            var sqlquery = @"
                SELECT i.id, i.direccion, i.uso, i.tipo, i.ambientes, i.coordenadas, i.precio, i.propietario_dni, i.estado,
                       p.nombre, p.apellido
                FROM inmuebles i
                JOIN propietarios p ON i.propietario_dni = p.dni;
            ";
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    // Lee 'uso' como string y luego intenta convertirlo a enum
                    string usoString = reader.GetString(reader.GetOrdinal("uso"));
                    Inmueble.UsoInmueble usoInmueble = Inmueble.UsoInmueble.Residencial;

                    if (!Enum.TryParse(usoString, true, out usoInmueble))
                    {
                        Console.WriteLine($"Error al convertir el uso '{usoString}' al enum UsoInmueble.");
                        usoInmueble = Inmueble.UsoInmueble.Residencial;
                    }
                    inmueble.Add(new Inmueble
                    {
                        Id = reader.GetInt32("id"),
                        Direccion = reader.GetString("direccion"),
                        Uso = usoInmueble,
                        Tipo = reader.GetString("tipo"),
                        Ambientes = reader.GetInt32("ambientes"),
                        Coordenadas = reader.GetString("coordenadas"),
                        Precio = reader.GetDecimal("precio"),
                        Propietario_dni = reader.GetInt64("propietario_dni"),
                        Estado = reader.GetBoolean("estado"),
                        Propietario = new Propietario
                        {
                            Nombre = reader.GetString("nombre"),
                            Apellido = reader.GetString("apellido")
                        }
                    });

                }
                return inmueble;
            }
        }
    }

    public Inmueble? Obtener(int Id)
    {
        Inmueble? inmueble = null;
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            var sqlquery = @"SELECT id, direccion, uso, tipo, ambientes, coordenadas, precio, propietario_dni, estado FROM inmuebles WHERE id=@Id;";
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@Id", Id);
                // Se abre la conexión a la base de datos para permitir que el comando SQL se ejecute.
                connection.Open();
                // ExecuteReader ejecuta la consulta SQL y devuelve un MySqlDataReader, que se utiliza para leer los datos devueltos por la consulta.
                using (var reader = command.ExecuteReader())
                {
                    // El método reader.Read() lee la primera fila del resultado de la consulta(si existe).
                    if (reader.Read())
                    {
                        var usoString = reader.GetString("Uso");
                        Inmueble.UsoInmueble uso;
                        if (!Enum.TryParse(usoString, true, out uso))
                        {
                            // Manejar el caso en que el valor no se puede convertir
                            throw new ArgumentException($"El valor '{usoString}' no es válido para el enum UsoInmueble.");
                        }
                        // Si se encuentra un registro, se crea una nueva instancia de Inmueble y se le asignan los valores obtenidos de la base de datos
                        inmueble = new Inmueble
                        {
                            Id = reader.GetInt32("id"),
                            Direccion = reader.GetString("direccion"),
                            Uso = uso,
                            Tipo = reader.GetString("tipo"),
                            Ambientes = reader.GetInt32("ambientes"),
                            Coordenadas = reader.GetString("coordenadas"),
                            Precio = reader.GetDecimal("precio"),
                            Propietario_dni = reader.GetInt64("propietario_dni"),
                            Estado = reader.GetBoolean("estado"),
                        };
                    }
                }
                connection.Close();
            }

        }
        return inmueble;
    }


    public int AltaInmueble(Inmueble inmueble)
    {
        int r = -1;
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            var sqlquery = @"INSERT INTO inmuebles(direccion, uso, tipo, ambientes, coordenadas, precio, propietario_dni, estado) 
                            VALUES (@direccion, @uso, @tipo, @ambientes, @coordenadas, @precio, @propietario_dni, @estado);";
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@direccion", inmueble.Direccion);
                command.Parameters.AddWithValue("@uso", inmueble.Uso.ToString());
                command.Parameters.AddWithValue("@tipo", inmueble.Tipo);
                command.Parameters.AddWithValue("@ambientes", inmueble.Ambientes);
                command.Parameters.AddWithValue("@coordenadas", inmueble.Coordenadas);
                command.Parameters.AddWithValue("@precio", inmueble.Precio);
                command.Parameters.AddWithValue("@propietario_dni", inmueble.Propietario_dni);
                command.Parameters.AddWithValue("@estado", 1);
                connection.Open();
                r = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return r;
    }

    public int BajaInmueble(int Id)
    {
        int r = -1;
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            var sqlquery = @"UPDATE inmuebles SET estado=0 WHERE id=@Id;";
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

    public int ActivarInmueble(int Id)
    {
        int r = -1;
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            var sqlquery = @"UPDATE inmuebles SET estado=1 WHERE id=@Id;";
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
    public int GuardarInmueble(Inmueble inmueble)
    {
        int r = -1;
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            var sqlquery = @"UPDATE inmuebles SET direccion=@direccion, uso=@uso, tipo=@tipo, ambientes=@ambientes, coordenadas=@coordenadas, precio=@precio, propietario_dni=@propietario_dni WHERE id=@Id;";
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@Id", inmueble.Id);
                command.Parameters.AddWithValue("@direccion", inmueble.Direccion);
                command.Parameters.AddWithValue("@uso", inmueble.Uso.ToString());
                command.Parameters.AddWithValue("@tipo", inmueble.Tipo);
                command.Parameters.AddWithValue("@ambientes", inmueble.Ambientes);
                command.Parameters.AddWithValue("@coordenadas", inmueble.Coordenadas);
                command.Parameters.AddWithValue("@precio", inmueble.Precio);
                command.Parameters.AddWithValue("@propietario_dni", inmueble.Propietario_dni);
                connection.Open();
                r = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return r;
    }
}
