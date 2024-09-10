using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Security.Claims;
namespace rivarola_riquelme_rastrilla.Models;
public class RepositorioUsuarios
{
    //Seteo la cadena de conexión
    string Conexion = "Server=localhost;User=root;Password=;Database=inmobiliaria;SslMode=none";

    //Obtener toda la lista de inquilinos
    public List<Usuarios> ObtenerUsuarios()
    {
        //Lista a la que se le van a agregar los inquilinos
        List<Usuarios> usuarios = new List<Usuarios>();
        //conexion a la bd
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            //query 
            var sqlquery = $@"SELECT {nameof(Usuarios.Id)},{nameof(Usuarios.Nombre)},{nameof(Usuarios.Apellido)},{nameof(Usuarios.Email)},{nameof(Usuarios.Contrasenia)},{nameof(Usuarios.Avatar)} FROM Usuarios;";
            //Comando para ejecutar la query
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    usuarios.Add(new Usuarios
                    {
                        Id = reader.GetInt32("Id"),
                        Nombre = reader.GetString("Nombre"),
                        Apellido = reader.GetString("Apellido"),
                        Email = reader.GetString("Email"),
                        Contrasenia = reader.GetString("Contrasenia"),
                        //Avatar
                    });
                }
                connection.Close();
            }
            return usuarios;
        }
    }

    //Obtener solamente un inquilino por Dni
    public Usuarios? ObtenerByEmail(string Email)
    {
        Usuarios? usuario = null;
        //conexion a la bd
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            //query 
            var sqlquery = @"SELECT Id, Nombre, Apellido, Email, Rol, Contrasenia FROM usuarios WHERE Email = @Email;";
            //Comando para ejecutar la query
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@Email", Email);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        usuario = new Usuarios
                        {
                            Id = reader.GetInt32("Id"),
                            Nombre = reader.GetString("Nombre"),
                            Apellido = reader.GetString("Apellido"),
                            Email = reader.GetString("Email"),
                            Rol = reader.GetString("Rol"),
                            Contrasenia = reader.GetString("Contrasenia"),
                        };
                    }
                };
                connection.Close();
            }
        }
        return usuario;
    }

    //Dar de alta un inquilino
    public int Crear(Usuarios usuario)
    {
        //variable que indica si se realizó o no el cambio
        int r = 0;
        //conexion a la bd
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            byte[] salt = new byte[10];
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: usuario.Contrasenia,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            ));
            Console.WriteLine("CONTRASEÑA HASHED:"+hashed);
            //query 
            var sqlquery = @"INSERT INTO usuarios(Nombre,Apellido,Email,Contrasenia, Rol)
            VALUES (@Nombre,@Apellido,@Email,@Contrasenia, 'Empleado');";
            //Comando para ejecutar la query
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                command.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                command.Parameters.AddWithValue("@Email", usuario.Email);
                command.Parameters.AddWithValue("@Contrasenia", hashed);
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