using System;
using System.Collections.Generic;
using System.IO;
using TiledIO.Entities;
using TiledIO.Extensions;
using Model = TiledIO.Models;


namespace TiledIO.Mapper
{
    public class ProjectMapper
    {
        public static void Map(Model.Project projectRaw, Entities.Project project, string  input, string appRoot)
        {
            project.Properties = PropertyMapper.Map(projectRaw.Properties);
            string inputPath = Path.GetDirectoryName(input);
            project.RootFolder = inputPath;


            if (projectRaw.Properties.ExistProperty("Includes"))
            {
                project.Includes.Append<string, Table>(ResolveTables(projectRaw.Properties.GetProperty("Includes"), appRoot));
            }


            project.PropertyTypes = projectRaw.PropertyTypes.FindAll(f=>f.Type == "enum").ConvertAll<Entities.PropertyType>(x=> new Entities.PropertyType() { Id =x.Id, Name = x.Name, StorageType = x.StorageType, Type = x.Type, Values = x.Values, ValuesAsFlags = x.ValuesAsFlags});
        }

        /// <summary>
        /// check if tables are available and load definition
        /// </summary>
        /// <param name="props"></param>
        private static Dictionary<string, Table> ResolveTables(string prop, string appRoot)
        {
            string[] tablesRaw = prop.Split('\n');           
            Dictionary<string, Table> tables = new();
            foreach (string table in tablesRaw)
            {
                string[] tableParts = table.Split(':');
                Table tableSettings = new Table() { Name = tableParts[0], FilePath = tableParts[1] };
                tables.Add(tableSettings.Name, tableSettings);
                Console.WriteLine($"Table={tableSettings.Name} Path={tableSettings.FilePath}");
            }
            return tables;
        }
    }
}
