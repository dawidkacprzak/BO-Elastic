
const serverStatusReducer = (state = [], action) => {
  switch (action.type) {
    case "ADD_NODE":
      return [
        ...state,
        {
          node: action.node
        }
      ];
    default: return state;
  }
};

export default serverStatusReducer;