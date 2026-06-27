using Entity = TiledIO.Entities;
using Model = TiledIO.Models;


namespace TiledIO.Mapper
{
    public class MetadataMapper
    {
        public static Entities.Metadata Map(Model.MetaData metadata)
        {
            Entity.Metadata meta = new();

            meta.Enabled = metadata.Enabled;
            meta.Name = metadata.Name;
            meta.Modified = metadata.Modified;
            meta.Path = metadata.Path;
            meta.Columns = metadata.Columns;
            meta.Height = metadata.Height;
            meta.Rows = metadata.Rows;
            meta.Columns = metadata.Columns;
            meta.Height = metadata.Height;
            return meta;
        }
    }
}
