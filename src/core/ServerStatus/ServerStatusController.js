//@ts-check

import RequestManager from "../RequestManager";
import store from "../../store/index";
import * as actions from "../../actions/serverStatusAction";

/** Klasa odpowiedzialna za dostarczenie i obsługę wszelkich danych związanych ze statusem klastra/nodów
 * @class ServerStatusController
 */
export default class ServerStatusController {
  /**
   * @memberof ServerStatusController
   * @description odpowiada za asynchronieczne odświeżanie podstawowych informacji o klastrze/nodach i wrzucenie ich do stanu aplikacji
   * @description powinna być wywołana tylko raz w {@link module:Main~initialize}
   */
  static beginRefreshState = async () => {
    if (RequestManager.clusterIp) {
      RequestManager.clusterGetPromise("/_cat/nodes?format=json")
        .then(data => {
          store.dispatch(actions.setNodeTree(data));
          RequestManager.clusterGetPromise("/_cluster/health").then(clusterData => {
            store.dispatch(actions.setClusterHealthInfo(clusterData))
            setTimeout(ServerStatusController.beginRefreshState, 5000);
          }).catch((err)=>{
            alert(err);
            setTimeout(ServerStatusController.beginRefreshState, 15000);
          })
        })
        .catch(err => {
          alert(err);
          setTimeout(ServerStatusController.beginRefreshState, 15000);
        });
    }
  };
}
