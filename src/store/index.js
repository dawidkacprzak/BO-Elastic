/**
 * Kontener na stan (state) aplikacji w architekturze FLUX - implementacja REDUX
 * @module Store
*/

import { createStore } from "redux";
import combineReducers from "../reducers/rootReducer";

/**
 * @description Tworzy i zwraca kontener na stan aplikacji wraz z opcją debugowania przez Redux DevTools. 
 * @description Powinien byc użyty tylko raz w module {@link module:Main}
 */
const store = createStore(
  combineReducers,
  window.__REDUX_DEVTOOLS_EXTENSION__ && window.__REDUX_DEVTOOLS_EXTENSION__()
);

export default store;
