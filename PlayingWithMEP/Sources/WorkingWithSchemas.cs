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
        public static Schema CreateSchema (Document doc, string name, Dictionary<string, Dictionary<string, Type>> fields, Guid uid)
        {
            Transaction tcreateSchema = new Transaction(doc, "tCreating Schema");
            tcreateSchema.Start();
            SchemaBuilder schemaBuilder = new SchemaBuilder(uid);
            schemaBuilder.SetReadAccessLevel(AccessLevel.Public);
            schemaBuilder.SetSchemaName(name);

            foreach (string key in fields.Keys.ToList())
            {

                string fieldType = fields[key].Keys.Last();

                if (fieldType == "simple")
                {
                    schemaBuilder.AddSimpleField(key, fields[key][fieldType]);
                    continue;
                }
                if (fieldType == "array")
                {
                    schemaBuilder.AddArrayField(key, fields[key][fieldType]);
                    continue;
                }


            }
            
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
