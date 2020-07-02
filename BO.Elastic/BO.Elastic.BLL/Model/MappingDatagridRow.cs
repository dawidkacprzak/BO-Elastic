using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using BO.Elastic.BLL.ElasticCore;
using BO.Elastic.BLL.Types;

namespace BO.Elastic.BLL.Model
{
    public class MappingDatagridRow : System.ComponentModel.INotifyPropertyChanged
    {
        public string ColumnName { get; set; }
        public EDBDataType DBDataType { get; set; }

        private EElasticDataTypes? selectedMapping;
        public EElasticDataTypes SelectedMapping
        {
            get
            {
                if (selectedMapping == null)
                {
                    selectedMapping = PossibleDataMappings.First();
                }
                return selectedMapping.Value;
            }
            set
            {
                selectedMapping = value;
                NotifyPropertyChanged();
            }
        }


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


        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}