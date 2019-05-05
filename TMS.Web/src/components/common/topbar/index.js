import React from 'react';
import './topbar.scss';

import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { switchdesksidebar, switchmobsidebar } from '../../../store/actions';

class Topbar extends React.Component {
    render() {
        return (
            <div className="container-fluid px-0 bg-white topbar d-block d-md-flex d-lg-flex">

                <div className="topbar-left d-flex align-items-center flex-grow-0">

                    <div className="logo float-left">
                        <img title="ASTRA MOTOR LOGO" alt="ASTRA MOTOR LOGO" src={require('../../../img/astra-logo.png')} />
                    </div>

                    <div className="topbar-burger">
                        <div className="burger-desktop" onClick={() => this.props.switchdesksidebar()}>
                            <i className="fas fa-bars"></i>
                        </div>
                        <div className="burger-mobile" onClick={() => this.props.switchmobsidebar()}>
                            <i className="fas fa-bars"></i>
                        </div>
                    </div>

                </div>

                <TopbarRight className="d-none d-md-flex d-lg-flex" />

                <div className="clearfix"></div>

            </div>
        );
    }
}

export function TopbarRight(props) {
    return (

        <div className={props.className + " topbar-right align-items-center text-center flex-grow-1 py-3 py-md-0"}>

            <div className="topbar-title text-uppercase">
                <p className="mb-0 font-weight-bold">TRANSPORTATION MANAGEMENT SYSTEM</p>
            </div>

            <div className="logo">
                <img title="ASTRA MOTOR LOGO" alt="ASTRA MOTOR LOGO" src={require('../../../img/honda-logo.png')} />
            </div>

        </div>
    );
}

const mapDispatchToProps = dispatch => (
    bindActionCreators({
        switchdesksidebar,
        switchmobsidebar
    }, dispatch)
);

export default connect(null, mapDispatchToProps)(Topbar);