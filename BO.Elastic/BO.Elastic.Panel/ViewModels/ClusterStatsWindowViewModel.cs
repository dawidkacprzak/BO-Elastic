using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BO.Elastic.BLL.Abstract;
using BO.Elastic.BLL.ElasticCore;
using BO.Elastic.BLL.Exceptions;
using BO.Elastic.BLL.Extension;
using BO.Elastic.BLL.Model;
using BO.Elastic.BLL.Types;
using BO.Elastic.Panel.Command;
using Nest;
using Timer = System.Timers.Timer;


namespace BO.Elastic.Panel.ViewModels
{
    public class ClusterStatsWindowViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<KeyValuePair<string, string>> clusterAttributes;
        private ClusterHealthResponse clusterHealthResponse;
        private string clusterName;
        private readonly NetworkAddress clusterNetworkAddress;
        private ClusterStateResponse clusterStateResponse;
        private ClusterStatsResponse clusterStatsResponse;

        private string clusterStatus;
        private readonly Timer myTimer = new Timer();

        private NextWrap nextWrap;

        private NodeInfo nodeInfo;
        private bool contextMenuEnabled;

        #region Mapping

        private string mappingPort;
        private string mappingUsername;
        private string mappingPassword;
        private string mappingDatabase;
        private string mappingIndexName;
        private int mappingReplicas;
        private ObservableCollection<SqlTableNamespace> fetchedTables;
        private int mappingShards;
        private bool databaseValidConnection;
        private DBMSSystem mappingDbms;
        private IDatabaseModelFetcher modelFetcher;

        public int MappingReplicas
        {
            get
            {
                return mappingReplicas;
            }
            set
            {
                mappingReplicas = value;
                NotifyPropertyChanged();
            }
        }

