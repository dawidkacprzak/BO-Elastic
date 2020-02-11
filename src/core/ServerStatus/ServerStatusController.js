import RequestManager from "../RequestManager";
import store from "../../store/index";
import * as actions from "../../actions/serverStatusAction";
import { wait } from "@testing-library/react";

export default class ServerStatusController {
  static beginRefreshState = async () => {
    if (RequestManager.clusterIp) {
      RequestManager.clusterGetPromise("/_nodes/_all/attributes")
        .then(data => {
          store.dispatch(actions.setNodeTree(data.nodes));
          setTimeout(this.beginRefreshState,5000)
        })
        .catch(err => {
          alert(err);
          setTimeout(this.beginRefreshState,15000)
        })
        .finally(() => {});
    }
  };
}
