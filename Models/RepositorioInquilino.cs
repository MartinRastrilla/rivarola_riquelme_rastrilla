using MySql.Data.MySqlClient;
namespace rivarola_riquelme_rastrilla.Models;
public class RepositorioInquilino
{
    //Seteo la cadena de conexión
    string Conexion = "Server=localhost;User=root;Password=;Database=bdinmobiliaria;SslMode=none";

    //Obtener toda la lista de inquilinos
    public List<Inquilino> ObtenerInquilinos()
    {
        //Lista a la que se le van a agregar los inquilinos
        List<Inquilino> inquilinos = new List<Inquilino>();
        //conexion a la bd
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            //query 
            var sqlquery = @"SELECT Dni, Nombre, Apellido, Telefono, Email FROM inquilinos;";
            //Comando para ejecutar la query
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    inquilinos.Add(new Inquilino
                    {
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

    //Obtener solamente un inquilino por Dni
    public Inquilino? Obtener(long Dni)
    {
        //variable a la que se le va almacenar el inquilino obtenido
        Inquilino? inquilino = null;
        //conexion a la bd
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            //query 
            var sqlquery = @"SELECT Dni, Nombre, Apellido, Telefono, Email FROM inquilinos WHERE Dni=@Dni;";
            //Comando para ejecutar la query
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@Dni", Dni);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        inquilino = new Inquilino
                        {
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
            var sqlquery = @"UPDATE inquilinos SET Nombre=@Nombre, Apellido=@Apellido, Telefono=@Telefono, Email=@Email WHERE Dni=@Dni;";
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