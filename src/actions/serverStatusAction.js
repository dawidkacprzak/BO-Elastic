export const addNode = nodeObject => ({
  type: "ADD_NODE",
  data: nodeObject
});

export const setNodeTree = nodeTree => ({
  type: "SET_NODES",
  data: nodeTree
})
