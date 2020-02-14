import "../typedef";

const initialState = {
  initialized: false,
  nodes: [],
  cluster_health: {}
};

/**
 * Reducer odpowiedzialny za dodawanie i aktualizacje statusu nodów / clustra w aplikacji
 * @function
 * @type {Reducer}
 * @param {Object} state domyślny state - typowe dla reducerów w redux - {@link https://redux.js.org/basics/reducers/}
 * @param {JSON} action klucz data musi zawierać model
 */
const serverStatusReducer = (state = initialState, action) => {
  switch (action.type) {
    case "ADD_NODE":
      state.initialized = true;
      state.nodes = [...state.nodes, action.data];
      return state;
    case "SET_NODES":
      state.initialized = true;
      state.nodes = action.data;
      return state;
    case "SET_CLUSTER_HEALTH": {
      state.cluster_health = action.data;
      return state;
    }
    default:
      return state;
  }
};

export default serverStatusReducer;
