
/**
  Reducer złożony z wielu reducerów
  @typedef CombinedReducer

  Reducer - odpowiada z obsługę i modyfikacje stanu aplikacji na podstawie akcji
  @typedef Reducer

  @typedef SingleNode
* @property {string} ip
* @property {number} heap.percent 
* @property {number} ram.percent 
* @property {number} cpu 
* @property {number} load_1m 
* @property {number} load_5m 
* @property {number} load_15m
* @property {string} node.role 
* @property {string} master 
* @property {string} name 

  @typedef ClusterHealth
* @property {string} cluster_name
* @property {string} status
* @property {boolean} timed_out
* @property {number} number_of_nodes 
* @property {number} number_of_data_nodes 
* @property {number} active_primary_shards 
* @property {number} active_shards
* @property {number} relocating_shards 
* @property {number} initializing_shards 
* @property {number} unassigned_shards 
* @property {number} delayed_unassigned_shards 
* @property {number} number_of_pending_tasks 
* @property {number} number_of_in_flight_fetch 
* @property {number} task_max_waiting_in_queue_millis 
* @property {number} active_shards_percent_as_number 
*/