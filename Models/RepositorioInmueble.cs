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
                SELECT i.id, i.direccion, i.uso, i.tipo_id, i.ambientes, i.coordenadas, i.precio, i.propietario_dni, i.estado,
                    t.nombre AS tipo_nombre,
                    p.nombre, p.apellido
                FROM inmuebles i
                JOIN tipos t ON i.tipo_id = t.id
                JOIN propietarios p ON i.propietario_dni = p.dni;
            ";
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string usoString = reader.GetString(reader.GetOrdinal("uso"));
                    Inmueble.UsoInmueble usoInmueble = Inmueble.UsoInmueble.Residencial;

                    if (!Enum.TryParse(usoString, true, out usoInmueble))
                    {
                        usoInmueble = Inmueble.UsoInmueble.Residencial;
                    }

                    inmueble.Add(new Inmueble
                    {
                        Id = reader.GetInt32("id"),
                        Direccion = reader.GetString("direccion"),
                        Uso = usoInmueble,
                        Tipo = new Tipo { Id = reader.GetInt32("tipo_id"), Nombre = reader.GetString("tipo_nombre") },
                        Ambientes = reader.GetInt32("ambientes"),
                        Coordenadas = reader.GetString("coordenadas"),
                        Precio = reader.GetDecimal("precio"),
                        Propietario_dni = reader.GetInt64("propietario_dni"),
                        Estado = reader.GetBoolean("estado"),
                        Propietario = new Propietarios
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
            var sqlquery = @"
                SELECT i.id, i.direccion, i.uso, i.tipo_id, t.nombre AS tipo_nombre, i.ambientes, 
                    i.coordenadas, i.precio, i.propietario_dni, i.estado
                FROM inmuebles i
                JOIN tipos t ON i.tipo_id = t.id
                WHERE i.id = @Id;
            ";
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@Id", Id);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var usoString = reader.GetString("uso");
                        Inmueble.UsoInmueble uso;
                        if (!Enum.TryParse(usoString, true, out uso))
                        {
                            throw new ArgumentException($"El valor '{usoString}' no es válido para el enum UsoInmueble.");
                        }

                        inmueble = new Inmueble
                        {
                            Id = reader.GetInt32("id"),
                            Direccion = reader.GetString("direccion"),
                            Uso = uso,
                            Tipo = new Tipo { Id = reader.GetInt32("tipo_id"), Nombre = reader.GetString("tipo_nombre") },
                            Ambientes = reader.GetInt32("ambientes"),
                            Coordenadas = reader.GetString("coordenadas"),
                            Precio = reader.GetDecimal("precio"),
                            Propietario_dni = reader.GetInt64("propietario_dni"),
                            Estado = reader.GetBoolean("estado")
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
            var sqlquery = @"INSERT INTO inmuebles (direccion, uso, tipo_id, ambientes, coordenadas, precio, propietario_dni, estado) 
                            VALUES (@direccion, @uso, @tipo_id, @ambientes, @coordenadas, @precio, @propietario_dni, @estado);";
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@direccion", inmueble.Direccion);
                command.Parameters.AddWithValue("@uso", inmueble.Uso.ToString());
                command.Parameters.AddWithValue("@tipo_id", inmueble.Tipo.Id);  // Aquí se inserta el ID de Tipo
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
            var sqlquery = @"UPDATE inmuebles 
                            SET direccion=@direccion, uso=@uso, tipo_id=@tipo_id, ambientes=@ambientes, 
                                coordenadas=@coordenadas, precio=@precio, propietario_dni=@propietario_dni 
                            WHERE id=@Id;";
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@Id", inmueble.Id);
                command.Parameters.AddWithValue("@direccion", inmueble.Direccion);
                command.Parameters.AddWithValue("@uso", inmueble.Uso.ToString());
                command.Parameters.AddWithValue("@tipo_id", inmueble.Tipo.Id);  // Se actualiza el tipo por su ID
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

    public List<Inmueble> ObtenerInmueblesPorPropietario(int propietarioDni)
    {
        var inmuebles = new List<Inmueble>();
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            connection.Open();
            var sqlquery = @"
                SELECT i.id, i.direccion, i.uso, i.tipo_id, t.nombre AS tipo_nombre, i.ambientes, 
                    i.coordenadas, i.precio, i.propietario_dni, i.estado
                FROM inmuebles i
                JOIN tipos t ON i.tipo_id = t.id
                WHERE i.propietario_dni = @PropietarioDni;
            ";
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@PropietarioDni", propietarioDni);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string usoString = reader.GetString("uso");
                        Inmueble.UsoInmueble usoInmueble;
                        if (!Enum.TryParse(usoString, true, out usoInmueble))
                        {
                            usoInmueble = Inmueble.UsoInmueble.Residencial;
                        }

                        inmuebles.Add(new Inmueble
                        {
                            Id = reader.GetInt32("id"),
                            Direccion = reader.GetString("direccion"),
                            Uso = usoInmueble,
                            Tipo = new Tipo { Id = reader.GetInt32("tipo_id"), Nombre = reader.GetString("tipo_nombre") },
                            Ambientes = reader.GetInt32("ambientes"),
                            Coordenadas = reader.GetString("coordenadas"),
                            Precio = reader.GetDecimal("precio"),
                            Propietario_dni = reader.GetInt64("propietario_dni"),
                            Estado = reader.GetBoolean("estado")
                        });
                    }
                }
            }
        }
        return inmuebles;
    }

    
}
