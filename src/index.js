import React from "react";
import ReactDOM from "react-dom";
import * as serviceWorker from "./serviceWorker";
import { BrowserRouter as Router, Switch, Route } from "react-router-dom";
import ServerStatus from "./pages/ServerStatus";
import "./style/shared.css";
import Template from "./template/Template";
import { createStore } from "redux";
import rootReducer from "./reducers/rootReducer";
import { Provider } from "react-redux";
import Logs from "./pages/Logs";

const store = createStore(rootReducer);

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
