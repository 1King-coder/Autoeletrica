using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;

namespace AutoEletrica.Sources
{
    public class SchemasManager
    {
        public static Schema CreateSchema (Document doc, string name, Guid uid)
        {
            Transaction tcreateSchema = new Transaction(doc, "tCreating Schema");
            tcreateSchema.Start();
            SchemaBuilder schemaBuilder = new SchemaBuilder(uid);
            schemaBuilder.SetReadAccessLevel(AccessLevel.Public);
            schemaBuilder.SetSchemaName(name);
          
            Schema schema = schemaBuilder.Finish();
            tcreateSchema.Commit();
            return schema;
        }

        public static Schema GetSchemaByName(string name)
        {
            return Schema.ListSchemas().Where((Schema x) => x.SchemaName == name).First();
        }

        public static Schema GetOrCreateSchema (Document doc,  string name)
        {
            Schema selectedSchema = SchemasManager.GetSchemaByName(name);

            //if (selectedSchema == null)
            //{
            //    SchemasManager.CreateSchema(doc);
            //}

            selectedSchema = SchemasManager.GetSchemaByName(name);

            return selectedSchema;
        }

        public static void setDataToSchema (Schema schema, Element element, ElementId elementId)
        {
            Entity entity = new Entity(schema);
            Field refId = schema.GetField("referenceId");
            
        }
    }
}
