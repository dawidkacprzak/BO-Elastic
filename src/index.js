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


const initialize = () => {
  RequestManager.clusterIp = "http://10.10.1.214:9201";
  ServerStatusController.beginRefreshState();
};

const Navigation = () => {
  return (
    <Router>
      <Template>
        <Switch>
          <Route exact path="/">
            <ServerStatus />
          </Route>
          <Route path="/Logs">
            <Logs />
          </Route>
        </Switch>
      </Template>
    </Router>
  );
};

ReactDOM.render(
  <Provider store={store}>
    <Navigation />
  </Provider>,
  document.getElementById("root")
);

serviceWorker.unregister();
initialize();