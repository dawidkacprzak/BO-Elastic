import React from "react";
import { Link } from "react-router-dom";
import "../style/template.css";
const path = require("path");

export default class Template extends React.Component {
  render() {
    return (
      <div id="template-container">
        <div id="template-menu-container" className="card">
          <Link to="/">
            <img alt="service status" src={"images/database.png"}></img>
          </Link>
          <Link to="/Logs">
            <img alt="service status" src={"images/logs.png"}></img>
          </Link>
        </div>
        <div id="template-content-container">{this.props.children}</div>
      </div>
    );
  }
}
