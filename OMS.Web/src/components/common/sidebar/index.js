import React from 'react';
import './sidebar.scss';
import classNames from 'classnames/bind';

import routes from '../../../routes.json'
import { connect } from 'react-redux';

class Sidebar extends React.Component {
    render() {
        return (
            <div className={classNames("sidebar h-100", {"sidebarCollapse": this.props.desktop})}>
                <div className="sidebar-profile media">
                    <img src={require('../../../img/avatar.png')} className="avatar align-self-center mr-3" alt="avatar" />
                    <div className="media-body align-self-center">
                        <p className="mb-2 text-white profile-primary">Alexander</p>
                        <p className="mb-0 text-white profile-secondary">Alexander 123</p>
                    </div>
                </div>
                <ul className="nav flex-column sidebar-links">
                    {
                        routes.routes.filter(x => x.showInSidebar).map(x => {
                            return (
                                <li key={x.name} className={classNames("nav-item flex-row align-items-center", {"active": (x.path === this.props.match.path)})} onClick={() => this.props.history.push(x.sidebarLink)}>
                                    <i className={x.icon}></i>
                                    <span>{x.name}</span>
                                </li>
                            )
                        })
                    }
                </ul>
            </div>
        );
    }
}

const mapStateToProps = (state) => {
    let { layout } = state,
        { sidebar } = layout,
        { desktop } = sidebar;
    return { desktop }
};

export default connect(mapStateToProps)(Sidebar);