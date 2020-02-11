
const initialState = {
  initialized: false,
  nodes: []
}
const serverStatusReducer = (state = initialState, action) => {
  switch (action.type) {
    case "ADD_NODE":
      state.initialized = true;
      state.nodes = [...state.nodes, action.data]
      return state;
    case "SET_NODES":
      state.initialized = true;
      state.nodes = action.data;
      return state;
    default: return state;
  }
};

export default serverStatusReducer;