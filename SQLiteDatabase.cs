using System;
using System.Data.OleDb;
using System.Data;
using System.IO;
using System.Windows;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using SistemaFichajeWPF;

namespace SistemaFichaje
{
    public static class SQLiteDatabase
    {
        private static string dbFilePath = GlobalData.BuildDir + GlobalData.ArchivoBD;
        private static string dbConnectionString = $"Data Source={dbFilePath};Version=3;";

        private static SQLiteConnection dbConnection = new SQLiteConnection(dbConnectionString);

        private static readonly string TablaTarjeta = "Tarjeta";
        private static readonly string TablaCalendario = "CalendarioLaboral";
        
        /// <summary>
        /// Checks if the database exists; if not, creates the file and the schema.
        /// </summary>
        public static void InicializarBaseDeDatos()
        {
            if (File.Exists(dbFilePath))
            {
                return;
            }
            
            // SQLite automatically creates the file when you open a connection 
            // if it doesn't exist, but we need to build the tables.
            SQLiteConnection.CreateFile(dbFilePath);
            CrearSchema();
        }
        
        // Recoge los datos de la lista de personal de la empresa
        #region LeerDatosPersonal
        public static ObservableCollection<Personal> LeerDatosPersonal()
        {
            var listaPersonal = new ObservableCollection<Personal>();

            var selectCmd = new SQLiteCommand
            {
                CommandType = CommandType.Text,
                CommandText = "SELECT * FROM Personal",
                Connection = dbConnection
            };

            try
            {
                if (dbConnection.State != ConnectionState.Open)
                    dbConnection.Open();

                var dataReader = selectCmd.ExecuteReader();

                while (dataReader.Read())
                {
                    listaPersonal.Add(
                        new Personal(Convert.ToInt32(dataReader.GetValue(0)),
                            dataReader.GetValue(1).ToString())
                        );
                }

                dbConnection.Close();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Ha habido un problema en la escritura a la base de datos. " +
                    "No se ha podido insertar la nueva información.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
#if Debug
                GlobalData.PrintDebug("ERROR ACCESS", "Mensaje: " + ex.Message);
                GlobalData.PrintDebug("ERROR ACCESS", "StackTrace: " + ex.StackTrace);
                GlobalData.PrintDebug("ERROR ACCESS", "Código error: " + ex.ErrorCode);
#endif
            }

            return listaPersonal;
        }
#endregion

        // Devuelve el nombre del personal con el ID de tarjeta indicado
        #region NombreDePersonalConIDTarjeta
        public static string NombreDePersonalConIDTarjeta(long tarjetaId)
        {
            var nombre = "";

            var selectPersonalIdCmd = new SQLiteCommand
            {
                CommandText = $"SELECT PersonalId FROM Tarjeta WHERE TarjetaId LIKE '{tarjetaId.ToString()}'",
                Connection = dbConnection
            };

            var selectNombreCmd = new SQLiteCommand
            {
                CommandText = "SELECT PersonalNombre FROM Personal WHERE PersonalId LIKE X",
                Connection = dbConnection
            };

            try
            {
                if (dbConnection.State != ConnectionState.Open)
                    dbConnection.Open();

                var dataReaderPersonalId = selectPersonalIdCmd.ExecuteReader();

                if (dataReaderPersonalId.HasRows)
                {
                    while (dataReaderPersonalId.Read())
                    {
                        selectNombreCmd.CommandText = selectNombreCmd.CommandText.Replace("X", dataReaderPersonalId.GetValue(0).ToString());
                    }
                }
                else
                {
                    return "null";
                }                

                var dataReaderNombre = selectNombreCmd.ExecuteReader();

                if (dataReaderNombre.HasRows)
                {
                    while (dataReaderNombre.Read())
                    {
                        nombre = dataReaderNombre.GetValue(0).ToString();
                    }
                }
                else
                {
                    return "null";
                }

                dbConnection.Close();

                return nombre;
            }
            catch (OleDbException ex)
            {
                MessageBox.Show("Ha habido un problema con la base de datos. ",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Warning);

#if Debug
                GlobalData.PrintDebug("ERROR ACCESS", "Mensaje: " + ex.Message);
                GlobalData.PrintDebug("ERROR ACCESS", "StackTrace: " + ex.StackTrace);
#endif

                return null;
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show("Ha habido un problema en la escritura a la base de datos. ",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Warning);

#if Debug
                GlobalData.PrintDebug("ERROR ACCESS", "Mensaje: " + ex.Message);
                GlobalData.PrintDebug("ERROR ACCESS", "StackTrace: " + ex.StackTrace);
#endif

                return null;
            }
        }
        #endregion

