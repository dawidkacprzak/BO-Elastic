//@ts-check
/** Podstawowy skrypt inicjujący
 *  Zawiera się w nim określenie zasad renderowania/nawigacji po oknach (w tym ścieżki - routing)
 *  a także podstawowa konfiguracja aplikacji wraz z wystartowaniem podstawowych zadań działających w tle
 * @module Main
 */

import React from "react";
import ReactDOM from "react-dom";
import * as serviceWorker from "./serviceWorker";
import { BrowserRouter as Router, Switch, Route } from "react-router-dom";
import ServerStatus from "./pages/ServerStatus";
import "./style/shared.css";
import Template from "./template/Template";
import { Provider } from "react-redux";
import Logs from "./pages/Logs";
import RequestManager from "./core/RequestManager";
import ServerStatusController from "./core/ServerStatus/ServerStatusController";
import store from "./store/index";

/**
 * @function initialize
 * @description Funkcja do inicjalizacji podstawowych parametrów aplikacji
   i uruchomienia podstawowych zadań w tle typu aktualizacja statusu klastra
 */
const initialize = () => {
  RequestManager.clusterIp = "http://10.10.1.214:9201";
  ServerStatusController.beginRefreshState();
};

/**
 * Tutaj następuje rozpoczęcie renderowania aplikacji, utworzenie routingu ('podstron') i dołączenie podstawowego szablonu
 */
ReactDOM.render(
  <Provider store={store}>
    <Router>
      <Template>
        <Switch>
          <Route exact path="/">
            <ServerStatus />
          </Route>
          <Route exact path="/Logs">
            <Logs />
          </Route>
        </Switch>
      </Template>
    </Router>
  </Provider>,
  document.getElementById("root")
);

serviceWorker.unregister();
initialize();
