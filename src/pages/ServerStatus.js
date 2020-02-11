import React from "react";
import { connect } from "react-redux";
import { withRouter } from "react-router";
class ServerStatus extends React.Component {
  render() {
    return <h1>Server Status</h1>;
  }
}

export default withRouter(connect()(ServerStatus));