        public int MappingShards
        {
            get
            {
                return mappingShards;
            }set
            {
                if(value!=mappingShards)
                {
                    mappingShards = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string MappingIndexName
        {
            get
            {
                return mappingIndexName;
            }
            set
            {
                if(value != mappingIndexName)
                {
                    mappingIndexName = value;
                }
                NotifyPropertyChanged();
            }
        }
        public DBMSSystem MappingDBMS
        {
            get { return mappingDbms; }
            set
            {
                if (!mappingDbms.Equals(value))
                {
                    mappingDbms = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string MappingHost
        {
            get { return clusterNetworkAddress.Ip; }
        }

        public ObservableCollection<SqlTableNamespace> FetchedTables
        {
            get
            {
                if (fetchedTables == null)
                {
                    return new ObservableCollection<SqlTableNamespace>();
                }

                return fetchedTables;
            }
            set
            {
                if (value == null)
                {
                    fetchedTables = new ObservableCollection<SqlTableNamespace>();
                    NotifyPropertyChanged();
                }
                else if (fetchedTables != value)
                {
                    fetchedTables = value;
                    FetchedTableSelected = fetchedTables.FirstOrDefault();
                    NotifyPropertyChanged();
                }
            }
        }

        public Visibility MappingConnectedElementsVisibility
        {
            get
            {
                if (DatabaseValidConnection)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Hidden;
                }
            }
        }

        public Visibility MappingGeneratedElementsVisibility
        {
            get
            {
                if (MappingRows.Count > 0)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Hidden;
                }
            }
        }

        public bool DatabaseValidConnection
        {
            get { return databaseValidConnection; }
            set
            {
                if (!databaseValidConnection.Equals(value))
                {
                    databaseValidConnection = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("MappingConnectedElementsVisibility");
                }
            }
        }

        public string MappingPort
        {
            get { return mappingPort; }
            set
            {
                if (mappingPort == null)
                {
                    mappingPort = string.Empty;
                }

                if (!mappingPort.Equals(value))
                {
                    mappingPort = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string MappingUsername
        {
            get { return mappingUsername; }
            set
            {
                if (string.IsNullOrEmpty(mappingUsername) || !mappingUsername.Equals(value))
                {
                    mappingUsername = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string MappingPassword
        {
            get { return mappingPassword; }
            set
            {
                if (string.IsNullOrEmpty(mappingPassword) || !mappingPassword.Equals(value))
                {
                    mappingPassword = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string MappingDatabase
        {
            get { return mappingDatabase; }
            set
            {
                if (string.IsNullOrEmpty(mappingDatabase) || !mappingDatabase.Equals(value))
                {
                    mappingDatabase = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public List<DBMSSystem> PossibleDBMSSystems
        {
            get { return Enum.GetValues(typeof(DBMSSystem)).Cast<DBMSSystem>().ToList(); }
        }

        public bool ContextMenuEnabled
        {
            get { return contextMenuEnabled; }
            set
            {
                if (contextMenuEnabled != value)
                {
                    contextMenuEnabled = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ICommand ConnectToDatabase => new BasicCommand(() =>
        {
            try
            {
                modelFetcher = IDatabaseModelFetcherIntanceManager.GetInstance(MappingDBMS, MappingHost, MappingPort,
                    MappingDatabase, MappingUsername, MappingPassword);
                bool isValid = modelFetcher.IsConnectionValid();
                FetchedTables = new ObservableCollection<SqlTableNamespace>(modelFetcher.GetTables());
                DatabaseValidConnection = isValid;
            }
            catch (Exception ex)
            {
                DatabaseValidConnection = false;
                FetchedTables = null;
                MessageBox.Show(ex.Message);
            }
        });

        public ICommand MapIndex => new BasicCommand(() =>
        {
            if (nextWrap.IndexExists(MappingIndexName))
            {
                MessageBox.Show("Indeks o tej nazwie już istnieje");
            }
            else
            {
                if(MappingShards <= 0)
                {
                    MessageBox.Show("Shards < 0");
                }
                else if (string.IsNullOrEmpty(MappingIndexName))
                {
                    MessageBox.Show("Mapping index cannot be empty");
                }
                else {
                    nextWrap.CreateIndex(MappingIndexName, MappingRows, MappingShards,MappingReplicas);
                }
            }
        });

        private SqlTableNamespace fetchedTableSelected;

        public SqlTableNamespace FetchedTableSelected
        {
            get
            {
                if (fetchedTableSelected == null)
                {
                    fetchedTableSelected = FetchedTables.FirstOrDefault();
                }

                return fetchedTableSelected;
            }
            set
            {
                fetchedTableSelected = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<MappingDatagridRow> mappingRows;

        public ObservableCollection<MappingDatagridRow> MappingRows
        {
            get
            {
                if (mappingRows == null)
                    mappingRows = new ObservableCollection<MappingDatagridRow>();
                return mappingRows;
            }
            set
            {
                mappingRows = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("MappingGeneratedElementsVisibility");
            }
        }
        public ObservableCollection<EElasticDataTypes> XX = new ObservableCollection<EElasticDataTypes>()
        {
            EElasticDataTypes.Boolean
        };
        public ICommand MapTable => new BasicCommand(() =>
        {
            try
            {
                if (FetchedTableSelected != null)
                {
                    if (FetchedTableSelected != null)
                    {
                        List<KeyValuePair<string, EDBDataType>> mapping =
                            modelFetcher.GetTableColumns(FetchedTableSelected).ToList();

                        MappingRows = new ObservableCollection<MappingDatagridRow>(mapping
                            .Select(x => new MappingDatagridRow(x.Key, x.Value)).ToList());
                    }
                }
            }
            catch (Exception ex)
            {
                DatabaseValidConnection = false;
                FetchedTables = null;
                MessageBox.Show(ex.Message);
            }
        });

        #endregion

        public ClusterStatsWindowViewModel(NetworkAddress networkAddress, bool startUpdateThread = true)
        {
            try
            {
                DatabaseValidConnection = false;
                ContextMenuEnabled = false;
                clusterNetworkAddress = networkAddress;
                nextWrap = new NextWrap(networkAddress);
                MappingShards = 5;
                MappingReplicas = 2;
                RefreshData();
            }
            catch (ClusterNotConnectedException)
            {
                ClusterStatus = "Offline";
            }

            if (startUpdateThread)
            {
                myTimer.Elapsed += SetClusterStats;
                myTimer.Interval = 1000;
                myTimer.Start();
            }
        }

        public ObservableCollection<KeyValuePair<string, string>> ClusterAttributes
        {
            get
            {
                clusterAttributes = new ObservableCollection<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Status", ClusterStatus),
                    new KeyValuePair<string, string>("UUID", GetNextWrapClusterData(EClusterAttributes.Uuid)),
                    new KeyValuePair<string, string>("HTTP URI", GetNextWrapClusterData(EClusterAttributes.HttpUri)),
                    new KeyValuePair<string, string>("CPU", GetNextWrapClusterData(EClusterAttributes.Cpu)),
                    new KeyValuePair<string, string>("RAM", GetNextWrapClusterData(EClusterAttributes.Ram)),
                    new KeyValuePair<string, string>("OS", GetNextWrapClusterData(EClusterAttributes.Os)),
                    new KeyValuePair<string, string>("ROLES", GetNextWrapClusterData(EClusterAttributes.Roles))
                };

                return clusterAttributes;
            }
            set
            {
                clusterAttributes = value;
                NotifyPropertyChanged();
            }
        }

        public ClusterStatsResponse ClusterStatsResponse
        {
            get
            {
                if (clusterStatsResponse == null)
                    clusterStatsResponse =
                        (ClusterStatsResponse) GetValueIfNextWrapIsInitialized(EClusterAttributes.Stats);
                return clusterStatsResponse;
            }
            set
            {
                clusterStatsResponse = value;
                NotifyPropertyChanged();
            }
        }

        public ClusterStateResponse ClusterStateResponse
        {
            get
            {
                if (clusterStateResponse == null)
                    clusterStateResponse =
                        (ClusterStateResponse) GetValueIfNextWrapIsInitialized(EClusterAttributes.State);
                return clusterStateResponse;
            }
            set
            {
                clusterStateResponse = value;
                NotifyPropertyChanged();
            }
        }

        public NodeInfo NodeInfo
        {
            get
            {
                if (nodeInfo == null)
                    nodeInfo = (NodeInfo) GetValueIfNextWrapIsInitialized(EClusterAttributes.NodeInfo);
                return nodeInfo;
            }
            set
            {
                if (value != nodeInfo)
                {
                    nodeInfo = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string ClusterStatus
        {
            get
            {
                if (ClusterHealthResponse == null) return "...";

                if (clusterStatus == null) clusterStatus = ClusterHealthResponse.Status.ToString();
                return clusterStatus;
            }
            set => clusterStatus = value;
        }

        public ClusterHealthResponse ClusterHealthResponse
        {
            get
            {
                if (clusterHealthResponse == null)
                    clusterHealthResponse =
                        (ClusterHealthResponse) GetValueIfNextWrapIsInitialized(EClusterAttributes.Health);
                return clusterHealthResponse;
            }
            set
            {
                if (value != clusterHealthResponse)
                {
                    clusterHealthResponse = value;
                    if (clusterHealthResponse != null && clusterHealthResponse.IsValid)
                    {
                        ContextMenuEnabled = true;
                    }
                    else
                    {
                        ContextMenuEnabled = false;
                    }

                    NotifyPropertyChanged();
                }
            }
        }

        public string PublishPort
        {
            get
            {
                if (nodeInfo != null)
                    return nodeInfo.Http.PublishAddress.Split(':')[1];
                return "...";
            }
        }

        public string TransportPort
        {
            get
            {
                if (nodeInfo != null)
                    return nodeInfo.TransportAddress.Split(':')[1];
                return "...";
            }
        }

        public string ClusterName
        {
            get => clusterName;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    clusterName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private object GetValueIfNextWrapIsInitialized(EClusterAttributes value)
        {
            if (nextWrap == null) return null;
            try
            {
                switch (value)
                {
                    case EClusterAttributes.Stats:
                        return nextWrap.GetClusterStats();
                    case EClusterAttributes.State:
                        return nextWrap.GetClusterState();
                    case EClusterAttributes.Health:
                        return nextWrap.GetClusterHealth();
                    case EClusterAttributes.NodeInfo:
                        return nextWrap.GetNodeInfo(clusterNetworkAddress);
                }
            }
            catch (Exception)
            {
                return null;
            }

            return null;
        }

        private string GetNextWrapClusterData(EClusterAttributes value)
        {
            if (nextWrap == null) return null;
            switch (value)
            {
                case EClusterAttributes.Uuid:
                    if (ClusterStateResponse != null)
                        return ClusterStateResponse.ClusterUUID;
                    else return "...";

                case EClusterAttributes.ClusterName:
                    if (ClusterStateResponse != null)
                        return ClusterStateResponse.ClusterName;
                    else return string.Empty;

                case EClusterAttributes.HttpUri:
                    if (ClusterStateResponse != null && ClusterStateResponse.ApiCall != null)
                        return ClusterStateResponse.ApiCall.Uri.AbsoluteUri;
                    else return string.Empty;

                case EClusterAttributes.Cpu:
                    if (ClusterStatsResponse != null && ClusterStatsResponse.Nodes != null)
                        return ClusterStatsResponse.Nodes.Process.Cpu.Percent + "%";
                    else return string.Empty;

                case EClusterAttributes.Ram:
                    if (ClusterStatsResponse != null && ClusterStatsResponse.Nodes != null)
                        return ClusterStatsResponse.Nodes.Jvm.Memory.HeapUsedInBytes / 1024 + "/" +
                               ClusterStatsResponse.Nodes.Jvm.Memory.HeapMaxInBytes / 1024;
                    else return string.Empty;

                case EClusterAttributes.Os:
                    if (NodeInfo != null)
                        return NodeInfo.OperatingSystem.PrettyName;
                    else return string.Empty;

                case EClusterAttributes.Roles:
                    if (NodeInfo != null)
                        return "[" + string.Join("] [", NodeInfo.Roles) + "]";
                    else return string.Empty;
            }

            throw new NotImplementedException("Brak definicji dla atrybutu klastra");
        }

        private void SetClusterStats(object sender, ElapsedEventArgs e)
        {
            new Thread(() =>
            {
                try
                {
                    RefreshData();
                }
                catch (ClusterNotConnectedException)
                {
                    ClusterStatus = "Offline";
                }
                finally
                {
                    NotifyPropertyChanged("ClusterAttributes");
                    NotifyPropertyChanged("PublishPort");
                    NotifyPropertyChanged("TransportPort");
                }
            }).Start();
        }

        private void RefreshData()
        {
            try
            {
                nextWrap = new NextWrap(clusterNetworkAddress);
                ClusterStatsResponse = nextWrap.GetClusterStats();
                ClusterStateResponse = nextWrap.GetClusterState();
                NodeInfo = nextWrap.GetNodeInfo(clusterNetworkAddress);
                ClusterHealthResponse = nextWrap.GetClusterHealth();
                ClusterStatus = ClusterHealthResponse.Status.ToString();
                ClusterName = GetNextWrapClusterData(EClusterAttributes.ClusterName);
            }
            catch (NodeNotConnectedException)
            {
                ClusterStatus = "Offline";
                nodeInfo = null;
                ContextMenuEnabled = false;
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}