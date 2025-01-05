using MySql.Data.MySqlClient;
namespace rivarola_riquelme_rastrilla.Models;
public class RepositorioInquilino
{
    //Seteo la cadena de conexión
    string Conexion = "Server=localhost;User=root;Password=;Database=inmobiliaria;SslMode=none";

    //Obtener toda la lista de inquilinos
    public List<Inquilino> ObtenerInquilinos()
    {
        //Lista a la que se le van a agregar los inquilinos
        List<Inquilino> inquilinos = new List<Inquilino>();
        //conexion a la bd
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            //query 
            var sqlquery = @"SELECT Id, Dni, Nombre, Apellido, Telefono, Email FROM inquilinos;";
            //Comando para ejecutar la query
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    inquilinos.Add(new Inquilino
                    {
                        Id = reader.GetInt64("Id"),
                        Dni = reader.GetInt64("Dni"),
                        Nombre = reader.GetString("Nombre"),
                        Apellido = reader.GetString("Apellido"),
                        Telefono = reader.GetInt64("Telefono"),
                        Email = reader.GetString("Email"),
                    });
                }
                connection.Close();
            }
            return inquilinos;
        }
    }

    public int ObtenerTotalInquilinos()
    {
        int total = 0;
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            var sqlquery = @"SELECT COUNT(*) FROM inquilinos;";
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                connection.Open();
                total = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
            }
        }
        return total;
    }

    public List<Inquilino> ObtenerPaginado(int page, int pageSize) {
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            connection.Open();
            var sqlquery = @"SELECT * FROM inquilinos LIMIT @Offset, @PageSize;";
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
                command.Parameters.AddWithValue("@PageSize", pageSize);

                var inquilinos = new List<Inquilino>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        inquilinos.Add(new Inquilino
                        {
                            Id = reader.GetInt64("Id"),
                            Dni = reader.GetInt64("Dni"),
                            Nombre = reader.GetString("Nombre"),
                            Apellido = reader.GetString("Apellido"),
                            Telefono = reader.GetInt64("Telefono"),
                            Email = reader.GetString("Email"),
                        });
                    }
                }
                return inquilinos;
            }
        }
    }

    //Obtener solamente un inquilino por Dni
    public Inquilino? Obtener(long? Dni = null, long? Id = null)
    {
        if (Dni == null && Id == null) {
            throw new ArgumentException("Debe ingresar un Dni o un Id");
        }

        //variable a la que se le va almacenar el inquilino obtenido
        Inquilino? inquilino = null;
        //conexion a la bd
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            //query 
            var sqlquery = Dni != null
                ? @"SELECT Id, Dni, Nombre, Apellido, Telefono, Email FROM inquilinos WHERE Dni = @Dni;"
                : @"SELECT Id, Dni, Nombre, Apellido, Telefono, Email FROM inquilinos WHERE Id = @Id;";
            //Comando para ejecutar la query
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                if (Dni != null)
                {
                    command.Parameters.AddWithValue("@Dni", Dni);
                }
                else if (Id != null)
                {
                    command.Parameters.AddWithValue("@Id", Id);
                }
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        inquilino = new Inquilino
                        {
                            Id = reader.GetInt64("Id"),
                            Dni = reader.GetInt64("Dni"),
                            Nombre = reader.GetString("Nombre"),
                            Apellido = reader.GetString("Apellido"),
                            Telefono = reader.GetInt64("Telefono"),
                            Email = reader.GetString("Email"),
                        };
                    }
                };
                connection.Close();
            }
        }
        return inquilino;
    }

    //Dar de alta un inquilino
    public int AltaInquilino(Inquilino inquilino)
    {
        //variable que indica si se realizó o no el cambio
        int r = 0;
        //conexion a la bd
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            //query 
            var sqlquery = @"INSERT INTO inquilinos(Dni,Nombre,Apellido,Telefono,Email)
            VALUES (@Dni,@Nombre,@Apellido,@Telefono,@Email);";
            //Comando para ejecutar la query
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@Dni", inquilino.Dni);
                command.Parameters.AddWithValue("@Nombre", inquilino.Nombre);
                command.Parameters.AddWithValue("@Apellido", inquilino.Apellido);
                command.Parameters.AddWithValue("@Telefono", inquilino.Telefono);
                command.Parameters.AddWithValue("@Email", inquilino.Email);
                connection.Open();
                r = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return r;
    }

    //Editar un Inquilin
    public int EditarInquilino(Inquilino inquilino)
    {
        //variable que indica si se realizó o no el cambio
        int r = -1;
        //conexion a la bd
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            //query 
            var sqlquery = @"UPDATE inquilinos SET Dni=@Dni, Nombre=@Nombre, Apellido=@Apellido, Telefono=@Telefono, Email=@Email WHERE Id=@Id;";
            //Comando para ejecutar la query
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@Id", inquilino.Id);
                command.Parameters.AddWithValue("@Dni", inquilino.Dni);
                command.Parameters.AddWithValue("@Nombre", inquilino.Nombre);
                command.Parameters.AddWithValue("@Apellido", inquilino.Apellido);
                command.Parameters.AddWithValue("@Telefono", inquilino.Telefono);
                command.Parameters.AddWithValue("@Email", inquilino.Email);
                connection.Open();
                r = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return r;
    }

    //Eliminar un Inquilino por Dni
    public int BorrarInquilino(long Dni)
    {
        //variable que indica si se realizó o no el cambio
        int r = -1;
        //conexion a la bd
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            //query
            var sqlquery = @"DELETE FROM inquilinos WHERE Dni=@Dni;";
            //Comando para ejecutar la query
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@Dni", Dni);
                connection.Open();
                r = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return r;
    }

}