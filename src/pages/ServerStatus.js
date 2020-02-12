//@ts-check
import React from "react";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import { bindActionCreators } from "redux";
import * as serverStatusActions from '../actions/serverStatusAction'
 
/**
 * Komponent odpowiedzialny za wyświetlanie danych o klastrze i nodach - jest jednym z podstawowych komponentów routingu
 * @type {React.Component}
 */
class ServerStatus extends React.Component {

  render() {
    return <h1>Server Status</h1>;
  }
}

/**
 * @description odpowiada za mapowanie akcji do props komponentu
 * @param {Object} state 
 * @memberof ServerStatus
 */
function mapStateToProps(state) {
  return { state }
}

/**
 * @description odpowiada za mapowanie akcji do props komponentu
 * @param {Object} dispatch 
 * @memberof ServerStatus
 */
function mapDispatchToProps(dispatch) {
  return {
    serverStatusActions: bindActionCreators(serverStatusActions, dispatch),
  }
}

export default withRouter(connect(mapStateToProps,mapDispatchToProps)(ServerStatus));
