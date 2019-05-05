import React from 'react';
import './authorisation.scss';

import ToggleBound from '../common/togglebound';
import Tabs from '../common/tabs';

const renderTabs = [
    {
        name: 'User Role',
        component: 'userrole',
        url: 'authorisation/tabs/userrole'
    },
    {
        name: 'Role Mangement',
        component: 'rolemanagement',
        url: 'authorisation/tabs/rolemanagement'
    },
    {
        name: 'User Application',
        component: 'userapplication',
        url: 'authorisation/tabs/userapplication'
    }
]

class Authorisation extends React.Component {
    constructor(props) {
        super(props);
        this.state = { inbound: true }
    }

    render() {
        return (
            <React.Fragment>
                <div className="text-right">
                    <ToggleBound toggle={this.state.inbound} onClick={() => this.setState({ inbound: !this.state.inbound })} />
                </div>
                <Tabs tabs={renderTabs} current={this.props.match.params.tab} onClick={x => this.props.history.push(`/authorisation/${x}`)} />
            </React.Fragment>
        );
    }
}

export default Authorisation;