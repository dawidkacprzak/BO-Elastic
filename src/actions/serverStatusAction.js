import '../typedef'


/**
 * @param {SingleNode} nodeTree 
 */
export const addNode = nodeObject => ({
  type: "ADD_NODE",
  data: nodeObject
});

/**
 * @param {SingleNode[]} nodeTree 
 */
export const setNodeTree = nodeTree => ({
  type: "SET_NODES",
  data: nodeTree
})

/**
 * @param {ClusterHealth} healthObject 
 */
export const setClusterHealthInfo = healthObject => ({
  type: "SET_CLUSTER_HEALTH",
  data: healthObject
})