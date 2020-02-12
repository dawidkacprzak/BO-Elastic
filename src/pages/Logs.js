import React from 'react'
import { withRouter } from 'react-router-dom'
import { connect } from 'react-redux'


/**
 * Komponent testowy - jeszcze bez wartości
 * @type {React.Component}
 */
class Logs extends React.Component
{
    render(){
        return(
            <h1>Logs</h1>
        )
    }
}

export default withRouter(connect()(Logs));