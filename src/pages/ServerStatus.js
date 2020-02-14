/* eslint-disable jsx-a11y/alt-text */
//@ts-check
import React from "react";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import { bindActionCreators } from "redux";
import * as serverStatusActions from "../actions/serverStatusAction";
import "../style/serverStatus.css";
import "../typedef";
import Typography from "@material-ui/core/Typography";
import Card from "@material-ui/core/Card";
import ServerStatusController from "../core/ServerStatus/ServerStatusController";
import RequestManager from "../core/RequestManager";
/**
 * @description Komponent odpowiedzialny za wyświetlanie danych o klastrze i nodach - jest jednym z podstawowych komponentów routingu
 * @type {React.Component}
 */
class ServerStatus extends React.Component {
  /**
   * @description Służy do zabezpieczenia przed wielokrotnym włączeniem aktualizacji w tle. Działać może tylko jedna
   * @field startedUpdating
   */
  static startedUpdating = false;

  /**
   * @description Funckcja renderująca widok
   */
  render() {
    return (
      <div>
        {this.getClusterRowHealthFromState(
          this.props.state.serverStatusReducer.cluster_health
        )}

        {this.getNodesRowsFromState(this.props.state.serverStatusReducer.nodes)}
      </div>
    );
  }

  /**
   * Implementacja antywzorca isMounted -> https://reactjs.org/blog/2015/12/16/ismounted-antipattern.html
   * @param {*} props
   */
  constructor(props) {
    super(props);
    this._isMounted = true;
  }

  /**
   * @description Po utworzeniu widoku następuje próba odświeżania aplikacji na podstawie stanu klasta/nodów
   * @description Implementacja antywzorca isMounted -> https://reactjs.org/blog/2015/12/16/ismounted-antipattern.html
   * @function
   */
  componentDidMount() {
    this._isMounted = true;
    if (!ServerStatus.startedUpdating) {
      ServerStatus.startedUpdating = true;
      this._isMounted &&
        setInterval(() => {
          this._isMounted && this.forceUpdate();
        }, 1000);
    }
  }

  /**
   * @description Implementacja antywzorca isMounted -> https://reactjs.org/blog/2015/12/16/ismounted-antipattern.html
   * @function
   */
  componentWillUnmount() {
    this._isMounted = false;
    ServerStatus.startedUpdating = false;
  }

  //#region nodeRows
  /**
   * @description Tworzy z listy nodów elementy widoku HTML
   * @function
   * @param {Array} nodes
   */
  getNodesRowsFromState(nodes) {
    return (
      <div>
        {nodes
          .sort((a, b) => this.nodeMasterComparer(a, b))
          .map(item => this.nodeRow(item, item.ip))}
      </div>
    );
  }

  /**
   * @description Funkcja porownujaca nody wypychajaca na samą górę master nody
   * @function
   * @param {SingleNode} a
   * @param {SingleNode} b
   */
  nodeMasterComparer(a, b) {
    if (a.master === "*") return -1;
    else if (b.master === "*") return 1;
    else return 0;
  }

  /**
   * @description Mapuje pojedyńczy node na widok - wiersz
   * @function
   * @param {SingleNode} node
   * @param {String} key klucz dla elementów listy - react wymusza
   */
  nodeRow = (node, key) => {
    let masterImage =
      node.master === "*" ? "images/crown.png" : "images/knee.png";

    let element = (
      <Card key={key} className="server-status-row">
        <div className="flex-row">
          <img
            className="node-master-rectangle"
            src={masterImage}
            alt="crown node"
          />
          <Typography variant="h6">{node.name}</Typography>
        </div>
        <div className="flex-row">
          {this.nodeImageElement(
            node["heap.percent"] + "%",
            "images/java.png",
            "java heap stack"
          )}
          {this.nodeImageElement(
            node["ram.percent"] + "%",
            "images/ram.png",
            "ram usage"
          )}
          {this.nodeImageElement(node.ip, "images/ip.png", "ip address")}
        </div>
      </Card>
    );
    return element;
  };

  /**
   * @description Tworzy element do wiersza w którym są informacje o nodzie, odpowiednik toolstrip'a
   * @function
   * @param {String|Number} value wartość obok obrazka
   * @param {String} imageSource ścieżka do obrazka (zazwyczaj images/nazwa.png)
   * @param {String} alt tekst alternatywny dla obrazka
   * Maps single node to HTML row
   */
  nodeImageElement = (value, imageSource, alt) => {
    return (
      <div className="flex-row node-row-element">
        <div className="flex-row">
          <img alt={alt} className="node-row-image" src={imageSource} />
          <Typography
            style={{
              minWidth: 30,
              textAlign: "center"
            }}
            className="space-right-15"
            variant="h6"
          >
            {value}
          </Typography>
        </div>
      </div>
    );
  };

  //#endregion

  //#region clusterRow

  /**
   * @description Mapuje model stanu klastra na widok
   * @function
   * @param {ClusterHealth} clusterHealth
   */
  getClusterRowHealthFromState(clusterHealth) {
    return (
      <Card
        key={"main_cluster"}
        className="server-status-row cluster-row-outline"
      >
        <div className="flex-row">
          <img
            className="node-master-rectangle"
            src={"images/cluster.png"}
            alt="crown node"
          />
          <Typography variant="h6">{clusterHealth.cluster_name}</Typography>
        </div>
        <div className="flex-row">
          {this.nodeImageElement(
            clusterHealth.number_of_nodes,
            "images/node.png",
            "java heap stack"
          )}
          {this.nodeImageElement(
            clusterHealth.active_shards,
            "images/shard.png",
            "java heap stack"
          )}
          {this.nodeImageElement(
            clusterHealth.unassigned_shards,
            "images/shard_unallocated.png",
            "java heap stack"
          )}
          {this.nodeImageElement(
            RequestManager.clusterIp
              ? RequestManager.clusterIp.toString().replace("http://", "")
              : "",
            "images/ip.png",
            "ip address"
          )}
        </div>
      </Card>
    );
  }
  //#endregion
}

/**
 * @description odpowiada za mapowanie akcji do props komponentu
 * @param {Object} state
 * @memberof ServerStatus
 */
function mapStateToProps(state) {
  return { state };
}

/**
 * @description odpowiada za mapowanie akcji do props komponentu
 * @param {Object} dispatch
 * @memberof ServerStatus
 */
function mapDispatchToProps(dispatch) {
  return {
    serverStatusActions: bindActionCreators(serverStatusActions, dispatch)
  };
}

export default withRouter(
  connect(mapStateToProps, mapDispatchToProps)(ServerStatus)
);
