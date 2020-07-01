using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BO.Elastic.BLL.ElasticCore;
using BO.Elastic.BLL.Types;

namespace BO.Elastic.BLL.Model
{
    public class MappingDatagridRow
    {
        public string ColumnName { get; set; }
        public EDBDataType DBDataType { get; set; }
        public EElasticDataTypes SelectedMapping { get; set; }
        
        public ObservableCollection<EElasticDataTypes> PossibleDataMappings
        {
            get
            {
                return new ObservableCollection<EElasticDataTypes>(DataTypeParser.GetPossibleElasticDataTypes(DBDataType));
            }
        }
        public MappingDatagridRow(string columnName, string columnType)
        {
            this.ColumnName = columnName;
            this.DBDataType = DataTypeParser.GetDBDateTypeFromString(columnType);
        }

        public MappingDatagridRow(string columnName, EDBDataType dataType)
        {
            this.ColumnName = columnName;
            this.DBDataType = dataType;
        }
        public List<EElasticDataTypes> AvailableElasticModelMappings
        {
            get { return DataTypeParser.GetPossibleElasticDataTypes(DBDataType); }
        }
    }
}