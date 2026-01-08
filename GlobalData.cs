using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace SistemaFichaje
{
    public static class GlobalData
    {
        // string con el nombre del archivo de la base de datos
        public static string ArchivoBD { get; set; } = "\\data.sqlite3";

        // path al directorio de ensamblaje, para las strings de conexión a las BD
        public static string BuildDir { get; } = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        #region PrintDebug
        public static void PrintDebug(string modo, string msg)
		{
			Debug.Print("{0} {1}: {2}", DateTime.Now, modo, msg);
		}
        #endregion
    }
}
