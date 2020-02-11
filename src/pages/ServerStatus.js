import React from "react";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import { bindActionCreators } from "redux";
import * as serverStatusActions from '../actions/serverStatusAction'
 
class ServerStatus extends React.Component {
  constructor(props) {
    super(props);
  }

  componentDidMount(){
    
  }

  render() {
    return <h1>Server Status</h1>;
  }
}


function mapStateToProps(state) {
  return { state }
}

function mapDispatchToProps(dispatch) {
  return {
    serverStatusActions: bindActionCreators(serverStatusActions, dispatch),
  }
}

export default withRouter(connect(mapStateToProps,mapDispatchToProps)(ServerStatus));
