using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Security.Claims;
namespace rivarola_riquelme_rastrilla.Models;
public class RepositorioUsuarios
{
    string Conexion = "Server=localhost;User=root;Password=;Database=inmobiliaria;SslMode=none";

    public List<Usuarios> ObtenerUsuarios()
    {
        List<Usuarios> usuarios = new List<Usuarios>();
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            var sqlquery = $@"SELECT {nameof(Usuarios.Id)},{nameof(Usuarios.Nombre)},{nameof(Usuarios.Apellido)},{nameof(Usuarios.Email)},{nameof(Usuarios.Contrasenia)},{nameof(Usuarios.Rol)},{nameof(Usuarios.Avatar)} FROM Usuarios;";
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
                        Rol = reader.GetString("Rol"),
                        Avatar = reader.GetString("Avatar"),
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
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            var sqlquery = @"SELECT Id, Nombre, Apellido, Email, Rol, Contrasenia, Avatar FROM usuarios WHERE Email = @Email;";
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
                            Avatar = reader.GetString("Avatar"),
                        };
                    }
                };
                connection.Close();
            }
        }
        return usuario;
    }

    public Usuarios? ObtenerById(int Id)
    {
        Usuarios? usuario = null;
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            var sqlquery = @"SELECT Id, Nombre, Apellido, Email, Rol, Contrasenia FROM usuarios WHERE Id = @Id;";
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@Id", Id);
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

    public int Crear(Usuarios usuario)
    {
        int r = 0;
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

            string defaultAvatarPath = "/Uploads/user_pic.jpg";
            usuario.Avatar = usuario.Avatar ?? defaultAvatarPath;

            var sqlquery = @"INSERT INTO usuarios(Nombre,Apellido,Email,Contrasenia, Rol, Avatar)
            VALUES (@Nombre,@Apellido,@Email,@Contrasenia, 'Empleado',@Avatar);";
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                command.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                command.Parameters.AddWithValue("@Email", usuario.Email);
                command.Parameters.AddWithValue("@Contrasenia", hashed);
                command.Parameters.AddWithValue("@Avatar", usuario.Avatar);
                connection.Open();
                r = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return r;
    }

    public int Editar(Usuarios usuario)
    {
        int r = -1;
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            var sqlquery = @"UPDATE usuarios SET Nombre=@Nombre, Apellido=@Apellido, Rol=@Rol, Email=@Email
            WHERE Id = @Id;";
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                command.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                command.Parameters.AddWithValue("@Rol", usuario.Rol);
                command.Parameters.AddWithValue("@Email", usuario.Email);
                command.Parameters.AddWithValue("@Id", usuario.Id);
                connection.Open();
                r = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return r;
    }

    public int EditarContrasenia(Usuarios usuario)
    {
        int r = -1;
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            var sqlquery = @"UPDATE usuarios SET Contrasenia=@Contrasenia
            WHERE Id = @Id;";
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@Contrasenia", usuario.Contrasenia);
                command.Parameters.AddWithValue("@Id", usuario.Id);
                connection.Open();
                r = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return r;
    }

    public int EditarAvatar(Usuarios usuario)
    {
        int r = -1;
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            var sqlquery = @"UPDATE usuarios SET Avatar=@Avatar
            WHERE Id = @Id;";
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@Avatar", usuario.Avatar);
                command.Parameters.AddWithValue("@Id", usuario.Id);
                connection.Open();
                r = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return r;
    }

    public int Borrar(int id)
    {
        int r = -1;
        using (MySqlConnection connection = new MySqlConnection(Conexion))
        {
            var sqlquery = @"DELETE FROM usuarios WHERE Id=@id;";
            using (MySqlCommand command = new MySqlCommand(sqlquery, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                r = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return r;
    }

}