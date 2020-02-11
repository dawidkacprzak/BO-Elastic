import React from "react";
import { Link } from "react-router-dom";
import '../style/template.css'
export default class Template extends React.Component {
  render() {
    return (
      <div id="template-container">
        <div id="template-menu-container" className='card'>
          <Link to="/">Server status</Link>
          <Link to="/Logs">Logs</Link>
        </div>
        <div id="template-content-container">{this.props.children}</div>
      </div>
    );
  }
}
