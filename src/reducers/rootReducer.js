
import { combineReducers } from "redux";
import serverStatusReducer from "./serverStatusReducer";
import '../typedef'

/**
 * @description Odpowiada za połączenie wszystkich reducerów w jeden a następnie zainjectowanie go do stanu przy tworzeniu kontenera stanu
 * @type {CombinedReducer}
 */
const rootReducer = combineReducers({
  serverStatusReducer: serverStatusReducer
});

export default rootReducer;