        // Devuelve el nombre del personal con el ID de personal indicado
        #region NombreDePersonalConIDPersonal
        public static string NombreDePersonalConIDPersonal(int personalId)
        {
            var nombre = "";

            var selectNombreCmd = new SQLiteCommand
            {
                CommandText = String.Format("SELECT PersonalNombre FROM Personal WHERE PersonalId LIKE {0}", personalId),
                Connection = dbConnection
            };

            try
            {
                if (dbConnection.State != ConnectionState.Open)
                    dbConnection.Open();

                var dataReaderNombre = selectNombreCmd.ExecuteReader();

                if (dataReaderNombre.HasRows)
                {
                    while (dataReaderNombre.Read())
                    {
                        nombre = dataReaderNombre.GetValue(0).ToString();
                    }
                }
                else
                {
                    return "null";
                }

                dbConnection.Close();

                return nombre;
            }
            catch (OleDbException ex)
            {
                MessageBox.Show("Ha habido un problema con la base de datos. ",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Warning);

#if Debug
                GlobalData.PrintDebug("ERROR ACCESS", "Mensaje: " + ex.Message);
                GlobalData.PrintDebug("ERROR ACCESS", "StackTrace: " + ex.StackTrace);
#endif

                return null;
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show("Ha habido un problema en la escritura a la base de datos. ",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Warning);

#if Debug
                GlobalData.PrintDebug("ERROR ACCESS", "Mensaje: " + ex.Message);
                GlobalData.PrintDebug("ERROR ACCESS", "StackTrace: " + ex.StackTrace);
#endif

                return null;
            }
        }
        #endregion

