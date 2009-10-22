using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web.DynamicData;
using System.Web.DynamicData.ModelProviders;

namespace BoC.DomainServices.Extensions
{
    //this extension method class uses a lot of dirty reflection to allow you to have associations crossing
    //multiple domainmodelproviders
    public static class MetaModelExtensions
    {
        static PropertyInfo isForeignKeyComponentProp = typeof(ColumnProvider).GetProperty("IsForeignKeyComponent");
        static PropertyInfo associationProp = typeof(ColumnProvider).GetProperty("Association");
        static PropertyInfo parentTableProp = typeof(MetaForeignKeyColumn).GetProperty("ParentTable");
        static PropertyInfo childrenTableProp = typeof(MetaChildrenColumn).GetProperty("ChildTable");
        static PropertyInfo columnInOtherTableProp = typeof(MetaChildrenColumn).GetProperty("ColumnInOtherTable");
        
        private static MethodInfo createColumnsMethod = typeof (MetaTable).GetMethod("CreateColumns",
                                                                                     BindingFlags.Instance |
                                                                                     BindingFlags.NonPublic);

        private static MethodInfo initializeMethod = typeof(MetaTable).GetMethod("Initialize",
                                                                                     BindingFlags.Instance |
                                                                                     BindingFlags.NonPublic);

        public static void FixForeignKeyColumns(this MetaModel model)
        {
            foreach (var column in model.Tables.SelectMany(t => t.Provider.Columns))
            {
                FixForeignKey(model, column);
            }

            foreach (var table in model.Tables)
            {
                RebuildMetaColumns(table);
            }

            foreach (var table in model.Tables)
            {
                ReInitializeMetaTable(table);
            }
        }

        //the DomainColumnProvider only queries the current domainmodelprovider for a foreign key, instead of querying the entire metamodel
        private static void FixForeignKey(MetaModel model, ColumnProvider columnProvider)
        {
            if (columnProvider.Association != null) //already found by system.domainservice stuff, just set the bool prop
            {
                return;
            }

            var myAssociation = columnProvider.Attributes.OfType<AssociationAttribute>().FirstOrDefault(a => a.IsForeignKey);
            if (myAssociation == null) //nothing to do
                return;

            MetaTable table;
            if (model.TryGetTable(columnProvider.ColumnType, out table))
            {
                var provider = table.Provider.DataModel.Tables.FirstOrDefault<TableProvider>(t => t.EntityType == columnProvider.ColumnType);
                ColumnProvider toColumn = provider.Columns.FirstOrDefault(
                    c => c.Attributes
                             .OfType<AssociationAttribute>()
                             .Any(a => a.Name == myAssociation.Name)
                    );

                if (toColumn == null)
                {
                    throw new Exception(string.Format("Can't find To column for column '{0}' (assocationname: {2}) on class {1}", columnProvider.Name, columnProvider.Table.Name, myAssociation.Name));
                }
                foreach (var keyMember in myAssociation.ThisKeyMembers)
                {
                    string member = keyMember;
                    var col = provider.Columns.FirstOrDefault(c => c.Name == member);
                    isForeignKeyComponentProp.SetValue(col, true, null);
                }
                //now set the association that points to a different tableprovider
                associationProp.SetValue(
                    columnProvider,
                    new DomainAssociationProvider(AssociationDirection.ManyToOne, columnProvider, toColumn,
                                                  myAssociation.ThisKeyMembers),
                    null);
                associationProp.SetValue(
                    toColumn,
                    new DomainAssociationProvider(AssociationDirection.OneToMany,
                                                  toColumn, columnProvider,
                                                  myAssociation.ThisKeyMembers),
                    null);

            }

        }

        //rebuild the metacolumns!
        private static void RebuildMetaColumns(MetaTable entityTable)
        {
            //first we call the internal CreateColumns to rebuild them, could be that they should become MetaForeignKeyColumn or MetaChildrenColumn now
            createColumnsMethod.Invoke(entityTable, null);
        }


        private static void ReInitializeMetaTable(MetaTable entityTable)
        {
            //now actually we would like to call the initialize, but the MetaColumns don't recognize ToColumns in other tableproviders' contexts :(
            //initializeMethod.Invoke(entityTable, null); 

            //so we look what initialize does, and do this ourselves:
            foreach (var column in entityTable.Columns)
            {
                if (column is MetaForeignKeyColumn && ((MetaForeignKeyColumn)column).ParentTable == null)
                {
                    //ParentTable is null so we have an uninitalized one here
                    parentTableProp.SetValue(
                        column,
                        column.Model.GetTable(column.Provider.Association.ToTable.EntityType),
                        null
                        );
                }
                else if (column is MetaChildrenColumn && ((MetaChildrenColumn)column).ChildTable == null)
                {
                    AssociationProvider association = column.Provider.Association;
                    childrenTableProp.SetValue(
                        column,
                        column.Model.GetTable(association.ToTable.EntityType),
                        null);
                    if (association.ToColumn != null)
                    {
                        columnInOtherTableProp.SetValue(
                            column,
                            ((MetaChildrenColumn)column).ChildTable.GetColumn(association.ToColumn.Name),
                            null
                            );
                    }

                }
            }
            
        }
    }

    //damn those ms internals
    internal class DomainAssociationProvider : AssociationProvider
    {
        public DomainAssociationProvider(AssociationDirection direction, ColumnProvider fromColumn, ColumnProvider toColumn, IEnumerable<string> foreignKeyNames)
        {
            Func<string, string> selector = null;
            this.Direction = direction;
            this.FromColumn = fromColumn;
            this.ToColumn = toColumn;
            if (selector == null)
            {
                selector = name => fromColumn.Name + "." + name;
            }
            this.ForeignKeyNames = new ReadOnlyCollection<string>(foreignKeyNames.Select<string, string>(selector).ToList<string>());
        }
    }

}