import React from 'react';
import './layout.scss';
import classNames from 'classnames/bind';

import { connect } from 'react-redux';

import routes from '../../../routes.json'

import Sidebar from '../sidebar';
import Topbar, { TopbarRight } from '../topbar';

function Layout(props) {
    return (
        <div className="layout-container d-flex flex-column">

            <div className="layout-topbar flex-grow-0">
                <Topbar />
            </div>

            <div className={classNames("layout-body d-flex flex-grow-1", { "sidebarCollapse": props.layout.sidebar.desktop }, { "sidebarShow": props.layout.sidebar.mobile })}>
                <div className="layout-sidebar h-100">
                    <Sidebar {...props} />
                </div>
                <div className="layout-content">
                    <TopbarRight className="d-block d-md-none d-lg-none topbarRightMob" />
                    {
                        [routes.routes.find(x => x.path === props.match.path)].map(x => {
                            const ThisComponent = React.lazy(() => import('../../' + x.component));
                            return <ThisComponent key={x.name} {...props} />
                        })
                    }
                </div>
            </div>

        </div>
    )
}

const mapStateToProps = (state) => {
    let { layout } = state;
    return { layout }
};

export default connect(mapStateToProps)(Layout);