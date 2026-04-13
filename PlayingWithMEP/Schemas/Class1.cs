using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoEletrica.Schemas
{
    class DiagramsSchema
    {
        public static void createSchema (Document doc)
        {
            using (Transaction t = new Transaction(doc, "Creating Diagrams schema"))
            {
                t.Start();
                SchemaBuilder schemaBuilder = new SchemaBuilder(new Guid("C88048E7-3FAB-4C6E-A73C-2D6263BA3F35"));
                schemaBuilder.SetReadAccessLevel(AccessLevel.Public);
                schemaBuilder.SetWriteAccessLevel(AccessLevel.Public);
                schemaBuilder.SetVendorId("AutoEletrica");
                schemaBuilder.SetSchemaName("DiagramsSchema");
                FieldBuilder panelFieldBuilder = schemaBuilder.AddSimpleField("PanelId", typeof(ElementId));
                FieldBuilder CircuitfieldBuilder = schemaBuilder.AddSimpleField("PanelId", typeof(ElementId));
                FieldBuilder fieldBuilder = schemaBuilder.AddSimpleField("PanelId", typeof(ElementId));

                fieldBuilder.SetDocumentation("ElementId of the element that the schema is attached to");
                Schema schema = schemaBuilder.Finish();
                
                t.Commit();
            }
        }
    }
}