        // Recoge los datos de la lista de tarjetas vinculadas
        #region LeerDatosTarjetas
        public static ObservableCollection<Tarjeta> LeerDatosTarjetas()
        {
            var listaTarjetas = new ObservableCollection<Tarjeta>();

            var selectTarjetasCmd = new SQLiteCommand
            {
                CommandType = CommandType.Text,
                CommandText = "SELECT * FROM Tarjeta",
                Connection = dbConnection
            };

            var selectPersonalCmd = new SQLiteCommand
            {
                CommandType = CommandType.Text,
                CommandText = "SELECT * FROM Personal",
                Connection = dbConnection
            };

            try
            {
                if (dbConnection.State != ConnectionState.Open)
                    dbConnection.Open();

                // Lee los resultados de la tabla Tarjetas y crea
                // la lista, sin los nombres

                var dataReader = selectTarjetasCmd.ExecuteReader();

                while (dataReader.Read())
                {
                    listaTarjetas.Add(
                        new Tarjeta(
                            dataReader.GetValue(0).ToString(), 
                            new Personal((int)dataReader.GetValue(1))
                            )                        
                        );
                }

                // Lee la tabla de Personal y añade los nombres
                // a la lista de tarjetas

                dataReader = selectPersonalCmd.ExecuteReader();

                while (dataReader.Read())
                {
                    foreach (var tarjeta in listaTarjetas)
                    {
                        if (tarjeta.Personal.Id == (int)dataReader.GetValue(0))
                        {
                            tarjeta.Personal.Nombre = dataReader.GetValue(1).ToString();
                        }
                    }
                }

                dbConnection.Close();
            }
            catch (OleDbException ex)
            {
                MessageBox.Show("Ha habido un problema en la escritura a la base de datos. " +
                    "No se ha podido insertar la nueva información.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

#if Debug
                GlobalData.PrintDebug("ERROR ACCESS", "Mensaje: " + ex.Message);
                GlobalData.PrintDebug("ERROR ACCESS", "StackTrace: " + ex.StackTrace);
                GlobalData.PrintDebug("ERROR ACCESS", "Código error: " + ex.ErrorCode);
#endif
            }

            return listaTarjetas;
        }
        #endregion

        // Comprueba si una tarjeta en concreto está vinculada ya
        #region ExisteTarjeta
        public static bool ExisteTarjeta(long tarjeta)
        {
            var selectTarjetasCmd = new SQLiteCommand
            {
                CommandType = CommandType.Text,
                CommandText = String.Format("SELECT * FROM {0} WHERE TarjetaId LIKE '{1}'", TablaTarjeta, tarjeta),
                Connection = dbConnection
            };

            try
            {
                if (dbConnection.State != ConnectionState.Open)
                    dbConnection.Open();

                // Lee los resultados de la tabla Tarjetas y crea
                // la lista, sin los nombres

                var dataReader = selectTarjetasCmd.ExecuteReader();

                if (dataReader.HasRows)
                {
                    dbConnection.Close();
                    return true;
                }
                else
                {
                    dbConnection.Close();
                    return false;
                }
            }
            catch(OleDbException)
            {
                return false;
            }
            catch(InvalidOperationException)
            {
                return false;
            }
        }
        #endregion

        // Recoge los datos de la lista de dias festivos
        #region LeerDatosCalendario
        public static ObservableCollection<FechaFestivo> LeerDatosCalendario()
        {
            var listaDiasFestivos = new ObservableCollection<FechaFestivo>();

            var selectCmd = new SQLiteCommand
            {
                CommandType = CommandType.Text,
                CommandText = String.Format("SELECT * FROM {0}", TablaCalendario),
                Connection = dbConnection
            };

            try
            {
                if (dbConnection.State != ConnectionState.Open)
                    dbConnection.Open();

                var dataReader = selectCmd.ExecuteReader();

                while (dataReader.Read())
                {
                    listaDiasFestivos.Add(new FechaFestivo(dataReader.GetDateTime(0).Date));
                }

                dbConnection.Close();
            }
            catch (OleDbException ex)
            {
                MessageBox.Show("Ha habido un problema en la escritura a la base de datos. " +
                    "No se ha podido insertar la nueva información.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

#if Debug
                GlobalData.PrintDebug("ERROR ACCESS", "Mensaje: " + ex.Message);
                GlobalData.PrintDebug("ERROR ACCESS", "StackTrace: " + ex.StackTrace);
                GlobalData.PrintDebug("ERROR ACCESS", "Código error: " + ex.ErrorCode);
#endif
            }

            return listaDiasFestivos;
        }
        #endregion
        
        // Elimina un usuario del personal
        #region EliminaPersonal
        public static bool EliminaPersonal(int personalId)
        {
            var deleteCmd = new SQLiteCommand
            {
                CommandText = String.Format("DELETE FROM Personal WHERE PersonalId = {0}", personalId),
                Connection = dbConnection
            };

            try
            {
                if (dbConnection.State != ConnectionState.Open)
                    dbConnection.Open();                

                deleteCmd.ExecuteNonQuery();

                dbConnection.Close();

                return true;
            }
            catch (OleDbException ex)
            {
                MessageBox.Show("Ha habido un problema en la escritura a la base de datos. ", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Warning);

#if Debug
                GlobalData.PrintDebug("ERROR ACCESS", "Mensaje: " + ex.Message);
                GlobalData.PrintDebug("ERROR ACCESS", "StackTrace: " + ex.StackTrace);
#endif

                return false;
            }     
            catch (InvalidOperationException ex)
            {
                MessageBox.Show("Ha habido un problema en la escritura a la base de datos. ",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Warning);

#if Debug
                GlobalData.PrintDebug("ERROR ACCESS", "Mensaje: " + ex.Message);
                GlobalData.PrintDebug("ERROR ACCESS", "StackTrace: " + ex.StackTrace);
#endif

                return false;
            }
        }
        #endregion

        // Desvincula una tarjeta
        #region DesvinculaTarjeta
        public static bool DesvinculaTarjeta(string tarjetaId)
        {
            try
            {
                if (dbConnection.State != ConnectionState.Open)
                    dbConnection.Open();

                var deleteCmd = new SQLiteCommand
                {
                    CommandText = String.Format("DELETE FROM Tarjeta WHERE TarjetaId = '{0}'", tarjetaId),
                    Connection = dbConnection
                };

                deleteCmd.ExecuteNonQuery();

                dbConnection.Close();

                return true;
            }
            catch (OleDbException ex)
            {
                MessageBox.Show("Ha habido un problema en la escritura a la base de datos. ",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Warning);

#if Debug
                GlobalData.PrintDebug("ERROR ACCESS", "Mensaje: " + ex.Message);
                GlobalData.PrintDebug("ERROR ACCESS", "StackTrace: " + ex.StackTrace);
#endif

                return false;
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show("Ha habido un problema en la escritura a la base de datos. ",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Warning);

#if Debug
                GlobalData.PrintDebug("ERROR ACCESS", "Mensaje: " + ex.Message);
                GlobalData.PrintDebug("ERROR ACCESS", "StackTrace: " + ex.StackTrace);
#endif

                return false;
            }
        }
        #endregion

        // Elimina un día festivo del calendario
        #region EliminaDiaFestivo
        public static bool EliminaDiaFestivo(DateTime date)
        {
            var deleteCmd = new SQLiteCommand
            {
                CommandText = "DELETE FROM CalendarioLaboral WHERE FechaFestivo = @Date",
                Connection = dbConnection
            };

            try
            {
                if (dbConnection.State != ConnectionState.Open)
                    dbConnection.Open();

                deleteCmd.Parameters.AddWithValue("@Date", date);
                deleteCmd.ExecuteNonQuery();

                dbConnection.Close();

                return true;
            }
            catch (OleDbException ex)
            {
                MessageBox.Show("Ha habido un problema en la escritura a la base de datos. ",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Warning);

#if Debug
                GlobalData.PrintDebug("ERROR ACCESS", "Mensaje: " + ex.Message);
                GlobalData.PrintDebug("ERROR ACCESS", "StackTrace: " + ex.StackTrace);
#endif

                return false;
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show("Ha habido un problema en la escritura a la base de datos. ",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Warning);

#if Debug
                GlobalData.PrintDebug("ERROR ACCESS", "Mensaje: " + ex.Message);
                GlobalData.PrintDebug("ERROR ACCESS", "StackTrace: " + ex.StackTrace);
#endif

                return false;
            }
        }
        #endregion

        // Insertar un nuevo registro en el historial
        #region InsertaNuevoRegistroHistorial
        public static bool InsertaNuevoRegistroHistorial(long tarjetaId, TipoRegistro tipoRegistro)
        {
            var selectCmd = new SQLiteCommand
            {
                CommandText = String.Format("SELECT PersonalId FROM Tarjeta WHERE TarjetaId LIKE {0}", tarjetaId),
                Connection = dbConnection
            };

            var insertCmd = new SQLiteCommand
            {
                CommandType = CommandType.Text,
                CommandText = String.Format("INSERT INTO Historial (PersonalId, HistorialTimeStamp, HistorialTipo) " +
                "VALUES (X,datetime('now', 'localtime'),{0})", (int)tipoRegistro),
                Connection = dbConnection
            };

            try
            {
                if (dbConnection.State != ConnectionState.Open)
                    dbConnection.Open();

                var dataReader = selectCmd.ExecuteReader();

                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        insertCmd.CommandText = insertCmd.CommandText.Replace("X", dataReader.GetValue(0).ToString());
                    }

                    insertCmd.ExecuteNonQuery();
                }
                else
                {
#if Debug
                    GlobalData.PrintDebug("BD","No se ha encontrado ningún PersonalId con esa tarjeta.");
#endif
                }

                dbConnection.Close();

                return true;
            }
            catch (OleDbException ex)
            {
                MessageBox.Show("Ha habido un problema en la escritura a la base de datos. " +
                    "No se ha podido insertar la nueva información.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

#if Debug
                GlobalData.PrintDebug("ERROR ACCESS", "Mensaje: " + ex.Message);
                GlobalData.PrintDebug("ERROR ACCESS", "StackTrace: " + ex.StackTrace);
#endif

                return false;
            }

        }
        #endregion

        // Insertar un nuevo día festivo al calendario
        #region InsertaNuevoDiaFestivo
        public static bool InsertaNuevoDiaFestivo(DateTime date)
        {
            var insertCmd = new SQLiteCommand
            {
                CommandType = CommandType.Text,
                CommandText = String.Format("INSERT INTO {0} (FechaFestivo) " +
                "VALUES (?)", TablaCalendario),
                Connection = dbConnection
            };

            try
            {
                if (dbConnection.State != ConnectionState.Open)
                    dbConnection.Open();

                insertCmd.Parameters.AddWithValue("@Date", date);
                insertCmd.ExecuteNonQuery();

                dbConnection.Close();

                return true;
            }
            catch (OleDbException ex)
            {
#if Debug
                GlobalData.PrintDebug("EXCEPCIÓN BD", ex.Message);
                GlobalData.PrintDebug("EXCEPCIÓN BD", ex.StackTrace);
#endif

                return false;  
            }
            catch (InvalidOperationException ex)
            {
#if Debug
                GlobalData.PrintDebug("EXCEPCIÓN BD", ex.Message);
                GlobalData.PrintDebug("EXCEPCIÓN BD", ex.StackTrace);
#endif

                return false;
            }

        }
        #endregion

        // Registra un nuevo usuario del personal.
        // Se puede registrar solamente el usuario,
        // o el usuario y además vincular una tarjeta.
        #region InsertaNuevoPersonal
        public static int InsertaNuevoPersonal(string nombre, int hSemanales, int hAnuales, string idTarjeta)
        {
            // Registra un nuevo usuario del personal.
            // Guarda datos en tres tablas:
            // 1. Tabla Personal: Genera un random ID, guarda nombre y apellidos
            // 2. Tabla Horario: Guarda los horarios y el ID personal
            // 3. Tabla Tarjeta: Guarda el ID de la tarjeta registrada con el usuario 

            #region Declaración SQLiteCommands 
            var insertaPersonalCmd = new SQLiteCommand
            {
                CommandText = String.Format("INSERT INTO Personal (PersonalNombre) VALUES ('{0}')", nombre),
                Connection = dbConnection
            };

            var insertaHorarioCmd = new SQLiteCommand
            {
                CommandText = String.Format("INSERT INTO Horario (PersonalId, NumHorasSemanales, NumHorasAnuales) VALUES (X, {0}, {1})", hSemanales, hAnuales),
                Connection = dbConnection
            };

            SQLiteCommand insertaTarjetaCmd = null;

            if (idTarjeta != "")
            {
                insertaTarjetaCmd = new SQLiteCommand
                {
                    CommandText = String.Format("INSERT INTO Tarjeta (TarjetaId, PersonalId) VALUES ('{0}', X)", idTarjeta),
                    Connection = dbConnection
                };
            }

            var selectComprovacionTarjetaCmd = new SQLiteCommand
            {
                CommandType = CommandType.Text,
                CommandText = String.Format("SELECT * FROM Tarjeta WHERE TarjetaId LIKE '{0}'", idTarjeta),
                Connection = dbConnection
            };

            var selectIdPersonalCmd = new SQLiteCommand
            {
                CommandType = CommandType.Text,
                CommandText = "SELECT * FROM Personal ORDER BY PersonalId DESC LIMIT 1",
                Connection = dbConnection
            };
            #endregion

            try
            {
                if (dbConnection.State != ConnectionState.Open)
                    dbConnection.Open();

                // Comprueba que la tarjeta que se quiere 
                // vincular no esté ya registrada
                var dataReaderTarjeta = selectComprovacionTarjetaCmd.ExecuteReader();

                if (dataReaderTarjeta.HasRows)
                {
                    return -1;
                }

                // Si no lo está, inserta el usuario
                insertaPersonalCmd.ExecuteNonQuery();

                // Después recoge el ID para poder insertarlo en las otras tablas
                var idPersonal = -2;
                var dataReader = selectIdPersonalCmd.ExecuteReader();
                while (dataReader.Read())
                {
                    insertaHorarioCmd.CommandText = insertaHorarioCmd.CommandText.Replace("X", dataReader.GetValue(0).ToString());
                    if (insertaTarjetaCmd != null) insertaTarjetaCmd.CommandText = insertaTarjetaCmd.CommandText.Replace("X", dataReader.GetValue(0).ToString());
                    idPersonal = (int)dataReader.GetValue(0);
                }

                // Y finalmente inserta el horario y 
                // la tarjeta (en caso de que tenga)
                insertaHorarioCmd.ExecuteNonQuery();
                if (insertaTarjetaCmd != null) insertaTarjetaCmd.ExecuteNonQuery();

                dbConnection.Close();

                return idPersonal;
            }
            catch (OleDbException ex)
            {
#if Debug
                GlobalData.PrintDebug("EXCEPCIÓN BD", ex.Message);
                GlobalData.PrintDebug("EXCEPCIÓN BD", ex.StackTrace);
#endif

                return -3;
            }
            catch (InvalidOperationException ex)
            {
#if Debug
                GlobalData.PrintDebug("EXCEPCIÓN BD", ex.Message);
                GlobalData.PrintDebug("EXCEPCIÓN BD", ex.StackTrace);
#endif

                return -4;
            }
        }
        #endregion

        // Vincula una tarjeta con un usuario. 
        // Un usuario puede tener varias tarjetas.
        #region RegistroTarjeta
        public static bool RegistroTarjeta(long idTarjeta, int idPersonal)
        {

            var query = String.Format("INSERT INTO Tarjeta (TarjetaId, PersonalId) VALUES ({0},{1});",
                                        idTarjeta,
                                        idPersonal
                                        );

            try
            {

                if (dbConnection.State != ConnectionState.Open)
                    dbConnection.Open();
                var insertaTarjetaCmd = new SQLiteCommand(query, dbConnection);
                insertaTarjetaCmd.ExecuteNonQuery();
                dbConnection.Close();
                Console.WriteLine(dbConnection.State);

                return true;
            }
            catch (OleDbException ex)
            {
                if (ex.Message.Contains("duplicate values"))
                {
                    MessageBox.Show("Esta tarjeta ya está vinculada con un usuario.", "",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Error de la base de datos.", "",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }

#if Debug
                GlobalData.PrintDebug("EXCEPCIÓN BD", ex.Message);
                GlobalData.PrintDebug("EXCEPCIÓN BD", ex.StackTrace);
#endif

                return false;
            }
            catch (InvalidOperationException ex)
            {
#if Debug
                GlobalData.PrintDebug("EXCEPCIÓN BD", ex.Message);
                GlobalData.PrintDebug("EXCEPCIÓN BD", ex.StackTrace);
#endif

                return false;
            }
        }
        #endregion
        
        private static void CrearSchema()
        {
            using (var connection = new SQLiteConnection(dbConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string[] tableCommands = {
                            @"CREATE TABLE Personal (
                                PersonalId INTEGER PRIMARY KEY AUTOINCREMENT,
                                PersonalNombre TEXT NOT NULL
                            );",
                            
                            @"CREATE TABLE Tarjeta (
                                TarjetaId TEXT PRIMARY KEY,
                                PersonalId INTEGER,
                                FOREIGN KEY(PersonalId) REFERENCES Personal(PersonalId) ON DELETE CASCADE
                            );",

                            @"CREATE TABLE Horario (
                                PersonalId INTEGER PRIMARY KEY,
                                NumHorasSemanales INTEGER,
                                NumHorasAnuales INTEGER,
                                FOREIGN KEY(PersonalId) REFERENCES Personal(PersonalId) ON DELETE CASCADE
                            );",

                            @"CREATE TABLE CalendarioLaboral (
                                FechaFestivo DATETIME PRIMARY KEY
                            );",

                            @"CREATE TABLE Historial (
                                HistorialId INTEGER PRIMARY KEY AUTOINCREMENT,
                                PersonalId INTEGER,
                                HistorialTimeStamp DATETIME,
                                HistorialTipo INTEGER,
                                FOREIGN KEY(PersonalId) REFERENCES Personal(PersonalId) ON DELETE CASCADE
                            );"
                        };

                        foreach (var sql in tableCommands)
                        {
                            using (var command = new SQLiteCommand(sql, connection))
                            {
                                command.ExecuteNonQuery();
                            }
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Error al crear la base de datos inicial: " + ex.Message);
                    }
                }
            }
        }
    }
}
